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
        state.RequireForUpdate<GameStartProperties>();
        state.RequireForUpdate<NetworkTime>();
        var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<MobaTeamRequest, ReceiveRpcCommandRequest>();
        state.RequireForUpdate(state.GetEntityQuery(builder));
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var championPrefab = SystemAPI.GetSingleton<MobaPrefabs>().Champion;

        var gamePropertyEntity = SystemAPI.GetSingletonEntity<GameStartProperties>();
        var gameStartProperties = SystemAPI.GetComponent<GameStartProperties>(gamePropertyEntity);
        var teamPlayerCounter = SystemAPI.GetComponent<TeamPlayerCounter>(gamePropertyEntity);
        var spawnOffsets = SystemAPI.GetBuffer<SpawnOffset>(gamePropertyEntity);

        foreach (var (teamRequest, requestSource, entity)
            in SystemAPI.Query<MobaTeamRequest, ReceiveRpcCommandRequest>().WithEntityAccess())
        {
            ecb.DestroyEntity(entity);
            ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

            var requestedTeamType = teamRequest.teamType;

            if (requestedTeamType == TeamType.AutoAssign)
            {
                requestedTeamType = teamPlayerCounter.BlueTeamPlayers < teamPlayerCounter.RedTeamPlayers ? TeamType.Blue : TeamType.Red;
            }

            var spawnPosition = new float3(0, 1, 0);

            switch (requestedTeamType)
            {
                case TeamType.Blue:
                    if (teamPlayerCounter.BlueTeamPlayers >= gameStartProperties.MaxPlayersPerTeam) continue;
                    spawnPosition = new float3(-50f, 1f, -50f);
                    spawnPosition += spawnOffsets[teamPlayerCounter.BlueTeamPlayers].Value;
                    teamPlayerCounter.BlueTeamPlayers++;
                    break;
                case TeamType.Red:
                    if (teamPlayerCounter.RedTeamPlayers >= gameStartProperties.MaxPlayersPerTeam) continue;
                    spawnPosition = new float3(50f, 1f, 50f);
                    spawnPosition += spawnOffsets[teamPlayerCounter.RedTeamPlayers].Value;
                    teamPlayerCounter.RedTeamPlayers++;
                    break;
                default:
                    continue;
            }

            var champ = ecb.Instantiate(championPrefab);
            var newTransform = LocalTransform.FromPosition(spawnPosition);

            var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
            ecb.SetComponent(champ, newTransform);
            ecb.SetComponent(champ, new GhostOwner { NetworkId = clientId, });
            ecb.SetComponent(champ, new MobaTeam { Value = requestedTeamType, });
            ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = champ, });

            ecb.SetComponent(champ, new NetworkEntityReference { Value = requestSource.SourceConnection });

            ecb.AddComponent(requestSource.SourceConnection, new PlayerSpawnInfo
            {
                MobaTeam = requestedTeamType,
                SpawnPosition = spawnPosition,
            });

            var playersRemainingToStart = gameStartProperties.MinPlayersToStartGame - teamPlayerCounter.TotalPlayers;

            var gameStartRpc = ecb.CreateEntity();
            if (playersRemainingToStart <= 0 && !SystemAPI.HasSingleton<GamePlayingTag>())
            {
                var simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
                var ticksUntilStart = (uint)(simulationTickRate * gameStartProperties.CountdownTime);
                var gameStartTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
                gameStartTick.Add(ticksUntilStart);

                ecb.AddComponent(gameStartRpc, new GameStartTickRpc { Value = gameStartTick });

                var gameStartEntity = ecb.CreateEntity();
                ecb.AddComponent(gameStartEntity, new GameStartTick { Value = gameStartTick });
            }
            else
            {
                ecb.AddComponent(gameStartRpc, new PlayersRemainingToStart { Value = playersRemainingToStart });
            }

            ecb.AddComponent<SendRpcCommandRequest>(gameStartRpc);
        }

        ecb.Playback(state.EntityManager);
        SystemAPI.SetSingleton(teamPlayerCounter);
    }
}
