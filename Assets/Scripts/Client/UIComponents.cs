﻿using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HealthBarUIReference : ICleanupComponentData
{
    public GameObject Value;
}

public struct HealthBarOffset : IComponentData
{
    public float3 Value;
}

public class SkillShotUIReference : ICleanupComponentData
{
    public GameObject Value;
}