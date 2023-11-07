using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Joywire;
using Joywire.Core;
using Joywire.Monetization;


public class PlayerProgressionManager : MonoBehaviour, IPlayerProgression
{

    [SerializeField] private PlayerProfile playerProfile;


    private IGameShopManager gameShopManager;

    private IEnumerator Start()
    {
        ThirdParties.Register<IPlayerProgression>(this);
        yield return new WaitForSeconds(1.0f);
        Initialize();



        ThirdParties.Find<IRuntimeDataManager>(out var dataManager);

        TransitionToNextScreen.OnLoadToNextLevel?.Invoke();
    }
    public virtual void Initialize()
    {
        ThirdParties.Find<IGameShopManager>(out gameShopManager);

        //check load data
        string data = "";

        if (PlayerPrefs.HasKey("data"))
        {
            data = PlayerPrefs.GetString("data");
        }

        Load(data);

    }

    public string SaveData => JsonUtility.ToJson(playerProfile);
    public PlayerProfile PlayerProfile => this.playerProfile;

    public void Load(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            playerProfile = JsonUtility.FromJson<PlayerProfile>(data);
            //Debug.LogError("load local data");
        }

        ThirdParties.Find<IRuntimeDataManager>(out var dataManager);
        if (playerProfile == null || string.IsNullOrEmpty(data))
        {
            playerProfile = new PlayerProfile(dataManager);

            var defaultItems = dataManager.GetDefaultItems();
            playerProfile.PlayerCustoms = new PlayerCustom[defaultItems.Length];

            for (int i = 0; i < playerProfile.PlayerCustoms.Length; i++)
            {
                playerProfile.PlayerCustoms[i] = new PlayerCustom();
                playerProfile.PlayerCustoms[i].playerCustomId = defaultItems[i].TypeID;
                playerProfile.PlayerCustoms[i].itemId = defaultItems[i].ItemID;
            }

        }


        playerProfile.PlayerProgress.Initialize();
        Save();

    }

    public void Save()
    {
        var data = SaveData;
        PlayerPrefs.SetString("data", data);

    }

    public void RequestPaymentProgress(string shopItemID, out PlayerPaymentProgress progress)
    {
        progress = null;
        for (int i = 0; i < playerProfile.paymentProgressList.Count; i++)
        {
            if (playerProfile.paymentProgressList[i].ShopItem == shopItemID)
            {
                progress = playerProfile.paymentProgressList[i];
                return;
            }
        }


        //Debug.LogError(shopItemID);
        var targetShopItem = gameShopManager.GetShopItem(shopItemID);
        progress = new PlayerPaymentProgress(shopItemID, 0, targetShopItem.payments[0].amount);
        playerProfile.paymentProgressList.Add(progress);
    }
}
