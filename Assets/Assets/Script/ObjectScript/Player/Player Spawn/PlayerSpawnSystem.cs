
using System;
using Unity.Collections;
using Unity.Entities;
using CoCa;

namespace CoCa.Player
{
    public partial struct PlayerSpawnSystem : ISystem
    {
        private bool IsSpawned;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CoCa.Map.Map>();
            IsSpawned = false;
        }

        public void OnUpdate(ref SystemState state)
        {
            if (IsSpawned == false)
            {
                SpawnPlayer(ref state);
                IsSpawned = true;
            }
        }

        private void SpawnPlayer(ref SystemState state)
        {
            #region Variables
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var playerPrefab = SystemAPI.GetSingleton<PlayerSpawn>().playerPrefab;
            var map = SystemAPI.GetSingleton<CoCa.Map.Map>();

            var greenPlayer = ecb.Instantiate(playerPrefab);
            var redPlayer = ecb.Instantiate(playerPrefab);
            #endregion

            #region Spawn Player
            ecb.AddComponent(greenPlayer, new PlayerMovement
            {
                playerColor = UniteData.Color.Green,
                playerMovementPosition = 0
            });
            ecb.AddComponent(greenPlayer, new MainPlayer { });

            ecb.AddComponent(redPlayer, new PlayerMovement
            {
                playerColor = UniteData.Color.Red,
                playerMovementPosition = map.mapHeight * map.mapWidth - 1
            });
            #endregion

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}