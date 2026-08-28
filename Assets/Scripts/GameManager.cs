using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text productsText;
    [SerializeField] private TMP_Text workersText;
    [SerializeField] private Button produceButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button hireWorkerButton;

    private int money = 0;
    private int products = 0;
    private int productionPerClick = 1;
    private int sellPrice = 5;
    private int upgradeCost = 25;
    private int workers = 0;
    private int workerCost = 100;
    private float workerTimer = 0f;

    private void Awake()
    {
        produceButton.onClick.AddListener(Produce);
        sellButton.onClick.AddListener(Sell);
        upgradeButton.onClick.AddListener(Upgrade);
        hireWorkerButton.onClick.AddListener(HireWorker);

        UpdateUI();
    }

    private void Update()
    {
        if (workers == 0)
        {
            return;
        }

        workerTimer += Time.deltaTime;

        if (workerTimer < 1f)
        {
            return;
        }

        int elapsedSeconds = Mathf.FloorToInt(workerTimer);
        products += workers * elapsedSeconds;
        workerTimer -= elapsedSeconds;
        UpdateUI();
    }

    private void OnDestroy()
    {
        produceButton.onClick.RemoveListener(Produce);
        sellButton.onClick.RemoveListener(Sell);
        upgradeButton.onClick.RemoveListener(Upgrade);
        hireWorkerButton.onClick.RemoveListener(HireWorker);
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

    private void HireWorker()
    {
        if (money < workerCost)
        {
            return;
        }

        money -= workerCost;
        workers++;
        workerCost = Mathf.CeilToInt(workerCost * 1.5f);
        UpdateUI();
    }

    private void UpdateUI()
    {
        moneyText.text = $"Money: ${money}";
        productsText.text = $"Products: {products}";
        workersText.text = $"Workers: {workers}";
        upgradeButton.GetComponentInChildren<TMP_Text>().text = $"UPGRADE - ${upgradeCost}";
        hireWorkerButton.GetComponentInChildren<TMP_Text>().text = $"HIRE WORKER - ${workerCost}";
    }
}
