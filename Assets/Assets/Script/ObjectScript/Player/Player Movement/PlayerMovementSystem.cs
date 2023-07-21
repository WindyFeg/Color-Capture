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
                #region Player 1
                //* Get current player turn
                _currentPlayer = mainPlayerEq.GetSingletonEntity();
                _playerMovementComponent = state.EntityManager.
            GetComponentData<PlayerMovement>(_currentPlayer);

                GetMoveInput(ref state);
                if (CheckLegitMove(ref state, _currentPlayer, _playerMovementComponent.playerMovementPosition))
                {
                    Move(ref state, _currentPlayer);
                    _playerTurn = false;

                    ReAiScore();
                }
                #endregion
            }
            else
            {
                #region Player 2
                //* Get current player turn
                _currentPlayer = enemyPlayerEq.GetSingletonEntity();
                _playerMovementComponent = state.EntityManager.
            GetComponentData<PlayerMovement>(_currentPlayer);

                if (_twoPlayerMode)
                {
                    //* Player VS Player
                    GetMoveInput(ref state);
                    if (CheckLegitMove(ref state, _currentPlayer, _playerMovementComponent.playerMovementPosition))
                    {
                        Move(ref state, _currentPlayer);
                        _playerTurn = true;
                    }
                    #endregion
                }
                else
                {
                    #region AI
                    //* Player VS AI
                    EnemyAutoSetLegitMove(ref state, _currentPlayer);
                    if (CheckLegitMove(ref state, _currentPlayer, _playerMovementComponent.playerMovementPosition))
                    {
                        Move(ref state, _currentPlayer);
                        _playerTurn = true;
                    }
                    else
                    {
                        state.Enabled = false;
                    }
                    #endregion
                }
            }

            UpdatePlayerPosition(ref state);
            #endregion
        }

        private void ReAiScore()
        {
            var aiMap = SystemAPI.GetSingleton<AiMap>();
            var ai = SystemAPI.GetSingleton<Ai.Ai>();
            var aiMapData = aiMap._aiMapData;
            int stuckSpot = 0;
            int wallOrColored = 0;

            for (int i = 0; i < aiMapData.Length; i++)
            {
                //* We Don't ReScore Wall Or Colored block
                if (aiMapData[i] == 0 || aiMapData[i] == -1)
                {
                    continue;
                }

                wallOrColored = 0;
                var upId = Functions.Functions.GetUpId(i, _map.mapWidth, _map.mapHeight);
                if (upId > 0)
                {
                    if (aiMapData[upId] == 0 || aiMapData[upId] == -1)
                    {
                        wallOrColored++;
                    }
                }

                var downId = Functions.Functions.GetDownId(i, _map.mapWidth, _map.mapHeight);
                if (downId > 0)
                {
                    if (aiMapData[downId] == 0 || aiMapData[downId] == -1)
                    {
                        wallOrColored++;
                    }
                }

                var leftId = Functions.Functions.GetLeftId(i, _map.mapWidth, _map.mapHeight);
                if (leftId > 0)
                {
                    if (aiMapData[leftId] == 0 || aiMapData[leftId] == -1)
                    {
                        wallOrColored++;
                    }
                }

                var rightId = Functions.Functions.GetRightId(i, _map.mapWidth, _map.mapHeight);
                if (rightId > 0)
                {
                    if (aiMapData[rightId] == 0 || aiMapData[rightId] == -1)
                    {
                        wallOrColored++;
                    }
                }

                if (wallOrColored >= 3)
                {
                    aiMapData[i] = ai._stuckSpot;
                    stuckSpot++;
                    // Debug.Log("Stuck spot position" + i);
                    // Debug.Log(upId + "<-up " + downId + "<-down " + leftId + "<-left " + rightId + "<-right ");
                }

            }
            Debug.Log("Stuck spot " + stuckSpot);
            SystemAPI.SetSingleton<AiMap>(aiMap);
        }



        //* Set legit next move on (Entity) base on _playerMovementComponent
        private void EnemyAutoSetLegitMove(ref SystemState state, Entity enemyPlayer)
        {
            NativeArray<UniteData.Color> mapData = SystemAPI.GetSingleton<MapData>()._mapData;
            EntityQuery enemyQuery = state.GetEntityQuery(typeof(OtherPlayer));
            Entity enemyEntity = enemyQuery.GetSingletonEntity();

            //$ AI
            var aiMap = SystemAPI.GetSingleton<AiMap>();
            var aiMapData = aiMap._aiMapData;
            var currentAiPlayerPosition = Functions.Functions.GetAiPlayerPosition(ref state);
            var optimalMove = 0;
            var aiMove = UniteData.Direction.Left;

            #region Check score around to Find optimal route 
            var upAiPlayerScore = Functions.Functions.GetUpId(currentAiPlayerPosition, _map.mapWidth, _map.mapHeight);
            if (upAiPlayerScore > 0 && optimalMove < aiMapData[upAiPlayerScore])
            {
                optimalMove = aiMapData[upAiPlayerScore];
                aiMove = UniteData.Direction.Up;
                Debug.Log("ai up score:" + aiMapData[upAiPlayerScore] + "at position :" + upAiPlayerScore);
            }
            // Debug.Log("ai up" + upAiPlayerScore);


            var leftAiPlayerScore = Functions.Functions.GetLeftId(currentAiPlayerPosition, _map.mapWidth, _map.mapHeight);
            if (leftAiPlayerScore > 0 && optimalMove < aiMapData[leftAiPlayerScore])
            {
                optimalMove = aiMapData[leftAiPlayerScore];
                aiMove = UniteData.Direction.Left;
                Debug.Log("ai left score:" + aiMapData[leftAiPlayerScore] + "at position :" + leftAiPlayerScore);
            }
            // Debug.Log("ai left" + leftAiPlayerScore);


            var downAiPlayerScore = Functions.Functions.GetDownId(currentAiPlayerPosition, _map.mapWidth, _map.mapHeight);
            if (downAiPlayerScore > 0 && optimalMove < aiMapData[downAiPlayerScore])
            {
                optimalMove = aiMapData[downAiPlayerScore];
                aiMove = UniteData.Direction.Down;
                Debug.Log("ai down score:" + aiMapData[downAiPlayerScore] + "at position :" + downAiPlayerScore);
            }
            // Debug.Log("ai down" + downAiPlayerScore);


            var rightAiPlayerScore = Functions.Functions.GetRightId(currentAiPlayerPosition, _map.mapWidth, _map.mapHeight);
            if (rightAiPlayerScore > 0 && optimalMove < aiMapData[rightAiPlayerScore])
            {
                optimalMove = aiMapData[rightAiPlayerScore];
                aiMove = UniteData.Direction.Right;
                Debug.Log("ai right score:" + aiMapData[rightAiPlayerScore] + "at position :" + rightAiPlayerScore);
            }
            // Debug.Log("ai right" + rightAiPlayerScore);
            Debug.Log("curentposition" + currentAiPlayerPosition + "AI MOVE " + aiMove);

            #endregion
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
        private bool CheckLegitMove(ref SystemState state, Entity player, int nextMovePosition)
        {
            var previousPosition = state.EntityManager.
            GetComponentData<PlayerMovement>(player);
            var mapData = SystemAPI.GetSingleton<MapData>()._mapData;
            var rangeOfBlock = _map.mapHeight * _map.mapWidth;

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
        private Direction Move(ref SystemState state, Entity currentPlayer)
        {
            state.EntityManager.SetComponentData(currentPlayer, _playerMovementComponent);

            #region Update AiMapData
            var aiMap = SystemAPI.GetSingleton<AiMap>();

            Debug.Log("move " + _playerMovementComponent.playerMovementPosition);
            Debug.Log("------------------------------- ");
            aiMap._aiMapData[_playerMovementComponent.playerMovementPosition] = -1;
            SystemAPI.SetSingleton<AiMap>(aiMap);
            #endregion

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