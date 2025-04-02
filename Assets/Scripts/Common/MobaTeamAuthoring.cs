﻿using Unity.Entities;
using UnityEngine;

public class MobaTeamAuthoring : MonoBehaviour
{
    public TeamType MobaTeam;

    public class Baker : Baker<MobaTeamAuthoring>
    {
        public override void Bake(MobaTeamAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MobaTeam { Value = authoring.MobaTeam });
        }
    }
}
