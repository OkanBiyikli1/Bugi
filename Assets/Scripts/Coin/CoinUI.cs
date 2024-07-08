using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinUI : MonoBehaviour
{
    public CoinData coin;
    [SerializeField] private TextMeshProUGUI coinText;

    public void DisplayCoinAmount()
    {
        coinText.text = coin.coinAmount.ToString();
    }

    void Start()
    {
        DisplayCoinAmount();
    }
}
