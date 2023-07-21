

using CoCa.Ai;
using CoCa.Player;
using Unity.Collections;
using Unity.Entities;

namespace CoCa.Functions
{
    public static class Functions
    {
        public static int GetUpId(int id, int mapWidth, int mapHeight)
        {
            var x = id % mapHeight;
            if (x < mapHeight - 1)
            {
                return id + 1;
            }
            else
            {
                return -1;
            }
        }

        public static int GetDownId(int id, int mapWidth, int mapHeight)
        {
            var x = id % mapHeight;
            if (x > 0 && x < mapHeight)
            {
                return id - 1;
            }
            else
            {
                return -1;
            }
        }

        public static int GetLeftId(int id, int mapWidth, int mapHeight)
        {
            //3 6 9
            //2 5 8
            //1 4 7
            var y = id / mapWidth;
            if (y > 0 && y < mapWidth)
            {
                return id - mapWidth;
            }
            else
            {
                return -1;
            }
        }

        public static int GetRightId(int id, int mapWidth, int mapHeight)
        {
            //3 6 9
            //2 5 8
            //1 4 7
            int y = id / mapWidth;
            if (y < (mapWidth - 1))
            {
                return id + mapWidth;
            }
            else
            {
                return -1;
            }
        }

        public static int GetMainPlayerPosition(ref SystemState state)
        {
            EntityQuery mainPlayerEQ = state.GetEntityQuery(typeof(MainPlayer));
            Entity playerEntity = mainPlayerEQ.GetSingletonEntity();
            var playerMovementPosition = state.EntityManager.GetComponentData<PlayerMovement>(playerEntity);

            return playerMovementPosition.playerMovementPosition;
        }

        public static int GetAiPlayerPosition(ref SystemState state)
        {
            EntityQuery mainPlayerEQ = state.GetEntityQuery(typeof(OtherPlayer));
            Entity playerEntity = mainPlayerEQ.GetSingletonEntity();
            var playerMovementPosition = state.EntityManager.GetComponentData<PlayerMovement>(playerEntity);

            return playerMovementPosition.playerMovementPosition;
        }

        //     public static NativeArray<int> GetAiMap()
        //     {
        //         return SystemAPI.GetSingleton<AiMap>()._aiMapData;
        //     }
    }
}