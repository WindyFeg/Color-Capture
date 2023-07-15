using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
public partial struct MapDataSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Map>();
    }

    public void OnUpdate(ref SystemState state)
    {
        RandomWall(ref state);
        state.Enabled = false;
    }

    private void RandomWall(ref SystemState state)
    {
        var gameManager = state.GetEntityQuery(typeof(Map)).GetSingletonEntity();
        var map = SystemAPI.GetSingleton<Map>();
        NativeArray<UniteData.Color> mapData = new NativeArray<UniteData.Color>(map.mapWidth * map.mapHeight, Allocator.TempJob);
        for (int i = 0; i < map.mapWidth * map.mapHeight; i++)
        {
            mapData[i] = (UniteData.Color)Random.Range(0, 4);
            Debug.Log(mapData[i]);
        }
        state.EntityManager.AddComponentData(gameManager, new MapData
        {
            _mapData = mapData,
        });
    }
}

