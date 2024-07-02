using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public int currentLevelIndex = 0; // Mevcut level indexi
    public List<LevelData> levels; // Tüm level'lar
    public List<Transform> positions; // Düşmanların yerleştirileceği pozisyonlar

    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private TextMeshProUGUI tipText;

    public int enemiesCount;

    public static LevelManager instance;

    [SerializeField] private List<string> tips = new List<string>
    {
        "Attack goblins with smash!",
        "Attack skeletons with smash!",
        "Attack minotaurs with slice!",
        "Attack spiders with smash!",
        "You can only attack when it is your turn.",
        "Defend against goblins with block!",
        "Defend against skeletons with block!",
        "Defend against minotaurs with dodge!",
        "Defend against spiders with block!",
        "Always attack the enemy at the beginning of the turn order!"
    };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartLevel(currentLevelIndex);

        enemiesCount = levels[currentLevelIndex].enemies.Count;
        nextLevelPanel.SetActive(false);
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
            //InitializeLevelManager();
            StartCoroutine(TurnManager.Instance.InitializeAfterDelay(1f));
            nextLevelPanel.SetActive(true);
            DisplayRandomTip();
            Debug.Log("next level babyyyyy");
        }
    }

    public void UpdateEnemiesCount()
    {
        enemiesCount = levels[currentLevelIndex].enemies.Count;
        Debug.Log("Enemies Count: " + enemiesCount);
    }

    private void DisplayRandomTip()
    {
        int randomIndex = Random.Range(0, tips.Count);
        tipText.text = "Tip: " + tips[randomIndex];
    }
}
