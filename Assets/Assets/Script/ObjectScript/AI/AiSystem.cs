
using System;
using CoCa.Map;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace CoCa.Ai
{
    public partial struct AiSystem : ISystem
    {
        private NativeArray<UniteData.Color> mapData;
        private NativeArray<int> aiMap;
        private bool IsInitAiMap;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MapData>();
            IsInitAiMap = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            mapData = SystemAPI.GetSingleton<MapData>()._mapData;


            //$ AI

            if (IsInitAiMap == false)
            {
                AddAiMap(ref state);

                var aiMapEntity = SystemAPI.GetSingleton<AiMap>();
                aiMap = aiMapEntity._aiMapData;
                aiMap = InitAiGroundMapData(aiMap, mapData);
                aiMap = InitAiWallMapData(aiMap);
                SystemAPI.SetSingleton<AiMap>(aiMapEntity);

                IsInitAiMap = true;
            }


            foreach (var item in aiMap)
            {
                Debug.Log(item);
            }

            state.Enabled = false;
        }



        private void AddAiMap(ref SystemState state)
        {
            var map = SystemAPI.GetSingleton<CoCa.Map.Map>();
            var totalBlock = map.mapWidth * map.mapHeight;

            //* Add AiMap to AiManager
            EntityQuery aiManagerQuery = state.GetEntityQuery(typeof(Ai));
            state.EntityManager.AddComponentData(
                aiManagerQuery.GetSingletonEntity(),
                new AiMap
                {
                    _aiMapData = new NativeArray<int>(map.mapWidth * map.mapHeight, Allocator.TempJob)
                }
            );
        }

        #region AI METHOD
        //* return Wall = 0 Other = 1
        public NativeArray<int> InitAiGroundMapData(NativeArray<int> nullMap, NativeArray<UniteData.Color> ObstacleMap)
        {
            for (int i = 0; i < ObstacleMap.Length; i++)
            {
                if (ObstacleMap[i] == UniteData.Color.Wall)
                {
                    nullMap[i] = 0;
                    continue;
                }
                nullMap[i] = 1;
            }
            return nullMap;
        }


        private NativeArray<int> InitAiWallMapData(NativeArray<int> aiMap)
        {

            for (int i = 0; i < aiMap.Length; i++)
            {
                // Add wall+ to aimpdata
            }
            return aiMap;
        }
        #endregion

    }
}