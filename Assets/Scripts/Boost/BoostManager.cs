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

    //[SerializeField] private GameObject healthObj;
    //[SerializeField] private Transform healthContainer;

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
    }

    void PurchaseBoost(BoostData boost)
    {
        if (coinData.coinAmount >= boost.cost)
        {
            coinData.coinAmount -= boost.cost;
            coinUI.DisplayCoinAmount();
            ApplyBoost(boost);
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    void ApplyBoost(BoostData boost)
    {
        switch (boost.boostType)
        {
            case BoostType.PermanentHealth:

                //player.maxHealth += boost.amount;
                if(player.currentHealth < player.maxHealth)
                {
                    player.currentHealth += boost.amount;
                    player.UpdateHeartsUI();
                }
                break;

            case BoostType.TemporaryHealth:

                if(player.currentHealth < player.maxHealth)
                {
                    player.currentHealth += boost.amount;
                    activeTemporaryBoosts.Add(boost);
                    player.UpdateHeartsUI();
                }
                break;

            case BoostType.TemporaryDamage:

                player.damage += boost.amount;
                activeTemporaryBoosts.Add(boost);
                break;

            case BoostType.PermanentDamage:

                player.damage += boost.amount;
                break;

            case BoostType.MaxHealthIncrease:

                player.maxHealth += boost.amount;
                player.UpdateHeartsUI();
                break;

            case BoostType.CoinMultiplier:

                GameManager.Instance.collectedCoin += boost.amount;
                break;
        }
    }

    public void RemoveTemporaryBoosts()
    {
        foreach (var boost in activeTemporaryBoosts)
        {
            switch (boost.boostType)
            {
                case BoostType.TemporaryHealth:
                    player.currentHealth -= boost.amount;
                    player.UpdateHeartsUI();
                    break;
                case BoostType.TemporaryDamage:
                    player.damage -= boost.amount;
                    break;
            }
        }
        activeTemporaryBoosts.Clear();
    }

    /*void IncreaseMaxHealthUI()
    {
        Instantiate(healthObj, healthContainer);
    }*/
}
