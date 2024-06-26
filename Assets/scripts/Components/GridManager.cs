using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Extensions.System;
using Extensions.Unity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Zenject;


namespace Components
{
    public partial class GridManager: SerializedMonoBehaviour
    {
        private List<Tile> _currMatchesDebug;
        [Inject] private GridEvents GridEvents { get; set; }
        [Inject] private InputEvents InputEvents{get; set;}
        [BoxGroup(Order = -999)]
        [TableMatrix(SquareCells = true,DrawElementMethod = nameof(DrawTile)), OdinSerialize]
        private Tile[,] _grid;  // Tileden coklu array oluşuturuyoruz _gird adında.
        
        
        [SerializeField] private List<GameObject> _tilePrefabs; // Listede GameObject tipinde tutuyoruz
        
        private int _gridSizeX;  // 2 tane X ve y değişken tanımladık.
        private int _gridSizeY;
        [SerializeField] private List<int> _prefabIds;
        [SerializeField] private  Bounds _gridBounds;
        private Tile _selectedTile; 
        private Vector3 _mouseDownPos;
        private Vector3 _mouseUpPos;
      
        
        private void OnEnable()
        {
            RegisterEvents();
        }
        private void OnDisable()
        {
            UnRegisterEvents();
        }

        private void Start()
        {
            GridEvents.GridLoaded?.Invoke(_gridBounds);
        }

        private void RegisterEvents()
        {
              InputEvents.MouseDownGrid += OnMouseDownGrid;
            InputEvents.MouseUpGrid += OnMouseUpGrid;
        }

        private void OnMouseUpGrid(Vector3 arg0)
        {
            _mouseUpPos = arg0;

            Vector3 dirVector = arg0 - _mouseDownPos;
            
            if (_selectedTile)
            {
                bool canMove = CanMove(_selectedTile, dirVector, out List < Tile > matches);
                Debug.LogWarning($"{canMove} CanMove,{matches.Count} macthes Count");
                if(! canMove) return;

                matches.Select(e => e.GetComponent<SpriteRenderer>().color = Color.black);

                _currMatchesDebug = matches;
                
                Debug.DrawLine(_mouseDownPos,_mouseUpPos,Color.blue,2F);
            }
        }
        
        
        
        [Button]
        private void TestGridDir(Vector2 input)
        {
            Debug.LogWarning(GridF.GetGridDir(input));
        }
        //Unit Test fonskiyonun calışıp calışmadğını kontrol etmek için.
        
        private void OnMouseDownGrid(Tile clickedTile, Vector3 dirVector)
        {
            _selectedTile = clickedTile;
            _mouseDownPos = dirVector;
        }

        private bool CanMove(Tile clickedTile, Vector3 inputVect,out List<Tile> matches)
        {
          matches = new List<Tile>();

            Vector2Int tileMoveCoord = clickedTile.Coord + GridF.GetGridDirVector(inputVect);

            if (_grid.IsInsideGrid(tileMoveCoord) == false) 
            {
                return false;
            }
            return HasMatch(clickedTile, tileMoveCoord,out matches);
        }

        
        private bool HasMatch(Tile fromTile, Vector2Int tileMoveCoord, out List<Tile>matches)
        {
            bool hasMatches = false;
            
             
            
            Tile toTile = _grid.Get(tileMoveCoord);
            _grid.Switch(fromTile,tileMoveCoord); 
            
            matches= _grid.GetMatchesY(toTile);
           matches.AddRange(_grid.GetMatchesX(toTile));
           matches.AddRange(_grid.GetMatchesY(fromTile));
           matches.AddRange(_grid.GetMatchesX(fromTile));

           if (matches.Count > 2)
           {
               hasMatches = true;
           }
          
            
            _grid.Switch(toTile,fromTile);
            return hasMatches;
        }

        private void UnRegisterEvents()
        { 
            InputEvents.MouseDownGrid -= OnMouseDownGrid;
            InputEvents.MouseUpGrid -= OnMouseUpGrid;
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
    


