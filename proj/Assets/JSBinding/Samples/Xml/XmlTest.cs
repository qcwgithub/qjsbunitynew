using UnityEngine;
using System.Collections;
using System.Xml;
using Lavie;

public class XmlTest : MonoBehaviour
{
    private void Start()
    {
        TextAsset textAssets = (TextAsset)Resources.Load("ShopConfig");

        if (textAssets != null)
        {
            Debug.Log(textAssets.text);
        }
        else
        {
            Debug.Log("unkonw error!");
        }

        XmlDocument xml = new XmlDocument();

        xml.LoadXml(textAssets.text);

        Debug.Log("xmlload");

        var xmlPackets = xml.SelectNodes("root/Packets/Packet");


        XmlNode pp = xmlPackets.Select<string>("ID", "1");


        Debug.Log("pp=" + pp);


        XmlNodeList daoJunodeList = pp.ChildNodes;
        Debug.Log("daojulist=" + daoJunodeList);


        BetterList<ShopItemData> daoJumdata = daoJunodeList.ConvertType<ShopItemData>("SubType");

        Debug.Log("daojudata" + daoJumdata);

        foreach (ShopItemData shopItemData in daoJumdata)
        {
            Debug.Log(shopItemData.ID);
        }
    }
}

public class ShopItemData
{
    public string ID;
    public ItemType ItemType;
    public string ItemId;


    public enumMoneyType Currency;

    public int OldPrice;
    public int CurPrice;
    public bool Recommend;


    public float RefreshRate;
    public int Globe;
    public bool IsPack;
    public int ItemNum;

    public ItemCell item;
    public LimeTimeNum TimeNum;
    public LimitVipDayNum VipDayNum;
    public LimitBuyNumPrices BuyNumPrice;


    public LimitVIPLevel VIPLevel;
    public LimitDayNum DayNum;

    public int hadBuyNum = 0;
    public int maxBuyNum = -1;
    public int ShopCategory;


    public void Clear()
    {
        item = null;
        TimeNum = null;
        VipDayNum = null;
        BuyNumPrice = null;
    }
}

public class LimitDayNum : IShopLime
{
    public int Count;
}

public class LimitVIPLevel
{
    public int Level;
}

public class LimitBuyNumPrices : IShopLime
{
    public int[] Prices;
}

public class LimitVipDayNum : IShopLime
{
    public int[] Count;
}

public class LimeTimeNum : IShopLime
{
    public int Interval;
    public int Count;
}

public interface IShopLime
{
}

public enum ItemType
{
    Normal = 1,
    Treaure = 2,
    TreasureSoul = 3,
    TreasureChip = 4,
    Equip = 5,
    EquipChip = 6,
    PartnerSoul = 7,
    PartnerChip = 8,
    Cheats = 9,
    Stone = 10,
}

public class ItemCell
{
    public int ID = -1;
    public string icon = "";
    public string iconAtlas = "";
    public string name = "";
    public int number = 0;
    public int phases = 0;
    public string description = "";
    public int type = -1;
    public int value = 0;
    public int maxStack = -1;
    public int timeLimit = 0;
    public bool bSold = false;
    public bool bDestoryed = false;
    public string sArtNamePath = "";
    public ColorSign color = 0;
    public int expSupply = 0;
}

public enum ColorSign
{
    White = 1,
    Green = 2,
    Blue = 3,
    Purple = 4,
    Orange = 5,
    Red = 6,
    Black
}

public enum enumMoneyType
{
    None = 0,
    Gold = 1,
    Diamon = 2,
    iGoldPill = 3,
    iDragonScale,
    iPhoenixFeather,
    iGodSoul,
    iBiasJade,
    iBuddHistrelics,
    iContribution,
    iPrestige
}