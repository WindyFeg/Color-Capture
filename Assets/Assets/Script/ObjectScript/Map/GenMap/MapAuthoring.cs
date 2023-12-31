using Unity.Collections;
using Unity.Entities;
using UnityEngine;
namespace CoCa.Map
{
    public class MapAuthoring : MonoBehaviour
    {
        public GameObject _mapBlockPrefab;
        public int _mapWidth;
        public int _mapHeight;
        public bool _twoPlayerMode;

    }

    public partial struct Map : IComponentData
    {
        public Entity mapBlockPrefab { get; set; }
        public int mapWidth { set; get; }
        public int mapHeight { set; get; }
        public bool TwoPlayerMode { set; get; }
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
                TwoPlayerMode = authoring._twoPlayerMode
            });


        }
    }
}