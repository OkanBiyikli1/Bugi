using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int damage;
    [SerializeField] private int order;
    [SerializeField] private string characterName;
    public PlayerAttackType playerType;
    public PlayerDefenceType playerDefenceType;

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
        switch (action)//OLUŞTURULAN PREFABLERİN BURADAKİ STRİNG'LER İLE AYNI İSMİ TAŞIYO OLMALI
        {
            case "Slice":
                Debug.Log(characterName + " is slicing " + damage + " damage.");
                playerType = PlayerAttackType.Slice;
                Attack();
                break;
            case "Smash":
                Debug.Log(characterName + " is smashing " + damage + " damage.");
                playerType = PlayerAttackType.Smash;
                Attack();
                break;
            case "Block":
                Debug.Log(characterName + " is blocking.");
                playerDefenceType = PlayerDefenceType.SliceDef;
                break;
            case "Dodge":
                Debug.Log(characterName + " is dodging.");
                playerDefenceType = PlayerDefenceType.SmashDef;
                break;
            default:
                Debug.Log("Unknown action: " + action);
                break;
        }
    }

    public void ExecutePlayerCommands()
    {
        if (GameManager.Instance.GetItemList().Count > 0)
        {
            string command = GameManager.Instance.GetItemList()[0].name;
            PerformAction(command);
            GameManager.Instance.RemoveFirstCommandFromList();
        }
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
                if (target.stats.defenceType == DefenceType.SliceDef)
                {
                    switch (playerType)
                    {
                        case PlayerAttackType.Slice:
                            Debug.Log("Yanlış saldırı");
                            break;
                        case PlayerAttackType.Smash:
                            target.TakeDamage(damage);
                            Debug.Log("Smashledim");
                            break;
                    }
                }
                else if (target.stats.defenceType == DefenceType.SmashDef)
                {
                    switch (playerType)
                    {
                        case PlayerAttackType.Slice:
                            target.TakeDamage(damage);
                            Debug.Log("Sliceledim");
                            break;
                        case PlayerAttackType.Smash:
                            Debug.Log("Yanlış saldırı");
                            break;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Sırayı kaçırdın");
        }
    }

    public void ResetDefensiveStates()
    {
        playerDefenceType = PlayerDefenceType.None;
        playerType = PlayerAttackType.None;
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
