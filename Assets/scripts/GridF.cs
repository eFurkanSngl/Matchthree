using System;
using System.Collections.Generic;
using Components;
using Extensions.System;
using OpenCover.Framework.Model;
using UnityEngine;

public static class GridF
    {
        private const int MatchOffset = 2;

        public static  void GetSpawnableColors(this Tile[,] grid, Vector2Int coord, List<int> results)
        {  
           
            
           /* int lastPrefabID = -1;
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
                    continue;
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
            */
        }
        public static List<Tile> GetMatchesX(this Tile[,] thisGrid, Tile tile) =>
            GetMatchesX(thisGrid, tile.Coord, tile.ID);

        public static List<Tile> GetMatchesX(this Tile[,] grid, Vector2Int coord, int prefabId)
        {
            Vector2Int prevCoord = coord;
            Tile thisTile = grid.Get(coord);
            List<Tile> matches = new();

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                Tile currTile = grid[x, coord.y];

                if (currTile.ID == prefabId)
                {
                    matches.Add(currTile);
                }
                //Eğer currTile id ile prefabId aynı ise eşleşmeye currTile ı ekle

                else if (matches.Contains(thisTile) == false)
                {
                    matches.Clear();
                }
                // burada eşleşen Contains da yoksa yani buTile  eşleşmeyi Sil
                else if (matches.Contains(thisTile))
                {
                    break;
                }
                // burada Matches grubunda thisTile varsa break diyip cıkıyoruz.
            }
            return matches; 
            // Buradaki fonksiyonun amacı : Grid boyunda spawnlanan renklere bize ( Thistile) eşit mi değil mi
            // eşleşmeleri matches a ekliyruz
            // Eşleşmede ThisTile yoksa clearlıyor. Eğer yeni eşleşme de bizim tile varsa ve ondan sonra farklı eşleşme gelirse quit atıyoruz.
            // çünkü matches grubunda benim Thistile'ım onlarla ile beraber.
        }

        public static List<Tile> GetMatchesY(this Tile[,] thisGrid, Tile tile) =>
            GetMatchesY(thisGrid, tile.Coord, tile.ID);
            
            
        public static List<Tile> GetMatchesY(this Tile[,] grid, Vector2Int coord, int prefabId)
        {
            Tile thisTile = grid.Get(coord);
            List<Tile> matches = new();

            for (int y = 0; y < grid.GetLength(1); y++)
            {
                Tile currTile = grid[coord.x ,y];

                if (currTile.ID == prefabId)
                {
                    matches.Add(currTile);
                }
                //Eğer currTile id ile prefabId aynı ise eşleşmeye currTile ı ekle

                else if (matches.Contains(thisTile) == false)
                {
                    matches.Clear();
                }
                // burada eşleşen Contains da yoksa yani buTile  eşleşmeyi Sil
                else if (matches.Contains(thisTile))
                {
                    break;
                }
                // burada Matches grubunda thisTile varsa break diyip cıkıyoruz.
            }
            return matches; 
            // Buradaki fonksiyonun amacı : Grid boyunda spawnlanan renklere bize ( Thistile) eşit mi değil mi
            // eşleşmeleri matches a ekliyruz
            // Eşleşmede ThisTile yoksa clearlıyor. Eğer yeni eşleşme de bizim tile varsa ve ondan sonra farklı eşleşme gelirse quit atıyoruz.
            // çünkü matches grubunda benim Thistile'ım onlarla ile beraber.
        }
        
        /*
        private static bool HastMatchTop(this Tile[,] grid, Vector2Int coord, int prefabId)
        {
            int topMax = coord.y + MatchOffset;
            // burada coord'tan y ile matchOffset toplamını toptMax e atıyoruz.

            topMax = ClampInsideGrid(topMax, grid.GetLength(1));
            // burada Clamp metod cağırıyoruz değişken veriyoruyz min ve maks değer arasında tam sayayı döndürecek

            
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
        */


        private static int ClampInsideGrid(int value, int gridSize)                 
            {
            return Mathf.Clamp(value, 0, gridSize - 1);
            // Clamp = Verilen değeri, minimum tam sayı ve maksimum tam sayı değerleri tanımlanan aralık arasına 
            // Sıkıştırır Min ve max arasında ise verilen değeri döndürür.
            }           
        
        public static bool IsInsideGrid(this Tile[,] grid, int axis, int axisIndex)
        {
            int min = 0;
            int max = grid.GetLength(axisIndex);
            // burada grid array olduğu için eksenin İndeksini max olarak alıyor.
            
            return axis >=min && axis < max;
            // burada axis 0 ve max arasında gelecek
        }

        public static bool IsInsideGrid(this Tile[,] grid, Vector2Int coord)
        {
            return grid.IsInsideGrid(coord.x, 0) && grid.IsInsideGrid(coord.y, 1);
        }

        public static GridDir GetGridDir(Vector3 input)
        {
            int maxAxis = 0;
            float maxAxisSign = input[0].Sign();
            float lastAxisLength = input[0].Abs();
            
            
            for (int axisIndex = 0; axisIndex < 3; axisIndex++)
            {
                float thisAxisLength = input[axisIndex];
                float thisAxisLengthAbs = thisAxisLength.Abs();

                if (thisAxisLength > thisAxisLengthAbs)
                {
                    lastAxisLength = thisAxisLength;
                    maxAxis = axisIndex;
                    maxAxisSign = thisAxisLength.Sign();
                }
            }

            return GetGridDir((maxAxis + 1) * maxAxisSign.CeilToInt()); //CeilToInt: Yuvarla ve Int tipine dön demek. 
        }
        
        public static Vector2Int GetGridDirVector(Vector3 input)
        {
            int maxAxis = 0;
            float maxAxisSign = input[0].Sign();
            float lastAxisLength = input[0].Abs();
            
            
            for (int axisIndex = 0; axisIndex < 3; axisIndex++)
            {
                float thisAxisLength = input[axisIndex];
                float thisAxisLengthAbs = thisAxisLength.Abs();

                if (thisAxisLength > thisAxisLengthAbs)
                {
                    lastAxisLength = thisAxisLength;
                    maxAxis = axisIndex;
                    maxAxisSign = thisAxisLength.Sign();
                }
            }

            return GetGridDir((maxAxis + 1) * maxAxisSign.CeilToInt()).ToVector(); //CeilToInt: Yuvarla ve Int tipine dön demek. 
        }
        
        /// <summary>
        /// Sıfır Olmayan Eksen Dizinini işaretiyle dönüştürme = acıklama
        /// </summary>
        /// <param name="axisSingIndex">Sıfırdan başlamamalı </param>  = parametre alması gereken
        /// <returns>Grid yön</returns> = döndürdüğü şeyin acıklaması 
       
        // Summary yukarıda verdiğimiz acıklama metinini fonksiyonun üstüne gelince bize gösterir.
        public static GridDir GetGridDir(int axisSingIndex)
        {
            switch (axisSingIndex)
            {
                case 1:
                    return GridDir.rigth;
                case 2:
                    return GridDir.up;
                case -1:
                    return GridDir.left;
                case -2:
                    return GridDir.down;
                default: return GridDir.Null;
            }
            
        }

        public static Vector2Int ToVector(this GridDir thisGridDir)
        {
            switch (thisGridDir)
            {
                case GridDir.Null : return Vector2Int.zero;
                case GridDir.left : return Vector2Int.left;
                case GridDir.rigth : return Vector2Int.right;
                case GridDir.up : return Vector2Int.up;
                case GridDir.down : return Vector2Int.down;
                default: throw new ArgumentException();
            }
        }


        public static Tile Get( this Tile[,] thisGrid, Vector2Int coord)
        {
            return thisGrid[coord.x, coord.y];
        }

        public static Tile Set(this Tile[,] thisGrid,Tile tileToSet, Vector2Int coord)
        {
            Tile tileAtCoord = thisGrid.Get(coord);

            thisGrid[coord.x, coord.y] = tileToSet;
            ICoordSet coordSet = tileToSet;
            coordSet.SetCoord(coord);
            
            return tileAtCoord;
         }

        public static void Switch(this Tile[,] thisGrid,Tile fromTile ,Vector2Int toCoord)
        {
            Vector2Int fromCoords = fromTile.Coord;
            
          Tile toTile = thisGrid.Set(fromTile,toCoord); 
          // parametre olan fromtile ve tocoordu fromCoords  a set ettik
          
          thisGrid.Set(toTile, fromCoords); 
        }

        public static void Switch(this Tile[,] thisGrid, Tile fromTile, Tile toTile)
        {
            Vector2Int fromCoords = fromTile.Coord;
            Vector2Int toCoords = toTile.Coord;

            thisGrid.Set(fromTile,toCoords);
           thisGrid.Set(toTile,fromCoords);
        }

    }
    

public enum GridDir
{
    Null,
    left,
    rigth,
    up,
    down
    
}


