﻿using System.Collections.Generic;
using Events;
using Extensions.System;
using Extensions.Unity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Zenject;


namespace Components
{
    public class GridManager: SerializedMonoBehaviour
    {
        [Inject] private InputEvents InputEvents{get; set;}
        [BoxGroup(Order = 999)]
        [TableMatrix(SquareCells = true) /*(DrawElementMethod = nameof(DrawTile))*/, OdinSerialize]
        private Tile[,] _grid;  // Tileden coklu array oluşuturuyoruz _gird adında.
        
        
        [SerializeField] private List<GameObject> _tilePrefabs; // Listede GameObject tipinde tutuyoruz
        
        private int _gridSizeX;  // 2 tane X ve y değişken tanımladık.
        private int _gridSizeY;
        [SerializeField] private List<int> _prefabIds;
        
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

        private Tile DrawTile(Rect rect,Tile tile)
        {
            UnityEditor.EditorGUI.DrawRect(rect, Color.gray);
            return tile;
        }

        [Button]
        private void CreateGrid(int sizeX, int sizeY)
        {

            _prefabIds = new();  // Yukarıda tanımladığımız bu değişken verileri int tipinde listede tutacak
            
            for(int id = 0; id < _tilePrefabs.Count; id++) _prefabIds.Add(id); // burada for ile tilePrefabs elemanları dönüyoruz ve daha sonra _prefabIds ye ekliyoruz.
            
            _gridSizeX = sizeX;   // yukarıda tanımladığımız değişkenler burada ki parametlerden alacaklar.
             _gridSizeY = sizeY;
                
            
            for (int x = 0; x < _gridSizeX; x++)
            for (int y = 0; y < _gridSizeY; y++)
            {
               
               // Vector2Int coord = new Vector2Int(x, _girdSizeY - y - 1); // grid alt üst etmek için İnspectorda görünen gibi göstermiyor Scende.
               List<int> spawnableId = new(_prefabIds);
               Vector2Int coord = new(x, _gridSizeY - y - 1);
                Vector3 pos = new(coord.x, coord.y,0f);
                // Vector3 pos değişkenine yeni x.y.z yi atıyoruz.


                _grid.GetSpawnableColors(coord, _prefabIds);
                
                int randomId = spawnableId.Random();

                GameObject tilePrefabRandom = _tilePrefabs[randomId];
                GameObject tileNew = Instantiate(tilePrefabRandom, pos, Quaternion.identity); //Randomn prefab başlatmak için.

                Tile tile = tileNew.GetComponent<Tile>();
                tile.Construct((coord));


            }
            
            
        }

        private void RegisterEvents()
        {
              InputEvents.MouseDownGrid += OnMouseDownGrid;
            InputEvents.MouseUpGrid += OnMouseUpGrid;
        }

        private void OnMouseUpGrid(Vector3 arg0)
        {
            _mouseUpPos = arg0;

            if (_selectedTile)
            {
                EDebug.Method();
                Debug.DrawLine(_mouseDownPos,_mouseUpPos,Color.blue,2F);
            }
        }

        private void OnMouseDownGrid(Tile arg0, Vector3 arg1)
        {
            _selectedTile = arg0;
            _mouseDownPos = arg1;
            EDebug.Method();
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
    


