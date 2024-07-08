using UnityEngine;

[CreateAssetMenu(fileName = "Boost", menuName = "Boost", order = 12)]
public class BoostData : ScriptableObject
{
    public string description;
    public int cost;
    public BoostType boostType;
    public int amount;
}

public enum BoostType
{
    PermanentHealth,
    TemporaryHealth,
    TemporaryDamage,
    PermanentDamage,
    MaxHealthIncrease,
    CoinMultiplier
}
