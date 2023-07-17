using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace CoCa.Player
{
    public class PlayerSpawnAuthoring : MonoBehaviour
    {
        public GameObject _playerPrefab;
    }
    public partial struct PlayerSpawn : IComponentData
    {
        public Entity playerPrefab { get; set; }
    }
    public class PlayerSpawnBaker : Baker<PlayerSpawnAuthoring>
    {
        public override void Bake(PlayerSpawnAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerSpawn
            {
                playerPrefab = GetEntity(authoring._playerPrefab)
            });
        }
    }

}
