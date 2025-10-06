using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    float fps;
    float timer;

    // Update is called once per frame
    void Update()
    {
        UpdateFpsCounter();

        string debugText = "Menu de debug de chiasse";

        debugText += "\n" + fps + " fps";
        text.text = debugText;
    }

    void UpdateFpsCounter()   
    {
        if (timer > 1f)
        {
            fps = (int)(1f / Time.unscaledDeltaTime); fps = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
