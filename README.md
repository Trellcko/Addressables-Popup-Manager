# Addressables-Popup-Manager

An **async Popup Manager** for Unity that uses **Addressables** + **UniTask** to load and manage popups efficiently.  

## ✨ Features
- Asynchronous popup loading  
- **Addressables** integration for asset management  
- **UniTask** for high-performance async/await  
- Simple API for showing and hiding popups  

---

## 📦 Installation

### 1️⃣ Install UniTask  
[UniTask GitHub Repository](https://github.com/Cysharp/UniTask)  

### 2️⃣ Install Addressables (Optional)  
The package will attempt to add Addressables automatically.  
If needed, install manually:  
[Addressables Installation Guide](https://docs.unity3d.com/Packages/com.unity.addressables@2.4/manual/installation-guide.html)  

### 3️⃣ Install via Unity Package Manager  
1. Open **Unity** → `Window > Package Manager`  
2. Click the **`+`** button → **Add package from git URL**  
3. Paste the repository URL:

## 🚀 Usage
To use the PopupManager in your own scripts, you must reference its assembly definition.

In your project, create an .asmdef file (if you don't already have one).
In the Inspector, add a reference to the com.trellcko.popupmanager assembly.
You can now use the package's namespaces in your code:

```csharp
public class Test : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private string[] _path;
    [SerializeField] private int _index;
    [SerializeField] private ShowPopupLogic _showPopupLogic;
    private PopupManager _popupManager;

    private void Start()
    {
        InitPopupManager();
    }

    private void InitPopupManager()
    {
        _popupManager = new(new AssetManagement());

        _popupManager.SetCanvas(_canvas);
    }

    [Button]
    private async void Show()
    {
        try
        {
            await _popupManager.ShowPopupAsync<Popup>(_path[_index], ShowPopupLogic.Stack);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
        }
    }

    [Button]
    private void Hide()
    {
        _popupManager.HidePopup(_path[_index], HidePopupLogic.Hide);
    }

    [Button]
    private void Destroy()
    {
        _popupManager.HidePopup(_path[_index], HidePopupLogic.Destroy);
    }
}
```
