using CoCa.MapBlock;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;


namespace CoCa.Map
{
    public partial struct MapSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Map>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SpawnMapBlock(ref state);
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            state.Enabled = false;
        }

        private EntityCommandBuffer SpawnMapBlock(ref SystemState state)
        {
            #region Variable
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var mapBlock = SystemAPI.GetSingleton<Map>();
            int mapWidth = mapBlock.mapWidth;
            int mapHeight = mapBlock.mapHeight;
            var mapBlockPrefab = mapBlock.mapBlockPrefab;
            #endregion

            #region SpawnMap
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    var mapBlockEntity = ecb.Instantiate(mapBlockPrefab);
                    ecb.SetComponent(mapBlockEntity, new LocalTransform
                    {
                        Position = new Unity.Mathematics.float3(x, y, 0),
                        Rotation = Unity.Mathematics.quaternion.identity,
                        Scale = 1f
                    });
                    ecb.AddComponent(mapBlockEntity, new BlockId
                    {
                        blockId = x * mapHeight + y,
                    });
                }
            }
            #endregion

            return ecb;
        }
    }

}

