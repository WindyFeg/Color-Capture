using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace CoCa.Map
{
    public partial struct MapData : IComponentData
    {
        public NativeArray<UniteData.Color> _mapData;
    }
}
