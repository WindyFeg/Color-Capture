using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UniteData;
using CoCa.Map;
using CoCa.Ai;

namespace CoCa.Player
{
    public partial struct PlayerMovementSystem : ISystem
    {
        private bool _twoPlayerMode;
        private bool _playerTurn;
        private Entity _currentPlayer;
        private Direction _direction;
        private PlayerMovement _playerMovementComponent;
        private CoCa.Map.Map _map;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMovement>();
            _playerTurn = true;

        }

        public void OnUpdate(ref SystemState state)
        {
            #region Variables
            _map = SystemAPI.GetSingleton<CoCa.Map.Map>();
            EntityQuery mainPlayerEq = state.GetEntityQuery(typeof(MainPlayer));
            EntityQuery enemyPlayerEq = state.GetEntityQuery(typeof(OtherPlayer));
            _twoPlayerMode = SystemAPI.GetSingleton<CoCa.Map.Map>().TwoPlayerMode;
            #endregion

            #region Turn base movement
            if (_playerTurn)
            {
                //* Get current player turn
                _currentPlayer = mainPlayerEq.GetSingletonEntity();
                _playerMovementComponent = state.EntityManager.
            GetComponentData<PlayerMovement>(_currentPlayer);

                GetMoveInput(ref state);
                if (CheckLegitMove(ref state, _currentPlayer))
                {
                    Move(ref state, _currentPlayer);
                    _playerTurn = false;
                }
            }
            else
            {
                //* Get current player turn
                _currentPlayer = enemyPlayerEq.GetSingletonEntity();
                _playerMovementComponent = state.EntityManager.
            GetComponentData<PlayerMovement>(_currentPlayer);

                if (_twoPlayerMode)
                {
                    //* Player VS Player
                    GetMoveInput(ref state);
                    if (CheckLegitMove(ref state, _currentPlayer))
                    {
                        Move(ref state, _currentPlayer);
                        _playerTurn = true;
                    }
                }
                else
                {
                    //* Player VS AI
                    EnemyAutoSetLegitMove(ref state, _currentPlayer);
                    Move(ref state, _currentPlayer);
                    _playerTurn = true;
                }
            }

            UpdatePlayerPosition(ref state);
            #endregion
        }



        //* Set legit next move on (Entity) base on _playerMovementComponent
        private void EnemyAutoSetLegitMove(ref SystemState state, Entity enemyPlayer)
        {
            NativeArray<UniteData.Color> mapData = SystemAPI.GetSingleton<MapData>()._mapData;
            EntityQuery enemyQuery = state.GetEntityQuery(typeof(OtherPlayer));
            Entity enemyEntity = enemyQuery.GetSingletonEntity();

            //$ AI
            // var aiMap = SystemAPI.GetSingleton<AiMap>()._aiMapData;

            var aiMove = UniteData.Direction.Left;
            SetPlayerMoveDirection(aiMove);

        }

        //* Set position of (Entity) base on _playerMovementComponent
        private void SetPlayerMoveDirection(UniteData.Direction direction)
        {
            switch (direction)
            {
                case UniteData.Direction.Up:
                    _playerMovementComponent.playerMovementPosition += 1;
                    break;
                case UniteData.Direction.Left:
                    _playerMovementComponent.playerMovementPosition -= _map.mapHeight;
                    break;
                case UniteData.Direction.Down:
                    _playerMovementComponent.playerMovementPosition -= 1;
                    break;
                case UniteData.Direction.Right:
                    _playerMovementComponent.playerMovementPosition += _map.mapHeight;
                    break;
            }
        }

        //* Wait User get input, update next move to _playerMovementComponent
        private void GetMoveInput(ref SystemState state)
        {
            #region Player Input
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _direction = Direction.Up;
                SetPlayerMoveDirection(_direction);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _direction = Direction.Left;
                SetPlayerMoveDirection(_direction);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _direction = Direction.Down;
                SetPlayerMoveDirection(_direction);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _direction = Direction.Right;
                SetPlayerMoveDirection(_direction);
            }
            #endregion

            return;
        }

        //* Check legit next move on (Entity) base on _playerMovementComponent
        private bool CheckLegitMove(ref SystemState state, Entity player)
        {
            var previousPosition = state.EntityManager.
            GetComponentData<PlayerMovement>(player);
            var mapData = SystemAPI.GetSingleton<MapData>()._mapData;
            var rangeOfBlock = _map.mapHeight * _map.mapWidth;
            var nextMovePosition = _playerMovementComponent.playerMovementPosition;
            var curMovePosition = previousPosition.playerMovementPosition;

            if (nextMovePosition >= rangeOfBlock || nextMovePosition < 0)
                return false;

            var curRow = curMovePosition % _map.mapHeight;
            var nextRow = nextMovePosition % _map.mapHeight;

            if ((curRow == (_map.mapHeight - 1) && nextRow == 0) || (curRow == 0 && nextRow == (_map.mapHeight - 1)))
                return false;

            return mapData[nextMovePosition] == UniteData.Color.Empty;
        }

        //* Apply movement to (Entity) base on _playerMovementComponent 
        private Direction Move(ref SystemState state, Entity mainPlayer)
        {
            state.EntityManager.SetComponentData(mainPlayer, _playerMovementComponent);
            return _direction;
        }

        //* Set position all player to their variable playerPosition
        private void UpdatePlayerPosition(ref SystemState state)
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