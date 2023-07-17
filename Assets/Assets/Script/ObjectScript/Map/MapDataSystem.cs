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
        int totalBlock = map.mapWidth * map.mapHeight;

        //Random half left
        for (int i = 0; i < totalBlock / 2; i++)
        {
            mapData[i] = RandomWall(25);
            Debug.Log(mapData[i]);
        }
        //Set empty for 4 corner
        mapData[0] = UniteData.Color.Empty;
        mapData[1] = UniteData.Color.Empty;
        mapData[map.mapHeight] = UniteData.Color.Empty;
        mapData[map.mapHeight + 1] = UniteData.Color.Empty;
        //Copy half left to half right
        for (int i = totalBlock / 2; i < totalBlock; i++)
        {
            mapData[i] = mapData[totalBlock - i - 1];
            Debug.Log(mapData[i]);
        }
        state.EntityManager.AddComponentData(gameManager, new MapData
        {
            _mapData = mapData,
        });
    }

    private UniteData.Color RandomWall(int wallRate)
    {
        int percent = Random.Range(0, 100);
        if (wallRate > percent)
        {
            return UniteData.Color.Wall;
        }
        else
        {
            return UniteData.Color.Empty;
        }
    }
}

