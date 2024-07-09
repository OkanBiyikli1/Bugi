using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoostManager : MonoBehaviour
{
    public Player player;
    public CoinData coinData;
    public CoinUI coinUI;

    [SerializeField] private List<BoostData> boostDataList; // Boost verilerini tutan liste
    [SerializeField] private List<Button> boostButtons; // UI buttonlarını tutan liste

    [SerializeField] private List<BoostData> activeTemporaryBoosts = new List<BoostData>();

    [Header("UI Stuff")]
    [SerializeField] private GameObject boostPanel;
    [SerializeField] private Button openPanelButton;
    [SerializeField] private Button closePanelButton;
    [SerializeField] private GameObject buttons;

    public TextMeshProUGUI popUpText;
    public GameObject popUp;

    public static BoostManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        for (int i = 0; i < boostButtons.Count; i++)
        {
            int index = i; // Closure sorununu önlemek için gerekli
            boostButtons[i].onClick.AddListener(() => PurchaseBoost(boostDataList[index]));

            // Child objeleri olan TextMeshPro bileşenlerini alalım
            TextMeshProUGUI descriptionText = boostButtons[i].transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI costText = boostButtons[i].transform.Find("CostText").GetComponent<TextMeshProUGUI>();

            // TextMeshPro bileşenlerine değerleri atayalım
            descriptionText.text = boostDataList[i].description;
            costText.text = boostDataList[i].cost.ToString();
        }

        boostPanel.SetActive(false);
        openPanelButton.onClick.AddListener(ActivatePanel);
        closePanelButton.onClick.AddListener(ClosePanel);
        popUp.SetActive(false);
    }

    void PurchaseBoost(BoostData boost)
    {
        if (coinData.coinAmount >= boost.cost)
        {
            coinData.coinAmount -= boost.cost;
            coinUI.DisplayCoinAmount();
            ApplyBoost(boost);
            ClosePanel();
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    void ApplyBoost(BoostData boost)
    {
        string boostText = "";

        switch (boost.boostType)
        {
            case BoostType.PermanentHealth:
                if (player.currentHealth < player.maxHealth)
                {
                    player.currentHealth += boost.amount;
                    player.UpdateHeartsUI();
                    boostText = "+" + boost.amount + " Health Up";
                }else
                {
                    boostText = "Max Health reached";
                }
                break;
            case BoostType.TemporaryHealth:
                if (player.currentHealth < player.maxHealth)
                {
                    player.AddTemporaryHealth(boost.amount);
                    activeTemporaryBoosts.Add(boost);
                    boostText = "+" + boost.amount + " Temporary Health";
                }else
                {
                    boostText = "Max Health reached";
                }
                break;
            case BoostType.TemporaryDamage:
                player.damage += boost.amount;
                activeTemporaryBoosts.Add(boost);
                boostText = "+" + boost.amount + " Temporary Damage";
                break;
            case BoostType.PermanentDamage:
                player.damage += boost.amount;
                boostText = "+" + boost.amount + " Damage Up";
                break;
            case BoostType.MaxHealthIncrease:
                if (player.maxHealth < player.hearts.Length)
                {
                    player.maxHealth += boost.amount;
                    player.UpdateHeartsArray(); // maxHealth arttığında hearts dizisini güncelle
                    player.UpdateHeartsUI(); // maxHealth arttığında currentHealth'i de güncelle
                    boostText = "+" + boost.amount + " Max Health";
                }else
                {
                    boostText = "Max Health reached";
                }
                break;
            case BoostType.CoinMultiplier:
                GameManager.Instance.collectedCoin += boost.amount;
                boostText = "+" + boost.amount + " Coin Multiplier";
                break;
        }

        if (!string.IsNullOrEmpty(boostText))
        {
            ShowPopUp(boostText);
        }
    }

    void ShowPopUp(string text)
    {
        popUpText.text = text;
        popUp.SetActive(true);
        Invoke("HidePopUp", 1.0f); // 1 saniye sonra HidePopUp fonksiyonunu çağır
    }

    void HidePopUp()
    {
        popUp.SetActive(false);
    }

    public void RemoveTemporaryBoosts()
    {
        foreach (var boost in activeTemporaryBoosts)
        {
            switch (boost.boostType)
            {
                case BoostType.TemporaryHealth:
                    player.RemoveTemporaryHealth(boost.amount);
                    break;
                case BoostType.TemporaryDamage:
                    player.damage -= boost.amount;
                    break;
            }
        }
        activeTemporaryBoosts.Clear();
    }

    private void ActivatePanel()
    {
        boostPanel.SetActive(true);
        openPanelButton.gameObject.SetActive(false);
        buttons.SetActive(false);

        //boostPanel.transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuad);
    }

    private void ClosePanel()
    {
        boostPanel.SetActive(false);
        openPanelButton.gameObject.SetActive(true);
        buttons.SetActive(true);

        //boostPanel.transform.DOScale(0, 0.5f).SetEase(Ease.InOutQuad);
    }
}
