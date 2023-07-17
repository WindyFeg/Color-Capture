
using System;
using Unity.Collections;
using Unity.Entities;
using CoCa.MapBlock;

namespace CoCa.Player
{
    public partial struct PlayerInvadedSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainPlayer>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            EntityQuery mainPlayerEq = state.GetEntityQuery(typeof(MainPlayer));
            Entity mainPlayer = mainPlayerEq.GetSingletonEntity();
            PlayerMovement playerComponent = state.EntityManager.
            GetComponentData<PlayerMovement>(mainPlayer);

            foreach (var (blockId, entity) in SystemAPI.Query<RefRW<BlockId>>().WithEntityAccess().WithNone<CoCa.Player.PlayerMovement>())
            {
                CoCa.MapBlock.MapBlock mapBlock;
                mapBlock = state.EntityManager.GetSharedComponent<CoCa.MapBlock.MapBlock>(entity);
                if (blockId.ValueRO.blockId == playerComponent.playerMovementPosition)
                {
                    mapBlock.mapBlockStatus = playerComponent.playerColor;
                }
                ecb.SetSharedComponentManaged(entity, mapBlock);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

    }
}