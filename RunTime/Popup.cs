using System;
using UnityEngine;

namespace Trell.AddressablesPopupManager
{
    public class Popup : MonoBehaviour
    {
        [SerializeField] private GameObject content;

        public string PopupID { get; private set; }
        
        public bool IsShowed { get; private set; }

        public event Action<Popup> Showed;
        public event Action<Popup> Hided;
        public event Action<Popup> Destroyed;

        public void SetPopupID(string id)
        {
            PopupID = id;
        }
        
        public void Hide(HidePopupLogic hidePopupLogic = HidePopupLogic.Hide)
        {
            if ((hidePopupLogic is HidePopupLogic.Hide or HidePopupLogic.HideThenDestroy) && IsShowed)
            {
                OnHided();
                Hided?.Invoke(this);
            }

            IsShowed = false;
            if (hidePopupLogic is HidePopupLogic.HideThenDestroy or HidePopupLogic.Destroy)
            {
                Destroyed?.Invoke(this);
                Destroy(gameObject);
            }
        }

        public void Show()
        {
            if(IsShowed)
                return;
            
            OnShowed();
            IsShowed = true;
            Showed?.Invoke(this);
        }

        protected virtual void OnShowed()
        {
            content.SetActive(true);
        }

        protected virtual void OnHided()
        {   
            content.SetActive(false);
        }
    }
}