using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Trell.AddressablesPopupManager
{
    public class AssetManagement : IAssetManagement
    {
        private readonly Dictionary<string, AsyncOperationHandle> _completed = new();
        
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();
        
        public async UniTask<T> Load<T>(string path) where T : class
        {
            if (_completed.TryGetValue(path, out AsyncOperationHandle result))
            {
                return (T)result.Result;
            }

            AsyncOperationHandle<T> asyncOperationHandle = Addressables.LoadAssetAsync<T>(path);

            asyncOperationHandle.Completed += h => { _completed.TryAdd(path, h); };

            AddHandle(path, asyncOperationHandle);

            return await asyncOperationHandle.Task;
        }

        public void CleanUp()
        {
            foreach (AsyncOperationHandle resourceHandle in _handles.Values.SelectMany(handleList => handleList))
            {
                Addressables.Release(resourceHandle);
            }
            _handles.Clear();
            _completed.Clear();
        }

        private void AddHandle<T>(string path, AsyncOperationHandle<T> asyncOperationHandle) where T : class
        {
            if (!_handles.TryGetValue(path, out List<AsyncOperationHandle> resourceHandle))
            {
                resourceHandle = new();
                _handles[path] = resourceHandle;
            }
            resourceHandle.Add(asyncOperationHandle);
        }
    }
}