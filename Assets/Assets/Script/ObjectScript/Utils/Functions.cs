

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

        public static int[,] CreatePattern(int size)
        {
            int[,] pattern = new int[size, size];
            int mid = size / 2;

            for (int i = 0; i <= mid; i++)
            {
                for (int j = i; j < size - i; j++)
                {
                    pattern[i, j] = pattern[j, i] = pattern[size - 1 - i, j] = pattern[j, size - 1 - i] = i + 1;
                }
            }

            return pattern;
        }

        public static int[] FlattenArray(int[,] array2D)
        {
            int rows = array2D.GetLength(0);
            int columns = array2D.GetLength(1);
            int[] array1D = new int[rows * columns];

            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    array1D[index] = array2D[i, j];
                    index++;
                }
            }

            return array1D;
        }
    }
}