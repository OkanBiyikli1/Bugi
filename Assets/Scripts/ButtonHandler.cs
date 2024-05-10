using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public GameObject attackPrefab;
    public GameObject defencePrefab;
    public GameObject dodgePrefab;

    public Button attackButton;
    public Button defenceButton;
    public Button dodgeButton;

    void Start()
    {
        attackButton.onClick.AddListener(() => AddItem(attackPrefab));
        defenceButton.onClick.AddListener(() => AddItem(defencePrefab));
        dodgeButton.onClick.AddListener(() => AddItem(dodgePrefab));
    }

    void AddItem(GameObject itemPrefab)
    {
        GameManager.Instance.AddToList(itemPrefab);
    }
}
