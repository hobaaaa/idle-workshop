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
    private const string ChairUnlockedKey = "ChairUnlocked";
    private const string ChairsKey = "Chairs";
    private const string ChairProductionPerClickKey = "ChairProductionPerClick";
    private const string ChairSellPriceKey = "ChairSellPrice";
    private const string ChairUpgradeCostKey = "ChairUpgradeCost";
    private const string ChairWorkersKey = "ChairWorkers";
    private const string ChairWorkerCostKey = "ChairWorkerCost";
    private const string ChairAutoSellersKey = "ChairAutoSellers";
    private const string ChairAutoSellerCostKey = "ChairAutoSellerCost";
    private const string LastSaveTimeKey = "LastSaveTime";
    private const int MaxOfflineSeconds = 8 * 60 * 60;

    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text productsText;
    [SerializeField] private TMP_Text workersText;
    [SerializeField] private TMP_Text workerEfficiencyText;
    [SerializeField] private TMP_Text autoSellerText;
    [SerializeField] private TMP_Text chairTitleText;
    [SerializeField] private TMP_Text chairsText;
    [SerializeField] private TMP_Text chairWorkersText;
    [SerializeField] private TMP_Text chairAutoSellerText;
    [SerializeField] private Button produceButton;
    [SerializeField] private Button sellButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button hireWorkerButton;
    [SerializeField] private Button workerEfficiencyUpgradeButton;
    [SerializeField] private Button hireAutoSellerButton;
    [SerializeField] private Button unlockChairButton;
    [SerializeField] private Button produceChairButton;
    [SerializeField] private Button sellChairButton;
    [SerializeField] private Button chairUpgradeButton;
    [SerializeField] private Button hireChairWorkerButton;
    [SerializeField] private Button resetSaveButton;
    [SerializeField] private Button sellAllProductsButton;
    [SerializeField] private Button sellAllChairsButton;
    [SerializeField] private Button hireChairAutoSellerButton;

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
    private bool chairUnlocked = false;
    private int chairs = 0;
    private int chairUnlockCost = 2500;
    private int chairProductionPerClick = 1;
    private int chairSellPrice = 50;
    private int chairUpgradeCost = 500;
    private int chairWorkers = 0;
    private int chairWorkerCost = 1500;
    private int chairAutoSellers = 0;
    private int chairAutoSellerCost = 3000;
    private float workerTimer = 0f;
    private float autoSellTimer = 0f;
    private float chairWorkerTimer = 0f;
    private float chairAutoSellTimer = 0f;
    private float autosaveTimer = 0f;

    public int LastOfflineSeconds { get; private set; }
    public int LastOfflineProducts { get; private set; }
    public int LastOfflineSoldProducts { get; private set; }
    public int LastOfflineMoneyEarned { get; private set; }
    public int LastOfflineChairs { get; private set; }

    private void Awake()
    {
        produceButton.onClick.AddListener(Produce);
        sellButton.onClick.AddListener(Sell);
        upgradeButton.onClick.AddListener(Upgrade);
        hireWorkerButton.onClick.AddListener(HireWorker);
        workerEfficiencyUpgradeButton.onClick.AddListener(UpgradeWorkerEfficiency);
        hireAutoSellerButton.onClick.AddListener(HireAutoSeller);
        unlockChairButton.onClick.AddListener(UnlockChair);
        produceChairButton.onClick.AddListener(ProduceChair);
        sellChairButton.onClick.AddListener(SellChair);
        chairUpgradeButton.onClick.AddListener(UpgradeChairProduction);
        hireChairWorkerButton.onClick.AddListener(HireChairWorker);
        resetSaveButton.onClick.AddListener(ResetSave);
        sellAllProductsButton.onClick.AddListener(SellAllProducts);
        sellAllChairsButton.onClick.AddListener(SellAllChairs);
        hireChairAutoSellerButton.onClick.AddListener(HireChairAutoSeller);

        LoadGame();
        UpdateUI();
    }

    private void Update()
    {
        if (workers == 0 && autoSellers == 0 && chairWorkers == 0 && chairAutoSellers == 0)
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

        if (chairUnlocked && chairWorkers > 0)
        {
            chairWorkerTimer += Time.deltaTime;

            if (chairWorkerTimer >= 2f)
            {
                int completedCycles = Mathf.FloorToInt(chairWorkerTimer / 2f);
                chairs += chairWorkers * completedCycles;
                chairWorkerTimer -= completedCycles * 2f;
                UpdateUI();
            }
        }

        if (chairUnlocked && chairAutoSellers > 0)
        {
            chairAutoSellTimer += Time.deltaTime;

            if (chairAutoSellTimer >= 1f)
            {
                int elapsedSeconds = Mathf.FloorToInt(chairAutoSellTimer);
                int sellCapacity = chairAutoSellers * elapsedSeconds;
                int actualSold = Mathf.Min(chairs, sellCapacity);

                chairs -= actualSold;
                money += actualSold * chairSellPrice;
                chairAutoSellTimer -= elapsedSeconds;
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
        unlockChairButton.onClick.RemoveListener(UnlockChair);
        produceChairButton.onClick.RemoveListener(ProduceChair);
        sellChairButton.onClick.RemoveListener(SellChair);
        chairUpgradeButton.onClick.RemoveListener(UpgradeChairProduction);
        hireChairWorkerButton.onClick.RemoveListener(HireChairWorker);
        resetSaveButton.onClick.RemoveListener(ResetSave);
        sellAllProductsButton.onClick.RemoveListener(SellAllProducts);
        sellAllChairsButton.onClick.RemoveListener(SellAllChairs);
        hireChairAutoSellerButton.onClick.RemoveListener(HireChairAutoSeller);
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

    private void SellAllProducts()
    {
        if (products <= 0)
        {
            return;
        }

        money += products * sellPrice;
        products = 0;
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

    private void UnlockChair()
    {
        if (chairUnlocked)
        {
            return;
        }

        if (money < chairUnlockCost)
        {
            return;
        }

        money -= chairUnlockCost;
        chairUnlocked = true;
        SaveGame();
        UpdateUI();
    }

    private void ProduceChair()
    {
        if (!chairUnlocked)
        {
            return;
        }

        chairs += chairProductionPerClick;
        SaveGame();
        UpdateUI();
    }

    private void SellChair()
    {
        if (!chairUnlocked)
        {
            return;
        }

        if (chairs <= 0)
        {
            return;
        }

        chairs--;
        money += chairSellPrice;
        SaveGame();
        UpdateUI();
    }

    private void SellAllChairs()
    {
        if (!chairUnlocked)
        {
            return;
        }

        if (chairs <= 0)
        {
            return;
        }

        int totalSaleValue = chairs * chairSellPrice;
        money += totalSaleValue;
        chairs = 0;
        SaveGame();
        UpdateUI();
    }

    private void UpgradeChairProduction()
    {
        if (!chairUnlocked)
        {
            return;
        }

        if (money < chairUpgradeCost)
        {
            return;
        }

        money -= chairUpgradeCost;
        chairProductionPerClick++;
        chairUpgradeCost *= 2;
        SaveGame();
        UpdateUI();
    }

    private void HireChairWorker()
    {
        if (!chairUnlocked)
        {
            return;
        }

        if (money < chairWorkerCost)
        {
            return;
        }

        money -= chairWorkerCost;
        chairWorkers++;
        chairWorkerCost = Mathf.CeilToInt(chairWorkerCost * 1.5f);
        SaveGame();
        UpdateUI();
    }

    private void HireChairAutoSeller()
    {
        if (!chairUnlocked)
        {
            return;
        }

        if (money < chairAutoSellerCost)
        {
            return;
        }

        money -= chairAutoSellerCost;
        chairAutoSellers++;
        chairAutoSellerCost = Mathf.CeilToInt(chairAutoSellerCost * 1.5f);
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
        PlayerPrefs.SetInt(ChairUnlockedKey, chairUnlocked ? 1 : 0);
        PlayerPrefs.SetInt(ChairsKey, chairs);
        PlayerPrefs.SetInt(ChairProductionPerClickKey, chairProductionPerClick);
        PlayerPrefs.SetInt(ChairSellPriceKey, chairSellPrice);
        PlayerPrefs.SetInt(ChairUpgradeCostKey, chairUpgradeCost);
        PlayerPrefs.SetInt(ChairWorkersKey, chairWorkers);
        PlayerPrefs.SetInt(ChairWorkerCostKey, chairWorkerCost);
        PlayerPrefs.SetInt(ChairAutoSellersKey, chairAutoSellers);
        PlayerPrefs.SetInt(ChairAutoSellerCostKey, chairAutoSellerCost);
        PlayerPrefs.SetString(LastSaveTimeKey, DateTime.UtcNow.Ticks.ToString());
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        LastOfflineSeconds = 0;
        LastOfflineProducts = 0;
        LastOfflineSoldProducts = 0;
        LastOfflineMoneyEarned = 0;
        LastOfflineChairs = 0;

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
        chairUnlocked = PlayerPrefs.GetInt(ChairUnlockedKey, chairUnlocked ? 1 : 0) == 1;
        chairs = PlayerPrefs.GetInt(ChairsKey, chairs);
        chairProductionPerClick = PlayerPrefs.GetInt(ChairProductionPerClickKey, chairProductionPerClick);
        chairSellPrice = PlayerPrefs.GetInt(ChairSellPriceKey, chairSellPrice);
        chairUpgradeCost = PlayerPrefs.GetInt(ChairUpgradeCostKey, chairUpgradeCost);
        chairWorkers = PlayerPrefs.GetInt(ChairWorkersKey, chairWorkers);
        chairWorkerCost = PlayerPrefs.GetInt(ChairWorkerCostKey, chairWorkerCost);
        chairAutoSellers = PlayerPrefs.GetInt(ChairAutoSellersKey, chairAutoSellers);
        chairAutoSellerCost = PlayerPrefs.GetInt(ChairAutoSellerCostKey, chairAutoSellerCost);

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
        PlayerPrefs.DeleteKey(ChairUnlockedKey);
        PlayerPrefs.DeleteKey(ChairsKey);
        PlayerPrefs.DeleteKey(ChairProductionPerClickKey);
        PlayerPrefs.DeleteKey(ChairSellPriceKey);
        PlayerPrefs.DeleteKey(ChairUpgradeCostKey);
        PlayerPrefs.DeleteKey(ChairWorkersKey);
        PlayerPrefs.DeleteKey(ChairWorkerCostKey);
        PlayerPrefs.DeleteKey(ChairAutoSellersKey);
        PlayerPrefs.DeleteKey(ChairAutoSellerCostKey);
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
        chairUnlocked = false;
        chairs = 0;
        chairProductionPerClick = 1;
        chairSellPrice = 50;
        chairUpgradeCost = 500;
        chairWorkers = 0;
        chairWorkerCost = 1500;
        chairAutoSellers = 0;
        chairAutoSellerCost = 3000;
        workerTimer = 0f;
        autoSellTimer = 0f;
        chairWorkerTimer = 0f;
        chairAutoSellTimer = 0f;
        autosaveTimer = 0f;
        LastOfflineSeconds = 0;
        LastOfflineProducts = 0;
        LastOfflineSoldProducts = 0;
        LastOfflineMoneyEarned = 0;
        LastOfflineChairs = 0;

        UpdateUI();
        SaveGame();
    }

    private void ApplyOfflineProgress(string lastSaveTime)
    {
        LastOfflineSoldProducts = 0;
        LastOfflineMoneyEarned = 0;
        LastOfflineChairs = 0;

        if (workers == 0 && autoSellers == 0 && chairWorkers == 0 && chairAutoSellers == 0)
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

        if (chairUnlocked && chairWorkers > 0)
        {
            int completedCycles = LastOfflineSeconds / 2;
            LastOfflineChairs = chairWorkers * completedCycles;
            chairs += LastOfflineChairs;
        }

        if (chairUnlocked && chairAutoSellers > 0)
        {
            int chairSellCapacity = chairAutoSellers * LastOfflineSeconds;
            int actualChairsSold = Mathf.Min(chairs, chairSellCapacity);
            chairs -= actualChairsSold;
            money += actualChairsSold * chairSellPrice;
        }

        SaveGame();
    }

    private void UpdateUI()
    {
        moneyText.text = $"Money: ${money}";
        productsText.text = $"Products: {products}";
        workersText.text = $"Workers: {workers}";
        workerEfficiencyText.text = $"Worker Efficiency: x{workerEfficiencyLevel}";
        autoSellerText.text = $"Auto Sellers: {autoSellers}";
        chairTitleText.text = "WOODEN CHAIR";
        chairsText.text = $"Chairs: {chairs}";
        chairWorkersText.text = $"Chair Workers: {chairWorkers}";
        chairAutoSellerText.text = $"Chair Auto Sellers: {chairAutoSellers}";
        upgradeButton.GetComponentInChildren<TMP_Text>().text = $"UPGRADE - ${upgradeCost}";
        hireWorkerButton.GetComponentInChildren<TMP_Text>().text = $"HIRE WORKER - ${workerCost}";
        workerEfficiencyUpgradeButton.GetComponentInChildren<TMP_Text>().text = $"IMPROVE WORKERS - ${workerEfficiencyUpgradeCost}";
        hireAutoSellerButton.GetComponentInChildren<TMP_Text>().text = $"HIRE AUTO SELLER - ${autoSellerCost}";
        unlockChairButton.GetComponentInChildren<TMP_Text>().text = $"UNLOCK CHAIRS - ${chairUnlockCost}";
        produceChairButton.GetComponentInChildren<TMP_Text>().text = "PRODUCE CHAIR";
        sellChairButton.GetComponentInChildren<TMP_Text>().text = $"SELL CHAIR - ${chairSellPrice}";
        chairUpgradeButton.GetComponentInChildren<TMP_Text>().text = $"UPGRADE CHAIR +{chairProductionPerClick} - ${chairUpgradeCost}";
        hireChairWorkerButton.GetComponentInChildren<TMP_Text>().text = $"HIRE CHAIR WORKER - ${chairWorkerCost}";
        resetSaveButton.GetComponentInChildren<TMP_Text>().text = "RESET SAVE";
        sellAllProductsButton.GetComponentInChildren<TMP_Text>().text = "SELL ALL PRODUCTS";
        sellAllChairsButton.GetComponentInChildren<TMP_Text>().text = "SELL ALL CHAIRS";
        hireChairAutoSellerButton.GetComponentInChildren<TMP_Text>().text = $"HIRE CHAIR AUTO SELLER - ${chairAutoSellerCost}";

        chairTitleText.gameObject.SetActive(chairUnlocked);
        chairsText.gameObject.SetActive(chairUnlocked);
        chairWorkersText.gameObject.SetActive(chairUnlocked);
        chairAutoSellerText.gameObject.SetActive(chairUnlocked);
        unlockChairButton.gameObject.SetActive(!chairUnlocked);
        produceChairButton.gameObject.SetActive(chairUnlocked);
        sellChairButton.gameObject.SetActive(chairUnlocked);
        chairUpgradeButton.gameObject.SetActive(chairUnlocked);
        hireChairWorkerButton.gameObject.SetActive(chairUnlocked);
        sellAllChairsButton.gameObject.SetActive(chairUnlocked);
        hireChairAutoSellerButton.gameObject.SetActive(chairUnlocked);
    }
}
