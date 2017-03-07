using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LgDepot : MonoBehaviour
{
    public ItemManager items = new ItemManager();
    public GameObject tile_1x1;
    public GameObject tile_1x2;
    public GameObject tile_1x3;
    public GameObject tile_1x4;
    public GameObject tile_2x1;
    public GameObject tile_2x2;
    public GameObject tile_2x3;
    public GameObject tile_2x4;

    public GameObject tooltip;

    private GameObject selectedTile;
    public UIAtlas atlasRoot;
    private bool attrChanged = false;


    void Start()
    {
        atlasRoot = tile_1x1.GetComponent<UISprite>().atlas;

        items.Start();
        for (int i = 0; i < Global.LocalHero.charactor.backpack.Count; i++)
        {
            if (Config.DataLoader.equipmentMaps.ContainsKey(Global.LocalHero.charactor.backpack[i]))
            {
                Config.Equipment eq = Config.DataLoader.equipmentMaps[Global.LocalHero.charactor.backpack[i]];

                OnAddItem(eq, false);
            }
            else
                Debug.LogWarning("not find " + Global.LocalHero.charactor.backpack[i]);
        }

        //tile_1x1.SetActive(false);
        //tile_1x2.SetActive(false);
        //tile_1x3.SetActive(false);
        //tile_1x4.SetActive(false);
        //tile_2x1.SetActive(false);
        //tile_2x2.SetActive(false);
        //tile_2x3.SetActive(false);
        //tile_2x4.SetActive(false);

        UpdateGold();

        Transform node = transform.FindChild("Camera/Anchor/Panel/Contain/RepairGold");
        UILabel repairGold = node.GetComponent<UILabel>();
        repairGold.text = Breakage.Cost.ToString();
    }

    public static byte GetAbjustTile(string icon, UIAtlas atlas)
    {
        try
        {
            UISpriteData data = atlas.GetSprite(icon);

            if (data.width <= 28 && data.height <= 28)
            {
                return 11;
            }
            else if (data.width <= 28 && data.height <= 56 && data.height > 28)
            {
                return 12;
            }
            else if (data.width <= 28 && data.height <= 84 && data.height > 56)
            {
                return 13;
            }
            else if (data.width <= 28 && data.height <= 112 && data.height > 84)
            {
                return 14;
            }
            else if (data.width <= 56 && data.height <= 28 && data.width > 28)
            {
                return 21;
            }
            else if (data.width <= 56 && data.height <= 56 && data.width > 28 && data.height > 28)
            {
                return 22;
            }
            else if (data.width <= 56 && data.height <= 112 && data.width > 28 && data.height > 84)
            {
                return 24;
            }
            else if (data.width <= 56 && data.height <= 84 && data.width > 28 && data.height > 56)
            {
                return 23;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("get tile " + icon);
        }

        return 23;
    }

    private bool OnAddItem(Config.Equipment eq, bool addBackpack)
    {
        if (eq.costSpace == 0)
            eq.costSpace = GetAbjustTile(eq.icon, atlasRoot);

        if (items.AddItem(eq))
        {
            if (eq.costSpace == 11)
                items.Update(tile_1x1, items.LastItem());
            else if (eq.costSpace == 12)
                items.Update(tile_1x2, items.LastItem());
            else if (eq.costSpace == 13)
                items.Update(tile_1x3, items.LastItem());
            else if (eq.costSpace == 14)
                items.Update(tile_1x4, items.LastItem());
            else if (eq.costSpace == 21)
                items.Update(tile_2x1, items.LastItem());
            else if (eq.costSpace == 22)
                items.Update(tile_2x2, items.LastItem());
            else if (eq.costSpace == 23)
                items.Update(tile_2x3, items.LastItem());
            else if (eq.costSpace == 24)
                items.Update(tile_2x4, items.LastItem());

            if (addBackpack)
                Global.LocalHero.charactor.backpack.Add(eq.ID);
            return true;
        }

        return false;
    }

    private void UpdateGold()
    {
        int n = 0;
        string money = "";
        string gold = Global.LocalHero.charactor.gold.ToString();
        for (int i = gold.Length - 1; i >= 0; i--)
        {
            money = gold[i] + money;
            if (n == 2 && i - 1 > 0)
                money = "," + money;

            n++;
        }

        Transform root = transform.FindChild("Camera/Anchor/Panel/Contain/Gold");
        UILabel lab = root.GetComponent<UILabel>();
        lab.text = money;
    }

    void OnClickTip(GameObject obj)
    {
        UserData ud = obj.GetComponent<UserData>();

        Transform root = transform.FindChild("Camera/Anchor/Panel/Tooltip_UI");

        ushort eid = ushort.Parse(ud.data);

        if (selectedTile != obj)
        {
            UITooltip.ShowText(Global.CombEquipmentTips(Global.LocalHero.charactor, eid, true, true));

            UISprite bg = root.GetComponent<UISprite>();

            UILabel lab = root.FindChild("Label").GetComponent<UILabel>();
            lab.transform.localPosition = new Vector2(0, -6);
            bg.width = Mathf.Max(bg.width + 8, 100);
            bg.height += 30;

            if (Config.DataLoader.equipmentMaps.ContainsKey(eid))
            {
                Config.Equipment equipment = Config.DataLoader.equipmentMaps[eid];
                Transform node = root.FindChild("SetupBtn");

                if (equipment.lvlRequest > Global.LocalHero.charactor.level || equipment.strRequest > Config.CharAttribute.Str(Global.LocalHero.charactor) ||
                    equipment.dexRequest > Config.CharAttribute.Dex(Global.LocalHero.charactor) || (equipment.profession != Config.Profession.None &&
                    equipment.profession != Global.LocalHero.charactor.profession))
                {
                    node.gameObject.SetActive(false);
                }
                else if (!node.gameObject.activeSelf)
                {
                    node.gameObject.SetActive(true);
                }

                if (equipment.type == Config.Equipment.Type.Chest)
                {
                    node.FindChild("Label").GetComponent<UILabel>().text = "打 开";
                }
                else
                {
                    node.FindChild("Label").GetComponent<UILabel>().text = "穿 戴";
                }
            }

            float dw = root.localPosition.x + bg.width - Screen.width / 2;
            float dh = Mathf.Abs(root.localPosition.y - bg.height) - Screen.height / 2;
            if (dh > 0)
                dh += root.localPosition.y;
            else
                dh = root.localPosition.y;

            if (dw > 0)
                dw = root.localPosition.x + bg.width / 2 - dw;
            else
                dw = root.localPosition.x + bg.width / 2;

            root.localPosition = new Vector3(dw, dh);

            selectedTile = obj;
        }
    }

    void OnCancelTip()
    {
        UITooltip.ShowText(null);
        selectedTile = null;
    }

    void OnSellItem()
    {
        Config.Equipment equipment = items.DelItem(selectedTile);

        if (equipment != null)
        {
            Global.LocalHero.charactor.gold += (uint)Global.Cost(equipment);

            ParseAgent.StorageToLocal();
            attrChanged = true;
            UpdateGold();
        }

        UITooltip.ShowText(null);
    }

    void OnPutOn()
    {
        if (selectedTile == null)
            return;

        UITooltip.ShowText(null);

        UserData ud = selectedTile.GetComponent<UserData>();
        ushort eid = ushort.Parse(ud.data);
        if (!Config.DataLoader.equipmentMaps.ContainsKey(eid))
            return;

        Config.Equipment eq = Config.DataLoader.equipmentMaps[ushort.Parse(ud.data)];
        if (eq.type == Config.Equipment.Type.Armor)
        {
            if (Global.LocalHero.charactor.armor > 0 && Config.DataLoader.equipmentMaps.ContainsKey(Global.LocalHero.charactor.armor))
            {
                if (ReplaceItem(eq, Global.LocalHero.charactor.armor))
                {
                    Global.LocalHero.charactor.armor = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
            else
            {
                Config.Equipment deEq = items.DelItem(selectedTile);
                if (deEq != null)
                {
                    Global.LocalHero.charactor.armor = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
        }
        else if (eq.type == Config.Equipment.Type.Boots)
        {
            if (Global.LocalHero.charactor.boots > 0 && Config.DataLoader.equipmentMaps.ContainsKey(Global.LocalHero.charactor.boots))
            {
                if (ReplaceItem(eq, Global.LocalHero.charactor.boots))
                {
                    Global.LocalHero.charactor.boots = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
            else
            {
                Config.Equipment deEq = items.DelItem(selectedTile);
                if (deEq != null)
                {
                    Global.LocalHero.charactor.boots = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
        }
        else if (eq.type == Config.Equipment.Type.Gloves)
        {
            if (Global.LocalHero.charactor.gloves > 0 && Config.DataLoader.equipmentMaps.ContainsKey(Global.LocalHero.charactor.gloves))
            {
                if (ReplaceItem(eq, Global.LocalHero.charactor.gloves))
                {
                    Global.LocalHero.charactor.gloves = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
            else
            {
                Config.Equipment deEq = items.DelItem(selectedTile);
                if (deEq != null)
                {
                    Global.LocalHero.charactor.gloves = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
        }
        else if (eq.type == Config.Equipment.Type.Headgear)
        {
            if (Global.LocalHero.charactor.headgear > 0 && Config.DataLoader.equipmentMaps.ContainsKey(Global.LocalHero.charactor.headgear))
            {
                if (ReplaceItem(eq, Global.LocalHero.charactor.headgear))
                {
                    Global.LocalHero.charactor.headgear = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
            else
            {
                Config.Equipment deEq = items.DelItem(selectedTile);
                if (deEq != null)
                {
                    Global.LocalHero.charactor.headgear = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
        }
        else if (eq.type == Config.Equipment.Type.Weapon)
        {
            if (eq.weaponType == Config.WeaponType.Shields)
            {
                if (Global.LocalHero.charactor.weaponR > 0 && Config.DataLoader.equipmentMaps.ContainsKey(Global.LocalHero.charactor.weaponR))
                {
                    if (ReplaceItem(eq, Global.LocalHero.charactor.weaponR))
                    {
                        Global.LocalHero.charactor.weaponR = eq.ID;
                        ParseAgent.StorageToLocal();
                        attrChanged = true;
                    }
                }
                else
                {
                    Config.Equipment deEq = items.DelItem(selectedTile);
                    if (deEq != null)
                    {
                        Global.LocalHero.charactor.weaponR = eq.ID;
                        ParseAgent.StorageToLocal();
                        attrChanged = true;
                    }
                }
            }
            else //if (eq.weaponType == Config.WeaponType.Sword)
            {
                if (Global.LocalHero.charactor.weaponL > 0 && Config.DataLoader.equipmentMaps.ContainsKey(Global.LocalHero.charactor.weaponL))
                {
                    if (ReplaceItem(eq, Global.LocalHero.charactor.weaponL))
                    {
                        Global.LocalHero.charactor.weaponL = eq.ID;
                        ParseAgent.StorageToLocal();
                        attrChanged = true;
                    }
                }
                else
                {
                    Config.Equipment deEq = items.DelItem(selectedTile);
                    if (deEq != null)
                    {
                        Global.LocalHero.charactor.weaponL = eq.ID;
                        ParseAgent.StorageToLocal();
                        attrChanged = true;
                    }
                }
            }
            //else if (eq.weaponType == Config.WeaponType.Polearms)
            //{
            //}
        }
        else if (eq.type == Config.Equipment.Type.Belts)
        {
            if (Global.LocalHero.charactor.belts > 0 && Config.DataLoader.equipmentMaps.ContainsKey(Global.LocalHero.charactor.belts))
            {
                if (ReplaceItem(eq, Global.LocalHero.charactor.belts))
                {
                    Global.LocalHero.charactor.belts = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
            else
            {
                Config.Equipment deEq = items.DelItem(selectedTile);
                if (deEq != null)
                {
                    Global.LocalHero.charactor.belts = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
        }
        else if (eq.type == Config.Equipment.Type.Amulets)
        {
            if (Global.LocalHero.charactor.amulets > 0 && Config.DataLoader.equipmentMaps.ContainsKey(Global.LocalHero.charactor.amulets))
            {
                if (ReplaceItem(eq, Global.LocalHero.charactor.amulets))
                {
                    Global.LocalHero.charactor.amulets = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
            else
            {
                Config.Equipment deEq = items.DelItem(selectedTile);
                if (deEq != null)
                {
                    Global.LocalHero.charactor.amulets = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
        }
        else if (eq.type == Config.Equipment.Type.Ring)
        {
            if (Global.LocalHero.charactor.ring > 0 && Config.DataLoader.equipmentMaps.ContainsKey(Global.LocalHero.charactor.ring))
            {
                if (ReplaceItem(eq, Global.LocalHero.charactor.ring))
                {
                    Global.LocalHero.charactor.ring = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
            else
            {
                Config.Equipment deEq = items.DelItem(selectedTile);
                if (deEq != null)
                {
                    Global.LocalHero.charactor.ring = eq.ID;
                    ParseAgent.StorageToLocal();
                    attrChanged = true;
                }
            }
        }
        else if (eq.type == Config.Equipment.Type.Chest)
        {
            Game.ChangeScene("Chest",0,false);
        }
    }

    private bool ReplaceItem(Config.Equipment eq, ushort newEq)
    {
        Config.Equipment deEq = items.DelItem(selectedTile);

        Config.Equipment bpEq = Config.DataLoader.equipmentMaps[newEq];
        if (!OnAddItem(bpEq, true))
        {
            OnAddItem(deEq, true);

            LgMessageBox.Show("仓库空间不足", UIWidget.Pivot.Center, null);
        }
        else
        {
            if (deEq != null)
                return true;
        }

        return false;
    }

    private bool ReplaceItem(Config.Equipment eq, ushort newEq1, ushort newEq2)
    {
        Config.Equipment deEq = items.DelItem(selectedTile);

        Config.Equipment bpEq1 = Config.DataLoader.equipmentMaps[newEq1];
        Config.Equipment bpEq2 = Config.DataLoader.equipmentMaps[newEq2];
        if (!items.IsFull(bpEq1) || !items.IsFull(bpEq2))
        {
            OnAddItem(deEq, true);

            LgMessageBox.Show("仓库空间不足", UIWidget.Pivot.Center, null);
        }
        else
        {
            if (deEq != null)
            {
                OnAddItem(bpEq1, true);
                OnAddItem(bpEq2, true);
                return true;
            }
        }

        return false;
    }

    void OnClose()
    {
        Game.ChangeScene("Main");

        if (attrChanged)
        {
            if (!Global.SolePlayerMode)
            {
                Global.LocalHero.SaveAsyc("");
            }
            else
            {
                ParseAgent.StorageToLocal();
            }
        }
    }

    void OnClickRepair()
    {
        if (Breakage.Cost > 0)
        {
            float r = (float)Global.LocalHero.charactor.gold / Breakage.Cost;
            if (r > 1)
                Global.LocalHero.charactor.gold -= Breakage.Cost;
            else
                Global.LocalHero.charactor.gold = 0;

            Global.LocalHero.charactor.durability = (short)Mathf.Min(100, r * 100);

            Transform node = transform.FindChild("Camera/Anchor/Panel/Contain/RepairGold");
            UILabel repairGold = node.GetComponent<UILabel>();
            repairGold.text = Breakage.Cost.ToString();

            ParseAgent.StorageToLocal();
            attrChanged = true;

            UpdateGold();
        }
    }
}

