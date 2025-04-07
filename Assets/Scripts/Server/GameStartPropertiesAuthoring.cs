
using Unity.Entities;
using UnityEngine;

public class GameStartPropertiesAuthoring : MonoBehaviour
{
    public int MaxPlayerPerTeam;
    public int MinPlayersToStartGame;
    public int CountdownTime;
    public Vector3[] SpawnOffsets;

    public class Baker : Baker<GameStartPropertiesAuthoring>
    {
        public override void Bake(GameStartPropertiesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameStartProperties
            {
                MaxPlayersPerTeam = authoring.MaxPlayerPerTeam,
                MinPlayersToStartGame = authoring.MinPlayersToStartGame,
                CountdownTime = authoring.CountdownTime,
            });
            AddComponent<TeamPlayerCounter>(entity);

            var spawnOffsets = AddBuffer<SpawnOffset>(entity);
            foreach (var spawnOffset in authoring.SpawnOffsets)
            {
                spawnOffsets.Add(new SpawnOffset { Value = spawnOffset });
            }
        }
    }
}
