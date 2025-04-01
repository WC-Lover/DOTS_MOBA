
using Unity.Entities;
using UnityEngine;

public class MobaPrefabsAuthoring : MonoBehaviour
{
    public GameObject Champion;

    public class Baker : Baker<MobaPrefabsAuthoring>
    {
        public override void Bake(MobaPrefabsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new MobaPrefabs
            {
                Champion = GetEntity(authoring.Champion, TransformUsageFlags.Dynamic),
            });
        }
    }
}