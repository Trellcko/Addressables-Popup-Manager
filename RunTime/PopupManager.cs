using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Trell.AddressablesPopupManager
{
    public class PopupManager : IDisposable, IPopupManager
    {
        public bool HasCanvas => _popupCanvas;

        private Canvas _popupCanvas;

        private readonly IAssetManagement _assetManagement;
        private readonly Dictionary<string, Popup> _popups = new();

        private readonly List<Popup> _popupsOrder = new();
        private Popup _currentPopup;

        public PopupManager(IAssetManagement assetManagement)
        {
            _assetManagement = assetManagement;
        }

        public void Dispose()
        {
            _assetManagement.CleanUp();
        }

        public void SetCanvas(Canvas canvas)
        {
            _popupCanvas = canvas;
        }

        public void HidePopup(string popupID, HidePopupLogic hidePopupLogic = HidePopupLogic.Destroy)
        {
            if (_popups.TryGetValue(popupID, out Popup popup) && popup)
            {
                popup.Hide(hidePopupLogic);
            }
            else
            {
                Debug.LogError($"There is no popup with Path: {popupID}");
            }
        }

        public async UniTask<T> ShowPopupAsync<T>(string popupID, ShowPopupLogic showPopupLogic = ShowPopupLogic.Over) where T : Popup
        {
            if (!HasCanvas)
            {
                Debug.LogError($"Canvas doesn't exist, please invoke {nameof(SetCanvas)}()");
                return null;
            }

            if (_assetManagement == null)
            {
                Debug.LogError($"AssetManagement is null");
                return null;
            }

            Popup popup = await GetPopup<T>(popupID);

            RunShowLogic<T>(showPopupLogic, popup);


            return popup as T;
        }

        private async UniTask<Popup> GetPopup<T>(string popupID) where T : Popup
        {
            if (!_popups.TryGetValue(popupID, out Popup popup))
            {
                return await CreatePopup<T>(popupID);
            }

            if (!popup)
            {
                Debug.LogError($"Cashed popup wasn't found, the popup {popupID} will be recreated");
                _popups.Remove(popupID);
                return await CreatePopup<T>(popupID);
            }

            return popup;
        }

        private void RunShowLogic<T>(ShowPopupLogic showPopupLogic, Popup popup) where T : Popup
        {
            switch (showPopupLogic)
            {
                case ShowPopupLogic.Over:
                    popup.Show();
                    break;
                case ShowPopupLogic.Queue:
                {
                    if (!_currentPopup)
                    {
                        ChangeCurrentPopupToNew();
                    }
                    else
                    {
                        _popupsOrder.Add(popup);
                    }
                }
                    break;
                case ShowPopupLogic.Stack:
                    if (_currentPopup)
                    {
                        _currentPopup.Hide();
                        _popupsOrder.Add(_currentPopup);
                    }
                    ChangeCurrentPopupToNew();
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(showPopupLogic), showPopupLogic, null);
            }

            return;

            void ChangeCurrentPopupToNew()
            {
                _currentPopup = popup;
                _currentPopup.Show();
            }
        }

        private async UniTask<Popup> CreatePopup<T>(string popupID) where T : Popup
        {
            GameObject popupPrefab = await _assetManagement.Load<GameObject>(popupID);
            GameObject[] spawned = await Object.InstantiateAsync(popupPrefab, _popupCanvas.transform).ToUniTask();
            if (spawned.Length == 0)
            {
                Debug.LogError($"Failed to instantiate popup '{popupID}'");
                return null;
            }

            T spawnedPopup = spawned[0].GetComponent<T>();
            InitPopup(spawnedPopup);   
            _popups.Add(popupID, spawnedPopup);
            return spawnedPopup;
        }

        private void InitPopup<T>(T spawnedPopup) where T : Popup
        {
            spawnedPopup.transform.localPosition = Vector3.zero;
            spawnedPopup.Destroyed += OnDestroyed;
            spawnedPopup.Hided += OnHided;
        }

        private void OnHided(Popup obj)
        {
            TryShowNextPopupFromQueue(obj);
        }

        private void OnDestroyed(Popup popup)
        {
            _popups.Remove(popup.PopupID);
            TryShowNextPopupFromQueue(popup);
            popup.Destroyed -= OnDestroyed;            
            popup.Hided -= OnHided;
        }

        private bool TryShowNextPopupFromQueue(Popup obj)
        {
            if (_currentPopup == obj && _popupsOrder.Count > 0)
            {
                _currentPopup = _popupsOrder[0];
                _popupsOrder.Remove(_currentPopup);
                _currentPopup.Show();
                return true;
            }

            return false;
        }
    }
}