using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private const string SaveExistsKey = "SaveExists";
    private const string MoneyKey = "Money";
    private const string ProductsKey = "Products";
    private const string ProductionPerClickKey = "ProductionPerClick";
    private const string SellPriceKey = "SellPrice";
    private const string UpgradeCostKey = "UpgradeCost";
    private const string WorkersKey = "Workers";
    private const string WorkerCostKey = "WorkerCost";

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
    private float autosaveTimer = 0f;

    private void Awake()
    {
        produceButton.onClick.AddListener(Produce);
        sellButton.onClick.AddListener(Sell);
        upgradeButton.onClick.AddListener(Upgrade);
        hireWorkerButton.onClick.AddListener(HireWorker);

        LoadGame();
        UpdateUI();
    }

    private void Update()
    {
        if (workers == 0)
        {
            return;
        }

        workerTimer += Time.deltaTime;
        autosaveTimer += Time.deltaTime;

        if (workerTimer >= 1f)
        {
            int elapsedSeconds = Mathf.FloorToInt(workerTimer);
            products += workers * elapsedSeconds;
            workerTimer -= elapsedSeconds;
            UpdateUI();
        }

        if (autosaveTimer >= 10f)
        {
            autosaveTimer = 0f;
            SaveGame();
        }
    }

    private void OnDestroy()
    {
        produceButton.onClick.RemoveListener(Produce);
        sellButton.onClick.RemoveListener(Sell);
        upgradeButton.onClick.RemoveListener(Upgrade);
        hireWorkerButton.onClick.RemoveListener(HireWorker);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void Produce()
    {
        products += productionPerClick;
        SaveGame();
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
        SaveGame();
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
        SaveGame();
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
        SaveGame();
        UpdateUI();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt(SaveExistsKey, 1);
        PlayerPrefs.SetInt(MoneyKey, money);
        PlayerPrefs.SetInt(ProductsKey, products);
        PlayerPrefs.SetInt(ProductionPerClickKey, productionPerClick);
        PlayerPrefs.SetInt(SellPriceKey, sellPrice);
        PlayerPrefs.SetInt(UpgradeCostKey, upgradeCost);
        PlayerPrefs.SetInt(WorkersKey, workers);
        PlayerPrefs.SetInt(WorkerCostKey, workerCost);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey(SaveExistsKey))
        {
            return;
        }

        money = PlayerPrefs.GetInt(MoneyKey, money);
        products = PlayerPrefs.GetInt(ProductsKey, products);
        productionPerClick = PlayerPrefs.GetInt(ProductionPerClickKey, productionPerClick);
        sellPrice = PlayerPrefs.GetInt(SellPriceKey, sellPrice);
        upgradeCost = PlayerPrefs.GetInt(UpgradeCostKey, upgradeCost);
        workers = PlayerPrefs.GetInt(WorkersKey, workers);
        workerCost = PlayerPrefs.GetInt(WorkerCostKey, workerCost);
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey(SaveExistsKey);
        PlayerPrefs.DeleteKey(MoneyKey);
        PlayerPrefs.DeleteKey(ProductsKey);
        PlayerPrefs.DeleteKey(ProductionPerClickKey);
        PlayerPrefs.DeleteKey(SellPriceKey);
        PlayerPrefs.DeleteKey(UpgradeCostKey);
        PlayerPrefs.DeleteKey(WorkersKey);
        PlayerPrefs.DeleteKey(WorkerCostKey);

        money = 0;
        products = 0;
        productionPerClick = 1;
        sellPrice = 5;
        upgradeCost = 25;
        workers = 0;
        workerCost = 100;
        workerTimer = 0f;
        autosaveTimer = 0f;

        UpdateUI();
        SaveGame();
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
