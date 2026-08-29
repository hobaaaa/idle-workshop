using System;
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
    private const string WorkerEfficiencyLevelKey = "WorkerEfficiencyLevel";
    private const string WorkerEfficiencyUpgradeCostKey = "WorkerEfficiencyUpgradeCost";
    private const string AutoSellersKey = "AutoSellers";
    private const string AutoSellerCostKey = "AutoSellerCost";
    private const string LastSaveTimeKey = "LastSaveTime";
    private const int MaxOfflineSeconds = 8 * 60 * 60;

    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text productsText;
    [SerializeField] private TMP_Text workersText;
    [SerializeField] private TMP_Text workerEfficiencyText;
    [SerializeField] private TMP_Text autoSellerText;
    [SerializeField] private Button produceButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button hireWorkerButton;
    [SerializeField] private Button workerEfficiencyUpgradeButton;
    [SerializeField] private Button hireAutoSellerButton;

    private int money = 0;
    private int products = 0;
    private int productionPerClick = 1;
    private int sellPrice = 5;
    private int upgradeCost = 25;
    private int workers = 0;
    private int workerCost = 100;
    private int workerEfficiencyLevel = 1;
    private int workerEfficiencyUpgradeCost = 250;
    private int autoSellers = 0;
    private int autoSellerCost = 750;
    private float workerTimer = 0f;
    private float autoSellTimer = 0f;
    private float autosaveTimer = 0f;

    public int LastOfflineSeconds { get; private set; }
    public int LastOfflineProducts { get; private set; }
    public int LastOfflineSoldProducts { get; private set; }
    public int LastOfflineMoneyEarned { get; private set; }

    private void Awake()
    {
        produceButton.onClick.AddListener(Produce);
        sellButton.onClick.AddListener(Sell);
        upgradeButton.onClick.AddListener(Upgrade);
        hireWorkerButton.onClick.AddListener(HireWorker);
        workerEfficiencyUpgradeButton.onClick.AddListener(UpgradeWorkerEfficiency);
        hireAutoSellerButton.onClick.AddListener(HireAutoSeller);

        LoadGame();
        UpdateUI();
    }

    private void Update()
    {
        if (workers == 0 && autoSellers == 0)
        {
            return;
        }

        autosaveTimer += Time.deltaTime;

        if (workers > 0)
        {
            workerTimer += Time.deltaTime;

            if (workerTimer >= 1f)
            {
                int elapsedSeconds = Mathf.FloorToInt(workerTimer);
                products += workers * workerEfficiencyLevel * elapsedSeconds;
                workerTimer -= elapsedSeconds;
                UpdateUI();
            }
        }

        if (autoSellers > 0)
        {
            autoSellTimer += Time.deltaTime;

            if (autoSellTimer >= 1f)
            {
                int elapsedSeconds = Mathf.FloorToInt(autoSellTimer);
                int sellCapacity = autoSellers * elapsedSeconds;
                int actualSold = Mathf.Min(products, sellCapacity);

                products -= actualSold;
                money += actualSold * sellPrice;
                autoSellTimer -= elapsedSeconds;
                UpdateUI();
            }
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
        workerEfficiencyUpgradeButton.onClick.RemoveListener(UpgradeWorkerEfficiency);
        hireAutoSellerButton.onClick.RemoveListener(HireAutoSeller);
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

    public void UpgradeWorkerEfficiency()
    {
        if (money < workerEfficiencyUpgradeCost)
        {
            return;
        }

        money -= workerEfficiencyUpgradeCost;
        workerEfficiencyLevel++;
        workerEfficiencyUpgradeCost *= 2;
        SaveGame();
        UpdateUI();
    }

    private void HireAutoSeller()
    {
        if (money < autoSellerCost)
        {
            return;
        }

        money -= autoSellerCost;
        autoSellers++;
        autoSellerCost = Mathf.CeilToInt(autoSellerCost * 1.5f);
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
        PlayerPrefs.SetInt(WorkerEfficiencyLevelKey, workerEfficiencyLevel);
        PlayerPrefs.SetInt(WorkerEfficiencyUpgradeCostKey, workerEfficiencyUpgradeCost);
        PlayerPrefs.SetInt(AutoSellersKey, autoSellers);
        PlayerPrefs.SetInt(AutoSellerCostKey, autoSellerCost);
        PlayerPrefs.SetString(LastSaveTimeKey, DateTime.UtcNow.Ticks.ToString());
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        LastOfflineSeconds = 0;
        LastOfflineProducts = 0;
        LastOfflineSoldProducts = 0;
        LastOfflineMoneyEarned = 0;

        if (!PlayerPrefs.HasKey(SaveExistsKey))
        {
            return;
        }

        string lastSaveTime = PlayerPrefs.GetString(LastSaveTimeKey, string.Empty);

        money = PlayerPrefs.GetInt(MoneyKey, money);
        products = PlayerPrefs.GetInt(ProductsKey, products);
        productionPerClick = PlayerPrefs.GetInt(ProductionPerClickKey, productionPerClick);
        sellPrice = PlayerPrefs.GetInt(SellPriceKey, sellPrice);
        upgradeCost = PlayerPrefs.GetInt(UpgradeCostKey, upgradeCost);
        workers = PlayerPrefs.GetInt(WorkersKey, workers);
        workerCost = PlayerPrefs.GetInt(WorkerCostKey, workerCost);
        workerEfficiencyLevel = PlayerPrefs.GetInt(WorkerEfficiencyLevelKey, workerEfficiencyLevel);
        workerEfficiencyUpgradeCost = PlayerPrefs.GetInt(WorkerEfficiencyUpgradeCostKey, workerEfficiencyUpgradeCost);
        autoSellers = PlayerPrefs.GetInt(AutoSellersKey, autoSellers);
        autoSellerCost = PlayerPrefs.GetInt(AutoSellerCostKey, autoSellerCost);

        ApplyOfflineProgress(lastSaveTime);
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
        PlayerPrefs.DeleteKey(WorkerEfficiencyLevelKey);
        PlayerPrefs.DeleteKey(WorkerEfficiencyUpgradeCostKey);
        PlayerPrefs.DeleteKey(AutoSellersKey);
        PlayerPrefs.DeleteKey(AutoSellerCostKey);
        PlayerPrefs.DeleteKey(LastSaveTimeKey);

        money = 0;
        products = 0;
        productionPerClick = 1;
        sellPrice = 5;
        upgradeCost = 25;
        workers = 0;
        workerCost = 100;
        workerEfficiencyLevel = 1;
        workerEfficiencyUpgradeCost = 250;
        autoSellers = 0;
        autoSellerCost = 750;
        workerTimer = 0f;
        autoSellTimer = 0f;
        autosaveTimer = 0f;
        LastOfflineSeconds = 0;
        LastOfflineProducts = 0;
        LastOfflineSoldProducts = 0;
        LastOfflineMoneyEarned = 0;

        UpdateUI();
        SaveGame();
    }

    private void ApplyOfflineProgress(string lastSaveTime)
    {
        LastOfflineSoldProducts = 0;
        LastOfflineMoneyEarned = 0;

        if (workers == 0 && autoSellers == 0)
        {
            return;
        }

        if (!long.TryParse(lastSaveTime, out long lastSaveTicks))
        {
            return;
        }

        if (lastSaveTicks < DateTime.MinValue.Ticks || lastSaveTicks > DateTime.MaxValue.Ticks)
        {
            return;
        }

        DateTime lastSaveUtc = new DateTime(lastSaveTicks, DateTimeKind.Utc);
        TimeSpan elapsedTime = DateTime.UtcNow - lastSaveUtc;

        if (elapsedTime.TotalSeconds <= 0)
        {
            return;
        }

        double elapsedSeconds = Math.Floor(elapsedTime.TotalSeconds);
        LastOfflineSeconds = (int)Math.Min(elapsedSeconds, MaxOfflineSeconds);
        LastOfflineProducts = workers * workerEfficiencyLevel * LastOfflineSeconds;
        products += LastOfflineProducts;

        int sellCapacity = autoSellers * LastOfflineSeconds;
        LastOfflineSoldProducts = Mathf.Min(products, sellCapacity);
        LastOfflineMoneyEarned = LastOfflineSoldProducts * sellPrice;
        products -= LastOfflineSoldProducts;
        money += LastOfflineMoneyEarned;

        SaveGame();
    }

    private void UpdateUI()
    {
        moneyText.text = $"Money: ${money}";
        productsText.text = $"Products: {products}";
        workersText.text = $"Workers: {workers}";
        workerEfficiencyText.text = $"Worker Efficiency: x{workerEfficiencyLevel}";
        autoSellerText.text = $"Auto Sellers: {autoSellers}";
        upgradeButton.GetComponentInChildren<TMP_Text>().text = $"UPGRADE - ${upgradeCost}";
        hireWorkerButton.GetComponentInChildren<TMP_Text>().text = $"HIRE WORKER - ${workerCost}";
        workerEfficiencyUpgradeButton.GetComponentInChildren<TMP_Text>().text = $"IMPROVE WORKERS - ${workerEfficiencyUpgradeCost}";
        hireAutoSellerButton.GetComponentInChildren<TMP_Text>().text = $"HIRE AUTO SELLER - ${autoSellerCost}";
    }
}
