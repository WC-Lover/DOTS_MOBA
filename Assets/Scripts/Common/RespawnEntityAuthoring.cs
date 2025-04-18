﻿using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class RespawnEntityAuthoring : MonoBehaviour
{
    public float RespawnTime;

    public NetCodeConfig NetCodeConfig;
    public int SimulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;

    public class Baker : Baker<RespawnEntityAuthoring>
    {
        public override void Bake(RespawnEntityAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<RespawnEntityTag>(entity);
            AddComponent(entity, new RespawnTickCount
            {
                Value = (uint)(authoring.RespawnTime * authoring.SimulationTickRate)
            });
            AddBuffer<RespawnBufferElement>(entity);
        }
    }
}
