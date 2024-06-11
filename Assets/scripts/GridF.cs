using System.Collections.Generic;
using Components;
using UnityEngine;

public static class GridF
    {
        private const int MatchOffset = 2;

        public static  void GetSpawnableColors(this Tile[,] grid, Vector2Int coord, List<int> results)
        {
            if (grid == null || results == null)
            {
                Debug.LogError("Grid or result list is null.");
                return;
            }
            
            int lastPrefabID = -1;
            int lastIDcounter = 0;

            int leftMax = coord.x - MatchOffset;
            int rightMax = coord.x + MatchOffset;
            // burada sağa ve sola gitmesi için tanımla yaptık 2 sola 2 sağa gitmesi için.


            leftMax = ClampInsideGrid(leftMax, grid.GetLength(0));
            rightMax = ClampInsideGrid(rightMax, grid.GetLength(0));
            // burada ClampMetod cağırıyoruz. grid array olduğu için x birinci index numarsı 0.
            // Grid.getLength(0) burada grid in 0 ıncı indexi uzunluğu kadar alıyor.

            for (int x = leftMax;x <= rightMax; x++)
            {
                Tile currTile = grid[x, coord.y];
                // currtTile Tile classından alıyor. 

                if (currTile == null)
                {
                    lastIDcounter = 0;
                    lastPrefabID = -1;

                    continue;
                }

                if (lastPrefabID == -1)
                {
                    lastPrefabID = currTile.ID;
                    lastIDcounter = 1; // Yeni tile için sayaıcı 1 e sıfırla.
                }
                else if (lastPrefabID == currTile.ID)
                {
                    lastIDcounter++;
                }
                else
                {
                    lastPrefabID = currTile.ID;
                    lastIDcounter = 1; 
                }

                if (lastPrefabID == MatchOffset) results.Remove(lastPrefabID);

            }

            lastPrefabID = -1;
            lastIDcounter = 0;

            int botMax = coord.y - MatchOffset;
            int topMax = coord.y + MatchOffset;

            botMax = ClampInsideGrid(botMax, grid.GetLength(1));
            topMax = ClampInsideGrid(topMax, grid.GetLength(1));
            // burada ClampMetod cağırıyoruz. grid array olduğu için y birinci index numarsı 1.


            for (int y = botMax; y <= topMax; y++)
            {
                Tile currtTile = grid[coord.x, y];
                // currtTile Tile classından alıyor.

                if (currtTile == null)
                {
                    lastIDcounter = 0;
                    lastPrefabID = -1;
                }

                if (lastPrefabID == -1)
                {
                    lastPrefabID = currtTile.ID;
                    lastIDcounter = 1; // Sayıcı başlatmak için 1 yapıyoruz
                }
                else if (lastPrefabID == currtTile.ID)
                {
                    lastIDcounter++;
                }
                else
                {
                    lastPrefabID = currtTile.ID;
                    lastIDcounter = 1; // yeni tile id için 1 e sıfırlıyoruz
                }

                if (lastIDcounter == MatchOffset) results.Remove(lastPrefabID);
        

            }
        }

        public static bool HasMatchesRight(this Tile[,] grid, Vector2Int abstractCoord, int prefabId)
        {

            if (grid.IsInsideGrid(abstractCoord))
            {
                
            }

            return default;
        }

        public static bool HasMatchRight(this Tile[,] grid, Vector2Int coord, int prefabId)
        {
            int rightMax = coord.x + MatchOffset;
            // burada coord'tan x ile matchOffset toplamını rightMax e atıyoruz.
            rightMax = ClampInsideGrid(rightMax, grid.GetLength(0));
            // burada Clamp metod cağırıyoruz değişken veriyoruyz min ve maks değer arasında tam sayayı döndürecek
            // min ve maksı grid array olduğu için 0 indexli elemanından alacak.

            int matchCounter = 0;

            for (int x = coord.x; x <= rightMax; x++)
            {
                if (grid[x, coord.y].ID == prefabId)
                {
                    matchCounter++;
                }
                else
                {
                    matchCounter = 0;
                }

                if (matchCounter == 3)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HastMatchLeft(this Tile[,] grid, Vector2Int coord, int prefabId)
        {
            int leftmax = coord.x - MatchOffset;
            leftmax = ClampInsideGrid(leftmax, grid.GetLength(0));

            int matchCoutner = 0;

            for (int x = leftmax; x <= coord.x; x++)
            {
                if (grid[x, coord.y].ID == prefabId)
                {
                    matchCoutner++;
                }
                else
                {
                    matchCoutner = 0;
                }

                if (matchCoutner == 3)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HastMatchTop(this Tile[,] grid, Vector2Int coord, int prefabId)
        {
            int topMax = coord.y + MatchOffset;
            topMax = ClampInsideGrid(topMax, grid.GetLength(1));
            
            int matchCounter = 0;

            for (int y = topMax; y <= coord.y; y++)
            {
                if (grid[coord.x,y].ID == prefabId)
                {
                    matchCounter++;
                }
                else
                {
                    matchCounter = 0;
                }

                if (matchCounter == 3)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool HastMatchBot(this Tile[,] grid, Vector2Int coord, int prefabId)
        {
            int botMax = coord.y + MatchOffset;
            botMax = ClampInsideGrid(botMax, grid.GetLength(1));

             int matchCounter = 0;

             for (int y = botMax; y <= coord.y; y++)
             {
                 if (grid[coord.x,y].ID == prefabId)
                 {
                     matchCounter++;
                 }
                 else
                 {
                     matchCounter = 0;
                 }

                 if (matchCounter == 3)
                 {
                     return true;
                 }
             }

             return false;
        }


        private static int ClampInsideGrid(int value, int gridSize)                 
            {
            return Mathf.Clamp(value, 0, gridSize - 1);
            // Clamp = Verilen değeri, minimum tam sayı ve maksimum tam sayı değerleri tanımlanan aralık arasına 
            // Sıkıştırır Min ve max arasında ise verilen değeri döndürür.
            }           
        
        private static bool IsInsideGrid(this Tile[,] grid, int axis, int axisIndex)
        {
            int min = 0;
            int max = grid.GetLength(axisIndex);
            // burada grid array olduğu için eksenin İndeksini max olarak alıyor.
            
            return axis >=0 && axis < max;
            // burada axis 0 ve max arasında gelecek
        }

        private static bool IsInsideGrid(this Tile[,] grid, Vector2Int coord)
        {
            return grid.IsInsideGrid(coord.x, 0) && grid.IsInsideGrid(coord.y, 1);
        }
    }


