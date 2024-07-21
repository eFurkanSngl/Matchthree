using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Components.UI;
using DG.Tweening;
using Events;
using Extensions.DoTween;
using Extensions.System;
using Extensions.Unity;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Sequence = DG.Tweening.Sequence;


namespace Components
{
    public partial class GridManager : SerializedMonoBehaviour, ITweenContainerBind
    {
        [Inject] private GridEvents GridEvents { get; set; }
        [Inject] private InputEvents InputEvents { get; set; }

        [BoxGroup(Order = 999)]
#if UNITY_EDITOR

        [TableMatrix(SquareCells = true, DrawElementMethod = nameof(DrawTile))]
#endif
        [OdinSerialize]
        private Tile[,] _grid; // Tileden coklu array oluşuturuyoruz _gird adında.

        [SerializeField] private List<GameObject> _tilePrefabs; // Listede GameObject tipinde tutuyoruz 
        [SerializeField] private int _gridSizeX; // 2 tane X ve y değişken tanımladık.
        [SerializeField] private int _gridSizeY;
        [SerializeField] private List<int> _prefabIds; //Prefab Idleri tutacak int liste
        [SerializeField] private Bounds _gridBounds;
        [SerializeField] private Transform _transform;
        [SerializeField] private List<GameObject> _tileBGs = new(); //GameObjelerden tile arka plan
        [SerializeField] private List<GameObject> _gridBorders = new(); // ''  grid'in Çerçevesi
        [SerializeField] private GameObject _tileBGPrefab; // arka Plan prefab        
        [SerializeField] private Transform _bGTrans; // transfrom
        [SerializeField] private GameObject _borderTopLeft;
        [SerializeField] private GameObject _borderTopRight;
        [SerializeField] private GameObject _borderBotLeft;
        [SerializeField] private GameObject _borderBotRight;
        [SerializeField] private GameObject _borderLeft;
        [SerializeField] private GameObject _borderRight;
        [SerializeField] private GameObject _borderTop;
        [SerializeField] private GameObject _borderBot;
        [SerializeField] private Transform _borderTrans;
        private Tile _selectedTile; // Seçilen Tile
        private Vector3 _mouseDownPos; // Mouse aşağı hareketi
        private Vector3 _mouseUpPos; // Mouse Yukarı Hareketi
        private List<MonoPool> _tilePoolsByPrefabID; // Havuzdaki Prefab Idlerin tutulduğu yer
        private MonoPool _tilePool0;
        private MonoPool _tilePool1;
        private MonoPool _tilePool2;
        private MonoPool _tilePool3;
        private Tile[,] _tilesToMove; // Tile hareketi için yaratılan gridteki tile
        [OdinSerialize] private List<List<Tile>> _lastMatches;
        private Tile _hintTile; // Tile hareket ipucu
        private GridDir _hintDir; // İpucu yönü 
        private Sequence _hintTween;
        private Coroutine _destroyRoutine;
        public ITweenContainer TweenContainer { get; set; }
        private Coroutine _hintRoutine;
        
        [SerializeField] private int _scoreMulti;
        
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private PlayerScoreTMP _playerScoreTMP;
        [SerializeField] private TMP_Text gameOverScoreText;
        private CanvasGroup _canvasGroup;
        [SerializeField] private GameObject _matchParticlePrefab;
        
        private void Awake()
        {
            _tilePoolsByPrefabID = new List<MonoPool>();

            for (int prefabId = 0; prefabId < _prefabIds.Count; prefabId++)
            {
                MonoPool tilePool = new
                (
                    new MonoPoolData(
                        _tilePrefabs[prefabId],
                        10,
                        _transform
                    )
                );

                _tilePoolsByPrefabID.Add(tilePool);
            }

            TweenContainer = TweenContain.Install(this);
        }

        private void Start()
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                Tile tile = _grid[x, y];

                SpawnTile(tile.ID, _grid.CoordsToWorld(_transform, tile.Coords), tile.Coords);
                tile.gameObject.Destroy();
            }

            IsGameOver(out _hintTile, out _hintDir);
            GridEvents.GridLoaded?.Invoke(_gridBounds);
            GridEvents.InputStart?.Invoke();
            gameOverPanel = GameObject.Find("GameOverCanvas");


            if (gameOverPanel != null)
            {
                _canvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
                gameOverScoreText = gameOverPanel.GetComponentInChildren<TMP_Text>();

                if (_canvasGroup == null)
                {
                    Debug.LogError("Canvas Group null");
                    return;
                }
            }

            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            
            gameOverPanel.SetActive(false);
            _playerScoreTMP = FindObjectOfType<PlayerScoreTMP>();
        }

        private void OnEnable()
        {
            RegisterEvents();
        }


        private void OnDisable()
        {
            UnRegisterEvents();
            TweenContainer.Clear();
        }

        private bool CanMove(Vector2Int tileMoveCoord) => _grid.IsInsideGrid(tileMoveCoord);

        private bool HasAnyMatches(out List<List<Tile>> matches)
        {
            matches = new List<List<Tile>>();

            foreach (Tile tile in _grid)
            {
                
                List<Tile> matchesAll = _grid.GetMatchesXAll(tile);
                matchesAll.AddRange(_grid.GetMatchesYAll(tile));

                if (matchesAll.Count > 0)
                {
                    matches.Add(matchesAll);
                }
            }

            matches = matches.OrderByDescending(e => e.Count).ToList();
            // OrderByDescending :  IEnumerable<T> koleksiyonlarını sıralamak için kullanılır
            // ve koleksiyondaki öğeleri belirtilen sıralama ölçütüne göre azalan sırayla sıralar

            for (int i = 0; i < matches.Count; i++)
            {
                List<Tile> match = matches[i];

                matches[i] = match.Where(e => e.ToBeDestroyed == false).DoToAll(e => e.ToBeDestroyed = true).ToList();
                // Bu komut match öğelerinden False olanları alır ve True yapar Listeye Ekler.Bir koşula göre çalışır.
            }

            matches = matches.Where(e => e.Count > 2).ToList();
            return matches.Count > 0;
            // Bu method Oyundaki tileleri kontrol ediyor eğer 3 veya daha fazla yatay
            // Dikey bir arada ise eşleşme olarak sayar.Eşleşen taşları listede tutar
            // Bu eşleşmeleri kontrol eder ve Gridten cıkarır.Eşleşen varsa True yoksa False döndürür.

        }

        private bool IsGameOver(out Tile hintTile, out GridDir hintDir)
        {
            hintDir = GridDir.Null;
            hintTile = null;

            List<Tile> matches = new();

            foreach (Tile fromTile in _grid)
            {
                hintTile = fromTile;

                Vector2Int thisCoord = fromTile.Coords;

                Vector2Int leftCoord = thisCoord + Vector2Int.left;
                Vector2Int topCoord = thisCoord + Vector2Int.up;
                Vector2Int rightCoord = thisCoord + Vector2Int.right;
                Vector2Int botCoord = thisCoord + Vector2Int.down;

                if (_grid.IsInsideGrid(leftCoord))
                {
                    Tile toTile = _grid.Get(leftCoord);

                    _grid.Swap(fromTile, toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile));

                    _grid.Swap(toTile, fromTile);

                    if (matches.Count > 0)
                    {
                        hintDir = GridDir.Left;
                        return false;
                    }
                }

                if (_grid.IsInsideGrid(topCoord))
                {
                    Tile toTile = _grid.Get(topCoord);
                    _grid.Swap(fromTile, toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile));

                    _grid.Swap(toTile, fromTile);

                    if (matches.Count > 0)
                    {
                        hintDir = GridDir.Up;
                        return false;
                    }
                }

                if (_grid.IsInsideGrid(rightCoord))
                {
                    Tile toTile = _grid.Get(rightCoord);
                    _grid.Swap(fromTile, toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile));

                    _grid.Swap(toTile, fromTile);

                    if (matches.Count > 0)
                    {
                        hintDir = GridDir.Right;
                        return false;
                    }
                }

                if (_grid.IsInsideGrid(botCoord))
                {
                    Tile toTile = _grid.Get(botCoord);
                    _grid.Swap(fromTile, toTile);

                    matches = _grid.GetMatchesX(fromTile);
                    matches.AddRange(_grid.GetMatchesY(fromTile));

                    _grid.Swap(toTile, fromTile);

                    if (matches.Count > 0)
                    {
                        hintDir = GridDir.Down;
                        return false;
                    }
                }
            }

            return matches.Count == 0;
        }

        private void SpawnAndAllocateTiles()
        {
            // _tilesToMove = new Tile[_gridSizeX,_gridSizeY];
            //
            // for(int y = 0; y < _gridSizeY; y ++)
            // {
            //     int spawnStartY = 0;
            //     
            //     for(int x = 0; x < _gridSizeX; x ++)
            //     {
            //         Vector2Int thisCoord = new(x, y);
            //         Tile thisTile = _grid.Get(thisCoord);
            //
            //         if(thisTile) continue;
            //
            //         int spawnPoint = _gridSizeY;
            //
            //         for(int y1 = y; y1 <= spawnPoint; y1 ++)
            //         {
            //             if(y1 == spawnPoint)
            //             {
            //                 if(spawnStartY == 0)
            //                 {
            //                     spawnStartY = thisCoord.y;
            //                 }
            //             
            //                 MonoPool randomPool = _tilePoolsByPrefabID.Random();
            //                 Tile newTile = SpawnTile
            //                 (
            //                     randomPool, 
            //                     _grid.CoordsToWorld(_transform, new Vector2Int(x, spawnPoint)),
            //                     thisCoord
            //                 );
            //             
            //                 _tilesToMove[thisCoord.x, thisCoord.y] = newTile;
            //                 break;
            //             }
            //
            //             Vector2Int emptyCoords = new(x, y1);
            //
            //             Tile mostTopTile = _grid.Get(emptyCoords);
            //
            //             if(mostTopTile)
            //             {
            //                 _grid.Set(null, mostTopTile.Coords);
            //                 _grid.Set(mostTopTile, thisCoord);
            //             
            //                 _tilesToMove[thisCoord.x, thisCoord.y] = mostTopTile;
            //
            //                 break;
            //             }
            //         }
            //     }
            // }
            //
            // StartCoroutine(RainDownRoutine());
            {
                _tilesToMove = new Tile[_gridSizeX, _gridSizeY];

                for (int y = 0; y < _gridSizeY; y++)
                {
                    int spawnStartY = 0;

                    for (int x = 0; x < _gridSizeX; x++)
                    {
                        Vector2Int thisCoord = new(x, y);
                        Tile thisTile = _grid.Get(thisCoord);

                        if (thisTile) continue;

                        int spawnPoint = _gridSizeY;

                        for (int y1 = y; y1 <= spawnPoint; y1++)
                        {
                            if (y1 == spawnPoint)
                            {
                                if (spawnStartY == 0)
                                {
                                    spawnStartY = thisCoord.y;
                                }

                                MonoPool randomPool = _tilePoolsByPrefabID.Random();
                                Tile newTile = SpawnTile
                                (
                                    randomPool,
                                    _grid.CoordsToWorld(_transform, new Vector2Int(x, spawnPoint)),
                                    thisCoord
                                );

                                _tilesToMove[thisCoord.x, thisCoord.y] = newTile;
                                break;
                            }

                            Vector2Int emptyCoords = new(x, y1);

                            Tile mostTopTile = _grid.Get(emptyCoords);

                            if (mostTopTile)
                            {
                                _grid.Set(null, mostTopTile.Coords);
                                _grid.Set(mostTopTile, thisCoord);

                                _tilesToMove[thisCoord.x, thisCoord.y] = mostTopTile;

                                break;
                            }
                        }
                    }
                }

                StartCoroutine(RainDownRoutine());
            }
        }



        private Tile SpawnTile(MonoPool randomPool, Vector3 spawnWorldPos, Vector2Int spawnCoords)
        {
            Tile newTile = randomPool.Request<Tile>();
            newTile.Teleport(spawnWorldPos);
            _grid.Set(newTile, spawnCoords);
            return newTile;
        }
        private Tile SpawnTile(int id, Vector3 worldPos,Vector2Int coords)=> SpawnTile(_tilePoolsByPrefabID[id], worldPos, coords);
        
        private IEnumerator RainDownRoutine()
        {
            int longestDistY = 0;
            Tween longestTween = null;
            
            for(int y = 0; y < _gridSizeY; y ++) // TODO: Should start from first tile that we are moving
            {
                bool shouldWait = false;
                
                for(int x = 0; x < _gridSizeX; x ++)
                {
                    Tile thisTile = _tilesToMove[x, y];

                    if(thisTile == false) continue;
                    
                    Tween thisTween = thisTile.DoMove(_grid.CoordsToWorld(_transform, thisTile.Coords));

                    shouldWait = true;

                    if(longestDistY < y)
                    {
                        longestDistY = y;
                        longestTween = thisTween;
                    }
                }

                if(shouldWait)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }

            if(longestTween != null)
            {
                longestTween.onComplete += delegate
                {
                    if(HasAnyMatches(out _lastMatches))
                    {
                        StartDestroyRoutine();
                    }
                    else
                    {
                        IsGameOver(out _hintTile, out _hintDir);
                        GridEvents.InputStart?.Invoke();
                    }
                };
            }
            else
            {
                Debug.LogWarning("This should not have happened!");
                GridEvents.InputStart?.Invoke();
            }
        }

        private void StartDestroyRoutine()
        {
            if(_destroyRoutine != null)
            {
                StopCoroutine(_destroyRoutine);
            }
            
            _destroyRoutine = StartCoroutine(DestroyRoutine());
        }
        private IEnumerator DestroyRoutine()
        {
            foreach (List<Tile> matches in _lastMatches)
            {
                IncScoreMulti();
                matches.DoToAll(DespawnTile);

                foreach (Tile tile in matches)
                {
                    Vector3 particlePosition = _grid.CoordsToWorld(_transform, tile.Coords);
                    Instantiate(_matchParticlePrefab, particlePosition, Quaternion.identity);

                }
                GridEvents.MatchGroupDespawn?.Invoke(matches.Count * _scoreMulti);

                yield return new WaitForSeconds(0.1f);
                
            }
            SpawnAndAllocateTiles();
            TestGameOver();
            
            // foreach(List<Tile> matches in _lastMatches)
            // {
            //     IncScoreMulti();
            //     matches.DoToAll(DespawnTile);
            //     
            //     GridEvents.MatchGroupDespawn?.Invoke(matches.Count * _scoreMulti);
            //
            //     yield return new WaitForSeconds(0.1f);
            // }
        }

        private void TestGameOver()
        {
            // bool isGameOver = IsGameOver(out Tile hintTile, out GridDir hintDir);
            //
            // Debug.LogWarning($"isGameOver{isGameOver} hintTile {hintTile} hintDir{hintDir}",hintTile);
            // if (isGameOver)
            // {
            //     ShowGamePanel();
            // }
            bool isGameOver = IsGameOver(out Tile hintTile, out GridDir hintDir);

            Debug.LogWarning($"isGameOver: {isGameOver}, hintTile {hintTile}, hintDir {hintDir}", hintTile);
            if (isGameOver)
            {
                ShowGameOverPanel();
            }
        }

        private void ShowGameOverPanel()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            Time.timeScale = 0;
            
            Debug.Log("Show gameover panel");
            gameOverPanel.SetActive(true);
            gameOverScoreText.text = $"Score:{_playerScoreTMP.GetCurrentScore()}";
        }

        private void HideGameOverPanel()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            Time.timeScale = 1;
            Debug.Log("Hide gameover panel");
            gameOverPanel.SetActive(false);
        }

        public void OnNewGameButtonClicked()
        {
            Debug.Log("New game button clicked , Restarting game");
            HideGameOverPanel();
            RestartGame();
        }

        private void RestartGame()
        {
            ClearGrid();
            ResetScoreMulti();
            StartGame();
        }

        private void ClearGrid()
        {
            foreach (Tile tile in _grid)
            {
                if (tile != null)
                {
                    DespawnTile(tile);
                }
            }
        }

        private void ResetScore()
        {
            _scoreMulti = 0;
            _playerScoreTMP.SetScore(0);
        }

        private void StartGame()
        {
            SpawnAndAllocateTiles();
        }


        private void DespawnTile(Tile e)
        {
            _grid.Set(null, e.Coords);
            _tilePoolsByPrefabID[e.ID].DeSpawn(e);
        }
        private void DoTileMoveAnim(Tile fromTile, Tile toTile, TweenCallback onComplete = null)
        {
            Vector3 fromTileWorldPos = _grid.CoordsToWorld(_transform, fromTile.Coords);
            fromTile.DoMove(fromTileWorldPos);
            Vector3 toTileWorldPos = _grid.CoordsToWorld(_transform, toTile.Coords);
            toTile.DoMove(toTileWorldPos, onComplete);
        }
        private void StartHintRoutine()
        {
            if(_hintRoutine != null)
            {
                StopCoroutine(_hintRoutine);
            }

            _hintRoutine = StartCoroutine(HintRoutineUpdate());
        }
        private void StopHintRoutine()
        {
            if(_hintTile)
            {
                _hintTile.Teleport(_grid.CoordsToWorld(_transform, _hintTile.Coords));
            }
            
            if(_hintRoutine != null)
            {
                StopCoroutine(_hintRoutine);
                _hintRoutine = null;
            }
        }
        
        private IEnumerator HintRoutineUpdate()
        {
            while(true)
            {
                yield return new WaitForSeconds(3f);
                TryShowHint();
            }
        }
        private void TryShowHint()
        {
            if(_hintTile)
            {
                Vector2Int gridMoveDir = _hintDir.ToVector();

                Vector3 moveCoords = _grid.CoordsToWorld(_transform, _hintTile.Coords + gridMoveDir);
                
                _hintTween = _hintTile.DoHint(moveCoords);
            }
        }
        private void ResetScoreMulti() {_scoreMulti = 0;}

        private void IncScoreMulti()
        {
            _scoreMulti ++;
        }
        private void RegisterEvents()
        {
            InputEvents.MouseDownGrid += OnMouseDownGrid;
            InputEvents.MouseUpGrid += OnMouseUpGrid;
            GridEvents.InputStart += OnInputStart;
            GridEvents.InputStop += OnInputStop;
        }
        private void OnInputStop()
        {
            StopHintRoutine();
        }

        private void OnInputStart()
        {
            StartHintRoutine();
            ResetScoreMulti();
        }
        private void OnMouseDownGrid(Tile clickedTile, Vector3 dirVector)
        {
            _selectedTile = clickedTile;
            _mouseDownPos = dirVector;

            if(_hintTween.IsActive())
            {
                _hintTween.Complete();
            }
        }
        private void OnMouseUpGrid(Vector3 mouseUpPos)
        {
            _mouseUpPos = mouseUpPos;

            Vector3 dirVector = mouseUpPos - _mouseDownPos;

            if(_selectedTile)
            {
                Vector2Int tileMoveCoord = _selectedTile.Coords + GridF.GetGridDirVector(dirVector);

                if(! CanMove(tileMoveCoord)) return;

                Tile toTile = _grid.Get(tileMoveCoord);

                _grid.Swap(_selectedTile, toTile);

                if(! HasAnyMatches(out _lastMatches))
                {
                    GridEvents.InputStop?.Invoke();

                    DoTileMoveAnim(_selectedTile, toTile,
                        delegate
                        {
                            _grid.Swap(toTile, _selectedTile);
                            
                            DoTileMoveAnim(_selectedTile, toTile,
                                delegate
                                {
                                    GridEvents.InputStart?.Invoke();
                                });
                        });
                }
                else
                {
                    GridEvents.InputStop?.Invoke();

                    DoTileMoveAnim
                    (
                        _selectedTile,
                        toTile,
                        StartDestroyRoutine
                    );
                }
            }
            
        }
        private void UnRegisterEvents()
        {
            InputEvents.MouseDownGrid -= OnMouseDownGrid;
            InputEvents.MouseUpGrid -= OnMouseUpGrid;
            GridEvents.InputStart -= OnInputStart;
            GridEvents.InputStop -= OnInputStop;
        }
    }
}


