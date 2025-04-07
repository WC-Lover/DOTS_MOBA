﻿using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

public struct RespawnEntityTag : IComponentData { }

public struct RespawnBufferElement : IBufferElementData
{
    [GhostField] public NetworkTick RepsawnTick;
    [GhostField] public Entity NetworkEntity;
    [GhostField] public int NetworkId;
}

public struct RespawnTickCount : IComponentData
{
    public uint Value;
}

public struct PlayerSpawnInfo : IComponentData
{
    public TeamType MobaTeam;
    public float3 SpawnPosition;
}

public struct NetworkEntityReference : IComponentData
{
    public Entity Value;
}