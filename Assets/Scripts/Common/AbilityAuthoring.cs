using Unity.Entities;
using UnityEngine;

public class AbilityAuthoring : MonoBehaviour
{
    public GameObject AoeAbility;

    public class Baker : Baker<AbilityAuthoring>
    {
        public override void Bake(AbilityAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AbilityPrefabs
            {
                AoeAbility = GetEntity(authoring.AoeAbility, TransformUsageFlags.Dynamic),
            });
        }
    }
}
