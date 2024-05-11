using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStats", menuName = "EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public string charName;
    public int health;
    public int damage;
    public int order;
    public Sprite sprite; // Karakterin görselini tutan sprite
}
