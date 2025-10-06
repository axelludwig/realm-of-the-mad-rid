using System.Text;
using TMPro;
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
        debugText += "\n" + GetConnectedPlayersAsString();

        text.text = debugText;
    }

    string GetConnectedPlayersAsString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Connected Players:");
        foreach (var p in GameManager.instance.Players)
        {
            if (p != null)
                sb.AppendLine($"- {p.name} - {p.transform.position:F2}");
        }
        return sb.ToString();   
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
