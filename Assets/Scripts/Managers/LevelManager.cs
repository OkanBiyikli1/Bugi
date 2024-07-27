using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening; // DOTween için gerekli

public class LevelManager : MonoBehaviour
{
    public int currentLevelIndex = 0; // Mevcut level indexi
    public List<LevelData> levels; // Tüm level'lar
    public List<Transform> positions; // Düşmanların yerleştirileceği pozisyonlar

    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private TextMeshProUGUI tipText;

    public int enemiesCount;

    [SerializeField] private CoinData coin;
    [SerializeField] private CoinUI coinUI;

    public static LevelManager instance;

    [SerializeField] private TextMeshProUGUI levelText; // Level bilgisi için TextMeshProUGUI
    [SerializeField] private GameObject levelTextBG; // Level bilgisi için arka plan (Image)

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
        // Kaydedilen seviyeyi yükleme ve 1 sonraki seviyeden başlatma
        currentLevelIndex = PlayerPrefs.GetInt("SavedLevel", 0);
        StartLevel(currentLevelIndex);

        enemiesCount = levels[currentLevelIndex].enemies.Count;
        nextLevelPanel.SetActive(false);

        ShowLevelText(); // Başlangıçta level bilgisini göster
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextLevel();
        }
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
            StartCoroutine(TurnManager.Instance.InitializeAfterDelay(1f));
            DisplayRandomTipandPanel();
            EarnedCoin(GameManager.Instance.earnedCoin);
            coinUI.DisplayCoinAmount();
            BoostManager.instance.RemoveTemporaryBoosts();
            Debug.Log("Next level, here we go!");

            // Her seviyeyi geçtikten sonra kaydet
            PlayerPrefs.SetInt("SavedLevel", currentLevelIndex);
            PlayerPrefs.Save(); // Verileri anında kaydetmek için kullanılır.
            Debug.Log("Level " + currentLevelIndex + " saved. Continue from next level.");

            ShowLevelText(); // Yeni level bilgisini göster
        }
    }

    public void UpdateEnemiesCount()
    {
        enemiesCount = levels[currentLevelIndex].enemies.Count;
        Debug.Log("Enemies Count: " + enemiesCount);
    }

    private void DisplayRandomTipandPanel()
    {
        int randomIndex = Random.Range(0, tips.Count);
        tipText.text = "Tip: " + tips[randomIndex];
        nextLevelPanel.SetActive(true);
    }

    public void EarnedCoin(int amount)
    {
        coin.coinAmount += amount;
    }

    private void ShowLevelText()
    {
        levelText.text = "Level: " + (currentLevelIndex + 1);
        levelTextBG.transform.localScale = Vector3.zero;
        levelTextBG.SetActive(true);
        
        Sequence sequence = DOTween.Sequence();
        sequence.Append(levelTextBG.transform.DOScale(0.5f, 0.3f).SetEase(Ease.OutBack))
                .AppendInterval(1f) // 1 saniye duraklama
                .Append(levelTextBG.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack))
                .OnComplete(() => levelTextBG.SetActive(false));
    }
}
