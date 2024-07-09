using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    private int originalHealth; // Geçici can eklendiğinde orijinal canı tutar
    public int damage;
    [SerializeField] private int order;
    [SerializeField] private string characterName;
    public PlayerAttackType playerType;
    public PlayerDefenceType playerDefenceType;

    [SerializeField] public GameObject[] hearts; // Parent objeleri temsil eder

    public Animator playerAnim;

    void Start()
    {
        currentHealth = maxHealth;
        originalHealth = currentHealth; // Orijinal canı başlangıçta currentHealth olarak ayarla
        UpdateHeartsArray(); // maxHealth ile hearts dizisini eşleştir
        UpdateHeartsUI(); // currentHealth ile child objelerini eşleştir
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
        }
    }

    public int GetOrder()
    {
        return order;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Orijinal canın altına düşme durumu kontrol edilir
        if (currentHealth < originalHealth)
        {
            currentHealth = originalHealth;
        }

        playerAnim.SetBool("takeHit", true);
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
                            playerAnim.SetBool("sliceAttack", true);
                            Debug.Log("Wrong attack");
                            break;
                        case PlayerAttackType.Smash:
                            target.TakeDamage(damage);
                            playerAnim.SetBool("smashAttack", true);
                            Debug.Log("Smashed");
                            break;
                    }
                }
                else if (target.stats.defenceType == DefenceType.SmashDef)
                {
                    switch (playerType)
                    {
                        case PlayerAttackType.Slice:
                            target.TakeDamage(damage);
                            playerAnim.SetBool("sliceAttack", true);
                            Debug.Log("Sliced");
                            break;
                        case PlayerAttackType.Smash:
                            Debug.Log("Wrong attack");
                            playerAnim.SetBool("smashAttack", true);
                            break;
                    }
                }
            }
        }
        else
        {
            Debug.Log("Missed turn");
        }
    }

    public void AnimationBoolFalse()
    {
        playerAnim.SetBool("smashAttack", false);
        playerAnim.SetBool("sliceAttack", false);
        playerAnim.SetBool("takeHit", false);
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
            // Parent objenin child'ını alın
            Transform heartChild = hearts[i].transform.GetChild(0);

            // Parent objeyi maxHealth'e göre aktif/deaktif yap
            hearts[i].SetActive(i < maxHealth);

            // Child objeyi currentHealth'e göre aktif/deaktif yap
            if (heartChild != null)
            {
                heartChild.gameObject.SetActive(i < currentHealth);
            }
        }
    }

    public void UpdateHeartsArray()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < maxHealth);
        }
    }

    public void AddTemporaryHealth(int amount)
    {
        originalHealth = currentHealth; // Geçici can eklenmeden önce orijinal canı kaydet
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth; // maxHealth'i aşmamalı
        UpdateHeartsUI();
    }

    public void RemoveTemporaryHealth(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < originalHealth)
        {
            currentHealth = originalHealth; // Eğer currentHealth orijinal canın altına düşerse, orijinal cana ayarla
        }
        UpdateHeartsUI();
    }
}
