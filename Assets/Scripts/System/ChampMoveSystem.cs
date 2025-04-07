using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct ChampMoveSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GamePlayingTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (localTransform, movePosition, moveSpeed)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                ChampMoveTargetPosition,
                CharacterMoveSpeed>().WithAll<Simulate>())
        {
            var moveTarget = movePosition.Value;
            moveTarget.y = localTransform.ValueRO.Position.y;

            if (math.distancesq(localTransform.ValueRO.Position, moveTarget) < 0.001f) continue;

            var moveDirection = math.normalize(moveTarget - localTransform.ValueRO.Position);
            var moveVector = moveDirection * moveSpeed.Value * deltaTime;

            localTransform.ValueRW.Position += moveVector;
            localTransform.ValueRW.Rotation = quaternion.LookRotationSafe(moveDirection, math.up());
        }
    }
}