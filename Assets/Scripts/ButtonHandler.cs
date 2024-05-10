using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject attackPrefab;
    public GameObject blockPrefab;
    public GameObject dodgePrefab;

    public Button attackButton;
    public Button blockButton;
    public Button dodgeButton;

    void Start()
    {
        attackButton.onClick.AddListener(() => AddItem("Attack", attackPrefab));
        blockButton.onClick.AddListener(() => AddItem("Block", blockPrefab));
        dodgeButton.onClick.AddListener(() => AddItem("Dodge", dodgePrefab));
    }

    void AddItem(string command, GameObject itemPrefab)
    {
        GameManager.Instance.AddToList(itemPrefab);
        GameManager.Instance.player.AddCommand(command);
    }
}
