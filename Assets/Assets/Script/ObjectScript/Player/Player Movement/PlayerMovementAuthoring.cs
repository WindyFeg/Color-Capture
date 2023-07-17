using Unity.Entities;
using UnityEngine;


namespace CoCa.Player
{
    public class PlayerMovementAuthoring : MonoBehaviour
    {
        public UniteData.Color _playerColor = UniteData.Color.Empty;
        public int _playerMovementPosition = 0;
    }
    public partial struct PlayerMovement : IComponentData
    {
        public UniteData.Color playerColor;
        public int playerMovementPosition;
    }


    public class PlayerMovementBaker : Baker<PlayerMovementAuthoring>
    {
        public override void Bake(PlayerMovementAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerMovement
            {
                playerColor = authoring._playerColor,
                playerMovementPosition = authoring._playerMovementPosition
            });
        }
    }
}