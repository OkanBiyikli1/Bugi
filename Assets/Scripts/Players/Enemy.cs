using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    private int currentHealth;

    void Start()
    {
        if (stats != null)
        {
            currentHealth = stats.health;
            StartCoroutine(AddEnemyWithDelay(1f)); // 1 saniye gecikme ile enemy ekleme
        }
    }

    private IEnumerator AddEnemyWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnemyManager.Instance.AddEnemy(this);
    }

    public void PerformAction()
    {
        Debug.Log(stats.charName + " is attacking with " + stats.damage + " damage.");
        Attack();
    }

    public int GetOrder()
    {
        return stats.order;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(stats.charName + " took " + damage + " damage. Health now: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(stats.charName + " has died.");
        TurnManager.Instance.RemoveCharacterFromList(this);
        EnemyManager.Instance.RemoveEnemy(this);
        Destroy(gameObject);
    }

    private void Attack()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            switch (stats.attackType)
            {
                case AttackType.Smash:
                    if (!player.IsDodging())
                    {
                        player.TakeDamage(stats.damage);
                    }
                    else
                    {
                        Debug.Log(player + " dodged the attack.");
                    }
                    break;
                case AttackType.Cutting:
                    if (!player.IsBlocking())
                    {
                        player.TakeDamage(stats.damage);
                    }
                    else
                    {
                        Debug.Log(player + " blocked the attack.");
                    }
                    break;
            }
            GameManager.Instance.RemoveFirstCommandFromList();
        }
    }
}
