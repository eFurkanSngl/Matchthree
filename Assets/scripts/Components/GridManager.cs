using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEngine;





namespace Components
{
    public class GridManager: SerializedMonoBehaviour
    {
        [BoxGroup(Order = 999)]
        [TableMatrix(SquareCells = true) /*(DrawElementMethod = nameof(DrawTile))*/, OdinSerialize]
        private Tile[,] _grid;
        
        [SerializeField] private List<GameObject> _tilePrefabs;
        private int _gridSizeX;
        private int _girdSizeY;
        
        private Tile DrawTile(Rect rect,Tile tile)
        {
            UnityEditor.EditorGUI.DrawRect(rect, Color.gray);
            return tile;
        }

        [Button]
        private void CreateGrid(int sizeX, int sizeY)
        {

            _gridSizeX = sizeX;
           _girdSizeY = sizeY;
                
            if (_grid != null)
            {   
                foreach (Tile o in _grid)
                {
                    DestroyImmediate(o.GameObject());
                    // Editörde silmek istersek DestroyImmediate şart.
                }
            }
            _grid = new Tile[_gridSizeX, _girdSizeY];

            for (int x = 0; x < _gridSizeX; x++)
            for (int y = 0; y < _girdSizeY; y++)
            {
                Tile tile = _grid[x, y];

                Vector2Int coord = new Vector2Int(x, _girdSizeY - y - 1);
                Vector3 pos = new(coord.x, coord.y,0f);
                int randomIndex = Random.Range(0, _tilePrefabs.Count); 
                GameObject tilePrefabsRandom = _tilePrefabs[randomIndex];
                GameObject tileNew = Instantiate(tilePrefabsRandom, pos, Quaternion.identity);
                tile = tileNew.GetComponent<Tile>();
                tile.Consturct(coord);
               _grid[x, y] = tile;
            }
        }
        
         
        /*[Button(ButtonSizes.Large)]
        private void InitializeGird()
        {
            Debug.LogWarning(_grid.GetLength(0)); 
            Debug.LogWarning(_grid.GetLength(1));

            int gridSizeX = _grid.GetLength(0);
            int gridSizeY = _grid.GetLength(1);

            for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
            
            {
                Debug.LogWarning($"{x},{y}");
                
                Tile tile = _grid[x ,gridSizeY - y - 1]; Burada ekrana gelen poziyonlarını ayarladık
                if (tile==null)
                {  
                    continue;
                }

                Vector3 pos = new(x, y, 0f);
                Instantiate(tile, pos,tile.transform.rotation); */
            }
    
        
            
        }
    


