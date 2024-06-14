using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject slicePrefab;
    public GameObject blockPrefab;
    public GameObject dodgePrefab;
    public GameObject smashPrefab;

    public Button sliceButton;
    public Button blockButton;
    public Button dodgeButton;
    public Button smashButton;

    void Start()
    {
        sliceButton.onClick.AddListener(() => AddItem("Slice", slicePrefab));
        blockButton.onClick.AddListener(() => AddItem("Block", blockPrefab));
        dodgeButton.onClick.AddListener(() => AddItem("Dodge", dodgePrefab));
        smashButton.onClick.AddListener(() => AddItem("Smash", smashPrefab));
    }

    void AddItem(string command, GameObject itemPrefab)
    {
        GameManager.Instance.AddToList(itemPrefab);
        GameManager.Instance.player.AddCommand(command);
    }
}
