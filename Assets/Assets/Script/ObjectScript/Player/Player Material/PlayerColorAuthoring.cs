using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoCa.Player
{
    public class PlayerColorAuthoring : MonoBehaviour
    {
        public Material _playerRedMaterial;
        public Material _playerGreenMaterial;
    }
    public partial struct PlayerColor : IComponentData
    {
        public BatchMaterialID playerRedMaterialID;
        public BatchMaterialID playerGreenMaterialID;
    }


    public class PlayerColorBaker : Baker<PlayerColorAuthoring>
    {
        public override void Bake(PlayerColorAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var hybridRender = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<Entities​Graphics​System>();
            AddComponent(entity, new PlayerColor
            {
                playerRedMaterialID = hybridRender.RegisterMaterial(authoring._playerRedMaterial),
                playerGreenMaterialID = hybridRender.RegisterMaterial(authoring._playerGreenMaterial)
            });
        }
    }
}