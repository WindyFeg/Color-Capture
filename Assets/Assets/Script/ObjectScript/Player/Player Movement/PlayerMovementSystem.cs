
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace CoCa.Player
{
    public partial struct PlayerMovementSystem : ISystem
    {
        private bool _playerTurn;
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMovement>();
            _playerTurn = true;
        }

        public void OnUpdate(ref SystemState state)
        {
            SetPlayerPosition(ref state);

            if (_playerTurn)
            {
                // map, playerMovementPosition
                Move(ref state);
            }
        }

        //* Wait User, Check valid move
        private void Move(ref SystemState state)
        {
            EntityQuery mainPlayerEq = state.GetEntityQuery(typeof(MainPlayer));
            Entity mainPlayer = mainPlayerEq.GetSingletonEntity();

            PlayerMovement playerComponent = state.EntityManager.
            GetComponentData<PlayerMovement>(mainPlayer);
            if (Input.GetKeyDown(KeyCode.W))
            {
                playerComponent.playerMovementPosition += 1;
            }
            state.EntityManager.SetComponentData(mainPlayer, playerComponent);
        }

        private void SetPlayerPosition(ref SystemState state)
        {
            var map = SystemAPI.GetSingleton<CoCa.Map.Map>();

            foreach (var (player, transform) in SystemAPI.Query<RefRO<PlayerMovement>, RefRW<LocalTransform>>())
            {
                var playerOnBlockId = player.ValueRO.playerMovementPosition;
                transform.ValueRW.Position = new Unity.Mathematics.float3
                {
                    x = playerOnBlockId / map.mapHeight,
                    y = playerOnBlockId % map.mapWidth,
                    z = -1
                };
            }
        }
    }
}