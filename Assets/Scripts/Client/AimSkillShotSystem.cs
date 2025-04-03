using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial struct AimSkillShotSystem : ISystem
{
    private CollisionFilter _selectionFilter;

    public void OnCreate(ref SystemState state)
    {
        _selectionFilter = new CollisionFilter
        {
            BelongsTo = 1 << 5, // Raycasts
            CollidesWith = 1 << 0, // GroundPlane
        };
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (aimInput, transform) in SystemAPI.Query<RefRW<AimInput>, LocalTransform>().WithAll<AimSkillShotTag, OwnerChampTag>())
        {
            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            var cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
            var mainCamera = state.EntityManager.GetComponentObject<MainCamera>(cameraEntity).Value;

            var mousePosition = Input.mousePosition;
            mousePosition.z = 1000f;
            var worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

            var selectionInput = new RaycastInput
            {
                Start = mainCamera.transform.position,
                End = worldPosition,
                Filter = _selectionFilter,
            };

            if (collisionWorld.CastRay(selectionInput, out var closestHit))
            {
                var directionToTarget = closestHit.Position - transform.Position;
                directionToTarget.y = transform.Position.y;
                directionToTarget = math.normalize(directionToTarget);
                aimInput.ValueRW.Value = directionToTarget;
            }
        }
    }
}