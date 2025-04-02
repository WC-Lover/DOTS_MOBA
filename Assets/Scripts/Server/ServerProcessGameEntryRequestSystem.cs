using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerProcessGameEntryRequestSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MobaPrefabs>();
        var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<MobaTeamRequest, ReceiveRpcCommandRequest>();
        state.RequireForUpdate(state.GetEntityQuery(builder));
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var championPrefab = SystemAPI.GetSingleton<MobaPrefabs>().Champion;

        foreach (var (teamRequest, requestSource, entity)
            in SystemAPI.Query<MobaTeamRequest, ReceiveRpcCommandRequest>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity);
            ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

            var requestedTeamType = teamRequest.teamType;

            if (requestedTeamType == TeamType.AutoAssign)
            {
                requestedTeamType = TeamType.Blue;
            }

            var spawnPosition = new float3(0, 1, 0);

            switch (requestedTeamType)
            {
                case TeamType.Blue:
                    spawnPosition = new float3(-50f, 1f, -50f);
                    break;
                case TeamType.Red:
                    spawnPosition = new float3(50f, 1f, 50f);
                    break;
                default:
                    continue;
            }

            var champ = ecb.Instantiate(championPrefab);
            var newTransform = LocalTransform.FromPosition(spawnPosition);
            ecb.SetComponent(champ, newTransform);

            var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
            ecb.SetComponent(champ, new GhostOwner
            {
                NetworkId = clientId,
            });
            ecb.SetComponent(champ, new MobaTeam
            {
                Value = requestedTeamType,
            });
            ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup
            {
                Value = champ,
            });
        }

        ecb.Playback(state.EntityManager);
    }
}
