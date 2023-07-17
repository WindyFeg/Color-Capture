
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using CoCa.Map;

namespace CoCa.MapBlock
{

    public partial struct BlockSystem : ISystem
    {
        public bool IsSetBlockStatus { set; get; }
        public void OnCreate(ref SystemState state)
        {
            //! Have Map data 
            state.RequireForUpdate<CoCa.Map.Map>();
            state.RequireForUpdate<MapData>();

            IsSetBlockStatus = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            if (IsSetBlockStatus == false)
            {
                SetBlockStatus(ref state);
                IsSetBlockStatus = true;
            }
            else
            {
                UpdateColorBaseOnStatus(ref state);
            }
        }

        private void UpdateColorBaseOnStatus(ref SystemState state)
        {
            foreach (var (materialMeshInfo, entity) in SystemAPI.Query<RefRW<MaterialMeshInfo>>().WithEntityAccess())
            {
                var mapBlock = state.EntityManager.GetSharedComponentManaged<MapBlock>(entity);
                Debug.Log(mapBlock.mapBlockStatus);
                switch (mapBlock.mapBlockStatus)
                {
                    case UniteData.Color.Red:
                        materialMeshInfo.ValueRW.MaterialID = mapBlock.redMaterialID;
                        break;
                    case UniteData.Color.Green:
                        materialMeshInfo.ValueRW.MaterialID = mapBlock.greenMaterialID;
                        break;
                    case UniteData.Color.Empty:
                        materialMeshInfo.ValueRW.MaterialID = mapBlock.emptyMaterialID;
                        break;
                    case UniteData.Color.Wall:
                        materialMeshInfo.ValueRW.MaterialID = mapBlock.wallMaterialID;
                        break;
                    default:
                        materialMeshInfo.ValueRW.MaterialID = mapBlock.wallMaterialID;
                        break;
                }
            }
        }

        private void SetBlockStatus(ref SystemState state)
        {
            #region Variable
            var map = SystemAPI.GetSingleton<CoCa.Map.Map>();
            var mapData = SystemAPI.GetSingleton<MapData>()._mapData;
            var totalBlock = map.mapWidth * map.mapHeight;
            var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
            #endregion

            #region Set map data to Block
            foreach (var (blockId, entity) in SystemAPI.Query<RefRO<BlockId>>().WithEntityAccess())
            {
                Debug.Log(mapData[blockId.ValueRO.blockId]);
                var MapBlock = state.EntityManager.GetSharedComponentManaged<MapBlock>(entity);
                MapBlock.mapBlockStatus = mapData[blockId.ValueRO.blockId];
                ecb.SetSharedComponentManaged(entity, MapBlock);
            }
            ecb.Playback(state.EntityManager);
            #endregion
        }
    }
}