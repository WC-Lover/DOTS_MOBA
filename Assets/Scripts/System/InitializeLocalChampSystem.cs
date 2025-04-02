using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct InitializeLocalChampSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkId>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (localTransform, entity)
            in SystemAPI.Query<LocalTransform>().WithAll<GhostOwnerIsLocal>().WithNone<OwnerChampTag>().WithEntityAccess())
        {
            ecb.AddComponent<OwnerChampTag>(entity);
            ecb.SetComponent(entity, new ChampMoveTargetPosition
            {
                Value = localTransform.Position,
            });
        }

        ecb.Playback(state.EntityManager);
    }
}
