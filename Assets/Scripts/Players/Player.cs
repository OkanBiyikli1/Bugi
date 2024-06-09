using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int damage;
    [SerializeField] private int order;
    [SerializeField] private string characterName; // Karakterin ismi
    private Queue<string> playerCommands = new Queue<string>();
    private bool isBlocking;
    private bool isDodging;

    [SerializeField] private Image[] hearts;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(2);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(2);
        }
    }

    public void PerformAction(string action)
    {
        switch (action)
        {
            case "Attack":
                Debug.Log(characterName + " is attacking with " + damage + " damage.");
                Attack();
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
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        Debug.Log(characterName + " took " + damage + " damage. Health now: " + currentHealth);
        UpdateHeartsUI();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(characterName + " has died.");
        TurnManager.Instance.RemoveCharacterFromList(this);
    }

    private void Attack()
    {
        if (TurnManager.Instance.IsPlayerTurn())
        {
            Enemy target = TurnManager.Instance.GetFirstEnemy();
            if (target != null)
            {
                /*TurnManager.Instance.StartCoroutine(TurnManager.Instance.AnimateAndPerformAction(transform, target.transform, () => {
                    target.TakeDamage(damage);
                }));*/
                target.TakeDamage(damage);
            }
        }
        else
        {
            Debug.Log("Sırayı kaçırdın");
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

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        Debug.Log(characterName + " healed by " + amount + ". Health now: " + currentHealth);
        UpdateHeartsUI();
    }

    public void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].gameObject.SetActive(true);
            }
            else
            {
                hearts[i].gameObject.SetActive(false);
            }
        }
    }
}
