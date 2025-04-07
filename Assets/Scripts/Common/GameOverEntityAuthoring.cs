using Unity.Entities;
using UnityEngine;

public class GameOverEntityAuthoring : MonoBehaviour
{
    public class Baker : Baker<GameOverEntityAuthoring>
    {
        public override void Bake(GameOverEntityAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent<GameOverTag>(entity);
            AddComponent<WinningTeam>(entity);
        }
    }
}
