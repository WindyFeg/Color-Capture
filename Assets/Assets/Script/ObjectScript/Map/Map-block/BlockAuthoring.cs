using Unity.Entities;
using UnityEngine;
using UniteData;
using Unity.Rendering;
using UnityEngine.Rendering;
using Unity.Burst;
using Unity.Transforms;

public class MapBlockAuthoring : MonoBehaviour
{
    public UniteData.Color _mapBlockStatus = UniteData.Color.Empty;
    public Material _emptyMaterial;
    public Material _wallMaterial;
    public Material _redMaterial;
    public Material _greenMaterial;
}
public partial struct MapBlock : IComponentData
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
        var hybirdRender = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<Entities​Graphics​System>();
        AddComponent(entity, new MapBlock
        {
            mapBlockStatus = authoring._mapBlockStatus,

            emptyMaterialID = hybirdRender.RegisterMaterial(authoring._emptyMaterial),
            wallMaterialID = hybirdRender.RegisterMaterial(authoring._wallMaterial),
            redMaterialID = hybirdRender.RegisterMaterial(authoring._redMaterial),
            greenMaterialID = hybirdRender.RegisterMaterial(authoring._greenMaterial)
        });
    }
}
