﻿
using Unity.Entities;
using UnityEngine;

public class HitPointsAuthoring : MonoBehaviour
{
    public int MaxHitPoints;

    public class Baker : Baker<HitPointsAuthoring>
    {
        public override void Bake(HitPointsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CurrentHitPoints { Value = authoring.MaxHitPoints });
            AddComponent(entity, new MaxHitPoints { Value = authoring.MaxHitPoints });
            AddBuffer<DamageBufferElement>(entity);
            AddBuffer<DamageThisTick>(entity);
        }
    }
}