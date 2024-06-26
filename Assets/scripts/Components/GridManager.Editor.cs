using System;
using System.Collections.Generic;
using Extensions.System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.VersionControl;
#endif
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;






namespace Components
{ 
    public partial  class GridManager 
    {
#if UNITY_EDITOR
        
        
      private Tile DrawTile(Rect rect,Tile tile)
        {
           Texture2D preview = AssetPreview.GetAssetPreview(tile.gameObject);
           
          //  Texture2D Icon =PrefabUtility.GetIconForGameObject(tile.gameObject);

            rect = rect.Padding(3);
            EditorGUI.DrawPreviewTexture(rect, preview);
            return tile;
        }
     
        private void OnDrawGizmos()
        {
            if(_currMatchesDebug == null)return;

            if (_currMatchesDebug.Count == 0) return;
            {
                
            }
            
            Gizmos.color = Color.blue;
            
            foreach (Tile tile in _currMatchesDebug)
            {
                Gizmos.DrawWireCube(tile.transform.position,Vector3.one);
            }
        }

        [Button]
        private void CalculateBounds()
        {
            _gridBounds = new Bounds();
            
            foreach (Tile tile in _grid)
            {
                Bounds spriteBounds = tile.GetComponent<SpriteRenderer>().bounds;
                _gridBounds.Encapsulate(spriteBounds);
            }
        }
        
        
        [Button]
        private void CreateGrid(int sizeX, int sizeY)
        {


            _grid = new Tile[_gridSizeX, _gridSizeY]; // burada Tile yaratıyoruz.

            _prefabIds = new(); // Yukarıda tanımladığımız bu değişken verileri int tipinde listede tutacak

            for (int id = 0; id < _tilePrefabs.Count; id++)
                _prefabIds.Add(
                    id); // burada for ile tilePrefabs elemanları dönüyoruz ve daha sonra _prefabIds ye ekliyoruz.

            _gridSizeX = sizeX; // yukarıda tanımladığımız değişkenler burada ki parametlerden alacaklar.
            _gridSizeY = sizeY;

         

            _grid = new Tile [_gridSizeX, _gridSizeY];

            for (int x = 0; x < _gridSizeX; x++)
            for (int y = 0; y < _gridSizeY; y++)
            {
                List<int> spawnableIds = new(_prefabIds);
                Vector2Int
                    coord = new(x,
                        _gridSizeY - y - 1); // grid alt üst etmek için İnspectorda görünen gibi göstermiyor Scende.
                Vector3 pos = new(coord.x, coord.y, 0f); // Vector3 pos değişkenine yeni x.y.z yi atıyoruz.

                _grid.GetSpawnableColors(coord, spawnableIds);

                int randomId = spawnableIds.Random();

                GameObject tilePrefabRandom = _tilePrefabs[randomId];
                GameObject tileNew = PrefabUtility.InstantiatePrefab(tilePrefabRandom,transform)as GameObject; // Random Prefab Instatiate
                tileNew.transform.position = pos;

                

                Tile tile = tileNew.GetComponent<Tile>();
                tile.Construct(coord);

                _grid[coord.x, coord.y] = tile; // Becarefull while assigning tile to inversed y coordinates!
            }
            CalculateBounds();
        }
#endif
    }
}

