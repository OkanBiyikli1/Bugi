using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private List<MonoBehaviour> turnList = new List<MonoBehaviour>();
    private int currentTurnIndex = 0; // Sıradaki karakterin indeksini takip eder

    // Karakterleri sıraya ekler
    public void AddCharacterToList(MonoBehaviour character)
    {
        turnList.Add(character);
    }

    // Sıralama değerine göre karakterleri sıraya sokar
    public void SortCharactersByOrder()
    {
        turnList.Sort((x, y) =>
        {
            var xOrder = GetOrder(x);
            var yOrder = GetOrder(y);
            return xOrder.CompareTo(yOrder);
        });
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
        return int.MaxValue; // Varsayılan en büyük değer
    }

    // Sıradaki karakterlerin aksiyonlarını sırayla gerçekleştirir
    public IEnumerator ExecuteTurns()
    {
        while (GameManager.Instance.GetItemList().Count > 0)
        {
            var character = turnList[currentTurnIndex];

            if (character is Enemy enemy)
            {
                enemy.PerformAction();
            }

            if (GameManager.Instance.GetItemList().Count > 0)
            {
                GameManager.Instance.player.ExecutePlayerCommands();
            }

            yield return new WaitForSeconds(1); // Bir aksiyondan sonra bekleme süresi

            // currentTurnIndex değerini bir sonraki karaktere geçecek şekilde artır
            currentTurnIndex = (currentTurnIndex + 1) % turnList.Count;

            // Eğer liste tamamen işlendiyse ve sona ulaştıysa, currentTurnIndex sıfırlanmasın
            if (currentTurnIndex == 0 && GameManager.Instance.GetItemList().Count == 0)
            {
                yield break; // Liste boşsa döngüyü durdur
            }
        }
    }

    public void ResetTurnIndex()
    {
        // Turn sırasını bir sonraki karakterden başlatmak için currentTurnIndex değerini artır
        currentTurnIndex = (currentTurnIndex + 1) % turnList.Count;
    }
}
