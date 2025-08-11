using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Trell.AddressablesPopupManager
{
    public interface IPopupManager
    {
        bool HasCanvas { get; }
        void SetCanvas(Canvas canvas);
        void HidePopup(string popupID, HidePopupLogic hidePopupLogic = HidePopupLogic.Destroy);
        UniTask<T> ShowPopupAsync<T>(string popupID, ShowPopupLogic showPopupLogic = ShowPopupLogic.Over) where T : Popup;
    }
}