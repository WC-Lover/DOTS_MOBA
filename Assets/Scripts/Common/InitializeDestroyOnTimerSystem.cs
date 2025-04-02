using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public partial struct InitializeDestroyOnTimerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkTime>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        var simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
        var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

        foreach (var (destroyOnTimer, entity) in SystemAPI.Query<DestroyOnTimer>().WithNone<DestroyAtTick>().WithEntityAccess())
        {
            var lifetimeInTick = (uint) (destroyOnTimer.Value * simulationTickRate);
            var targetTick = currentTick;
            targetTick.Add(lifetimeInTick);
            ecb.AddComponent(entity, new DestroyAtTick
            {
                Value = targetTick,
            });

        }
        ecb.Playback(state.EntityManager);
    }
}
