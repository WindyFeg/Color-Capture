// using System.Collections;
// using System.Collections.Generic;
// using Unity.Entities;
// using Unity.Rendering;
// using UnityEngine;
// using UnityEngine.Rendering;

// public class PlayerAuthoring : MonoBehaviour
// {
//     public UniteData.Color _playerColor = UniteData.Color.Empty;
//     public Material _playerRedMaterial;
//     public Material _playerGreenMaterial;
// }
// public partial struct Player : IComponentData
// {
//     public UniteData.Color playerColor;
//     public BatchMaterialID playerRedMaterialID;
//     public BatchMaterialID playerGreenMaterialID;
// }


// public class PlayerBaker : Baker<PlayerAuthoring>
// {
//     public override void Bake(PlayerAuthoring authoring)
//     {
//         var entity = GetEntity(TransformUsageFlags.Dynamic);
//         var hybridRender = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<Entities​Graphics​System>();
//         AddComponent(entity, new Player
//         {
//             playerColor = authoring._playerColor,

//             playerRedMaterialID = hybridRender.RegisterMaterial(authoring._playerRedMaterial),
//             playerGreenMaterialID = hybridRender.RegisterMaterial(authoring._playerGreenMaterial)
//         });
//     }
// }