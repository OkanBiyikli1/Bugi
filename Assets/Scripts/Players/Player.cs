using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;
    public int damage;
    public int order;
    public string characterName; // Karakterin ismi
    private Queue<string> playerCommands = new Queue<string>();
    public bool isBlocking;
    public bool isDodging;

    void Start()
    {
        // Başlangıç değerlerini ayarlayabilirsiniz
    }

    public void PerformAction(string action)
    {
        switch (action)
        {
            case "Attack":
                if (TurnManager.Instance.IsPlayerTurn())
                {
                    Debug.Log(characterName + " is attacking with " + damage + " damage.");
                    // Saldırı işlemi
                    Attack();
                }
                else
                {
                    Debug.Log("Cannot attack during enemy turn.");
                }
                break;
            case "Block":
                Debug.Log(characterName + " is blocking.");
                isBlocking = true;
                isDodging = false;
                break;
            case "Dodge":
                Debug.Log(characterName + " is dodging.");
                isDodging = true;
                isBlocking = false;
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

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(characterName + " took " + damage + " damage. Health now: " + health);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Karakteri sahneden kaldır
        Debug.Log(characterName + " has died.");
        TurnManager.Instance.RemoveCharacterFromList(this);
        // Oyuncunun ölmesiyle ilgili diğer işlemler
    }

    private void Attack()
    {
        // Hareket sırasının en başındaki düşmana saldır
        Enemy target = TurnManager.Instance.GetFirstEnemy();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
        else
        {
            Debug.Log("No enemies to attack.");
        }
    }

    public bool IsBlocking()
    {
        return isBlocking;
    }

    public bool IsDodging()
    {
        return isDodging;
    }

    public void ResetDefensiveStates()
    {
        isBlocking = false;
        isDodging = false;
    }
}
