using Cysharp.Threading.Tasks;

namespace Trell.AddressablesPopupManager
{
    public interface IAssetManagement
    {
        UniTask<T> Load<T>(string path) where T : class;
        void CleanUp();
    }
}