﻿
using Unity.Entities;
using UnityEngine;

public class MobaPrefabsAuthoring : MonoBehaviour
{
    [Header("Entities")]
    public GameObject Champion;
    public GameObject Minion;
    public GameObject GameOverEntity;
    public GameObject RespawnEntity;
    [Header("GameObjects")]
    public GameObject HealthBarPrefab;
    public GameObject SkillShotAimPrefab;

    public class Baker : Baker<MobaPrefabsAuthoring>
    {
        public override void Bake(MobaPrefabsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new MobaPrefabs
            {
                Champion = GetEntity(authoring.Champion, TransformUsageFlags.Dynamic),
                Minion = GetEntity(authoring.Minion, TransformUsageFlags.Dynamic),
                GameOverEntity = GetEntity(authoring.GameOverEntity, TransformUsageFlags.None),
                RespawnEntity = GetEntity(authoring.RespawnEntity, TransformUsageFlags.None),
            });
            AddComponentObject(entity, new UIPrefabs
            {
                HealthBar = authoring.HealthBarPrefab,
                SkillShot = authoring.SkillShotAimPrefab,
            });
        }
    }
}