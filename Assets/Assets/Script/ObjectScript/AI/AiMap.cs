using Unity.Collections;
using Unity.Entities;

namespace CoCa.Ai
{
    public partial struct AiMap : IComponentData
    {
        public NativeArray<int> _aiMapData;

        
    }
}