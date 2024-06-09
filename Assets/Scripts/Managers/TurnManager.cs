using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DoTween kütüphanesini ekliyoruz

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public List<MonoBehaviour> turnList = new List<MonoBehaviour>();
    [SerializeField] private int currentTurnIndex = 1; // Sıradaki karakterin indeksini takip eder

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
        StartCoroutine(InitializeAfterDelay(1f)); // Start the coroutine with a 2-second delay
    }

    IEnumerator InitializeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

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

    public void SortCharactersByOrder()
    {
        turnList.Sort((x, y) =>
        {
            var xOrder = GetOrder(x);
            var yOrder = GetOrder(y);
            return xOrder.CompareTo(yOrder);
        });
        //ActivateIconForCurrent(); // Liste sıralandığında aktif karakterin ikonunu aktifleştir
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
                ActivateIconForCurrent();
                Debug.Log("Durdurdum");
                yield break;
            }

            var character = turnList[currentTurnIndex];

            if (character is Player player)
            {
                Enemy target = GetFirstEnemy();
                if (target != null)
                {
                    yield return StartCoroutine(AnimateAndPerformAction(player.transform, target.transform, () => player.ExecutePlayerCommands()));
                }
            }

            if (character is Enemy enemy)
            {
                Player target = FindObjectOfType<Player>();
                if (target != null)
                {
                    yield return StartCoroutine(AnimateAndPerformAction(enemy.transform, target.transform, () => {
                        if (GameManager.Instance.GetItemList().Count > 0)
                        {
                            GameManager.Instance.player.ExecutePlayerCommands();
                        }
                        enemy.PerformAction();
                    }));
                }
            }

            Player playerRef = FindObjectOfType<Player>();
            playerRef.ResetDefensiveStates();

            currentTurnIndex++;
            if(currentTurnIndex >= turnList.Count)
            {
                currentTurnIndex = 0;
            }

            GameManager.Instance.RemoveFirstCommandFromList();
        }
    }

    private IEnumerator AnimateAndPerformAction(Transform attacker, Transform target, System.Action action)
    {
        Vector3 originalScaleAttacker = attacker.localScale;
        Vector3 originalScaleTarget = target.localScale;
        
        // Saldırı yapan ve hedef karakterin ölçeğini büyütme
        attacker.DOScale(originalScaleAttacker * 2, 0.5f);
        target.DOScale(originalScaleTarget * 2, 0.5f);
        yield return new WaitForSeconds(0.5f); // Animasyonun bitmesini bekleme

        action.Invoke(); // Aksiyonu gerçekleştirme
        yield return new WaitForSeconds(1f); // Aksiyonun bitmesini bekleme

        // Saldırı yapan ve hedef karakterin ölçeğini normale döndürme
        attacker.DOScale(originalScaleAttacker, 0.5f);
        target.DOScale(originalScaleTarget, 0.5f);
        yield return new WaitForSeconds(0.5f); // Animasyonun bitmesini bekleme
    }

    private void ActivateIconForCurrent()
    {
        //DeactivateAllIcons();
        GameObject currentCharacter = turnList[currentTurnIndex].gameObject;
        Transform turnIconParent = currentCharacter.transform.Find("TurnIconParent");
        turnIconParent.GetChild(0).gameObject.SetActive(true);
        //Debug.Log("Ikonlar aktif");
    }

    public void DeactivateAllIcons()
    {
        foreach (var character in turnList)
        {
            Transform turnIconParent = character.gameObject.transform.Find("TurnIconParent");
            turnIconParent.GetChild(0).gameObject.SetActive(false);
            //Debug.Log("Ikonlar aktif degil");
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
}
