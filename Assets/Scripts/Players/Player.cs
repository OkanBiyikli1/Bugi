using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int damage;
    [SerializeField] private int order;
    public string characterName;
    public PlayerAttackType playerAttackType;
    public PlayerDefenceType playerDefenceType;

    public GameObject[] hearts; // Parent objeleri temsil eder

    public Animator playerAnim;

    void Start()
    {
        // Load player data first to set initial values
        LoadPlayerData();

        // Load boosts and apply their effects
        BoostManager.instance.LoadBoosts();

        // Ensure currentHealth is correctly initialized if not loaded
        if (currentHealth == 0)
        {
            currentHealth = maxHealth;
        }

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
                playerAttackType = PlayerAttackType.Slice;
                Attack();
                break;
            case "Smash":
                Debug.Log(characterName + " is smashing " + damage + " damage.");
                playerAttackType = PlayerAttackType.Smash;
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
        // Temporary Health Boostları kontrol et ve kaldır
        for (int i = BoostManager.instance.activeTemporaryBoosts.Count - 1; i >= 0 && damage > 0; i--)
        {
            var boost = BoostManager.instance.activeTemporaryBoosts[i];
            if (boost.boostType == BoostType.TemporaryHealth)
            {
                // Temporary Health Boost kaldır ve canı düşür
                BoostManager.instance.activeTemporaryBoosts.RemoveAt(i);
                currentHealth -= 1; // Boost kaldırıldığında currentHealth 1 azalır
                damage--;
                BoostManager.instance.SaveBoosts(); // Boostları kaydet
            }
        }

        // Eğer hasar kaldıysa, currentHealth'ten düş
        if (damage > 0)
        {
            currentHealth -= damage;
        }

        playerAnim.SetBool("takeHit", true);
        if (currentHealth < 0) currentHealth = 0;
        Debug.Log(characterName + " took " + damage + " damage. Health now: " + currentHealth);
        UpdateHeartsUI();
        SavePlayerData(); // Can verisini kaydet
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        Debug.Log(characterName + " healed by " + amount + ". Health now: " + currentHealth);
        UpdateHeartsUI();
        SavePlayerData(); // Can verisini kaydet
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
                    switch (playerAttackType)
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
                    switch (playerAttackType)
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
        playerAttackType = PlayerAttackType.None;
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

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("PlayerCurrentHealth", currentHealth);
        PlayerPrefs.SetInt("PlayerDamage", damage);
        PlayerPrefs.Save(); // Verileri anında kaydet

        Debug.Log("Player data saved. Health: " + currentHealth + ", Damage: " + damage);
    }

    void LoadPlayerData()
    {
        // Can ve hasar değerlerini yükle
        currentHealth = PlayerPrefs.GetInt("PlayerCurrentHealth", maxHealth);
        damage = PlayerPrefs.GetInt("PlayerDamage", damage);
        UpdateHeartsUI(); // UI'yi güncelle

        Debug.Log("Player data loaded. Health: " + currentHealth + ", Damage: " + damage);
    }

    public void AddTemporaryHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth; // maxHealth'i aşmamalı
        UpdateHeartsUI();
        //SavePlayerData(); // Can verisini kaydet
    }

    public void RemoveTemporaryHealth(int amount)
    {
        currentHealth -= amount;
        UpdateHeartsUI();
        //SavePlayerData(); // Can verisini kaydet
    }
}
