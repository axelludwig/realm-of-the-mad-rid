using System.Text;
using TMPro;
using Unity.Netcode;
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
        debugText.AppendLine("Menu de debug de chiasse");
        debugText.AppendLine($"{fps} fps");
        debugText.AppendLine();
        debugText.AppendLine(GetHealthAndXpAsString());
        debugText.AppendLine(GetConnectedPlayersAsString());

        text.text = debugText.ToString();
    }

    string GetHealthAndXpAsString()
    {
        if (NetworkManager.Singleton == null)
            return "";

        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (playerObject == null) return "";
        var player = playerObject.GetComponent<Entity>();
        if (player == null) return "";

        StringBuilder debugText = new StringBuilder();
        debugText.AppendLine($"PV {player.Stats.Health.CurrentValue}/{player.Stats.Health.MaxValue}");
        debugText.AppendLine($"Lvl {player.Experience.Level}");
        debugText.AppendLine($"Exp {player.Experience.ExperiencePoints}/{player.Experience.ExperienceForNextLevel}");

        return debugText.ToString();
    }

    string GetConnectedPlayersAsString()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Connected Players:");

        foreach (Player v_Player in GameManager.Instance.GetPlayerObjects())
        {
            if (v_Player == null)
                continue;

            var v_Pos = v_Player.transform.position;
            sb.AppendLine($"- {v_Player.name} @ {v_Pos:F2}");
            GetItemsAsString(v_Player, sb);
        }

        sb.AppendLine();
        sb.AppendLine("IAs:");
        foreach (var ai in GameManager.Instance.GetAIObjects())
        {
            if (ai != null)
                sb.AppendLine($"- {ai.name} @ {ai.transform.position:F2}");
        }

        return sb.ToString();

        void GetItemsAsString(Player p_Player, StringBuilder sb)
        {
            var v_Inventory = p_Player.GetComponent<PlayerInventory>();


            if (v_Inventory != null && v_Inventory.GetInventory().Count > 0)
            {
                foreach (var v_Item in v_Inventory.GetInventory())
                {
                    sb.AppendLine($"    - {v_Item.Name}  (global id: {v_Item.GlobalId}, unique id: {v_Item.UniqueId})");

                    // 💡 Afficher les stats détaillées de l’item
                    foreach (var v_Stat in v_Item.Stats)
                    {
                        sb.AppendLine($"        {v_Stat.Type}: +{v_Stat.Value:F1}");
                    }
                }
            }
            else
            {
                sb.AppendLine("    (aucun item)");
            }
        }
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
