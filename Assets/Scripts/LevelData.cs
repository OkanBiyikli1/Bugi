using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level", order = 0)]
public class LevelData : ScriptableObject
{
    public List<GameObject> enemies; // Bu level'de kullanılacak düşman prefab'ları
}
