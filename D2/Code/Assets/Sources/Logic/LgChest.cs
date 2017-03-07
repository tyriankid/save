using System.Collections.Generic;
using UnityEngine;
using System.Collections;



public class LgChest : MonoBehaviour
{
    private int cids = 1563;

    private ItemManager items = new ItemManager();
    private UIAtlas atlasRoot;
    private UISprite sprite;

    void Start()
    {
        items.Start();

        sprite = transform.FindChild("Camera/Anchor/Panel/Equipment").GetComponent<UISprite>();
        atlasRoot = sprite.atlas;

        for (int i = 0; i < Global.LocalHero.charactor.backpack.Count; i++)
        {
            Config.Equipment eq = Config.DataLoader.equipmentMaps[Global.LocalHero.charactor.backpack[i]];

            if (eq != null && eq.costSpace == 0)
                eq.costSpace = LgDepot.GetAbjustTile(eq.icon, atlasRoot);

            if (!items.AddItem(eq))
            {
                Debug.Log("start 空间不足");
            }
        }

        Config.Equipment genEq = null;
        bool taken = false;

        ushort id = (ushort)Random.Range(1001, 1001 + Config.DataLoader.equipmentMaps.Count);
        if (Config.DataLoader.equipmentMaps.ContainsKey(id))
        {
            genEq = Config.DataLoader.equipmentMaps[id];
            taken = genEq.lvlRequest < Global.LocalHero.charactor.level + 30;
        }

        items.DelItem(cids);
        if (taken)
        {
            genEq.costSpace = LgDepot.GetAbjustTile(genEq.icon, atlasRoot);
            UISpriteData usd = atlasRoot.GetSprite(genEq.icon);
            sprite.width = usd.width;
            sprite.height = usd.height;

            if ((taken = items.IsFull(genEq)))
            {
                Global.LocalHero.charactor.backpack.Add(genEq.ID);
            }
        }

        StartCoroutine(OnAnimFinish(taken, genEq));
    }

    IEnumerator OnAnimFinish(bool taken, Config.Equipment eq)
    {
        yield return new WaitForSeconds(2.2f);

        GameObject.DestroyImmediate(transform.FindChild("Camera/Anchor/Panel/Anim").gameObject);

        if (taken)
        {
            sprite.gameObject.SetActive(true);
            sprite.spriteName = eq.icon;
        }

        if (!Global.SolePlayerMode)
        {
            Global.LocalHero.SaveAsyc("", true);
        }
        else
        {
            ParseAgent.StorageToLocal();
        }

        yield return new WaitForSeconds(2);

        Game.ChangeScene("Depot");
    }
}

