using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace CoCa.MapBlock
{
    public class MapBlockAuthoring : MonoBehaviour
    {
        public UniteData.Color _mapBlockStatus = UniteData.Color.Empty;
        public Material _emptyMaterial;
        public Material _wallMaterial;
        public Material _redMaterial;
        public Material _greenMaterial;
    }
    public partial struct MapBlock : ISharedComponentData
    {
        public UniteData.Color mapBlockStatus;
        public BatchMaterialID emptyMaterialID;
        public BatchMaterialID wallMaterialID;
        public BatchMaterialID redMaterialID;
        public BatchMaterialID greenMaterialID;
    }
    public class MapBlockBaker : Baker<MapBlockAuthoring>
    {
        public override void Bake(MapBlockAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var hybridRender = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<Entities​Graphics​System>();
            AddSharedComponent(entity, new MapBlock
            {
                mapBlockStatus = authoring._mapBlockStatus,

                emptyMaterialID = hybridRender.RegisterMaterial(authoring._emptyMaterial),
                wallMaterialID = hybridRender.RegisterMaterial(authoring._wallMaterial),
                redMaterialID = hybridRender.RegisterMaterial(authoring._redMaterial),
                greenMaterialID = hybridRender.RegisterMaterial(authoring._greenMaterial)
            });
        }
    }

}
