using Unity.Entities;
using UnityEngine;

public class GameOverOnDestroyAuthoring : MonoBehaviour
{
    public class Baker : Baker<GameOverOnDestroyAuthoring>
    {
        public override void Bake(GameOverOnDestroyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<GameOverOnDestroyTag>(entity);
        }
    }
}
