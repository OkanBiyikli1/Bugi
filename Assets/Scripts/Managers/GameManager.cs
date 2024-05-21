using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> orderList = new List<GameObject>();
    public Transform listContainer; // Objelerin ekleneceği parent transform
    public AutoScroll autoScroll;  // AutoScroll scripti referansı

    public Player player;
    private Coroutine turnCoroutine;

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

    public void AddToList(GameObject itemPrefab)
    {
        GameObject newItem = Instantiate(itemPrefab, listContainer);
        orderList.Add(newItem);
        //Debug.Log("Item added: " + newItem.name);

        // Yeni item eklendiğinde otomatik olarak en aşağıya kaydır
        autoScroll.ScrollToBottom();
    }

    public void RemoveFromList(GameObject item)
    {
        if (orderList != null && orderList.Contains(item))
        {
            orderList.Remove(item);
            //Debug.Log("Item removed: " + item.name);
        }
    }

    public void RemoveFirstCommandFromList()
    {
        if (orderList.Count > 0)
        {
            GameObject firstItem = orderList[0];
            orderList.RemoveAt(0);
            Destroy(firstItem);
            //Debug.Log("First item removed");

            if (orderList.Count == 0 && turnCoroutine != null)
            {
                StopCoroutine(turnCoroutine);
                TurnManager.Instance.ResetTurnIndex(); // Turn sırasını bir sonraki karakterden başlat
                //Debug.Log("No more commands. Stopping turns.");
            }
        }
    }

    public List<GameObject> GetItemList()
    {
        return orderList;
    }

    public bool CanStartGeneration()
    {
        return orderList.Count > 0;//BURASI BELKİ İLERDE 1 OLARAK AYARLANABİLİR
    }

    public void OnGenerateButtonClicked()
    {
        if (CanStartGeneration())
        {
            // Komutları işleme başla
            if (turnCoroutine != null)
            {
                StopCoroutine(turnCoroutine);
            }
            turnCoroutine = StartCoroutine(TurnManager.Instance.ExecuteTurns());
            TurnManager.Instance.DeactivateAllIcons();
        }
        else
        {
            Debug.Log("1'den fazla koymalisiniz.");
        }
    }
}
