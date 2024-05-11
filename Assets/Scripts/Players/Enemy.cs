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
    }

    public int GetOrder()
    {
        return stats.order;
    }
}
