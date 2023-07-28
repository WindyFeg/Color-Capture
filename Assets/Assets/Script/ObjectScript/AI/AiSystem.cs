
using System;
using CoCa.Map;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using CoCa.Functions;
using CoCa.Player;

namespace CoCa.Ai
{
    public partial struct AiSystem : ISystem
    {
        private NativeArray<UniteData.Color> mapData;
        private NativeArray<int> aiMap;
        private CoCa.Map.Map map;
        private bool IsInitAiMap;
        private Ai ai;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MapData>();
            IsInitAiMap = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            mapData = SystemAPI.GetSingleton<MapData>()._mapData;
            map = SystemAPI.GetSingleton<CoCa.Map.Map>();
            ai = SystemAPI.GetSingleton<Ai>();

            //$ AI

            if (IsInitAiMap == false)
            {
                AddAiMap(ref state);

                var aiMapEntity = SystemAPI.GetSingleton<AiMap>();
                aiMap = aiMapEntity._aiMapData;
                aiMap = InitAiCentreMapData(aiMap);
                aiMap = InitAiGroundMapData(aiMap, mapData);
                aiMap = InitAiWallMapData(aiMap);
                aiMap = CurrentAiPlayer(ref state, aiMap);
                aiMap = AvoidStuckWall(aiMap);
                SystemAPI.SetSingleton<AiMap>(aiMapEntity);

                IsInitAiMap = true;
            }
            // PrintAiMap();

            state.Enabled = false;

        }

        private void PrintAiMap()
        {
            for (int i = 0; i < aiMap.Length; i++)
            {
                Debug.Log("Position " + i + " score :" + aiMap[i]);
            }
        }

        private NativeArray<int> InitAiCentreMapData(NativeArray<int> aiMap)
        {
            var sizeMap = map.mapWidth;
            int[,] pattern = Functions.Functions.CreatePattern(sizeMap);
            int[] patternFlatten = Functions.Functions.FlattenArray(pattern);

            for (int i = 0; i < map.mapHeight * map.mapWidth; i++)
            {
                aiMap[i] += patternFlatten[i];
            }

            return aiMap;
        }

        private NativeArray<int> AvoidStuckWall(NativeArray<int> aiMap)
        {
            for (int i = 0; i < map.mapHeight * map.mapWidth; i++)
            {
                if (aiMap[i] > ai._scoreWall * 3)
                {
                    aiMap[i] = 1;
                }
            }
            return aiMap;
        }

        private NativeArray<int> CurrentAiPlayer(ref SystemState state, NativeArray<int> aiMap)
        {
            int aiPlayerPosition = Functions.Functions.GetAiPlayerPosition(ref state);
            aiMap[aiPlayerPosition] = -1;
            int playerPosition = Functions.Functions.GetMainPlayerPosition(ref state);
            aiMap[playerPosition] = -1;
            return aiMap;
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
                nullMap[i] += 1;
            }
            return nullMap;
        }


        private NativeArray<int> InitAiWallMapData(NativeArray<int> aiMap)
        {
            #region Add Score around map
            for (int i = 0; i < map.mapHeight * map.mapWidth; i++)
            {
                if (aiMap[i] == 0)
                {
                    continue;
                }
                else
                {
                    //* Check around map left
                    //* 1->9
                    if (i < map.mapWidth && i > 0)
                        aiMap[i] += ai._scoreWall;
                    //* Check around map right
                    //* 91->99
                    if (i < map.mapWidth * map.mapHeight && i > map.mapWidth * map.mapHeight - map.mapWidth)
                        aiMap[i] += ai._scoreWall;
                    //* Check around map top
                    //* 0,10,20,30,40,50,60,70,80
                    if (i % map.mapWidth == 0)
                        aiMap[i] += ai._scoreWall;
                    //* Check around map bottom
                    //* 9,19,29,39,49,59,69,79,89
                    if (i % map.mapWidth == map.mapWidth - 1)
                        aiMap[i] += ai._scoreWall;
                }
            }
            #endregion

            #region Add Score Around Wall
            for (int i = 0; i < map.mapHeight * map.mapWidth; i++)
            {
                //* if i is wall
                if (aiMap[i] == 0)
                {
                    //* grade aground wall
                    var up = Functions.Functions.GetUpId(i, map.mapWidth, map.mapHeight);
                    var left = Functions.Functions.GetLeftId(i, map.mapWidth, map.mapHeight);
                    var down = Functions.Functions.GetDownId(i, map.mapWidth, map.mapHeight);
                    var right = Functions.Functions.GetRightId(i, map.mapWidth, map.mapHeight);

                    if (up != -1 && aiMap[up] != 0)
                    {
                        aiMap[up] += ai._scoreWall;
                    }
                    if (left != -1 && aiMap[left] != 0)
                    {
                        aiMap[left] += ai._scoreWall;
                    }
                    if (down != -1 && aiMap[down] != 0)
                    {
                        aiMap[down] += ai._scoreWall;
                    }
                    if (right != -1 && aiMap[right] != 0)
                    {
                        aiMap[right] += ai._scoreWall;
                    }
                }
            }
            #endregion

            return aiMap;
        }
        #endregion
    }
}