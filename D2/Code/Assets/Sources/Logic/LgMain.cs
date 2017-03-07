using UnityEngine;
using System.Collections;


public class LgMain : MonoBehaviour
{


    void Start()
    {
        Transform root = transform.FindChild("Camera/Anchor/Panel/Roles/A" + (int)Global.LocalHero.charactor.profession);
        root.gameObject.SetActive(true);

        root = transform.FindChild("Camera/Anchor/Panel/Name");
        UILabel lab = root.GetComponent<UILabel>();
        lab.text = Global.LocalHero.charactor.name;

        root = transform.FindChild("Camera/Anchor/Panel/Rank");
        lab = root.GetComponent<UILabel>();
        lab.text = "世界排名:" + Global.LocalHero.charactor.rank;

        if (Global.LocalHero.charactor.sceneID == 0)
        {
            Global.LocalHero.charactor.sceneID = 101;
        }

        ///////////////////////////////////////////////////
        int act = Global.LocalHero.charactor.sceneID / 100;
        Config.Scene yd = Config.DataLoader.sceneMaps.Find(delegate(Config.Scene sc) { return sc.ID == act * 100; });

        Transform node = transform.FindChild("Camera/Anchor/Panel/Act");
        UILabel actLab = node.GetComponent<UILabel>();
        actLab.text = "第" + Config.CharAttribute.ConvertCHNum(act) + "幕 " + yd.name;
        ///////////////////////////////////////////////////
        int idleDot = (Global.LocalHero.charactor.level - 1) * 5 - Global.LocalHero.charactor.str -
               Global.LocalHero.charactor.dex - Global.LocalHero.charactor.vit - Global.LocalHero.charactor.eng;

        node = transform.FindChild("Camera/Anchor/Panel/Shutcut/DotAttr");
        node.gameObject.SetActive(idleDot > 0);

        node = transform.FindChild("Camera/Anchor/Panel/Shutcut/DotSkill");
        node.gameObject.SetActive(Global.LocalHero.charactor.level - SkillInfo.getCharDot() > 0);
        ///////////////////////////////////////////////////
        node = transform.FindChild("Camera/Anchor/Panel/Breakage");
        Breakage.Update(node);
        ///////////////////////////////////////////////////
        if (Global.RankHeros.Count == 0)
            Global.GenRankHeros();
    }

    void OnClickSkill()
    {
        Game.ChangeScene("Skill");
    }

    void OnClickChar()
    {
        Game.ChangeScene("CharProp");
    }

    void OnClickRank()
    {
        if (Global.LocalHero.charactor.level >= 10)
        {
            if (Global.SolePlayerMode)
                Game.ChangeScene("Rank");
            else
                ParseAgent.handle.ReflushRank();
        }
        else
        {
            LgMessageBox.Show("该功能需要角色达到十级", UIWidget.Pivot.Center, null);
        }
    }

    void OnClickCopy()
    {
        Game.ChangeScene("GameCopy");
    }

    void OnClickTask()
    {
        Game.ChangeScene("Task");
    }

    void OnClickDepot()
    {
        Game.ChangeScene("Depot");
    }

    void OnClickEquip()
    {
        Game.ChangeScene("Backpack");
    }

    void OnClickSystemSetting()
    {
        if (LgSystemSetting.Visible)
            LgSystemSetting.Hide();
        else
            LgSystemSetting.Show();
    }

    void OnClose()
    {
        Game.ChangeScene("Main");
    }
}