using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Stats stats;

    void Start()
    {
        stats = GetComponent<Stats>();
    }

    public void PerformAction()
    {
        // Düşmanın saldırı işlemleri
        Debug.Log(stats.characterName + " is attacking");
    }

    public int GetOrder()
    {
        return stats.order;
    }
}
