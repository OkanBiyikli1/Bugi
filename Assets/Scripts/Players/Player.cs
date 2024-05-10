using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Stats stats;
    private Queue<string> playerCommands = new Queue<string>();

    void Start()
    {
        stats = GetComponent<Stats>();
    }

    public void PerformAction(string action)
    {
        switch (action)
        {
            case "Attack":
                Debug.Log(stats.characterName + " is attacking");
                // Saldırı işlemi
                break;
            case "Block":
                Debug.Log("Player is blocking.");
                // Blok işlemi
                break;
            case "Dodge":
                Debug.Log("Player is dodging.");
                // Kaçınma işlemi
                break;
            default:
                Debug.Log("Unknown action: " + action);
                break;
        }
    }

    public void ExecutePlayerCommands()
    {
        if (playerCommands.Count > 0)
        {
            string command = playerCommands.Dequeue();
            PerformAction(command);
            GameManager.Instance.RemoveFirstCommandFromList(); // Gerçekleşen aksiyonu listeden sil
        }
    }

    public void AddCommand(string command)
    {
        playerCommands.Enqueue(command);
    }

    public int GetOrder()
    {
        return stats.order;
    }

    // Dinamik olarak statları güncellemek için metodlar ekleyin
    public void IncreaseHealth(int amount)
    {
        stats.health += amount;
        Debug.Log("Player health increased by " + amount + ". New health: " + stats.health);
    }

    public void IncreaseDamage(int amount)
    {
        stats.damage += amount;
        Debug.Log("Player damage increased by " + amount + ". New damage: " + stats.damage);
    }
}
