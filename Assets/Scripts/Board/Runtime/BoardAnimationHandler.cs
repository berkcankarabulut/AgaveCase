using System;
using System.Collections.Generic;
using UnityEngine;
using Grid.Runtime;
using AgaveCase.Elements.Runtime;
using DG.Tweening;

namespace AgaveCase.Board.Runtime
{
    /// <summary>
    /// Board üzerindeki element animasyonlarını yöneten sınıf
    /// </summary>
    public class BoardAnimationHandler
    {
        // Aktif animasyon sayacı
        private int _activeAnimationsCount = 0;
        
        // Tüm animasyonlar tamamlandığında çağrılacak callback listesi
        private List<Action> _allAnimationCompletedCallbacks = new List<Action>();
        
        // Board yöneticisine referans
        private readonly BoardManager _boardManager;
        
        public BoardAnimationHandler(BoardManager boardManager)
        {
            _boardManager = boardManager;
        }
        
        /// <summary>
        /// Eşleşen elementleri animasyonla kaldırır, yer çekimi uygular ve boşlukları doldurur
        /// </summary>
        public void ProcessMatchedElements(List<Vector2Int> positions, float matchAnimationDelay)
        {
            if (positions == null || positions.Count == 0) return;
            
            // Başlangıçta aktif animasyon yok
            _activeAnimationsCount = 0;
            
            // Önce elementleri kaldır
            RemoveElements(positions);
            
            // Eşleşme animasyonu sonrasında yer çekimi uygula
            DOVirtual.DelayedCall(matchAnimationDelay, () => {
                ApplyGravity();
            });
        }
        
        /// <summary>
        /// Element eşleşmesini işler ve tüm animasyonlar bittiğinde callback'i çağırır
        /// </summary>
        public void ProcessMatchedElementsWithCallback(List<Vector2Int> positions, float matchAnimationDelay, Action onCompleted)
        {
            if (onCompleted != null)
            {
                AddAnimationCompletedCallback(onCompleted);
            }
            
            ProcessMatchedElements(positions, matchAnimationDelay);
        }
        
        /// <summary>
        /// Verilen pozisyonlardaki elementleri eşleşme animasyonu ile kaldırır
        /// </summary>
        private void RemoveElements(List<Vector2Int> positions)
        {
            if (positions == null) return;
            
            foreach (Vector2Int pos in positions)
            {
                GridCell cell = _boardManager.GetGridAt(pos);
                if (cell == null) continue;

                ElementBase element = cell.GetCandy();
                if (element == null) continue;
                
                // Eşleşme animasyonu çağır (yok olma efekti)
                // Element kendisi animasyon tamamlandığında havuza döner
                element.OnMatched();
                
                // Cell'den referansı kaldır
                cell.SetCandy(null);
            }
        }
        
        /// <summary>
        /// Yer çekimi efekti uygular (boşlukların üstündeki elementleri aşağı düşürür)
        /// </summary>
        private void ApplyGravity()
        {
            bool anyElementMoved = false;
            int gridWidth = _boardManager.GridWidth;
            int gridHeight = _boardManager.GridHeight;
            
            // Her sütunu aşağıdan yukarıya doğru kontrol et
            for (int x = 0; x < gridWidth; x++)
            {
                // Kaç tane eleman düşecek, düşme sırası için kullanılacak
                int fallCount = 0;
                
                // Aşağıdan yukarıya doğru tarama
                for (int y = 0; y < gridHeight; y++)
                {
                    // Boş bir hücre bulundu
                    GridCell currentCell = _boardManager.GetGridAt(x, y);
                    if (currentCell == null || currentCell.GetCandy() != null) continue;
                    
                    // Boş hücrenin üstündeki ilk dolu hücreyi bul
                    GridCell upperCellWithCandy = FindUpperCellWithCandy(x, y);
                    if (upperCellWithCandy == null) continue;
                    
                    // Üstteki elementi aşağıdaki boş hücreye taşı
                    MoveElementToEmptyCell(upperCellWithCandy, currentCell, fallCount, 
                        _boardManager.FallDuration, _boardManager.FallingEase, _boardManager.DelayBetweenFalls);
                    fallCount++;
                    anyElementMoved = true;
                    
                    // Aynı sütunda daha fazla düşüş olabileceği için y'yi azalt ve tekrar kontrol et
                    y--;
                }
            }
            
            // Hiç element hareket etmediyse, direkt boşlukları doldur
            if (!anyElementMoved)
            {
                DOVirtual.DelayedCall(0.2f, FillEmptyCells);
            }
        }
        
        /// <summary>
        /// Belirtilen konumun üstündeki ilk dolu hücreyi bulur
        /// </summary>
        private GridCell FindUpperCellWithCandy(int x, int startY)
        {
            int gridHeight = _boardManager.GridHeight;
            
            for (int y = startY + 1; y < gridHeight; y++)
            {
                GridCell cell = _boardManager.GetGridAt(x, y);
                if (cell != null && cell.GetCandy() != null)
                {
                    return cell;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Bir elementi başka bir hücreye taşır (Element animasyon API'si ile)
        /// </summary>
        private void MoveElementToEmptyCell(GridCell sourceCell, GridCell targetCell, int fallOrder, 
                                         float duration, Ease easeType, float delayBetweenFalls)
        {
            // Kaynak hücreden elementi al
            ElementBase element = sourceCell.GetCandy();
            if (element == null) return;
            
            // Hücre referanslarını güncelle
            targetCell.SetCandy(element);
            sourceCell.SetCandy(null);
            
            // Elementi parent'ını güncelle
            element.transform.SetParent(targetCell.transform);
            
            // Düşme animasyonu için animasyon sayacını artır
            _activeAnimationsCount++;
            
            // Gecikme ekle (zincirleme yer çekimi etkisi için)
            float delay = fallOrder * delayBetweenFalls;
            
            // Element kendi animasyon API'si ile düşme animasyonu
            element.PlayFallAnimation(
                targetCell.transform.position, // Hedef pozisyon
                duration,                    // Süre
                easeType,                    // Ease türü
                delay,                       // Gecikme
                () => {                      // Tamamlandığında
                    // Animasyon tamamlandığında sayacı azalt
                    _activeAnimationsCount--;
                    
                    // Son animasyon tamamlandığında boşlukları doldur
                    if (_activeAnimationsCount == 0)
                    {
                        DOVirtual.DelayedCall(0.2f, FillEmptyCells);
                    }
                }
            );
        }
        
        /// <summary>
        /// Boş hücrelere yeni elementler ekler
        /// </summary>
        private void FillEmptyCells()
        {
            int gridWidth = _boardManager.GridWidth;
            int gridHeight = _boardManager.GridHeight;
            
            for (int x = 0; x < gridWidth; x++)
            {
                int newElementCount = 0;
                
                for (int y = 0; y < gridHeight; y++)
                {
                    GridCell cell = _boardManager.GetGridAt(x, y);
                    if (cell == null) continue;
     
                    if (cell.GetCandy() == null)
                    { 
                        // Yeni elementi oluştur ve hücreye yerleştir - animasyonlu
                        _boardManager.SpawnElementAtCellWithAnimation(cell, newElementCount, OnNewElementSpawned, OnNewElementAnimationCompleted);
                        newElementCount++;
                    }
                }
            }
            
            // Hiç yeni element oluşturulmadıysa bunu bildirmek için
            if (_activeAnimationsCount == 0)
            {
                OnAllAnimationsCompleted();
            }
        }
        
        /// <summary>
        /// Yeni bir element oluşturulduğunda çağrılır
        /// </summary>
        private void OnNewElementSpawned()
        {
            _activeAnimationsCount++;
        }
        
        /// <summary>
        /// Yeni bir elementin animasyonu tamamlandığında çağrılır
        /// </summary>
        private void OnNewElementAnimationCompleted()
        {
            _activeAnimationsCount--;
            
            // Son animasyon tamamlandığında callback'leri çağır
            if (_activeAnimationsCount == 0)
            {
                OnAllAnimationsCompleted();
            }
        }
        
        /// <summary>
        /// Tüm element animasyonları tamamlandığında çağrılacak callback'leri ekler
        /// </summary>
        public void AddAnimationCompletedCallback(Action callback)
        {
            if (callback != null)
            {
                _allAnimationCompletedCallbacks.Add(callback);
            }
        }
        
        /// <summary>
        /// Tüm element animasyonları tamamlandığında çağrılacak 
        /// </summary>
        private void OnAllAnimationsCompleted()
        {
            // Kayıtlı tüm callback'leri çağır
            foreach (var callback in _allAnimationCompletedCallbacks)
            {
                callback?.Invoke();
            }
            
            // Callback listesini temizle
            _allAnimationCompletedCallbacks.Clear();
        }
        
        /// <summary>
        /// Tüm animasyonları temizler
        /// </summary>
        public void CleanupAnimations()
        {
            DOTween.Kill(_boardManager.transform);
        }
    }
}