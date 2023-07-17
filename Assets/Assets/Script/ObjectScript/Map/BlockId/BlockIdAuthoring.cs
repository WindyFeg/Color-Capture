using Unity.Entities;

namespace CoCa.MapBlock
{
    public partial struct BlockId : IComponentData
    {
        public int blockId { get; set; }
    }
}
