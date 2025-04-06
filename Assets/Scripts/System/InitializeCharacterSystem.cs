
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct InitializeCharacterSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        
        foreach (var (physicsMass, mobaTeam, entity)
            in SystemAPI.Query<
            RefRW<PhysicsMass>,
            MobaTeam>().WithAny<NewChampTag, NewMinionTag>().WithEntityAccess())
        {
            physicsMass.ValueRW.InverseInertia[0] = 0;
            physicsMass.ValueRW.InverseInertia[1] = 0;
            physicsMass.ValueRW.InverseInertia[2] = 0;

            var teamColor = mobaTeam.Value switch
            {
                TeamType.Blue => new float4(0, 0, 1, 1),
                TeamType.Red => new float4(1, 0, 0, 1),
                _ => new float4(1)
            };

            ecb.SetComponent(entity, new URPMaterialPropertyBaseColor
            {
                Value = teamColor,
            });
            ecb.RemoveComponent<NewChampTag>(entity);
            ecb.RemoveComponent<NewMinionTag>(entity);
        }

        ecb.Playback(state.EntityManager);
    }
}
