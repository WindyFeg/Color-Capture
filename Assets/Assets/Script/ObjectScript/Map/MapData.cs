using Unity.Collections;
using Unity.Entities;
using UnityEngine;
public partial struct MapData : IComponentData
{
    public NativeArray<UniteData.Color> _mapData;
}