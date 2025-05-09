﻿
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class NpcAttackAuthoring : MonoBehaviour
{
    public float NpcTargetRadius;
    public Vector3 FirePointOffset;
    public float AttackCooldownTime;
    public GameObject AttackPrefab;

    public NetCodeConfig NetCodeConfig;
    public int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;

    public class Baker : Baker<NpcAttackAuthoring>
    {
        public override void Bake(NpcAttackAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NpcTargetRadius { Value = authoring.NpcTargetRadius });
            AddComponent(entity, new NpcAttackProperties
            {
                FirePointOffset = authoring.FirePointOffset,
                CooldownTickCount = (uint)(authoring.AttackCooldownTime * authoring.SimulationTickRate),
                AttackPrefab = GetEntity(authoring.AttackPrefab, TransformUsageFlags.Dynamic)
            });
            AddComponent<NpcTargetEntity>(entity);
            AddBuffer<NpcAttackCooldown>(entity);
        }
    }
}
