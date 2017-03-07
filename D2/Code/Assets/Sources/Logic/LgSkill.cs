using UnityEngine;
using System.Collections;


public class LgSkill : MonoBehaviour
{
    private SkillInfo skillInfo;
    private GameObject selectedTile;
    private bool updated = false;

    private Transform[] skillNode = new Transform[3];
    private int toggledIndex = 0;

    public static LgSkill handle;


    void Start()
    {
        handle = this;

        if (Global.LocalHero == null) return;

        switch (Global.LocalHero.charactor.profession)
        {
            case Config.Profession.Amazon:
                skillInfo = new SkillAma();
                break;
            case Config.Profession.Assassin:
                skillInfo = new SkillAss();
                break;
            case Config.Profession.Barbarian:
                skillInfo = new SkillBar();
                break;
            case Config.Profession.Druid:
                skillInfo = new SkillDru();
                break;
            case Config.Profession.Necromancer:
                skillInfo = new SkillNec();
                break;
            case Config.Profession.Paladin:
                skillInfo = new SkillPal();
                break;
            case Config.Profession.Sorceress:
                skillInfo = new SkillSor();
                break;
        }
        skillInfo.Init();

        string root = "Camera/Anchor/Panel/" + Global.LocalHero.charactor.profession;
        transform.FindChild(root).gameObject.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            Transform node = transform.FindChild(root + "/Skill_" + (i+1));
            skillNode[i] = node;
        }

        for (int i = 0; i < Global.LocalHero.charactor.skilltree.Count; i++)
        {
            int sid = SkillInfo.Parse(Global.LocalHero.charactor.skilltree[i], true);
            int lvl = SkillInfo.Parse(Global.LocalHero.charactor.skilltree[i], false);

            Dat d = skillInfo.getskill(sid);
            if (d != null)
                d.lvl = lvl;
        }

        skillInfo.Update();

        UpdateSkillLevelAndDot(true);
    }

    void OnDestroy()
    {
        handle = null;
    }

    private void UpdateSkillLevelAndDot(bool first)
    {
        string root = "Camera/Anchor/Panel/" + Global.LocalHero.charactor.profession;

        int allDot = skillInfo.getDot();

        UILabel dot = transform.FindChild("Camera/Anchor/Panel/RemDot").GetComponent<UILabel>();
        dot.text = (Global.LocalHero.charactor.level - allDot).ToString();

        for (int i = 0; i < 3; i++)
        {
            if (first)
            {
                string str = root + "/Tab" + (i + 1) + "/Label";
                UILabel lab = transform.FindChild(str).GetComponent<UILabel>();
                lab.text = skillInfo.Tabs[2 - i].name;
            }

            for (int j = 0; j < skillInfo.Tabs[i].skills.Count; j++)
            {
                Dat dat = skillInfo.Tabs[i].skills[j];
                SkillTile skill = transform.FindChild(root + "/Skill_" + (i + 1) + "/Template" + dat.id).GetComponent<SkillTile>();

                if (first)
                {
                    dat.lvlRequest = skill.lvlRequest;
                    dat.tile = skill;
                }

                bool canAddDot = false;
                if (dat.lvl < 20 && allDot < Global.LocalHero.charactor.level && 
                    dat.lvlRequest <= Global.LocalHero.charactor.level)
                {
                    Dat par = skillInfo.getskill(dat.parent);
                    if (dat.parent == 0 || par.lvl > 0)
                    {
                        canAddDot = true;
                    }
                }

                skill.transform.FindChild("Sprite").gameObject.SetActive(!canAddDot);

                UILabel lvllab = skill.transform.FindChild("Label").GetComponent<UILabel>();
                lvllab.text = dat.lvl.ToString();
            }
        }
    }

    string CombSkillTips(SkillTile tile)
    {
        Dat dat = skillInfo.getskill(tile.sid);

        string tips = skillInfo.display(dat);

        return tips;
    }

    void OnClickTip(GameObject obj)
    {
        SkillTile ud = obj.GetComponent<SkillTile>();
        if (ud == null)
            return;

        if (selectedTile != obj)
        {
            UITooltip.ShowText(CombSkillTips(ud));
            UpdateTipsLocation();

            selectedTile = obj;
        }
        else
        {
            Dat dat = skillInfo.getskill(ud.sid);
            if (skillInfo.canAddDot(dat))
            {
                dat.lvl++;

                skillInfo.FillAttribute(dat);
                // 更新剩余点数
                UpdateSkillLevelAndDot(false);

                // 更新TIP
                UITooltip.ShowText(CombSkillTips(dat.tile));
                UpdateTipsLocation();

                Global.LocalHero.charactor.skilltree.Add(dat.CombID);

                updated = true;
            }
            //
        }
    }

    private void UpdateTipsLocation()
    {
        Transform root = transform.FindChild("Camera/Anchor/Panel/Tooltip_UI");
        UISprite bg = root.GetComponent<UISprite>();

        UILabel lab = root.FindChild("Label").GetComponent<UILabel>();
        lab.transform.localPosition = new Vector2(0, -6);
        bg.width = Mathf.Max(bg.width + 8, 100);
        bg.height += 10;

        float dw = root.localPosition.x + bg.width - Screen.width / 2;
        float dh = root.localPosition.y - bg.height + Screen.height / 2;
        if (dh < 0)
            dh = root.localPosition.y - dh;
        else
            dh = root.localPosition.y;

        if (dw > 0)
            dw = root.localPosition.x + bg.width / 2 - dw;
        else
            dw = root.localPosition.x + bg.width / 2;

        root.localPosition = new Vector3(dw, dh);
    }

    public void OnChangeTab1()
    {
        if (skillNode[0].gameObject.activeSelf)
        {
            toggledIndex = 0;
        }
        else
        {
            UITooltip.ShowText(null);
        }
    }

    public void OnChangeTab2()
    {
        if (skillNode[1].gameObject.activeSelf)
        {
            toggledIndex = 1;
        }
        else
        {
            UITooltip.ShowText(null);
        }
    }

    public void OnChangeTab3()
    {
        if (skillNode[2].gameObject.activeSelf)
        {
            toggledIndex = 2;
        }
        else
        {
            UITooltip.ShowText(null);
        }
    }

    void OnCancelTip()
    {
        UITooltip.ShowText(null);
        selectedTile = null;
    }

    void OnOK(params object[] args)
    {
        LgMessageBox.Hide();

        updated = true;
        Global.LocalHero.charactor.skilltree.Clear();

        string root = "Camera/Anchor/Panel/" + Global.LocalHero.charactor.profession;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < skillInfo.Tabs[i].skills.Count; j++)
            {
                Dat dat = skillInfo.Tabs[i].skills[j];
                dat.lvl = 0;

                bool canAddDot = false;
                if (dat.lvl < 20 && dat.lvlRequest <= Global.LocalHero.charactor.level)
                {
                    Dat par = skillInfo.getskill(dat.parent);
                    if (dat.parent == 0 || par.lvl > 0)
                    {
                        canAddDot = true;
                    }
                }

                dat.tile.transform.FindChild("Sprite").gameObject.SetActive(!canAddDot);

                UILabel lvllab = dat.tile.transform.FindChild("Label").GetComponent<UILabel>();
                lvllab.text = dat.lvl.ToString();
            }
        }

        UILabel dot = transform.FindChild("Camera/Anchor/Panel/RemDot").GetComponent<UILabel>();
        dot.text = Global.LocalHero.charactor.level.ToString();
    }

    void OnResetDot()
    {
        UITooltip.ShowText(null);

        if (Global.LocalHero.charactor.skilltree.Count > 0)
        {
            LgMessageBox.Show("是否重置技能属性点", UIWidget.Pivot.Center, OnOK);
        }
    }

    public void OnClose()
    {
        skillInfo.save();
        if (updated)
        {
            if (Global.SolePlayerMode)
                ParseAgent.StorageToLocal();
            else
            {
                Global.LocalHero.SaveAsyc("Main");
                return;
            }
        }

        Game.ChangeScene("Main");
    }
}