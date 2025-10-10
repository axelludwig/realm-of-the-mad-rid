using System.Text;
using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private float fps;
    private float timer;

    void Update()
    {
        UpdateFpsCounter();

        StringBuilder debugText = new StringBuilder();
        debugText.AppendLine("Menu de debug de chiasse 💩");
        debugText.AppendLine($"{fps} fps");
        debugText.AppendLine();
        debugText.AppendLine(GetConnectedPlayersAsString());

        text.text = debugText.ToString();
    }

    string GetConnectedPlayersAsString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Connected Players:");

        foreach (var v_Player in GameManager.Instance.GetPlayerObjects())
        {
            if (v_Player == null)
                continue;

            var v_Pos = v_Player.transform.position;
            var v_Inventory = v_Player.GetComponent<PlayerInventory>();

            sb.AppendLine($"- {v_Player.name} @ {v_Pos:F2}");

            if (v_Inventory != null && v_Inventory.Items.Count > 0)
            {
                foreach (var v_Item in v_Inventory.Items)
                {
                    sb.AppendLine($"    - {v_Item.Data.ItemName}");

                    // 💡 Afficher les stats détaillées de l’item
                    foreach (var v_Stat in v_Item.FinalStats)
                    {
                        sb.AppendLine($"        {v_Stat.Key}: +{v_Stat.Value:F1}");
                    }
                }
            }
            else
            {
                sb.AppendLine("    (aucun item)");
            }
        }

        sb.AppendLine();
        sb.AppendLine("IAs:");
        foreach (var ai in GameManager.Instance.GetAIObjects())
        {
            if (ai != null)
                sb.AppendLine($"- {ai.name} @ {ai.transform.position:F2}");
        }

        return sb.ToString();
    }

    void UpdateFpsCounter()
    {
        if (timer > 1f)
        {
            fps = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
