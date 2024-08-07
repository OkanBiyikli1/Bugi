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

        LevelManager.instance.enemiesCount--;
    }

    private void Attack()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            if(stats.attackType == AttackType.Smash)
            {
                Debug.Log("enemy smashing");
                switch (player.playerDefenceType)
                {
                    case PlayerDefenceType.SliceDef:
                        player.TakeDamage(stats.damage);
                        break;
                    case PlayerDefenceType.SmashDef:
                        Debug.Log(player + " dodged the attack.");
                        break;
                    case PlayerDefenceType.None:
                        player.TakeDamage(stats.damage);
                        break;
                }
            }
            else if(stats.attackType == AttackType.Cutting)
            {
                Debug.Log("enemy cutting");
                switch (player.playerDefenceType)
                {
                    case PlayerDefenceType.SliceDef:
                        Debug.Log(player + " blocked the attack.");
                        break;
                    case PlayerDefenceType.SmashDef:
                        player.TakeDamage(stats.damage);
                        break;
                    case PlayerDefenceType.None:
                        player.TakeDamage(stats.damage);
                        break;
                }
            }
        }
    }
}
