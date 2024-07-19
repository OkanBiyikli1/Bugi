using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DoTween kütüphanesini ekliyoruz
using UnityEngine.UI;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public List<MonoBehaviour> turnList = new List<MonoBehaviour>();
    [SerializeField] private int currentTurnIndex = 1; // Sıradaki karakterin indeksini takip eder
    [SerializeField] private GameObject bg; // Background GameObject'i
    [SerializeField] private Canvas canvas; // Oyun canvas'ı

    private Vector3 originalPositionPlayer;
    private Vector3 originalScalePlayer;
    private Vector3 originalPositionEnemy;
    private Vector3 originalScaleEnemy;

    [Header("UI")]
    [SerializeField] private GameObject turnListPanel; // Turn listesinin gösterileceği panel
    [SerializeField] private GameObject[] turnListItems; // Turn listesinde kullanılacak UI elemanları (image + text)
    [SerializeField] private Button openButton, closeButton; // Turn listesini açmak için kullanılan buton
    [SerializeField] private GameObject buttons;//turn list aktif edildiğinde kapatılacak itemler
    [SerializeField] private GameObject boostPanelButton;//Turn list açıldığında kapatılacak diğer button BU KISMI İLERDE DEĞİŞİCEZ

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
        StartCoroutine(InitializeAfterDelay(1f)); // Start the coroutine with a 1-second delay
        openButton.onClick.AddListener(OpenTurnListPanel); // OpenButton tıklama olayı
        closeButton.onClick.AddListener(CloseTurnListPanel);
        turnListPanel.SetActive(false);
    }

    public IEnumerator InitializeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        LevelManager.instance.UpdateEnemiesCount();
        AddAllCharactersToList();
        SortCharactersByOrder();
        ActivateIconForCurrent(); // Execute after delay
    }

    private void AddAllCharactersToList()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Player player = FindObjectOfType<Player>();

        foreach (var enemy in enemies)
        {
            AddCharacterToList(enemy);
        }

        if (player != null)
        {
            AddCharacterToList(player);
        }
        DeactivateAllIcons();
    }

    public void AddCharacterToList(MonoBehaviour character)
    {
        turnList.Add(character);
    }

    public void RemoveCharacterFromList(MonoBehaviour character)
    {
        if (turnList.Contains(character))
        {
            int index = turnList.IndexOf(character);
            turnList.Remove(character);
            if (index <= currentTurnIndex)
            {
                currentTurnIndex = Mathf.Max(currentTurnIndex - 1, 0);
            }
            SortCharactersByOrder();
        }
    }

    public void RemoveAllCharactersFromList()
    {
        turnList.Clear();
    }

    public void SortCharactersByOrder()
    {
        turnList.Sort((x, y) =>
        {
            var xOrder = GetOrder(x);
            var yOrder = GetOrder(y);
            return xOrder.CompareTo(yOrder);
        });

        UpdateTurnListUI();
    }

    private int GetOrder(MonoBehaviour character)
    {
        if (character is Player player)
        {
            return player.GetOrder();
        }
        if (character is Enemy enemy)
        {
            return enemy.GetOrder();
        }
        return int.MaxValue;
    }

    public IEnumerator ExecuteTurns()
    {
        while (true)
        {
            if (GameManager.Instance.GetItemList().Count == 0)
            {
                if(LevelManager.instance.enemiesCount == 0)
                {
                    //RemoveAllCharactersFromList();
                    turnList.Clear();
                    LevelManager.instance.NextLevel();
                    Debug.Log("durdum ve level geçtim");
                    yield break;
                }

                ActivateIconForCurrent();
                Debug.Log("Durdurdum");
                yield break;
            }
            else if(LevelManager.instance.enemiesCount == 0)
            {
                //RemoveAllCharactersFromList();
                turnList.Clear();
                LevelManager.instance.NextLevel();
                GameManager.Instance.ClearOrderList();
                Debug.Log("asfasmasafaz 2 2 2 2 ");
                yield break;
            }


            var character = turnList[currentTurnIndex];

            if (character == null)
            {
                currentTurnIndex++;
                if (currentTurnIndex >= turnList.Count)
                {
                    currentTurnIndex = 0;
                }
                continue;
            }

            Player player = FindAnyObjectByType<Player>();
            Enemy target = GetFirstEnemy();

            if (character is Player player1)
            {
                Debug.Log("Player oynuyor");
                originalPositionPlayer = player1.transform.position;
                originalScalePlayer = player1.transform.localScale;
                originalPositionEnemy = target.transform.position;
                originalScaleEnemy = target.transform.localScale;
                yield return StartCoroutine(ScaleAndMoveToCenter(player1, target));
                yield return new WaitForSeconds(2f);
                player1.ExecutePlayerCommands();
                yield return new WaitForSeconds(1f);
                player1.AnimationBoolFalse();
                yield return StartCoroutine(ScaleAndMoveBack(player1, target));
            }

            if (character is Enemy enemy)
            {
                Debug.Log("Enemy oynuyor");
                if (player != null)
                {
                    originalPositionEnemy = enemy.transform.position;
                    originalScaleEnemy = enemy.transform.localScale;
                    originalPositionPlayer = player.transform.position;
                    originalScalePlayer = player.transform.localScale;
                    yield return StartCoroutine(ScaleAndMoveToCenter(player, enemy));
                    player.ExecutePlayerCommands();
                    yield return new WaitForSeconds(2f);
                    enemy.PerformAction();
                    yield return new WaitForSeconds(1f);
                    player.AnimationBoolFalse();
                    yield return StartCoroutine(ScaleAndMoveBack(player, enemy));
                }
            }

            player.ResetDefensiveStates();

            currentTurnIndex++;
            if (currentTurnIndex >= turnList.Count)
            {
                currentTurnIndex = 0;
            }

            GameManager.Instance.RemoveFirstCommandFromList();
            yield return new WaitForSeconds(.5f);
            // GameManager.Instance.RemoveFirstCommandFromList();
            // yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator ScaleAndMoveToCenter(Player player, Enemy enemy)
    {
        // Ekran pozisyonlarını hesapla
        Vector2 playerTargetPosition = new Vector2(Screen.width * 0.25f, Screen.height * 0.5f);
        Vector2 enemyTargetPosition = new Vector2(Screen.width * 0.75f, Screen.height * 0.5f);

        // Ekran pozisyonlarını dünya pozisyonlarına çevir
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, playerTargetPosition, canvas.worldCamera, out Vector3 playerWorldPosition);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, enemyTargetPosition, canvas.worldCamera, out Vector3 enemyWorldPosition);

        bg.SetActive(true); // Arka planı aktif hale getir

        // Oyuncu ve düşmanı merkeze taşı ve ölçekle
        player.transform.DOMove(playerWorldPosition, 0.5f).SetEase(Ease.InOutQuad);
        enemy.transform.DOMove(enemyWorldPosition, 0.5f).SetEase(Ease.InOutQuad);

        player.transform.DOScale(player.transform.localScale * 4, 0.5f).SetEase(Ease.InOutQuad);
        enemy.transform.DOScale(enemy.transform.localScale * 4, 0.5f).SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(1f); // Ölçekleme ve taşımanın tamamlanmasını bekle
    }

    private IEnumerator ScaleAndMoveBack(Player player, Enemy enemy)
    {
        Debug.Log("Back");

        // Oyuncu ve düşmanı orijinal pozisyonlarına ve ölçeklerine geri taşı
        if (player != null)
        {
            player.transform.DOScale(originalScalePlayer, 0.5f).SetEase(Ease.InOutQuad);
            player.transform.DOMove(originalPositionPlayer, 0.5f).SetEase(Ease.InOutQuad);
        }

        if (enemy != null)
        {
            enemy.transform.DOScale(originalScaleEnemy, 0.5f).SetEase(Ease.InOutQuad);
            enemy.transform.DOMove(originalPositionEnemy, 0.5f).SetEase(Ease.InOutQuad);
        }

        yield return new WaitForSeconds(1f); // Ölçekleme ve geri taşımanın tamamlanmasını bekle

        bg.SetActive(false); // Arka planı pasif hale getir
    }

    private void ActivateIconForCurrent()
    {
        GameObject currentCharacter = turnList[currentTurnIndex].gameObject;
        Transform turnIconParent = currentCharacter.transform.Find("TurnIconParent");
        turnIconParent.GetChild(0).gameObject.SetActive(true);
    }

    public void DeactivateAllIcons()
    {
        foreach (var character in turnList)
        {
            Transform turnIconParent = character.gameObject.transform.Find("TurnIconParent");
            turnIconParent.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void ResetTurnIndex()
    {
        currentTurnIndex = (currentTurnIndex + 1) % turnList.Count;
        ActivateIconForCurrent(); // Turn index sıfırlandığında yeni aktif karakterin ikonunu aktifleştir
    }

    public bool IsPlayerTurn()
    {
        return turnList[currentTurnIndex] is Player;
    }

    public Enemy GetFirstEnemy()
    {
        foreach (var character in turnList)
        {
            if (character is Enemy enemy)
            {
                return enemy;
            }
        }
        return null;
    }

    private void OpenTurnListPanel()
    {
        turnListPanel.SetActive(true);
        UpdateTurnListUI();
        buttons.SetActive(false);
        boostPanelButton.SetActive(false);
        openButton.gameObject.SetActive(false);
    }

    private void CloseTurnListPanel()
    {
        turnListPanel.SetActive(false);
        buttons.SetActive(true);
        boostPanelButton.SetActive(true);
        openButton.gameObject.SetActive(true);
    }

    private void UpdateTurnListUI()
    {
        for (int i = 0; i < turnListItems.Length; i++)
        {
            if (i < turnList.Count)
            {
                turnListItems[i].SetActive(true);

                Image img = turnListItems[i].GetComponent<Image>();
                TextMeshProUGUI txt = turnListItems[i].GetComponentInChildren<TextMeshProUGUI>();

                if (turnList[i] is Player player)
                {
                    img.sprite = player.GetComponent<SpriteRenderer>().sprite;
                    txt.text = player.characterName;
                }
                else if (turnList[i] is Enemy enemy)
                {
                    img.sprite = enemy.stats.sprite;
                    txt.text = enemy.stats.charName;
                }
            }
            else
            {
                turnListItems[i].SetActive(false);
            }
        }

        Debug.Log("tur siralamasi UI'da yapildi");
    }
}
