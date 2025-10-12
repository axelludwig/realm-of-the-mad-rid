using UnityEngine;
using UnityEngine.TextCore.Text;

public class FontPreloader : MonoBehaviour
{
    [SerializeField] private FontAsset[] fontsToPreload;

    void Awake()
    {
        foreach (var font in fontsToPreload)
        {
            if (font == null) continue;
            // Force la lecture des d�finitions sur le main thread
            font.ReadFontAssetDefinition();
        }
    }
}
