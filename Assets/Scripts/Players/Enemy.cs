using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats stats;
    private int currentHealth;

    void Start()
    {
        if (stats != null)
        {
            currentHealth = stats.health;
            // Görseli ayarlamak için sprite renderer veya UI image kullanabilirsiniz
            // GetComponent<SpriteRenderer>().sprite = stats.sprite;
        }
    }

    public void PerformAction()
    {
        // Düşmanın saldırı işlemleri
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
        // Karakteri sahneden kaldır
        Debug.Log(stats.charName + " has died.");
        TurnManager.Instance.RemoveCharacterFromList(this);
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
                    // Minotaur saldırısı, dodge yapılmazsa canı azaltır
                    if (!player.IsDodging())
                    {
                        player.TakeDamage(stats.damage);
                    }
                    else
                    {
                        Debug.Log(player.characterName + " dodged the attack.");
                    }
                    break;
                case AttackType.Cutting:
                    // Diğer yaratıkların saldırısı, block yapılmazsa canı azaltır
                    if (!player.IsBlocking())
                    {
                        player.TakeDamage(stats.damage);
                    }
                    else
                    {
                        Debug.Log(player.characterName + " blocked the attack.");
                    }
                    break;
            }
        }
    }
}
