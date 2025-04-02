using Unity.Entities;
using UnityEngine;

public class MainCamera : IComponentData
{
    public Camera Value;
}

public class MainCameraAuthoring : MonoBehaviour
{
    public class Baker : Baker<MainCameraAuthoring>
    {
        public override void Bake(MainCameraAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new MainCamera());
            AddComponent<MainCameraTag>(entity);
        }
    }
}

public struct MainCameraTag : IComponentData
{

}