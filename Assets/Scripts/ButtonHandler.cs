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
        sliceButton.onClick.AddListener(() => AddItem(slicePrefab));
        blockButton.onClick.AddListener(() => AddItem(blockPrefab));
        dodgeButton.onClick.AddListener(() => AddItem(dodgePrefab));
        smashButton.onClick.AddListener(() => AddItem(smashPrefab));
    }

    void AddItem(GameObject itemPrefab)
    {
        GameManager.Instance.AddToList(itemPrefab);
    }
}
