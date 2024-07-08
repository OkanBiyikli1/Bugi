using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BoostUI : MonoBehaviour
{
    [SerializeField] private GameObject boostPanel;
    [SerializeField] private Button panelButton;
    [SerializeField] private Button closePanelButton;
    [SerializeField] private GameObject buttons;
    // Start is called before the first frame update
    void Start()
    {
        boostPanel.SetActive(false);
        panelButton.onClick.AddListener(ActivatePanel);
        closePanelButton.onClick.AddListener(ClosePanel);
    }

    private void ActivatePanel()
    {
        boostPanel.SetActive(true);
        panelButton.gameObject.SetActive(false);
        buttons.SetActive(false);

        //boostPanel.transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuad);
    }

    private void ClosePanel()
    {
        boostPanel.SetActive(false);
        panelButton.gameObject.SetActive(true);
        buttons.SetActive(true);

        //boostPanel.transform.DOScale(0, 0.5f).SetEase(Ease.InOutQuad);
    }
}
