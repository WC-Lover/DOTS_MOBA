using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientRequestGameEntrySystem : ISystem
{
    private EntityQuery _pendingNetworkIdQuery;

    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
        _pendingNetworkIdQuery = state.GetEntityQuery(builder);
        state.RequireForUpdate(_pendingNetworkIdQuery);
        state.RequireForUpdate<ClientTeamRequest>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var requestTeam = SystemAPI.GetSingleton<ClientTeamRequest>().teamType;
        var ecb = new EntityCommandBuffer(Allocator.Temp);


        var pendingNetworkIds = _pendingNetworkIdQuery.ToEntityArray(Allocator.Temp);
        
        foreach (var pendingNetworkId in pendingNetworkIds)
        {
            ecb.AddComponent<NetworkStreamInGame>(pendingNetworkId);
            var requestTeamEnity = ecb.CreateEntity();
            
            ecb.AddComponent(requestTeamEnity, new MobaTeamRequest
            {
                teamType = requestTeam,
            });

            ecb.AddComponent(requestTeamEnity, new SendRpcCommandRequest
            {
                TargetConnection = pendingNetworkId
            });
        }

        ecb.Playback(state.EntityManager);
    }
}

