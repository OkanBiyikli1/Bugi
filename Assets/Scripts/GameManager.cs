using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> itemList = new List<GameObject>();
    public Transform listContainer; // Objelerin ekleneceği parent transform
    public AutoScroll autoScroll;  // AutoScroll scripti referansı

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
        itemList.Add(newItem);
        Debug.Log("Item added: " + newItem.name);

        // Yeni item eklendiğinde otomatik olarak en aşağıya kaydır
        autoScroll.ScrollToBottom();
    }

    public void RemoveFromList(GameObject item)
    {
        if (itemList.Contains(item))
        {
            itemList.Remove(item);
            Debug.Log("Item removed: " + item.name);
        }
    }

    public List<GameObject> GetItemList()
    {
        return itemList;
    }
}
