using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class MapAuthoring : MonoBehaviour
{
    public GameObject _mapBlockPrefab;
    public int _mapWidth;
    public int _mapHeight;
}

public partial struct Map : IComponentData
{
    public Entity mapBlockPrefab { get; set; }
    public int mapWidth { set; get; }
    public int mapHeight { set; get; }
}

public class MapBaker : Baker<MapAuthoring>
{
    public override void Bake(MapAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new Map
        {
            mapBlockPrefab = GetEntity(authoring._mapBlockPrefab),
            mapWidth = authoring._mapWidth,
            mapHeight = authoring._mapHeight,
        });


    }
}