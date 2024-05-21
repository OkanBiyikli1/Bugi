using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public int currentLevelIndex = 0; // Mevcut level indexi
    public List<LevelData> levels; // Tüm level'lar
    public List<Transform> positions; // Düşmanların yerleştirileceği pozisyonlar

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        StartCoroutine(StartLevelWithDelay(currentLevelIndex, .1f));
    }

    private IEnumerator StartLevelWithDelay(int levelIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartLevel(levelIndex);
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

        // Initialize managers with a slight delay
        StartCoroutine(InitializeManagersAfterDelay(1f));
    }

    private IEnumerator InitializeManagersAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnemyManager.Instance.Initialize();
        TurnManager.Instance.Initialize();
    }

    public void RestartLevel()
    {
        Initialize();
    }

    public void NextLevel()
    {
        if (currentLevelIndex + 1 < levels.Count)
        {
            currentLevelIndex++;
            Initialize();
        }
    }

    public void OnAllEnemiesDefeated()
    {
        Debug.Log("All enemies defeated. Proceeding to next level.");
        NextLevel();
    }
}
