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
        newItem.name = itemPrefab.name;  // Prefab ismini komut olarak kullanıyoruz
        orderList.Add(newItem);

        // Yeni item eklendiğinde otomatik olarak en aşağıya kaydır
        autoScroll.ScrollToBottom();
    }

    public void RemoveFromList(GameObject item)
    {
        if (orderList.Contains(item))
        {
            orderList.Remove(item);
        }
    }

    public void ClearOrderList()
    {
        foreach (GameObject item in orderList)
        {
            Destroy(item);
        }
        orderList.Clear();
    }

    public void RemoveFirstCommandFromList()
    {
        if (orderList.Count > 0)
        {
            GameObject firstItem = orderList[0];
            orderList.RemoveAt(0);
            Destroy(firstItem);
        }
    }

    public List<GameObject> GetItemList()
    {
        return orderList;
    }

    public void OnGenerateButtonClicked()
    {
        if (orderList.Count > 1)
        {
            StartCoroutine(TurnManager.Instance.ExecuteTurns());
            TurnManager.Instance.DeactivateAllIcons();
        }
        else
        {
            Debug.Log("Order list is empty. Cannot start generation.");
        }
    }
}
