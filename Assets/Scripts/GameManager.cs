using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text productsText;
    [SerializeField] private Button produceButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button upgradeButton;

    private int money = 0;
    private int products = 0;
    private int productionPerClick = 1;
    private int sellPrice = 5;
    private int upgradeCost = 25;

    private void Awake()
    {
        produceButton.onClick.AddListener(Produce);
        sellButton.onClick.AddListener(Sell);
        upgradeButton.onClick.AddListener(Upgrade);

        UpdateUI();
    }

    private void OnDestroy()
    {
        produceButton.onClick.RemoveListener(Produce);
        sellButton.onClick.RemoveListener(Sell);
        upgradeButton.onClick.RemoveListener(Upgrade);
    }

    private void Produce()
    {
        products += productionPerClick;
        UpdateUI();
    }

    private void Sell()
    {
        if (products < 1)
        {
            return;
        }

        products--;
        money += sellPrice;
        UpdateUI();
    }

    private void Upgrade()
    {
        if (money < upgradeCost)
        {
            return;
        }

        money -= upgradeCost;
        productionPerClick++;
        upgradeCost *= 2;
        UpdateUI();
    }

    private void UpdateUI()
    {
        moneyText.text = $"Money: ${money}";
        productsText.text = $"Products: {products}";
        upgradeButton.GetComponentInChildren<TMP_Text>().text = $"UPGRADE - ${upgradeCost}";
    }
}
