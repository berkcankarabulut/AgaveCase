using System;
using System.Collections.Generic;
using Grid.Runtime;
using AgaveCase.Elements.Runtime;
using Pooler;
using UnityEngine;

namespace AgaveCase.Board.Runtime
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private GridConfigSO _gridConfig;
        [SerializeField] private Transform _gridContainer;
        [SerializeField] private float _cellSize = 1.0f;
        
        [Header("Prefabs")] 
        [SerializeField] private GridCell _gridPrefab;
        [SerializeField] private ElementBase _elementPrefab;

        [Header("Element Setup")] 
        [SerializeField] private ElementDataSO[] _availableElements;
        
        [Header("Animation Settings")]
        [SerializeField] private float _fallingDuration = 0.3f;
        [SerializeField] private float _newElementFallingDuration = 0.5f;
        [SerializeField] private DG.Tweening.Ease _fallingEase = DG.Tweening.Ease.OutBounce;
        [SerializeField] private float _delayBetweenFalls = 0.05f;
        [SerializeField] private float _matchAnimationDelay = 0.5f;

        private int _width = 8;
        private int _height = 8;
        private GridCell[,] _gridCells;
        private GridSetupHandler _gridSetupHandler;
        private ElementSetupHandler _elementSetupHandler;
        private BoardAnimationHandler _animationHandler;
        private Camera _mainCamera;
        
        // Grid boyutlarına dışarıdan erişim için property'ler
        public int GridWidth => _width;
        public int GridHeight => _height;
        
        // Animasyon değerlerine erişim için property'ler
        public float FallDuration => _fallingDuration;
        public float NewElementDuration => _newElementFallingDuration;
        public DG.Tweening.Ease FallingEase => _fallingEase;
        public float DelayBetweenFalls => _delayBetweenFalls;
        public float MatchAnimationDelay => _matchAnimationDelay;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            if (_gridConfig == null) return;
            _mainCamera = Camera.main;
            _width = _gridConfig.Width;
            _height = _gridConfig.Height;

            _gridSetupHandler = new GridSetupHandler(_gridPrefab, _gridContainer, _cellSize);
            _elementSetupHandler = new ElementSetupHandler(_availableElements, _elementPrefab, ObjectPooler.Instance);
            _animationHandler = new BoardAnimationHandler(this);
 
            if (_gridCells != null)
            {
                _gridSetupHandler.CleanupGrid(_gridCells);
            }
 
            Vector3 gridPosition = CalculateIdealGridPosition();
 
            _gridCells = _gridSetupHandler.CreateGrid(_width, _height, gridPosition);
 
            _elementSetupHandler.SetupElements(_gridCells);

            _gridContainer.forward = _mainCamera.transform.forward;
        }

        private Vector3 CalculateIdealGridPosition()
        {
            Vector3 gridPosition;
            return new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, 0);
        }

        public GridCell GetGridAt(int x, int y)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height && _gridCells != null)
            {
                return _gridCells[x, y];
            }

            return null;
        }

        public GridCell GetGridAt(Vector2Int position)
        {
            return GetGridAt(position.x, position.y);
        } 
        
        /// <summary>
        /// Bir konumdaki elementi kaldırır ve eğer varsa havuza döndürür
        /// </summary>
        public void RemoveElementAt(Vector2Int position)
        {
            GridCell cell = GetGridAt(position);
            if (cell == null) return;

            ElementBase element = cell.GetCandy();
            if (element == null) return;
 
            ObjectPooler.Instance.Release(element.GameObject, element);
            cell.SetCandy(null);
        }
 
        /// <summary>
        /// Verilen pozisyonlardaki elementleri direkt olarak kaldırır (animasyonsuz)
        /// </summary>
        public void RemoveElements(List<Vector2Int> positions)
        {
            if (positions == null) return;

            foreach (Vector2Int pos in positions)
            {
                RemoveElementAt(pos);
            } 
        }
        
        /// <summary>
        /// Yeni bir elementi hücreye oluşturur ve isteğe bağlı olarak animasyon ile gösterir
        /// </summary>
        public void SpawnElementAtCellWithAnimation(GridCell cell, int spawnOrder, Action onSpawned = null, Action onCompleted = null)
        {
            if (cell == null || _elementPrefab == null) return;
            
            ElementDataSO elementData = GetRandomElementType();
            if (elementData == null) return;
            
            // Elementi yukarıdan düşecek şekilde oluştur (hücrenin üzerinde)
            Vector3 spawnPosition = cell.transform.position + new Vector3(0, 1.5f, 0); // 1.5 birim yukarıdan başlat
            GameObject elementObj = ObjectPooler.Instance.Spawn(_elementPrefab.gameObject, spawnPosition, Quaternion.identity);
            if (elementObj == null) return;

            ElementBase element = elementObj.GetComponent<ElementBase>();
            if (element == null) return;
            
            element.Initialize(elementData);

            // Element boyutunu hücreye göre ayarla
            AdjustElementSize(element, cell);

            // Elementi hücreye bağla
            element.transform.SetParent(cell.transform);
            cell.SetCandy(element);
            
            // Spawn callback'i çağır
            onSpawned?.Invoke();
            
            // Element animasyon API'sini kullanarak düşme animasyonu
            float delay = spawnOrder * _delayBetweenFalls;
            element.PlaySpawnAnimation(
                spawnPosition,
                cell.transform.position,
                _newElementFallingDuration,
                _fallingEase,
                onCompleted
            );
        }
 
        /// <summary>
        /// Eşleşen elementleri kaldırır, yer çekimi uygular ve boşlukları doldurur
        /// </summary>
        public void ProcessMatchedElements(List<Vector2Int> positions)
        {
            if (positions == null || positions.Count == 0) return;
            
            _animationHandler.ProcessMatchedElements(positions, _matchAnimationDelay);
        }
        
        /// <summary>
        /// Eşleşen elementleri işler ve tüm animasyonlar bittiğinde callback'i çağırır
        /// </summary>
        public void ProcessMatchedElementsWithCallback(List<Vector2Int> positions, Action onCompleted)
        {
            if (positions == null || positions.Count == 0)
            {
                onCompleted?.Invoke();
                return;
            }
            
            _animationHandler.ProcessMatchedElementsWithCallback(positions, _matchAnimationDelay, onCompleted);
        }
        
        /// <summary>
        /// Animasyon tamamlandığında çağrılacak callback ekler
        /// </summary>
        public void AddAnimationCompletedCallback(Action callback)
        {
            _animationHandler.AddAnimationCompletedCallback(callback);
        }

        /// <summary>
        /// Rastgele bir element tipi seçer
        /// </summary>
        private ElementDataSO GetRandomElementType()
        {
            if (_availableElements == null || _availableElements.Length == 0) return null;

            int randomIndex = UnityEngine.Random.Range(0, _availableElements.Length);
            return _availableElements[randomIndex];
        }
        
        /// <summary>
        /// Element boyutunu hücre boyutuna göre ayarlar
        /// </summary>
        private void AdjustElementSize(ElementBase element, GridCell cell)
        {
            if (element == null || cell == null) return;

            SpriteRenderer cellRenderer = cell.GetComponent<SpriteRenderer>();
            if (cellRenderer == null) return;

            SpriteRenderer elementRenderer = element.GetComponent<SpriteRenderer>();
            if (elementRenderer == null) return;

            Vector2 cellSize = cellRenderer.bounds.size;
            Vector2 originalElementSize = elementRenderer.bounds.size;

            // Element'in hücrenin %80'ini kaplamasını sağla
            float scaleX = (cellSize.x * 0.8f) / originalElementSize.x;
            float scaleY = (cellSize.y * 0.8f) / originalElementSize.y;

            float scale = Mathf.Min(scaleX, scaleY);

            element.transform.localScale = new Vector3(scale, scale, 1f);
        }

        // Clean up before destruction
        private void OnDestroy()
        {
            // Tüm animasyonları temizle
            if (_animationHandler != null)
            {
                _animationHandler.CleanupAnimations();
            }
            
            if (_gridCells != null && _elementSetupHandler != null)
            {
                _elementSetupHandler.ClearElements(_gridCells);
            }

            if (_gridCells != null && _gridSetupHandler != null)
            {
                _gridSetupHandler.CleanupGrid(_gridCells);
            }
        }
    }
}