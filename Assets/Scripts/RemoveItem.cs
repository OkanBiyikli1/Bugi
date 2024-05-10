using UnityEngine;
using UnityEngine.UI;

public class RemoveItem : MonoBehaviour
{
    private Button button;
    private GameManager gameManager;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnRemove);
        gameManager = GameManager.Instance;
    }

    public void OnRemove()
    {
        gameManager.RemoveFromList(gameObject);
        Destroy(gameObject);
    }
}
