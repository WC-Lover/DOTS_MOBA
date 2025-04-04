
using Unity.Entities;
using UnityEngine;

public class MobaPrefabsAuthoring : MonoBehaviour
{
    [Header("Entities")]
    public GameObject Champion;
    [Header("GameObjects")]
    public GameObject HealthBarPrefab;

    public class Baker : Baker<MobaPrefabsAuthoring>
    {
        public override void Bake(MobaPrefabsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new MobaPrefabs
            {
                Champion = GetEntity(authoring.Champion, TransformUsageFlags.Dynamic),
            });
            AddComponentObject(entity, new UIPrefabs
            {
                HealthBar = authoring.HealthBarPrefab,
            });
        }
    }
}