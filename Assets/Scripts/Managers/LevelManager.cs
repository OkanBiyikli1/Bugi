using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int currentLevelIndex = 0; // Mevcut level indexi
    public List<LevelData> levels; // Tüm level'lar
    public List<Transform> positions; // Düşmanların yerleştirileceği pozisyonlar

    void Start()
    {
        StartLevel(currentLevelIndex);
    }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            LevelData levelData = levels[levelIndex];
            for (int i = 0; i < levelData.enemies.Count && i < positions.Count; i++)
            {
                Instantiate(levelData.enemies[i], positions[i].position, Quaternion.identity, positions[i]);
            }
        }
    }

    public void RestartLevel()
    {
        StartLevel(currentLevelIndex);
    }

    public void NextLevel()
    {
        if (currentLevelIndex + 1 < levels.Count)
        {
            currentLevelIndex++;
            StartLevel(currentLevelIndex);
        }
    }
}
