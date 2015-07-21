using SharpKit.JavaScript;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Reflection;
using Lavie;

public class MindActUpMide<T>
{
    public T Value;
}

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/XmlTest.javascript")]
public class XmlTest : MonoBehaviour
{
    private void Start()
    {
        enumMoneyType ABC = enumMoneyType.Gold;
        Debug.Log(ABC.ToString());

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

        XmlNodeList xmlPackets = xml.SelectNodes("root/Packets/Packet");

        XmlNode pp = xmlPackets.Select<string>("ID", "1");
        Debug.Log("pp=" + pp.ToString());

        ItemType v1 = pp.NodeValue<ItemType>("ID");
        Debug.Log("ID == " + v1.ToString());


        XmlNodeList daoJunodeList = pp.ChildNodes;
        Debug.Log("daojulist Count = " + daoJunodeList.Count);


        List<ShopItemData> daoJumdata = daoJunodeList.CreateObjectFromXml<ShopItemData>("SubType"); 
        Debug.Log("daojudata Count = " + daoJumdata.Count);

        foreach (ShopItemData shopItemData in daoJumdata)
        {
            Debug.Log(shopItemData.ID);
        }
    }
}

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/ShopItemData.javascript")]
public class ShopItemData
{
    public string ID { get; set; }
    public ItemType ItemType;
    public string ItemId { get; set; }


    public enumMoneyType Currency { get; set; }

    public int OldPrice { get; set; }
    public int CurPrice { get; set; }
    public bool Recommend { get; set; }


    public float RefreshRate { get; set; }
    public int Globe { get; set; }
    public bool IsPack { get; set; }
    public int ItemNum { get; set; }

    public ItemCell item { get; set; }
    public LimeTimeNum TimeNum { get; set; }
    public LimitVipDayNum VipDayNum { get; set; }
    public LimitBuyNumPrices BuyNumPrice { get; set; }


    public LimitVIPLevel VIPLevel { get; set; }
    public LimitDayNum DayNum { get; set; }

    public int hadBuyNum { get; set; }
    public int maxBuyNum { get; set; }
    public int ShopCategory { get; set; }


    public void Clear()
    {
        item = null;
        TimeNum = null;
        VipDayNum = null;
        BuyNumPrice = null;
    }
}

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/LimitDayNum.javascript")]
public class LimitDayNum : IShopLime
{
    public int Count { get; set; }
}

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/LimitVIPLevel.javascript")]
public class LimitVIPLevel
{
    public int Level { get; set; }
}

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/LimitBuyNumPrices.javascript")]
public class LimitBuyNumPrices : IShopLime
{
    public int[] Prices{get;set;}
}

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/LimitVipDayNum.javascript")]
public class LimitVipDayNum : IShopLime
{
    public int[] Count { get; set; }
}

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/LimeTimeNum.javascript")]
public class LimeTimeNum : IShopLime
{
    public int Interval { get; set; }
    public int Count { get; set; }
}

[JsType(JsMode.Clr, "../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/IShopLime.javascript")]
public interface IShopLime
{
}

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/ItemType.javascript")]
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

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/ItemCell.javascript")]
public class ItemCell
{
    public int ID { get; set; }
    public string icon  { get; set; }
    public string iconAtlas  { get; set; }
    public string name  { get; set; }
    public int number { get; set; }
    public int phases  { get; set; }
    public string description  { get; set; }
    public int type  { get; set; }
    public int value  { get; set; }
    public int maxStack  { get; set; }
    public int timeLimit  { get; set; }
    public bool bSold  { get; set; }
    public bool bDestoryed  { get; set; }
    public string sArtNamePath { get; set; }
    public ColorSign color { get; set; }
    public int expSupply { get; set; }
}

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/ColorSign.javascript")]
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

[JsType(JsMode.Clr,"../../../StreamingAssets/JavaScript/SharpKitGenerated/JSBinding/Samples/Xml/enumMoneyType.javascript")]
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