
using System;
using Unity.Collections;
using Unity.Entities;
using CoCa.MapBlock;
using CoCa.Map;

namespace CoCa.Player
{
    public partial struct PlayerInvadedSystem : ISystem
    {
        private CoCa.MapBlock.MapBlock mapBlock;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainPlayer>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var mapData = SystemAPI.GetSingleton<MapData>();

            var ecb = ApplyPlayerColorToBlock(ref state, mapData);

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        //* Apply all player color to mapBlock
        private EntityCommandBuffer ApplyPlayerColorToBlock(ref SystemState state, MapData mapData)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (blockId, entity) in SystemAPI.Query<RefRW<BlockId>>().WithEntityAccess().WithNone<CoCa.Player.PlayerMovement>())
            {
                #region Loop Through all player to set block status

                mapBlock = state.EntityManager.GetSharedComponent<CoCa.MapBlock.MapBlock>(entity);
                foreach (var playerMovement in SystemAPI.Query<RefRO<PlayerMovement>>())
                {
                    if (blockId.ValueRO.blockId == playerMovement.ValueRO.playerMovementPosition)
                    {
                        //* Block
                        mapBlock.mapBlockStatus = playerMovement.ValueRO.playerColor;
                        //* MapData
                        mapData._mapData[blockId.ValueRO.blockId] = playerMovement.ValueRO.playerColor;
                    }
                }

                #endregion
                ecb.SetSharedComponentManaged(entity, mapBlock);
                SystemAPI.SetSingleton(mapData);
            }

            return ecb;
        }
    }
}