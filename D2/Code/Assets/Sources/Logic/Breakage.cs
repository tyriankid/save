using System.Collections.Generic;
using UnityEngine;


public static class Breakage
{
    //private static bool breakage = false;
    private static bool full = false;

    private static bool IsDress()
    {
        return Global.LocalHero.charactor.armor > 0 || Global.LocalHero.charactor.belts > 0
                || Global.LocalHero.charactor.boots > 0 || Global.LocalHero.charactor.headgear > 0
                || Global.LocalHero.charactor.weaponL > 0 || Global.LocalHero.charactor.weaponR > 0;
    }

    public static void Update(Transform trans)
    {
        if (Global.LocalHero.charactor.durability > 65)
            return;

        trans.gameObject.SetActive(true);

        Transform t22 = trans.FindChild("breakage_22");
        t22.gameObject.SetActive(true);
        Transform t23 = trans.FindChild("breakage_23");
        t23.gameObject.SetActive(true);
        Transform t24 = trans.FindChild("breakage_24");
        t24.gameObject.SetActive(true);
        Transform t25 = trans.FindChild("breakage_25");
        t25.gameObject.SetActive(true);
        Transform t26 = trans.FindChild("breakage_26");
        t26.gameObject.SetActive(true);
        Transform t27 = trans.FindChild("breakage_27");
        t27.gameObject.SetActive(true);
        Transform t28 = trans.FindChild("breakage_28");
        t28.gameObject.SetActive(true);

        if (Global.LocalHero.charactor.durability < 5)
        {
            if (Global.LocalHero.charactor.gloves > 0)
                SetBreakageSprite(trans, "breakage_22");
            //else
            //    t22.gameObject.SetActive(false);
        }
        if (Global.LocalHero.charactor.durability < 15)
        {
            if (Global.LocalHero.charactor.headgear > 0)
                SetBreakageSprite(trans, "breakage_23");
            //else
            //    t23.gameObject.SetActive(false);
        }
        if (Global.LocalHero.charactor.durability < 25)
        {
            if (Global.LocalHero.charactor.belts > 0)
                SetBreakageSprite(trans, "breakage_24");
            //else
            //    t24.gameObject.SetActive(false);
        }
        if (Global.LocalHero.charactor.durability < 35)
        {
            if (Global.LocalHero.charactor.boots > 0)
                SetBreakageSprite(trans, "breakage_25");
            //else
            //    t25.gameObject.SetActive(false);
        }
        if (Global.LocalHero.charactor.durability < 45)
        {
            if (Global.LocalHero.charactor.armor > 0)
                SetBreakageSprite(trans, "breakage_26");
            //else
            //    t26.gameObject.SetActive(false);
        }
        if (Global.LocalHero.charactor.durability < 55)
        {
            if (Global.LocalHero.charactor.weaponR > 0)
                SetBreakageSprite(trans, "breakage_27");
            //else
            //    t27.gameObject.SetActive(false);
        }
        if (Global.LocalHero.charactor.durability < 65)
        {
            if (Global.LocalHero.charactor.weaponL > 0)
                SetBreakageSprite(trans, "breakage_28");
            //else
            //    t28.gameObject.SetActive(false);
        }

        //if (breakage)
        //{
        //    t22.gameObject.SetActive(true);
        //    t23.gameObject.SetActive(true);
        //    t24.gameObject.SetActive(true);
        //    t25.gameObject.SetActive(true);
        //    t26.gameObject.SetActive(true);
        //    t27.gameObject.SetActive(true);
        //    t28.gameObject.SetActive(true);
        //}

        full = IsDress();
    }

    private static void SetBreakageSprite(Transform trans, string name)
    {
        Transform t = trans.FindChild(name);
        UISprite sp = t.GetComponent<UISprite>();
        sp.spriteName = name;

        //breakage = true;
    }

    public static uint Cost
    {
        get
        {
            if (full)
                return (uint)((100 - Global.LocalHero.charactor.durability) * Global.LocalHero.charactor.level * 1.3f);
            else
                return 0;
        }
    }
}
