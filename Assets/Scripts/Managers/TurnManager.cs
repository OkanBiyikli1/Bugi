using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    private List<MonoBehaviour> turnList = new List<MonoBehaviour>();
    private int currentTurnIndex = 0; // Sıradaki karakterin indeksini takip eder

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
        AddAllCharactersToList();
        SortCharactersByOrder();
        ActivateIconForCurrent(); // Oyun başladığında aktif karakterin ikonunu aktifleştir
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
        ActivateIconForCurrent(); // Liste sıralandığında aktif karakterin ikonunu aktifleştir
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
            var character = turnList[currentTurnIndex];

            if (character is Player player)
            {
                player.ExecutePlayerCommands();
            }

            if (character is Enemy enemy)
            {
                if (GameManager.Instance.GetItemList().Count > 0)
                {
                    GameManager.Instance.player.ExecutePlayerCommands();
                }

                yield return new WaitForSeconds(1);

                enemy.PerformAction();
            }

            yield return new WaitForSeconds(3);
            currentTurnIndex = (currentTurnIndex + 1) % turnList.Count;

            if (currentTurnIndex == 0)
            {
                Player playerRef = FindObjectOfType<Player>();
                if (playerRef != null)
                {
                    playerRef.ResetDefensiveStates();
                }

                if (GameManager.Instance.GetItemList().Count == 0)
                {
                    DeactivateAllIcons(); // Tüm ikonları deaktifleştir
                    yield break; // Liste boşsa döngüyü durdur
                }
            }
        }
    }

    private void ActivateIconForCurrent()
    {
        DeactivateAllIcons();
        GameObject currentCharacter = turnList[currentTurnIndex].gameObject;
        currentCharacter.transform.Find("TurnIcon").gameObject.SetActive(true);
    }

    public void DeactivateAllIcons()
    {
        foreach (var character in turnList)
        {
            character.gameObject.transform.Find("TurnIcon").gameObject.SetActive(false);
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
