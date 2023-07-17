
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
            foreach (var (block, materialMeshInfo) in SystemAPI.Query<RefRO<MapBlock>, RefRW<MaterialMeshInfo>>())
            {
                Debug.Log(block.ValueRO.mapBlockStatus);
                switch (block.ValueRO.mapBlockStatus)
                {
                    case UniteData.Color.Red:
                        materialMeshInfo.ValueRW.MaterialID = block.ValueRO.redMaterialID;
                        break;
                    case UniteData.Color.Green:
                        materialMeshInfo.ValueRW.MaterialID = block.ValueRO.greenMaterialID;
                        break;
                    case UniteData.Color.Empty:
                        materialMeshInfo.ValueRW.MaterialID = block.ValueRO.emptyMaterialID;
                        break;
                    case UniteData.Color.Wall:
                        materialMeshInfo.ValueRW.MaterialID = block.ValueRO.wallMaterialID;
                        break;
                    default:
                        materialMeshInfo.ValueRW.MaterialID = block.ValueRO.wallMaterialID;
                        break;
                }
            }
        }

        private void SetBlockStatus(ref SystemState state)
        {
            var map = SystemAPI.GetSingleton<CoCa.Map.Map>();
            var mapData = SystemAPI.GetSingleton<MapData>()._mapData;
            var totalBlock = map.mapWidth * map.mapHeight;
            foreach (var (blockId, mapBlock) in SystemAPI.Query<RefRO<BlockId>, RefRW<MapBlock>>())
            {
                Debug.Log(mapData[blockId.ValueRO.blockId]);
                mapBlock.ValueRW.mapBlockStatus = mapData[blockId.ValueRO.blockId];
            }
        }
    }
}