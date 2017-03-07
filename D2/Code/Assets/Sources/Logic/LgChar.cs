using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LgChar : MonoBehaviour
{
    private string root = "Camera/Panel/Anchor/Property/";
    private bool updated = false;


    void Start()
    {
        int idleDot = (Global.LocalHero.charactor.level - 1) * 5 - Global.LocalHero.charactor.str -
               Global.LocalHero.charactor.dex - Global.LocalHero.charactor.vit - Global.LocalHero.charactor.eng;

        transform.FindChild(root + "DexButton").gameObject.SetActive(idleDot > 0);
        transform.FindChild(root + "StrButton").gameObject.SetActive(idleDot > 0);
        transform.FindChild(root + "VitButton").gameObject.SetActive(idleDot > 0);
        transform.FindChild(root + "EngButton").gameObject.SetActive(idleDot > 0);

        UILabel l = transform.FindChild(root + "RemateDot").GetComponent<UILabel>();
        if (idleDot > 0)
            l.text = "剩余属性点数\n" + idleDot;
        l.gameObject.SetActive(idleDot > 0);

        UILabel lab = transform.FindChild(root + "Name").GetComponent<UILabel>();
        lab.text = Global.LocalHero.charactor.name;

        lab = transform.FindChild(root + "Level").GetComponent<UILabel>();
        lab.text = "等级\n"+Global.LocalHero.charactor.level;

        lab = transform.FindChild(root + "CurrentExp").GetComponent<UILabel>();
        lab.text = "当前经验\n" + Global.LocalHero.charactor.exp;

        lab = transform.FindChild(root + "NextExp").GetComponent<UILabel>();
        lab.text = "下一级经验\n" + Config.CharAttribute.CaleNeedExp(Global.LocalHero.charactor.level);

        lab = transform.FindChild(root + "Profision").GetComponent<UILabel>();
        lab.text = Global.Profession(Global.LocalHero.charactor.profession);

        UpdateAttribute();
    }



    private void UpdateAttribute()
    {
        UILabel lab = transform.FindChild(root + "Str").GetComponent<UILabel>();
        lab.text = "" + Config.CharAttribute.Str(Global.LocalHero.charactor);

        lab = transform.FindChild(root + "Dex").GetComponent<UILabel>();
        lab.text = "" + Config.CharAttribute.Dex(Global.LocalHero.charactor);

        lab = transform.FindChild(root + "Vit").GetComponent<UILabel>();
        lab.text = "" + Config.CharAttribute.Vit(Global.LocalHero.charactor);

        lab = transform.FindChild(root + "Eng").GetComponent<UILabel>();
        lab.text = "" + Config.CharAttribute.Eng(Global.LocalHero.charactor);

        lab = transform.FindChild(root + "AttackL").GetComponent<UILabel>();
        lab.text = "" + Config.CharAttribute.DamagePhyx(Global.LocalHero.charactor);

        lab = transform.FindChild(root + "AttackRate").GetComponent<UILabel>();
        lab.text = "" + Config.CharAttribute.CaleAttackRating(Global.LocalHero.charactor);

        lab = transform.FindChild(root + "Defence").GetComponent<UILabel>();
        lab.text = "" + Config.CharAttribute.Defence(Global.LocalHero.charactor);

        ushort attr = Config.CharAttribute.CaleAttr(Global.LocalHero.charactor, Config.CharAttribute.Type.Erg);
        lab = transform.FindChild(root + "ErgCur").GetComponent<UILabel>();
        lab.text = "" + attr;

        lab = transform.FindChild(root + "ErgMax").GetComponent<UILabel>();
        lab.text = "" + attr;

        int h = Config.CharAttribute.HP(Global.LocalHero.charactor);
        lab = transform.FindChild(root + "HPCur").GetComponent<UILabel>();
        lab.text = "" + h;

        lab = transform.FindChild(root + "HPMax").GetComponent<UILabel>();
        lab.text = "" + h;

        attr = Config.CharAttribute.CaleAttr(Global.LocalHero.charactor, Config.CharAttribute.Type.Stamina);
        lab = transform.FindChild(root + "MgcCur").GetComponent<UILabel>();
        lab.text = "" + attr;

        lab = transform.FindChild(root + "MgcMax").GetComponent<UILabel>();
        lab.text = "" + attr;
    }

    void OnClose()
    {
        if (updated)
        {
            if (Global.SolePlayerMode)
            {
                ParseAgent.StorageToLocal();
                Game.ChangeScene("Main");
            }
            else
                Global.LocalHero.SaveAsyc("Main");
        }
        else
            Game.ChangeScene("Main");
    }

    void OnAddStrDot(GameObject arg)
    {
        Global.LocalHero.charactor.str++;

        UpdateButtonState(arg);

        updated = true;
    }

    private void UpdateButtonState(GameObject arg)
    {
        int idleDot = (Global.LocalHero.charactor.level - 1) * 5 - Global.LocalHero.charactor.str -
               Global.LocalHero.charactor.dex - Global.LocalHero.charactor.vit - Global.LocalHero.charactor.eng;

        bool hasDot = idleDot > 0;
        Transform trans = transform.FindChild(root + "DexButton");
        trans.gameObject.SetActive(hasDot);

        trans = transform.FindChild(root + "EngButton");
        trans.gameObject.SetActive(hasDot);

        trans = transform.FindChild(root + "StrButton");
        trans.gameObject.SetActive(hasDot);

        trans = transform.FindChild(root + "VitButton");
        trans.gameObject.SetActive(hasDot);

        UILabel lab = transform.FindChild(root + "RemateDot").GetComponent<UILabel>();
        lab.gameObject.SetActive(hasDot);
        if (hasDot)
            lab.text = "剩余属性点数\n" + idleDot;

        UpdateAttribute();
    }

    void OnAddDexDot(GameObject arg)
    {
        Global.LocalHero.charactor.dex++;

        UpdateButtonState(arg);

        updated = true;
    }

    void OnAddVitDot(GameObject arg)
    {
        Global.LocalHero.charactor.vit++;

        UpdateButtonState(arg);

        updated = true;
    }

    void OnAddEngDot(GameObject arg)
    {
        Global.LocalHero.charactor.eng++;

        UpdateButtonState(arg);

        updated = true;
    }
}