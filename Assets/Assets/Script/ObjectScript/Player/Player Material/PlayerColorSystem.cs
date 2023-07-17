
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

namespace CoCa.Player
{
    public partial struct PlayerColorSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerColor>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerColor, playerMovement, materialMeshInfo) in SystemAPI.Query<RefRO<PlayerColor>, RefRO<PlayerMovement>, RefRW<MaterialMeshInfo>>())
            {
                materialMeshInfo.ValueRW.MaterialID = playerMovement.ValueRO.playerColor == UniteData.Color.Red ? playerColor.ValueRO.playerRedMaterialID : playerColor.ValueRO.playerGreenMaterialID;
            }
        }
    }
}