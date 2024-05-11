using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;
    public int damage;
    public int order;
    public string characterName; // Karakterin ismi
    private Queue<string> playerCommands = new Queue<string>();

    void Start()
    {
        // Başlangıç değerlerini ayarlayabilirsiniz
    }

    public void PerformAction(string action)
    {
        switch (action)
        {
            case "Attack":
                Debug.Log(characterName + " is attacking with " + damage + " damage.");
                // Saldırı işlemi
                break;
            case "Block":
                Debug.Log(characterName + " is blocking.");
                // Blok işlemi
                break;
            case "Dodge":
                Debug.Log(characterName + " is dodging.");
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
        return order;
    }

    // Dinamik olarak statları güncellemek için metodlar ekleyin
    public void IncreaseHealth(int amount)
    {
        health += amount;
        Debug.Log(characterName + " health increased by " + amount + ". New health: " + health);
    }

    public void IncreaseDamage(int amount)
    {
        damage += amount;
        Debug.Log(characterName + " damage increased by " + amount + ". New damage: " + damage);
    }
}
