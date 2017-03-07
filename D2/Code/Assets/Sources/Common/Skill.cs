using System;
using System.Collections.Generic;



public class Tabs
{
    public string name;
    public List<Dat> skills = new List<Dat>();
    // 次序由下到上
}

public class Dat
{
    public int id;
    public string name;
    public string desc;
    public string attr;
    public string extr = "";
    public int lvl = 0;
    public int parent;
    public int lvlRequest;
    public SkillTile tile;

    public int CombID
    {
        get
        {
            return (id << 8) + lvl;
        }
    }
}

public abstract class SkillInfo
{
    protected Tabs[] tabs = new Tabs[3];

    public Tabs[] Tabs
    {
        get { return tabs; }
    }

    public void save()
    {
        Global.LocalHero.charactor.skilltree.Clear();
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < tabs[i].skills.Count; ++j)
            {
                if (tabs[i].skills[j].lvl > 0)
                    Global.LocalHero.charactor.skilltree.Add(tabs[i].skills[j].CombID);
            }
        }
    }

    public static int getCharDot()
    {
        int allDot = 0;
        for (int i = 0; i < Global.LocalHero.charactor.skilltree.Count; i++)
        {
            allDot += SkillInfo.Parse(Global.LocalHero.charactor.skilltree[i], false);
        }

        return allDot;
    }

    public int getDot()
    {
        int dot = 0;
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < tabs[i].skills.Count; ++j)
            {
                dot += tabs[i].skills[j].lvl;
            }
        }
        return dot;
    }

    public Dat getskill(int s)
    {
        for (int i = 0; i < 3; ++i)
            for (int j = 0; j < tabs[i].skills.Count; ++j)
                if (tabs[i].skills[j].id == s)
                    return tabs[i].skills[j];

        return null;
    }

    public Dat getskill(int tab, int s)
    {
        for (int j = 0; j < tabs[tab].skills.Count; ++j)
            if (tabs[tab].skills[j].id == s)
                return tabs[tab].skills[j];

        return null;
    }

    public bool canAddDot(Dat dat)
    {
        if (dat.lvl < 20 && getDot() < Global.LocalHero.charactor.level &&
            dat.lvlRequest <= Global.LocalHero.charactor.level)
        {
            Dat par = getskill(dat.parent);
            if (dat.parent == 0 || par.lvl > 0)
            {
                return true;
            }
        }
        
        return false;
    }

    public string display(Dat desc)
    {
        string tips = "[22EE00]" + (desc.lvl == 0 ? "你还没有学到这项技能\n\n" : "") + desc.name + "[-]\n";

        bool request = canAddDot(desc);

        tips += (!request ? "[EC0000]" : "[FFFFFF]") + desc.desc + "\n";

        if (!request && Global.LocalHero.charactor.level < desc.lvlRequest)
        {
            tips += "需要等级 : " + desc.lvlRequest + "\n";
        }
        if (desc.lvl > 0)
        {
            tips += "当前技能等级:" + desc.lvl + "\n";
            tips += desc.attr + desc.extr + "[-]";
        }
        else
        {
            tips += desc.extr + "[-]";
        }

        return tips;
    }

    public static int ln(int l, int a, int b=0, int c = 0, int d = 0, int e = 0, int f = 0)
    {
        c = c != 0 ? c : b != 0 ? b : 0;
        d = d != 0 ? d : c != 0 ? c : 0;
        e = e != 0 ? e : d != 0 ? d : 0;
        f = f != 0 ? f : e != 0 ? e : 0;
        return a + (
            l > 28 ? 7 * b + 8 * c + 6 * d + 6 * e + (l - 28) * f :
            l > 22 ? 7 * b + 8 * c + 6 * d + (l - 22) * e :
            l > 16 ? 7 * b + 8 * c + (l - 16) * d :
            l > 8 ? 7 * b + (l - 8) * c :
            l > 0 ? (l - 1) * b :
            -a);
    }

    public static int dec(float n, float d) { double f = Math.Pow(10, d); return (int)(Math.Floor(n * f) / f); }

    public static int max(int a, int b) { return Math.Max(a, b); }
    public static int min(int a, int b) { return Math.Min(a, b); }

    public int blvl(int s)
    {
        Dat t = getskill(s);
        if (t == null) return 0;
        return t.lvl;
    }

    public int tlvl(int s)
    {
        return lvl(s);
    }

    public int lvl(int s)
    {
        Dat t = getskill(s);
        if (t == null) return 0;
        int b = t.lvl;
        return b;//b>0?b+parseInt(tabs[t[0]].plus.value)
        //+parseInt(tabs.plus.value):0
    }

    public int dm(int l, int a=0, int b=0)
    {
        return a + dec((b - a) * dec(110 * l / (l + 6), 0) / 100, 0);
    }

    public string sign(int n)
    {
        return n > 0 ? "+" + n : n.ToString();
    }

    public virtual void Init()
    {
        for (int i = 0; i < 3; i++)
            tabs[i] = new Tabs();
    }

    public abstract void FillAttribute(int tab, int index);
    public abstract void FillAttribute(Dat dat);

    public void Update()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < tabs[i].skills.Count; j++)
            {
                FillAttribute(i, j);
            }
        }
    }

    public static int Parse(int uid, bool getid)
    {
        if (getid)
            return uid >> 8;
        else
            return uid & 0x00FF;
    }
}

public class SkillAma : SkillInfo
{       
    public override void Init()
    {
        base.Init();
        //tabs[0].sID = new byte[10] { 6, 7, 11, 12, 16, 21, 22, 26, 27, 31 };
        Tabs[0].name = "弓和\n十字弓\n技能";
        //tabs[1].sID = new byte[10] { 8, 9, 13, 17, 18, 23, 28, 29, 32, 33 };
        Tabs[1].name = "被动\n和魔法\n技能";
        //tabs[2].sID = new byte[10] { 10, 14, 15, 19, 20, 24, 25, 30, 34, 35 };
        Tabs[2].name = "标枪\n和长矛\n技能";

        InitTab1();
        InitTab2();
        InitTab3();
    }

    private void InitTab3()
    {
        Dat d20 = new Dat();
        d20.id = 10;
        d20.name = "戳刺";
        d20.desc = "用标枪或长矛类武器多次连续攻击敌人";
        Tabs[2].skills.Add(d20);

        Dat d21 = new Dat();
        d21.id = 14;
        d21.name = "威力一击";
        d21.desc = "使用标枪或长矛武器时附加闪电伤害";
        Tabs[2].skills.Add(d21);

        Dat d22 = new Dat();
        d22.id = 15;
        d22.name = "毒枪";
        d22.desc = "标枪会魔法般地留下毒气云雾";
        Tabs[2].skills.Add(d22);

        Dat d23 = new Dat();
        d23.id = 19;
        d23.name = "刺爆";
        d23.desc = "借由降低武器耐久度来增加伤害";
        Tabs[2].skills.Add(d23);

        Dat d24 = new Dat();
        d24.id = 20;
        d24.name = "闪电球";
        d24.desc = "掷出的标枪变成闪电球";
        Tabs[2].skills.Add(d24);

        Dat d25 = new Dat();
        d25.id = 24;
        d25.name = "充能一击";
        d25.desc = "在标枪或长矛附加充能的光弹";
        Tabs[2].skills.Add(d25);

        Dat d26 = new Dat();
        d26.id = 25;
        d26.name = "瘟疫标枪";
        d26.desc = "让掷出的标枪爆出剧毒的云团";
        Tabs[2].skills.Add(d26);

        Dat d27 = new Dat();
        d27.id = 30;
        d27.name = "击退";
        d27.desc = "同时攻击所有的附近目标";
        Tabs[2].skills.Add(d27);

        Dat d28 = new Dat();
        d28.id = 34;
        d28.name = "闪电攻击";
        d28.desc = "在使用的标枪和矛类武器上增加了闪电的伤害，\n同时在击中目标的时候释放出连环闪电";
        Tabs[2].skills.Add(d28);

        Dat d29 = new Dat();
        d29.id = 35;
        d29.name = "闪电之怒";
        d29.desc = "在投掷出的标枪上加入了强有力的闪电伤害力，\n并在命中目标的时候释放出闪电";
        Tabs[2].skills.Add(d29);
    }

    private void InitTab2()
    {
        Dat d10 = new Dat();
        d10.id = 8;
        d10.name = "内视";
        d10.desc = "使怪物发光并降低它们的防御";
        Tabs[1].skills.Add(d10);

        Dat d11 = new Dat();
        d11.id = 9;
        d11.name = "致命打击";
        d11.desc = "被动 - 你的攻击有概率造成双倍伤害";
        Tabs[1].skills.Add(d11);

        Dat d12 = new Dat();
        d12.id = 13;
        d12.name = "闪避";
        d12.desc = "被动 - 增加站立或战斗状态时躲避近身攻击的能力";
        Tabs[1].skills.Add(d12);

        Dat d13 = new Dat();
        d13.id = 17;
        d13.name = "慢速箭";
        d13.desc = "使附近的对手呈现高亮\n并降低它们射击攻击速度";
        Tabs[1].skills.Add(d13);

        Dat d14 = new Dat();
        d14.id = 18;
        d14.name = "躲避";
        d14.desc = "借由降低武器耐久度来增加伤害";
        Tabs[1].skills.Add(d14);

        Dat d15 = new Dat();
        d15.id = 23;
        d15.name = "刺入";
        d15.desc = "被动 - 增加攻击命中率";
        Tabs[1].skills.Add(d15);

        Dat d16 = new Dat();
        d16.id = 28;
        d16.name = "诱饵";
        d16.desc = "创造一个自身的幻象来迷惑敌人";
        Tabs[1].skills.Add(d16);

        Dat d17 = new Dat();
        d17.id = 29;
        d17.name = "逃避";
        d17.desc = "被动 - 增加在移动过程中\n对近身或远程攻击的躲避能力";
        Tabs[1].skills.Add(d17);

        Dat d18 = new Dat();
        d18.id = 32;
        d18.name = "女武神";
        d18.desc = "召唤出一个强有力的女武神协同作战";
        Tabs[1].skills.Add(d18);

        Dat d19 = new Dat();
        d19.id = 33;
        d19.name = "穿透";
        d19.desc = "被动 - 使亚马逊战士的发射物\n有几率穿透命中的敌人";
        Tabs[1].skills.Add(d19);
    }

    private void InitTab1()
    {
        Dat d00 = new Dat();
        d00.id = 6;
        d00.name = "魔法箭";
        d00.desc = "射出一束箭状魔力束\n给对手造成额外的伤害\n并且不会消耗箭支。\n";
        Tabs[0].skills.Add(d00);

        Dat d01 = new Dat();
        d01.id = 7;
        d01.name = "火焰箭";
        d01.desc = "在箭矢或弓弹上附带了魔法火焰\n并在打击时增加了火焰的伤害。\n";
        Tabs[0].skills.Add(d01);

        Dat d02 = new Dat();
        d02.id = 11;
        d02.name = "冰箭";
        d02.desc = "在箭矢或弓弹上增加额外的冰冻伤害和减慢效果\n冰冻伤害只能造成正常时一半的伤害\n";
        Tabs[0].skills.Add(d02);

        Dat d03 = new Dat();
        d03.id = 12;
        d03.name = "多重箭";
        d03.desc = "由一枝分成多枝射出的魔法般的箭矢或弓弹";
        Tabs[0].skills.Add(d03);

        Dat d04 = new Dat();
        d04.id = 16;
        d04.name = "爆裂箭";
        d04.desc = "让箭矢或弓弹爆裂开\n击中中间和附近的敌人";
        Tabs[0].skills.Add(d04);

        Dat d05 = new Dat();
        d05.id = 21;
        d05.name = "急冻箭";
        d05.desc = "在箭矢或弓弹上增加冰冻伤害\n并冻结住你的敌人";
        Tabs[0].skills.Add(d05);

        Dat d06 = new Dat();
        d06.id = 22;
        d06.name = "向导箭";
        d06.desc = "让箭矢或弓弹自动追踪目标\n一定命中";
        Tabs[0].skills.Add(d06);

        Dat d07 = new Dat();
        d07.id = 26;
        d07.name = "炮轰";
        d07.desc = "让射出的箭矢自动瞄准攻击\n多个附近的敌人";
        Tabs[0].skills.Add(d07);

        Dat d08 = new Dat();
        d08.id = 27;
        d08.name = "祭奠之箭";
        d08.desc = "让箭矢或弓弹附加强烈火焰魔法\n并燃起强烈的火焰";
        Tabs[0].skills.Add(d08);

        Dat d09 = new Dat();
        d09.id = 31;
        d09.name = "冻结之箭";
        d09.desc = "冰冻加强的箭矢或弓弹\n可以冻结多个敌人";
        Tabs[0].skills.Add(d09);
    }

    public override void FillAttribute(int tab, int index)
    {
        Dat desc = Tabs[tab].skills[index];
        FillAttribute(desc);
    }

    public override void FillAttribute(Dat desc)
    {
        int lvl = desc.lvl + Global.LocalHero.charactor.ski; // +附加值      
        desc.attr = "";

        switch (desc.id)
        {
            case 6:
                desc.attr = "转化 " + (1 + (lvl - 1) * 1) + "% 物理伤害为元素伤害\n";
                desc.attr += "命中率: +" + ln(lvl, 10, 9) + "%\n";
                desc.attr += "伤害: " + dec((lvl << 8) / 256, 0) + "\n";
                desc.attr += "魔法消耗: " + dec(Math.Max(ln(lvl, 12, -1) << 5, 0) / 256, 1) + "\n";
                break;

            case 7:
                desc.attr = "转化 " + (3 + (lvl - 1) * 2) + "% 物理伤害为元素伤害\n";
                desc.attr += "命中率: +" + ln(lvl, 10, 9) + "%\n";
                desc.attr += "火焰伤害: " + (dec(dec((ln(lvl, 1, 2, 3, 6, 12, 24) << 8) * (100 + ((blvl(16)) * 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 4, 2, 3, 7, 14, 27) << 8) * (100 + ((blvl(16)) * 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 24, 1) << 5, 1 << 8) / 256, 1) + "\n";
                break;

            case 8:
                desc.attr = "持续时间: " + dec((ln(lvl, 200, 100)) / 25, 1) + " 秒\n";
                desc.attr += "敌人防御: " + dec(-dec(dec((ln(lvl, 40, 25, 45, 60, 80, 100) << 8), 0) / 256, 0), 0) + "\n";
                desc.attr += "半径: " + dec(dec(ln(lvl, 20, 0), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 10, 0) << 7, 1 << 8) / 256, 1) + "\n";

                break;
            case 9:
                desc.attr = "" + dec(dm(lvl, 5, 80), 0) + "% 概率\n";

                break;
            case 10:
                desc.attr = "命中率: +" + sign(dec(ln(lvl, 10, 9), 0)) + "%\n";
                desc.attr += "伤害: " + sign(dec(ln(lvl, -15, 3), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 8, 1) << 6, 1 << 8) / 256, 1) + "\n";

                break;
            case 11:
                desc.attr = "转化 " + (3 + (lvl - 1) * 2) + "% 物理伤害为元素伤害\n";
                desc.attr += "命中率: +" + ln(lvl, 10, 9) + "%";
                desc.attr += "冰冷伤害: " + (dec(dec((ln(lvl, 6, 4, 5, 8, 16, 42) << 7) * (100 + ((blvl(21)) * 12)) / 100, 0) / 256, 0)) + "-"
                                            + (dec(dec((ln(lvl, 8, 4, 5, 9, 17, 44) << 7) * (100 + ((blvl(21)) * 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "冰冻时间: " + dec(dec(ln(lvl, 100, 30), 0) / 25, 1) + " 秒\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 28, 1) << 5, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]冰箭 由以下技能得到额外加成:\n[-]急冻箭: +12% 冰冷伤害每一技能等级\n";

                break;
            case 12:
                desc.attr = "" + dec(min(24, ln(lvl, 2, 1)), 0) + " 枝";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 4, 1) << 8, 1 << 8) / 256, 1);
                desc.attr += "3/4 武器伤害";

                break;
            case 13:
                desc.attr = "" + dec(dm(lvl, 10, 65), 0) + "% 概率";

                break;
            case 14:
                desc.attr = "命中率: +" + ln(lvl, 20, 12) + "%\n";
                desc.attr += "闪电伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(34) + blvl(20) + blvl(24) + blvl(35)) * 10)) / 100, 0) / 256, 0)) + '-' + (dec(dec((ln(lvl, 16, 18, 36, 54, 72, 90) << 8) * (100 + ((blvl(34) + blvl(20) + blvl(24) + blvl(35)) * 10)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 8, 1) << 6, 1 << 8) / 256, 1);
                desc.extr += "[22EE00]威力一击 由以下技能得到额外加成:\n[-]闪电球: +10% 闪电伤害每一技能等级\n充能一击: +10% 闪电伤害每一技能等级\n闪电攻击: +10% 闪电伤害每一技能等级\n闪电之怒: +10% 闪电伤害每一技能等级\n";

                break;
            case 15:
                desc.attr = "毒素伤害: " + (dec((dec(ln(lvl, 32, 16, 32, 48, 64, 96) * (100 + ((blvl(25)) * 12)) / 100, 0)) * (dec(ln(lvl, 200, 50), 0)) / 256, 0)) + '-' + (dec((dec(ln(lvl, 48, 16, 36, 52, 68, 84) * (100 + ((blvl(25)) * 12)) / 100, 0)) * (dec(ln(lvl, 200, 50), 0)) / 256, 0)) + "\n结束于 " + dec(dec(ln(lvl, 200, 50), 0) / 25, 1) + " 秒";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 16, 1) << 6, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]毒枪 由以下技能得到额外加成[-]\n瘟疫标枪: +12% 毒素伤害每一技能等级\n";

                break;
            case 16:
                desc.attr = "命中率: +" + ln(lvl, 20, 9) + "%";
                desc.attr += "火焰伤害: " + (dec(dec((ln(lvl, 2, 5, 7, 9, 12, 20) << 8) * (100 + ((blvl(7)) * 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 6, 5, 8, 11, 14, 23) << 8) * (100 + ((blvl(7)) * 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 10, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]爆裂箭 由以下技能得到额外加成:[-]\n火焰箭: +12% 火焰伤害每一技能等级\n";

                break;
            case 17:
                desc.attr = "持续时间: " + dec((ln(lvl, 300, 150)) / 25, 1) + " 秒\n";
                desc.attr += "减缓敌人速度 " + dec(ln(lvl, 33, 0), 0) + "%\n";
                desc.attr += "半径: " + dec(dec(ln(lvl, 20, 0), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 10, 0) << 7, 1 << 8) / 256, 1) + "\n";

                break;
            case 18:
                desc.attr = "" + dec(dm(lvl, 15, 75), 0) + "% 概率\n";

                break;
            case 19:
                desc.attr = "伤害: " + sign(dec(ln(lvl, 300, 25), 0)) + "%\n";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 100, 25), 0)) + "%\n";
                desc.attr += "武器耐久度损失: " + dec(50 - dm(lvl, 0, 30), 0) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 3, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 20:
                desc.attr = "闪电伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(34) + blvl(14) + blvl(24) + blvl(35)) * 3)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 40, 12, 18, 28, 48, 88) << 8) * (100 + ((blvl(34) + blvl(14) + blvl(24) + blvl(35)) * 3)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 24, 1) << 6, 1 << 8) / 256, 1) + "\n";
                desc.attr += "3/4 武器伤害\n转变 100% 物理伤害为元素伤害\n";
                desc.extr += "[22EE00]闪电球 由以下技能得到额外加成:[-]\n威力一击: +3% 闪电伤害每一技能等级\n充能一击: +3% 闪电伤害每一技能等级\n闪电攻击: +3% 闪电伤害每一技能等级\n闪电之怒: +3% 闪电伤害每一技能等级\n";

                break;
            case 21:
                desc.attr = "命中率: +" + ln(lvl, 20, 9) + "%\n";
                desc.attr += "冰冷伤害: " + (dec(dec((ln(lvl, 6, 6, 12, 18, 26, 36) << 8) * (100 + ((blvl(11)) * 8)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 10, 6, 13, 19, 27, 38) << 8) * (100 + ((blvl(11)) * 8)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "冰冻时间 " + dec((dec(ln(lvl, 50, 5) * (100 + ((blvl(31)) * 5)) / 100, 0)) / 25, 1) + " 秒\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 16, 1) << 6, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]急冻箭 由以下技能得到额外加成:[-]\n冰箭: +8% 冰冷伤害每一技能等级\n冻结之箭: +5% 冰冻时间每一技能等级\n";

                break;
            case 22:
                desc.attr = "伤害: " + sign(dec(ln(lvl, 0, 5), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 32, -1) << 6, 1 << 8) / 256, 1) + "\n";

                break;
            case 23:
                desc.attr = "命中率: " + sign(dec(ln(lvl, 35, 10), 0)) + "%\n";

                break;
            case 24:
                desc.attr = "释放 " + dec(3 + lvl / 5, 0) + " 闪电弹";
                desc.attr += "闪电伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(34) + blvl(20) + blvl(14) + blvl(35)) * 10)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 30, 12, 16, 20, 24, 28) << 8) * (100 + ((blvl(34) + blvl(20) + blvl(14) + blvl(35)) * 10)) / 100, 0) / 256, 0));
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 16, 1) << 6, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]充能一击 由以下技能得到额外加成:[-]\n威力一击: +10% 闪电伤害每一技能等级\n闪电球: +10% 闪电伤害每一技能等级\n闪电攻击: +10% 闪电伤害每一技能等级\n闪电之怒: +10% 闪电伤害每一技能等级\n";

                break;
            case 25:
                desc.attr = "命中率: " + sign(dec(ln(lvl, 30, 9), 0)) + "%\n";
                desc.attr += "毒素伤害: " + (dec((dec((ln(lvl, 10, 6, 12, 20, 40, 60) << 3) * (100 + ((blvl(15)) * 10)) / 100, 0)) * (dec(ln(lvl, 75, 10), 0)) / 256, 0)) + "-" + (dec((dec((ln(lvl, 16, 6, 12, 20, 40, 60) << 3) * (100 + ((blvl(15)) * 10)) / 100, 0)) * (dec(ln(lvl, 75, 10), 0)) / 256, 0)) + "\n结束于 " + dec(dec(ln(lvl, 75, 10), 0) / 25, 1) + " 秒";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 14, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]瘟疫标枪 由以下技能得到额外加成:[-]\n毒枪: +10% 毒素伤害每一技能等级\n";

                break;
            case 26:
                desc.attr = "攻击目标数 " + dec(min(lvl + 4, 10), 0) + "\n";
                desc.attr += "伤害: " + sign(dec((lvl * 5), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 11, 0) << 8, 1 << 8) / 256, 1) + "\n3/4 武器伤害\n";

                break;
            case 27:
                desc.attr = "命中率: " + sign(dec(ln(lvl, 30, 9), 0)) + "%\n";
                desc.attr += "爆炸伤害: " + (dec(dec((ln(lvl, 10, 10, 20, 30, 32, 34) << 8) * (100 + ((blvl(16)) * 10)) / 100, 0) / 256, 0)) + '-' + (dec(dec((ln(lvl, 20, 10, 20, 30, 32, 34) << 8) * (100 + ((blvl(16)) * 10)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "火焰持续时间: " + dec((ln(lvl, 75, 0)) / 25, 1) + " 秒\n";
                desc.attr += "平均火焰伤害: " + dec(dec((ln(lvl, 7, 5) << 2) * (100 + (blvl(7) * 5)) / 100, 0) * 75 / 256, 0) + '-' + dec(dec((ln(lvl, 9, 5) << 2) * (100 + (blvl(7) * 5)) / 100, 0) * 25 / 256, 0) * 3 + " 每秒\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 12, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]祭奠之箭 由以下技能得到额外加成:[-]\n火焰箭: +5% 平均火焰伤害每秒 每一技能等级\n爆裂箭: +10% 火焰伤害每一技能等级\n";

                break;
            case 28:
                desc.attr = "生命: " + sign(dec(lvl * 10, 0)) + "%\n";
                desc.attr += "持续时间: " + dec((ln(lvl, 250, 125)) / 25, 1) + " 秒";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 76, -3) << 6, 1 << 8) / 256, 1) + "\n";

                break;
            case 29:
                desc.attr = "" + dec(dm(lvl, 10, 65), 0) + "% 概率";

                break;
            case 30:
                desc.attr = "攻击力: " + sign(dec(ln(lvl, 40, 10), 0)) + "%\n";
                desc.attr += "伤害: " + sign(dec(ln(lvl, 70, 10), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 5, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 31:
                desc.attr = "命中率: " + sign(dec(ln(lvl, 40, 9), 0)) + "%";
                desc.attr += "冰冷伤害: " + (dec(dec((ln(lvl, 40, 10, 15, 20, 22, 24) << 8) * (100 + ((blvl(11)) * 12)) / 100, 0) / 256, 0)) + '-' + (dec(dec((ln(lvl, 50, 10, 15, 20, 22, 24) << 8) * (100 + ((blvl(11)) * 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "冰冻时间 " + dec((dec(ln(lvl, 50, 0) * (100 + ((blvl(21)) * 5)) / 100, 0)) / 25, 1) + " 秒";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 18, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.attr += "半径: " + dec(5 * 2 / 3, 1) + " 码";
                desc.extr += "[22EE00]冻结之箭 由以下技能得到额外加成:[-]\n冰箭: +12% 冰冷伤害每一技能等级\n急冻箭: +5% 冰冻时间每一技能等级\n";

                break;
            case 32:
                desc.attr = "生命: " + dec(400 * (100 + (20 * (lvl - 1) + blvl(28) * 20)) / 100 + (0), 0) + " - " + dec(480 * (100 + (20 * (lvl - 1) + blvl(28) * 20)) / 100 + (0), 0) + "\n";
                desc.attr += "伤害: " + sign(dec(25 * (lvl - 1), 0)) + "%\n";
                desc.attr += "命中率: +" + (40 * lvl + 40 * blvl(23)) + "\n";
                desc.attr += "防御: " + sign(dec((lvl - 1) * 10, 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 25, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]女武神 由以下技能得到额外加成:[-]\n诱饵: +20% 生命每一技能等级\n";
                desc.extr += "刺入: +" + dec(40, 0) + " 攻击命中率每一技能等级\n双倍打击\n闪避\n躲避\n逃避\n";

                break;
            case 33:
                desc.attr = "" + dec(dm(lvl, 10, 100), 0) + "% 概率\n";

                break;
            case 34:
                desc.attr = "" + dec(ln(lvl, 2, 1), 0) + " 次\n";
                desc.attr += "闪电伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(24) + blvl(20) + blvl(14) + blvl(35)) * 8)) / 100, 0) / 256, 0)) + '-' + (dec(dec((ln(lvl, 25, 10, 15, 20, 25, 30) << 8) * (100 + ((blvl(24) + blvl(20) + blvl(14) + blvl(35)) * 8)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 9, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]闪电攻击 由以下技能得到额外加成:[-]\n威力一击: +8% 闪电伤害每一技能等级\n闪电球: +8% 闪电伤害每一技能等级\n充能一击: +8% 闪电伤害每一技能等级\n闪电之怒: +8% 闪电伤害每一技能等级\n";

                break;
            case 35:
                desc.attr = "释放 " + dec(ln(lvl, 2, 1), 0) + " 闪电球";
                desc.attr += "闪电伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(24) + blvl(20) + blvl(14) + blvl(34)) * 1)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 40, 20, 30, 40, 50) << 8) * (100 + ((blvl(24) + blvl(20) + blvl(14) + blvl(34)) * 1)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 20, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]闪电之怒 由以下技能得到额外加成:[-]\n威力一击: +1% 闪电伤害每一技能等级\n闪电球: +1% 闪电伤害每一技能等级\n充能一击: +1% 闪电伤害每一技能等级\n闪电攻击: +1% 闪电伤害每一技能等级\n";

                break;
        }
    }
}

public class SkillAss : SkillInfo
{
    public override void Init()
    {
        base.Init();
        //tabs[0].sID = new byte[10] { 251,256,257,261,262,266,271,272,276,277 };
        Tabs[0].name = "陷阱\n艺术\n技能";
        //tabs[1].sID = new byte[10] { 252,253,258,263,264,267,268,273,278,279 };
        Tabs[1].name = "影子\n训练\n技能";
        //tabs[2].sID = new byte[10] { 254,255,259,260,265,269,270,274,275,280 };
        Tabs[2].name = "武学\n艺术\n技能";

        InitTab1();
        InitTab2();
        InitTab3();
    }

    private void InitTab1()
    {
        Dat d00 = new Dat();
        d00.id = 251;
        d00.name = "火焰爆震";
        d00.desc = "投掷火焰炸弹\n把你的敌人炸成碎片\n";
        Tabs[0].skills.Add(d00);

        Dat d01 = new Dat();
        d01.id = 256;
        d01.name = "闪电网";
        d01.desc = "掷出一个闪电网\n电击你的敌人\n";
        Tabs[0].skills.Add(d01);

        Dat d02 = new Dat();
        d02.id = 257;
        d02.name = "刃之守护";
        d02.desc = "掷出一个旋转的刀刃\n在你和目标间巡逻\n";
        Tabs[0].skills.Add(d02);

        Dat d03 = new Dat();
        d03.id = 261;
        d03.name = "电能守护";
        d03.desc = "设置一个电能陷阱\n发出闪电弹攻击其附近的敌人\n";
        Tabs[0].skills.Add(d03);

        Dat d04 = new Dat();
        d04.id = 262;
        d04.name = "火焰复生";
        d04.desc = "一个会释放火焰波浪的陷阱\n";
        Tabs[0].skills.Add(d04);

        Dat d05 = new Dat();
        d05.id = 266;
        d05.name = "刃之怒";
        d05.desc = "掷出旋转刀刃\n切碎你的敌人\n";
        Tabs[0].skills.Add(d05);

        Dat d06 = new Dat();
        d06.id = 271;
        d06.name = "雷光守卫";
        d06.desc = "设置一个陷阱\n发出闪电烧焦经过的敌人\n";
        Tabs[0].skills.Add(d06);

        Dat d07 = new Dat();
        d07.id = 272;
        d07.name = "复生狱火";
        d07.desc = "一个喷射火焰烧灼敌人的陷阱\n";
        Tabs[0].skills.Add(d07);

        Dat d08 = new Dat();
        d08.id = 276;
        d08.name = "亡者守卫";
        d08.desc = "这个陷阱被触发后将会释放出\n闪电攻击附近的敌人或是引爆敌人的尸体\n";
        Tabs[0].skills.Add(d08);

        Dat d09 = new Dat();
        d09.id = 277;
        d09.name = "刀刃之盾";
        d09.desc = "释放几个刀刃围绕在刺客身旁\n攻击任何靠得太近的敌人\n";
        Tabs[0].skills.Add(d09);
    }

    private void InitTab2()
    {
        Dat d10 = new Dat();
        d10.id = 252;
        d10.name = "利爪掌握";
        d10.desc = "被动 - 增强你对爪类武器的使用技能\n";
        Tabs[1].skills.Add(d10);

        Dat d11 = new Dat();
        d11.id = 253;
        d11.name = "心灵战锤";
        d11.desc = "用精神力量发出一个精神战追\n打击并震退你的敌人\n";
        Tabs[1].skills.Add(d11);

        Dat d12 = new Dat();
        d12.id = 258;
        d12.name = "速度爆发";
        d12.desc = "在一段时间内增加攻击和移动速度\n";
        Tabs[1].skills.Add(d12);

        Dat d13 = new Dat();
        d13.id = 263;
        d13.name = "武器格挡";
        d13.desc = "被动 - 当装备双爪时增加武器格挡概率\n";
        Tabs[1].skills.Add(d13);

        Dat d14 = new Dat();
        d14.id = 264;
        d14.name = "魔影斗篷";
        d14.desc = "在一小段时间内使敌人瞎掉\n并降低敌人的防御力\n";
        Tabs[1].skills.Add(d14);

        Dat d15 = new Dat();
        d15.id = 267;
        d15.name = "消退";
        d15.desc = "在一段时间提高元素抗性并减少诅咒时间\n";
        Tabs[1].skills.Add(d15);

        Dat d16 = new Dat();
        d16.id = 268;
        d16.name = "影子战士";
        d16.desc = "召唤一个自己的影子\n她能够使用你正在使用的两种技能\n";
        Tabs[1].skills.Add(d16);

        Dat d17 = new Dat();
        d17.id = 273;
        d17.name = "心灵爆震";
        d17.desc = "使用精神力量震晕一小队敌人\n并使意志薄弱者倒戈攻击\n";
        Tabs[1].skills.Add(d17);

        Dat d18 = new Dat();
        d18.id = 278;
        d18.name = "毒牙";
        d18.desc = "在武器上增加毒素伤害\n";
        Tabs[1].skills.Add(d18);

        Dat d19 = new Dat();
        d19.id = 279;
        d19.name = "影子大师";
        d19.desc = "召唤一个强大的影子在你身边战斗\n";
        Tabs[1].skills.Add(d19);
    }

    private void InitTab3()
    {
        Dat d20 = new Dat();
        d20.id = 254;
        d20.name = "虎击";
        d20.desc = "聚气性技能技能\n\n连击可累加最后一击所造成的伤害\n必须用龙系最后一击或普通攻击来释放\n";
        Tabs[2].skills.Add(d20);

        Dat d21 = new Dat();
        d21.id = 255;
        d21.name = "龙爪";
        d21.desc = "最后一击\n\n把敌人踢开\n可以加上聚气性技能加成的伤害\n";
        Tabs[2].skills.Add(d21);

        Dat d22 = new Dat();
        d22.id = 259;
        d22.name = "焰拳";
        d22.desc = "聚气性技能技能\n\n连击可累加最后一击火焰伤害的程度\n只能在装配爪类武器时使用\n必须用龙系最后一击或普通攻击来释放\n";
        Tabs[2].skills.Add(d22);

        Dat d23 = new Dat();
        d23.id = 260;
        d23.name = "双龙爪";
        d23.desc = "最后一击\n\n使用双爪将敌人撕成碎片\n可以在双爪上加上聚气性技能加成的伤害\n";
        Tabs[2].skills.Add(d23);

        Dat d24 = new Dat();
        d24.id = 265;
        d24.name = "眼镜蛇攻击击";
        d24.desc = "聚气性技能技能\n\n连击累加在最后一击时的生命和魔法偷取\n必须用龙系最后一击或普通攻击来释放\n";
        Tabs[2].skills.Add(d24);

        Dat d25 = new Dat();
        d25.id = 269;
        d25.name = "闪电爪";
        d25.desc = "聚气性技能\n\n连击可以累加在最后一击上的闪电伤害s\n只能在装配爪类武器时使用\n必须用龙系最后一击或普通攻击来释放\n";
        Tabs[2].skills.Add(d25);

        Dat d26 = new Dat();
        d26.id = 270;
        d26.name = "神龙摆尾";
        d26.desc = "最后一击\n\n爆炸性的踢击并震退敌人\n可以加上聚气性技能加成的伤害\n";
        Tabs[2].skills.Add(d26);

        Dat d27 = new Dat();
        d27.id = 274;
        d27.name = "寒冰刃";
        d27.desc = "聚气性技能技能\n\n连续攻击在最后一击中加入冰冷伤害\n只能在装配爪类武器时使用\n必须用龙系最后一击或普通攻击来释放\n";
        Tabs[2].skills.Add(d27);

        Dat d28 = new Dat();
        d28.id = 275;
        d28.name = "飞龙在天";
        d28.desc = "最后一击\n\n瞬间传送到敌人前释放踢击\n可以加上聚气性技能加成的伤害\n";
        Tabs[2].skills.Add(d28);

        Dat d29 = new Dat();
        d29.id = 280;
        d29.name = "凤翼天翔";
        d29.desc = "聚气性技能技能\n\n在最后一击上加上元素新星伤害\n必须用龙系最后一击或普通攻击来释放\n";
        Tabs[2].skills.Add(d29);
    }

    public override void FillAttribute(int tab, int index)
    {
        Dat desc = Tabs[tab].skills[index];
        FillAttribute(desc);
    }

    public override void FillAttribute(Dat desc)
    {
        int lvl = desc.lvl; // +附加值            
        desc.attr = "";
        desc.extr = "";

        switch (desc.id)
        {
            case 251:
                desc.attr = "火焰伤害: " + (dec(dec((ln(lvl, 6, 3, 8, 20, 38, 58) << 7) * (100 + ((blvl(256) + blvl(276) + blvl(261) + blvl(271) + blvl(262) + blvl(272)) * 9)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 8, 5, 11, 24, 44, 66) << 7) * (100 + ((blvl(256) + blvl(276) + blvl(261) + blvl(271) + blvl(262) + blvl(272)) * 9)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 24, 1) << 5, 1 << 8) / 256, 1) + "\n";
                desc.attr += "半径: " + dec(5 * 2 / 3, 1) + " 码\n";
                desc.extr += "[22EE00]火焰爆震 由以下技能得到额外加成:[-]\n闪电网: +9% 火焰伤害每一技能等级\n电能守护: +9% 火焰伤害每一技能等级\n火焰复生: +9% 火焰伤害每一技能等级\n雷光守卫: +9% 火焰伤害每一技能等级\n复生狱火: +9% 火焰伤害每一技能等级\n亡者守卫: +9% 火焰伤害每一技能等级\n";

                break;
            case 252:
                desc.attr = "伤害: " + sign(dec(ln(lvl, 35, 4), 0)) + "%\n";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 30, 10), 0)) + "%\n";
                desc.attr += "+" + (dm(lvl, 0, 25)) + "% 概率一击必杀\n";

                break;
            case 253:
                desc.attr = "伤害: " + dec((ln(lvl, 2, 2, 3, 4, 5, 6) << 7) / 256, 0) + "-" + dec((ln(lvl, 6, 3, 4, 5, 6, 7) << 7) / 256, 0) + "\n";
                desc.attr += "魔法伤害: " + (dec(dec((ln(lvl, 2, 2, 3, 4, 5, 6) << 7), 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 6, 3, 4, 5, 6, 7) << 7), 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 16, 1) << 6, 1 << 8) / 256, 2) + "\n";

                break;
            case 254:
                desc.attr = "聚气1级 - " + sign(dec(ln(lvl, 100, 20), 0)) + "% 伤害\n";
                desc.attr += "聚气2级 - " + sign(dec(2 * ln(lvl, 100, 20), 0)) + "% 伤害\n";
                desc.attr += "聚气3级 - " + sign(dec(3 * ln(lvl, 100, 20), 0)) + "% 伤害\n";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 15, 7), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 1, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 255:
                desc.attr = "+" + dec(lvl / 6 + 1, 0) + " Kicks\n";
                desc.attr += "Kick 伤害: " + sign(dec(ln(lvl, 5, 7), 0)) + "%\n";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 20, 25), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 6, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 256:
                desc.attr = "电刺数量: " + dec(6 + lvl / 4 + blvl(251) / 3, 0) + "\n";
                desc.attr += "持续时间: " + dec((ln(lvl, 90, 0)) / 25, 1) + " 秒\n";
                desc.attr += "闪电伤害: " + dec(dec(dec((ln(lvl, 2, 0) << 7) * (100 + ((blvl(261) + blvl(271) + blvl(276)) * 11)) / 100, 0) / 256, 0), 0) + "-" + dec(dec(dec((ln(lvl, 20, 6, 12, 20, 30, 42) << 7) * (100 + ((blvl(261) + blvl(271) + blvl(276)) * 11)) / 100, 0) / 256, 0), 0) + " 每秒";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 6, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]闪电网 由以下技能得到额外加成:[-]\n火焰爆震: +1 导弹每3技能等级\n电能守护: +11% 闪电伤害每一技能等级\n雷光守卫: +11% 闪电伤害每一技能等级\n亡者守卫: +11% 闪电伤害每一技能等级\n";

                break;
            case 257:
                desc.attr = "持续时间: " + dec((ln(lvl, 100, 12)) / 25, 1) + " 秒";
                desc.attr += "伤害: " + dec((ln(lvl, 6, 3, 4, 5) << 8) / 256, 0) + "-" + dec((ln(lvl, 10, 3, 4, 5) << 8) / 256, 0) + "\n";
                desc.attr += "3/8 武器伤害\n魔法消耗: " + dec(max(ln(lvl, 7, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 258:
                desc.attr = "攻击速度: " + sign(dec(dm(lvl, 15, 60), 0)) + "%\n";
                desc.attr += "移动速度: " + sign(dec(dm(lvl, 15, 70), 0)) + " %\n";
                desc.attr += "持续时间: " + dec((ln(lvl, 3000, 300)) / 25, 1) + " 秒\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 10, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 259:
                desc.attr = "聚气1级 - 火焰伤害: " + (dec(dec((ln(lvl, 6, 5, 10, 20, 30, 40) << 8) * (100 + ((blvl(280)) * 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 10, 5, 11, 22, 33, 44) << 8) * (100 + ((blvl(280)) * 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "聚气2级 - 火焰伤害半径: " + dec(4 * 2 / 3, 1) + " 码\n";
                desc.attr += "聚气3级 - 火焰伤害: " + dec((dec((ln(lvl, 6, 5, 10, 16, 22, 30) << 3) * (100 + ((blvl(280)) * 6)) / 100, 0) * 50 / 256) / 2, 0) * 2 + "-" + dec((dec((ln(lvl, 10, 5, 10, 17, 24, 32) << 3) * (100 + ((blvl(280)) * 6)) / 100, 0) * 50 / 256) / 2, 0) * 2 + " 每秒\n";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 15, 7), 0)) + "%";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 2, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]焰拳 由以下技能得到额外加成:[-]\n凤翼天翔: +12% 火焰伤害每一技能等级\n";

                break;
            case 260:
                desc.attr = "伤害: " + sign(dec(ln(lvl, 50, 5), 0)) + "%";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 40, 25), 0)) + "%";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 2, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 261:
                desc.attr = "闪电伤害: " + (dec(dec((ln(lvl, 2, 0) << 7) * (100 + ((blvl(251) + blvl(271) + blvl(276)) * 6)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 14, 6, 8, 12, 14, 16) << 7) * (100 + ((blvl(251) + blvl(271) + blvl(276)) * 6)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "射击 " + dec(5 + blvl(271) / 4, 0) + " 次\n";
                desc.attr += "释放 " + dec(ln(lvl, 5, 0) + blvl(256) / 3, 0) + " 闪电弹\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 13, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]电能守护 由以下技能得到额外加成:[-]\n闪电网: +1 闪电弹每3技能等级\n雷光守卫: +1 射击次数每4技能等级\n火焰爆震: +6% 闪电伤害每一技能等级\n雷光守卫: +6% 闪电伤害每一技能等级\n亡者守卫: +6% 闪电伤害每一技能等级\n";

                break;
            case 262:
                desc.attr = "火焰伤害: " + (dec(dec((ln(lvl, 5, 2, 3, 5, 7, 9) << 8) * (100 + ((blvl(251) + blvl(272)) * 8)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 10, 2, 3, 6, 8, 10) << 8) * (100 + ((blvl(251) + blvl(272)) * 8)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 13, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "射击 " + dec(5, 0) + " 次\n";
                desc.extr += "[22EE00]火焰复生 由以下技能得到额外加成:[-]\n火焰爆震: +8% 火焰伤害每一技能等级\n复生狱火: +8% 火焰伤害每一技能等级\n";

                break;
            case 263:
                desc.attr = "" + dec(dm(lvl, 20, 65), 0) + "% 概率\n";

                break;
            case 264:
                desc.attr = "持续时间: " + dec((ln(lvl, 200, 25)) / 25, 1) + " 秒\n";
                desc.attr += "防御提升: " + sign(dec(ln(lvl, 10, 3), 0)) + "%\n";
                desc.attr += "敌人防御: " + sign(dec(-min(ln(lvl, 15, 3), 95), 0)) + "%\n";
                desc.attr += "半径: " + dec(dec(dm(lvl, 30, 30), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 13, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 265:
                desc.attr = "聚气1级 - " + sign(dec(ln(lvl, 40, 5), 0)) + "% 生命偷取\n";
                desc.attr += "聚气2级 - " + sign(dec(ln(lvl, 40, 5), 0)) + "% 生命和魔法偷取\n";
                desc.attr += "聚气3级 - " + sign(dec(2 * ln(lvl, 40, 5), 0)) + "% 生命和魔法偷取\n";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 15, 7), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 2, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 266:
                desc.attr = "伤害: " + dec((ln(lvl, 8, 3, 5, 8) << 8) / 256, 0) + "-" + dec((ln(lvl, 10, 3, 5, 8) << 8) / 256, 0) + "\n";
                desc.attr += "+3/4 武器伤害\n";
                desc.attr += "魔法消耗: " + dec((max(ln(lvl, 8, 1) << 5, 0)) / 256, 1) + " 每刀\n";
                desc.attr += "最少魔法需求: " + dec(3, 0) + "\n";

                break;
            case 267:
                desc.attr = "减少诅咒持续时间 " + dec(dm(lvl, 40, 90), 0) + "%\n";
                desc.attr += "所有抗性: " + dec(dm(lvl, 10, 75), 0) + "%\n";
                desc.attr += "持续时间: " + dec((ln(lvl, 3000, 300)) / 25, 1) + " 秒\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 10, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 268:
                desc.attr = "生命: " + dec(376 * (100 + ((lvl - 1) * 15)) / 100 + (0), 0) + "\n";
                desc.attr += "命中率: +" + lvl * 40 + "\n";
                desc.attr += "防御力加成: " + sign(dec((lvl - 1) * 12, 0)) + "%";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 54, 1) << 7, 1 << 8) / 256, 0) + "\n";

                break;
            case 269:
                desc.attr = "聚气1级 - 闪电伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(280)) * 8)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 80, 20, 40, 60, 80, 100) << 8) * (100 + ((blvl(280)) * 8)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "聚气2级 - 新星伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(280)) * 8)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 30, 15, 25, 35, 45, 65) << 8) * (100 + ((blvl(280)) * 8)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "聚气3级 - 闪电球伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(280)) * 8)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 40, 20, 40, 60, 80, 100) << 8) * (100 + ((blvl(280)) * 8)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 15, 7), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]闪电爪 由以下技能得到额外加成:[-]\n凤翼天翔: +8% 闪电伤害每一技能等级\n";

                break;
            case 270:
                desc.attr = "火焰伤害: " + sign(dec(ln(lvl, 50, 10), 0)) + "%\n";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 20, 15), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 10, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "半径: " + dec(6 * 2 / 3, 1) + " 码";

                break;
            case 271:
                desc.attr = "闪电伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(256) + blvl(261) + blvl(276)) * 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 20, 10, 16, 24, 34, 44) << 8) * (100 + ((blvl(256) + blvl(261) + blvl(276)) * 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 20, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "射击 " + dec(10, 0) + " 次\n";
                desc.extr += "[22EE00]雷光守卫 由以下技能得到额外加成:[-]\n闪电网: +12% 闪电伤害每一技能等级\n电能守护: +12% 闪电伤害每一技能等级\n亡者守卫: +12% 闪电伤害每一技能等级\n";

                break;
            case 272:
                desc.attr = "火焰伤害: " + dec(dec((ln(lvl, 20, 17, 21, 26, 32, 39) << 4) * (100 + ((blvl(251) + blvl(276)) * 10 + blvl(262) * 7)) / 100, 0) * 25 / 768, 0) + "-" + dec(dec((ln(lvl, 50, 19, 23, 28, 34, 41) << 4) * (100 + ((blvl(251) + blvl(276)) * 10 + blvl(262) * 7)) / 100, 0) * 25 / 768, 0) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 20, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "射击 " + dec(10, 0) + " 次";
                desc.extr += "[22EE00]复生狱火 由以下技能得到额外加成:[-]\n火焰复生: +0.5 码每一技能等级\n火焰爆震: +10% 火焰伤害每一技能等级\n火焰复生: +7% 火焰伤害每一技能等级\n亡者守卫: +10% 火焰伤害每一技能等级\n";

                break;
            case 273:
                desc.attr = "伤害: " + dec((ln(lvl, 10, 2, 5, 8) << 8) / 256, 0) + "-" + dec((ln(lvl, 20, 2, 5, 8) << 8) / 256, 0) + "\n";
                desc.attr += "眩晕时间: " + dec((min(250, dec(ln(lvl, 50, 5), 0))) / 25, 1) + " 秒\n";
                desc.attr += "转化概率: " + dec(dm(lvl, 15, 40), 0) + "%\n";
                desc.attr += "持续时间: " + (150 / 25) + "-" + (250 / 25) + " 秒\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 15, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 274:
                desc.attr = "聚气1级 - 冰冷伤害: " + (dec(dec((ln(lvl, 15, 8, 10, 20, 30, 40) << 8) * (100 + ((blvl(280)) * 8)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 35, 8, 10, 22, 32, 42) << 8) * (100 + ((blvl(280)) * 8)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "聚气2级 - 冰冷伤害半径: " + dec(6 * 2 / 3, 1) + " 码\n";
                desc.attr += "聚气3级 - 冰冻持续时间: " + dec((dec(ln(lvl, 100, 10), 0)) / 25, 1) + " 秒\n";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 15, 7), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 3, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]寒冰刃 由以下技能得到额外加成:[-]\n凤翼天翔: +8% 冰冷伤害每一技能等级\n";

                break;
            case 275:
                desc.attr = "踢击伤害: " + sign(dec(ln(lvl, 100, 25), 0)) + "%";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 60, 25), 0)) + "%";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 15, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 276:
                desc.attr = "半径: " + dec((10 + 1 * (lvl - 1)) / 3, 1) + " 码\n";
                desc.attr += "闪电伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(271)) * 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 50, 8, 14, 22, 28, 34) << 8) * (100 + ((blvl(271)) * 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 20, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "尸体爆炸伤害: " + dec(40, 0) + "-" + dec(80, 0) + "% 尸体生命";
                desc.attr += "射击 " + dec(5 + blvl(251) / 3, 0) + " 次\n";
                desc.extr += "[22EE00]亡者守卫 由以下技能得到额外加成:[-]\n火焰爆震: +1 射击每3技能等级\n雷光守卫: +12% 闪电伤害每一技能等级\n";

                break;
            case 277:
                desc.attr = "伤害: " + dec((ln(lvl, 1, 5, 6, 7) << 8) / 256, 0) + "-" + dec((ln(lvl, 30, 5, 6, 7) << 8) / 256, 0) + "\n";
                desc.attr += "+1/4 武器伤害\n";
                desc.attr += "持续时间: " + dec((ln(lvl, 500, 125)) / 25, 1) + " 秒";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 27, 2) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 278:
                desc.attr = "毒素伤害: " + (dec((dec((ln(lvl, 24, 6, 8, 10, 12, 14) << 6), 0)) * (dec(ln(lvl, 10, 0), 0)) / 256, 0)) + "-" + (dec((dec((ln(lvl, 32, 6, 8, 10, 12, 14) << 6), 0)) * (dec(ln(lvl, 10, 0), 0)) / 256, 0)) + "\n结束于 " + dec(dec(ln(lvl, 10, 0), 0) / 25, 1) + " 秒";
                desc.attr += "持续时间: " + dec((ln(lvl, 3000, 100)) / 25, 1) + " 秒\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 12, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 279:
                desc.attr = "生命: " + dec(376 * (100 + ((lvl - 1) * 15)) / 100 + (0), 0) + "\n";
                desc.attr += "命中率: +" + lvl * 40 + "\n";
                desc.attr += "所有抗性: " + sign(dec(((110 * (lvl - 1)) / (lvl + 5) * (80 - 5)) / 100 + 5, 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 70, 1) << 7, 1 << 8) / 256, 0) + "\n";

                break;
            case 280:
                desc.attr = "聚气1级 - 陨石伤害: " + (dec(dec((ln(lvl, 20, 10, 19, 29, 38, 46) << 8) * (100 + ((blvl(259)) * 10)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 40, 10, 21, 33, 42, 50) << 8) * (100 + ((blvl(259)) * 10)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "火焰爆炸伤害: " + dec(dec(dec((ln(lvl, 6, 5, 10, 16, 22, 30) << 3) * (100 + ((blvl(259)) * 6)) / 100, 0) * 75 / 256 / 3, 0) * 3, 0) + "-" + dec(dec(dec((ln(lvl, 10, 5, 10, 17, 24, 32) << 3) * (100 + ((blvl(259)) * 6)) / 100, 0) * 75 / 256 / 3, 0) * 3, 0) + " 每秒\n";
                desc.attr += "聚气2级 - 连锁闪电伤害: " + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(269)) * 13)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 40, 20, 40, 60, 80, 100) << 8) * (100 + ((blvl(269)) * 13)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "聚气3级 - 混沌冰弹伤害: " + (dec(dec((ln(lvl, 16, 4, 8, 12, 20, 28) << 8) * (100 + ((blvl(274)) * 10)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 32, 4, 8, 13, 21, 29) << 8) * (100 + ((blvl(274)) * 10)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "命中率: " + sign(dec(ln(lvl, 15, 7), 0)) + "%\n";
                desc.attr += "魔法消耗: " + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]凤翼天翔 由以下技能得到额外加成:[-]\n焰拳: +10% 火焰伤害每一技能等级\n焰拳: +6% 平均每秒火焰伤害每一技能等级\n闪电爪: +13% 闪电伤害每一技能等级\n寒冰刃: +10% 冰冷伤害每一技能等级\n";

                break;
        }
    }
}

public class SkillBar : SkillInfo
{
    public override void Init()
    {
        base.Init();
        //tabs[0].sID = new byte[10] { 126,132,133,139,140,143,144,147,151,152 };
        Tabs[0].name = "战斗\n技能";
        //tabs[1].sID = new byte[10] { 127,128,129,134,135,136,141,145,148,153 };
        Tabs[1].name = "战斗\n专家\n技能";
        //tabs[2].sID = new byte[10] { 130,131,137,138,142,146,149,150,154,155 };
        Tabs[2].name = "呐喊\n技能";

        InitTab1();
        InitTab2();
        InitTab3();
    }

    private void InitTab1()
    {
        Dat d00 = new Dat();
        d00.id = 126;
        d00.name = "重击";
        d00.desc = "强力打击增加造成的伤害\n并震退敌人\n";
        Tabs[0].skills.Add(d00);

        Dat d01 = new Dat();
        d01.id = 132;
        d01.name = "跳跃";
        d01.desc = "跳跃离开危险或跳入战斗之中\n";
        Tabs[0].skills.Add(d01);

        Dat d02 = new Dat();
        d02.id = 133;
        d02.name = "双手挥击";
        d02.desc = "当装备两把武器时\n可在一次攻击中击中两个敌人\n或者攻击一个敌人两次\n";
        Tabs[0].skills.Add(d02);

        Dat d03 = new Dat();
        d03.id = 139;
        d03.name = "击晕";
        d03.desc = "成功的攻击将使敌人晕眩\n并提高你的命中率\n";
        Tabs[0].skills.Add(d03);

        Dat d04 = new Dat();
        d04.id = 140;
        d04.name = "双手投掷";
        d04.desc = "可同时投掷两件不同的武器\n";
        Tabs[0].skills.Add(d04);

        Dat d05 = new Dat();
        d05.id = 143;
        d05.name = "跳跃攻击";
        d05.desc = "跳向目标并在落地时攻击\n";
        Tabs[0].skills.Add(d05);

        Dat d06 = new Dat();
        d06.id = 144;
        d06.name = "专心";
        d06.desc = "不间断地攻击\n并提高攻击和防御的等级\n";
        Tabs[0].skills.Add(d06);

        Dat d07 = new Dat();
        d07.id = 147;
        d07.name = "狂乱";
        d07.desc = "武器一旦成功击中目标\n可以让挥动武器的速度加倍\n须装备两把武器\n";
        Tabs[0].skills.Add(d07);

        Dat d08 = new Dat();
        d08.id = 151;
        d08.name = "旋风";
        d08.desc = "旋转的死亡之舞\n挥砍所有在旋转路径上的敌人\n";
        Tabs[0].skills.Add(d08);

        Dat d09 = new Dat();
        d09.id = 152;
        d09.name = "狂暴";
        d09.desc = "威力强大但不计后果的攻击\n可以增加伤害和命中率\n但忽视防御\n";
        Tabs[0].skills.Add(d09);
    }

    private void InitTab2()
    {
        Dat d10 = new Dat();
        d10.id = 127;
        d10.name = "剑系掌握";
        d10.desc = "被动 - 增加你使用剑系武器的战斗技能\n";
        Tabs[1].skills.Add(d10);

        Dat d11 = new Dat();
        d11.id = 128;
        d11.name = "斧系掌握";
        d11.desc = "被动 - 增加你使用斧系武器的战斗技能\n";
        Tabs[1].skills.Add(d11);

        Dat d12 = new Dat();
        d12.id = 129;
        d12.name = "锤系掌握";
        d12.desc = "被动 - 增加你使用锤系武器的战斗技能\n";
        Tabs[1].skills.Add(d12);

        Dat d13 = new Dat();
        d13.id = 134;
        d13.name = "长棍掌握";
        d13.desc = "被动 - 增加你用长棍武器的战斗技能\n";
        Tabs[1].skills.Add(d13);

        Dat d14 = new Dat();
        d14.id = 135;
        d14.name = "投掷掌握";
        d14.desc = "被动 - 增加你使用投掷武器的战斗技能\n";
        Tabs[1].skills.Add(d14);

        Dat d15 = new Dat();
        d15.id = 136;
        d15.name = "矛系掌握";
        d15.desc = "被动 - 增加你使用矛系武器的战斗技能\n";
        Tabs[1].skills.Add(d15);

        Dat d16 = new Dat();
        d16.id = 141;
        d16.name = "增加耐力";
        d16.desc = "被动 - 提升你的耐力\n";
        Tabs[1].skills.Add(d16);

        Dat d17 = new Dat();
        d17.id = 145;
        d17.name = "铁布衫";
        d17.desc = "被动 - 增加防御\n";
        Tabs[1].skills.Add(d17);

        Dat d18 = new Dat();
        d18.id = 148;
        d18.name = "加速";
        d18.desc = "被动 - 增加移动速度\n";
        Tabs[1].skills.Add(d18);

        Dat d19 = new Dat();
        d19.id = 153;
        d19.name = "自然抵抗";
        d19.desc = "被动 - 增加对元素伤害的抗性\n";
        Tabs[1].skills.Add(d19);
    }

    private void InitTab3()
    {
        Dat d20 = new Dat();
        d20.id = 130;
        d20.name = "狂嗥";
        d20.desc = "让附近的怪物因恐惧和四散奔逃\n";
        Tabs[2].skills.Add(d20);

        Dat d21 = new Dat();
        d21.id = 131;
        d21.name = "寻找药剂";
        d21.desc = "搜寻怪物的尸体\n看是否能找到药剂\n";
        Tabs[2].skills.Add(d21);

        Dat d22 = new Dat();
        d22.id = 137;
        d22.name = "嘲弄";
        d22.desc = "激怒一个怪物使之与你战斗\n";
        Tabs[2].skills.Add(d22);

        Dat d23 = new Dat();
        d23.id = 138;
        d23.name = "大叫";
        d23.desc = "警告队友迫近的危险\n并提升他们的防御力\n";
        Tabs[2].skills.Add(d23);

        Dat d24 = new Dat();
        d24.id = 142;
        d24.name = "寻找物品";
        d24.desc = "搜寻怪物的尸体\n查找隐藏的物品\n";
        Tabs[2].skills.Add(d24);

        Dat d25 = new Dat();
        d25.id = 146;
        d25.name = "战嗥";
        d25.desc = "发出令人恐惧的叫声\n降低敌人的防御力和伤害力\n";
        Tabs[2].skills.Add(d25);

        Dat d26 = new Dat();
        d26.id = 149;
        d26.name = "战斗体制";
        d26.desc = "增加队友的生命、魔法和耐力值\n";
        Tabs[2].skills.Add(d26);

        Dat d27 = new Dat();
        d27.id = 150;
        d27.name = "残酷吓阻";
        d27.desc = "利用怪物的尸体\n创造一个恐怖的图腾\n吓跑附近的怪物\n";
        Tabs[2].skills.Add(d27);

        Dat d28 = new Dat();
        d28.id = 154;
        d28.name = "战斗狂嗥";
        d28.desc = "伤害附近的敌人并使其晕眩\n";
        Tabs[2].skills.Add(d28);

        Dat d29 = new Dat();
        d29.id = 155;
        d29.name = "战斗指挥";
        d29.desc = "临时增加你和队友的技能1级\n";
        Tabs[2].skills.Add(d29);
    }

    public override void FillAttribute(int tab, int index)
    {
        Dat desc = Tabs[tab].skills[index];
        FillAttribute(desc);
    }

    public override void FillAttribute(Dat desc)
    {
        int lvl = desc.lvl; // +附加值            
        desc.attr = "";

        switch (desc.id)
        {
            case 126:

                desc.attr += "命中率:" + sign(dec((15 + lvl * 5 + blvl(144) * 5), 0)) + "%\n";
                desc.attr += "伤害:" + sign(dec(ln(lvl, 50, 5) + blvl(139) * 5, 0)) + "\n";
                desc.attr += "伤害: " + lvl + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 2, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]重击 由以下技能得到额外加成:[-]\n";
                desc.extr += "击晕: +5% 伤害每一技能等级\n";
                desc.extr += "专心: +5% 攻击命中率每一技能等级\n";

                break;
            case 127:
                desc.attr += "伤害:" + sign(dec(ln(lvl, 28, 5), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 28, 8), 0)) + "\n";
                desc.attr += dm(lvl, 0, 25) + "% 概率一击必杀\n";

                break;
            case 128:
                desc.attr += "伤害:" + sign(dec(ln(lvl, 28, 5), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 28, 8), 0)) + "\n";
                desc.attr += (dm(lvl, 0, 25)) + "% 概率一击必杀\n";

                break;
            case 129:
                desc.attr += "伤害:" + sign(dec(ln(lvl, 28, 5), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 28, 8), 0)) + "\n";
                desc.attr += (dm(lvl, 0, 25)) + "% 概率一击必杀\n";

                break;
            case 130:
                desc.attr += "敌人跑开 " + dec(dec(ln(lvl, 24, 5), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "逃散持续 " + dec((ln(lvl, 75, 25)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;
            case 131:
                desc.attr += dec(dm(lvl, 0, 100), 0) + "% 概率\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 2, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 132:
                desc.attr += "半径:" + dec(dec(dm(lvl, 4, 30), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 2, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;

            case 133:
                desc.attr += "命中率:" + sign(dec(ln(lvl, 15, 5), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 8, -1) << 5, 0) / 128, 0) + "\n";
                desc.attr += "伤害:" + sign(dec(blvl(126) * 10, 0)) + "\n";
                desc.extr += "[22EE00]双手挥击 由以下技能得到额外加成:[-]\n";
                desc.extr += "重击: +10% 伤害每一技能等级\n";

                break;

            case 134:
                desc.attr += "伤害:" + sign(dec(ln(lvl, 28, 5), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 30, 8), 0)) + "\n";
                desc.attr += dm(lvl, 0, 25) + "% 概率一击必杀\n";
                break;

            case 135:
                desc.attr += "伤害:" + sign(dec(ln(lvl, 28, 5), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 30, 8), 0)) + "\n";
                desc.attr += dm(lvl, 0, 25) + "% 概率一击必杀\n";
                break;

            case 136:
                desc.attr += "伤害:" + sign(dec(ln(lvl, 28, 5), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 30, 8), 0)) + "\n";
                desc.attr += dm(lvl, 0, 25) + "% 概率一击必杀\n";
                break;

            case 137:
                desc.attr += "目标伤害:" + dec(ln(lvl, -5, -2), 0) + "%\n";
                desc.attr += "目标命中率:" + dec(ln(lvl, -5, -2), 0) + "%\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 3, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;

            case 138:
                desc.attr += "防御:" + sign(dec(ln(lvl, 100, 10), 0)) + "\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 500, 250) + (blvl(149) + blvl(155)) * 125) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 6, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]大叫 由以下技能得到额外加成:[-]\n";
                desc.extr += "战斗体制: +" + dec(125 / 25, 0) + " 秒 每一技能等级\n";
                desc.extr += "战斗指挥: +" + dec(125 / 25, 0) + " 秒 每一技能等级\n";
                break;

            case 139:

                desc.attr += "命中率:" + dec((10 + lvl * 5 + blvl(144) * 5), 0) + "%\n";
                desc.attr += "持续时间:" + dec((min(250, dec(ln(lvl, 30, 5, 5, 2) * (100 + (blvl(154) * 5)) / 100, 0))) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 2, 0) << 8, 1 << 8) / 256, 1) + "\n";

                desc.attr += "伤害:" + sign(dec(blvl(126) * 8, 0)) + "\n";

                desc.extr += "[22EE00]击晕 由以下技能得到额外加成:[-]\n";
                desc.extr += "重击: +8% 伤害每一技能等级\n";
                desc.extr += "专心: +5% 攻击命中率每一技能等级\n";
                desc.extr += "战斗狂嗥: +5% 持续时间每一技能等级\n";
                break;

            case 140:
                desc.attr += "命中率: +" + ln(lvl, 20, 10) + "%\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 1, 0) << 8, 1 << 8) / 128, 0) + "\n";
                desc.attr += "伤害:" + sign(dec(blvl(133) * 8, 0)) + "\n";
                desc.extr += "[22EE00]双手投掷 由以下技能得到额外加成:[-]\n";
                desc.extr += "双手挥击: +8% 伤害每一技能等级\n";
                break;

            case 141:
                desc.attr += "耐力加成:" + sign(dec(ln(lvl, 30, 15), 0)) + "\n";
                break;

            case 142:
                desc.attr += dec(dm(lvl, 5, 60), 0) + "% 概率\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 7, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;

            case 143:
                desc.attr += "伤害:" + sign(dec(ln(lvl, 100, 30) + blvl(132) * 10, 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 50, 15), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 9, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]跳跃攻击 由以下技能得到额外加成:[-]\n";
                desc.extr += "跳跃: +10% 伤害每一技能等级\n";
                break;
            case 144:

                desc.attr += "防御加成:" + sign(dec(ln(lvl, 100, 10), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 60, 10), 0)) + "\n";
                desc.attr += "伤害:" + sign(dec(ln(lvl, 70, 5) + blvl(126) * 5 + blvl(149) * 10, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 2, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "魔法伤害:" + sign(dec(blvl(152), 0)) + "\n";
                desc.extr += "[22EE00]专心 由以下技能得到额外加成:[-]\n";
                desc.extr += "重击: +5% 伤害每一技能等级\n";
                desc.extr += "战斗体制: +10% 伤害每一技能等级\n";
                desc.extr += "狂暴: +1% 魔法伤害每一技能等级\n";
                break;
            case 145:
                desc.attr += "防御加成:" + "+" + (ln(lvl, 30, 10)) + "%\n";
                break;
            case 146:
                desc.attr += "持续时间:" + dec((ln(lvl, 300, 60)) / 25, 1) + " 秒 \n";
                desc.attr += "防御:" + dec(ln(lvl, -50, -2), 0) + "%\n";
                desc.attr += "伤害:" + dec(ln(lvl, -25, -1), 0) + "%\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 5, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;

            case 147:
                desc.attr += "伤害:" + sign(dec(ln(lvl, 90, 5) + (blvl(133) + blvl(137)) * 8, 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 100, 7), 0)) + "\n";
                desc.attr += "攻击速度: +" + dec(15 * (50 - 0) / 100 + 0, 0) + "-" + dec(dm(lvl, 0, 50), 0) + "%\n";
                desc.attr += "移动速度: +" + dec(15 * (200 - 20) / 100 + 20, 0) + "-" + dec(dm(lvl, 20, 200), 0) + "%\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 3, 0) << 7, 1 << 8) / 128, 0) + "\n";
                desc.attr += "魔法伤害:" + sign(dec(blvl(152), 0)) + "\n";
                desc.attr += "持续时间:" + dec((150) / 25, 1) + " 秒 \n";
                desc.extr += "[22EE00]狂乱 由以下技能得到额外加成:[-]\n";
                desc.extr += "双手挥击: +8% 伤害每一技能等级\n";
                desc.extr += "嘲弄: +8% 伤害每一技能等级\n";
                desc.extr += "狂暴: +1% 魔法伤害每一技能等级\n";
                break;
            case 148:
                desc.attr += "移动速度:" + sign(dec(dm(lvl, 7, 50), 0)) + "\n";
                break;
            case 149:
                desc.attr += "持续时间:" + dec((ln(lvl, 750, 250) + (blvl(138) + blvl(155)) * 125) / 25, 1) + " 秒 \n";
                desc.attr += "最大耐力:" + dec(ln(lvl, 35, 3), 0) + "%\n";
                desc.attr += "最大生命:" + dec(ln(lvl, 35, 3), 0) + "%\n";
                desc.attr += "最大魔法:" + dec(ln(lvl, 35, 3), 0) + "%\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 7, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]战斗体制 由以下技能得到额外加成:[-]\n";
                desc.extr += "大叫: +" + dec(125 / 25, 0) + " 秒 每一技能等级\n";
                desc.extr += "战斗指挥: +" + dec(125 / 25, 0) + " 秒 每一技能等级\n";
                break;
            case 150:
                desc.attr += "持续时间:" + dec((ln(lvl, 1000, 0)) / 25, 1) + " 秒 \n";
                desc.attr += "半径:" + dec(dec(ln(lvl, 3, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 151:
                desc.attr += "伤害:" + sign(dec(ln(lvl, -50, 8), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 0, 5), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 50, 1) << 7, 1 << 8) / 256, 0) + "\n";
                break;
            case 152:
                desc.attr += "命中率:" + sign(dec(ln(lvl, 100, 15), 0)) + "\n";
                desc.attr += "魔法伤害:" + sign(dec(ln(lvl, 150, 15) + (blvl(130) + blvl(138)) * 10, 0)) + "\n";
                desc.attr += "持续时间:" + dec((75 - min(((110 * lvl) / (lvl + 6) * (75 - 25) / 100), (75 - 25))) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]狂暴 由以下技能得到额外加成:[-]\n";
                desc.extr += "狂嗥: +10% 伤害每一技能等级\n";
                desc.extr += "大叫: +10% 伤害每一技能等级\n";
                break;
            case 153:
                desc.attr += "元素抗性:" + sign(dec(dm(lvl, 0, 80), 0)) + "\n";
                break;
            case 154:
                desc.attr += "伤害:" + dec((ln(lvl, 20, 6, 7, 8, 9, 10) << 8) * (100 + (blvl(130) + blvl(137) + blvl(146)) * 6) / 25600, 0) + "-" + dec((ln(lvl, 30, 6, 7, 8, 9, 10) << 8) * (100 + (blvl(130) + blvl(137) + blvl(146)) * 6) / 25600, 0) + "\n";
                desc.attr += "击晕时间:" + dec((min(250, ln(lvl, 25, 5))) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 10, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]战斗狂嗥 由以下技能得到额外加成:[-]\n";
                desc.extr += "狂嗥: +6% 伤害每一技能等级\n";
                desc.extr += "嘲弄: +6% 伤害每一技能等级\n";
                desc.extr += "战嗥: +6% 伤害每一技能等级\n";
                break;
            case 155:
                desc.attr += "持续时间:" + dec((ln(lvl, 125, 250) + (blvl(138) + blvl(149)) * 125) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 11, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]战斗指挥 由以下技能得到额外加成:[-]\n";
                desc.extr += "大叫: +" + dec(125 / 25, 0) + " 秒 每一技能等级\n";
                desc.extr += "战斗体制: +" + dec(125 / 25, 0) + " 秒 每一技能等级\n";
                break;
        }
    }
}

public class SkillDru : SkillInfo
{
    public override void Init()
    {
        base.Init();
        //tabs[0].sID = new byte[10] { 221,222,226,227,231,236,237,241,246,247 };
        Tabs[0].name = "召唤\n技能";
        //tabs[1].sID = new byte[10] { 223,224,228,232,233,238,239,242,243,248 };
        Tabs[1].name = "变身\n技能";
        //tabs[2].sID = new byte[10] { 225,229,230,234,235,240,244,245,249,250 };
        Tabs[2].name = "元素\n技能";

        InitTab1();
        InitTab2();
        InitTab3();
    }

    private void InitTab1()
    {
        Dat d00 = new Dat();
        d00.id = 221;
        d00.name = "乌鸦";
        d00.desc = "召唤乌鸦啄食敌人的眼睛\n";
        Tabs[0].skills.Add(d00);

        Dat d01 = new Dat();
        d01.id = 222;
        d01.name = "毒藤";
        d01.desc = "召唤一条藤蔓\n对接触的敌人传播瘟疫\n";
        Tabs[0].skills.Add(d01);

        Dat d02 = new Dat();
        d02.id = 226;
        d02.name = "橡木智者";
        d02.desc = "召唤灵兽为你和队友增加生命\n";
        Tabs[0].skills.Add(d02);

        Dat d03 = new Dat();
        d03.id = 227;
        d03.name = "召唤灵狼";
        d03.desc = "召唤一只带有灵力的狼\n在你身旁战斗\n";
        Tabs[0].skills.Add(d03);

        Dat d04 = new Dat();
        d04.id = 231;
        d04.name = "食腐藤";
        d04.desc = "召唤一条藤蔓吞食敌人的尸体\n来增加你的生命\n";
        Tabs[0].skills.Add(d04);

        Dat d05 = new Dat();
        d05.id = 236;
        d05.name = "狼獾之心";
        d05.desc = "召唤一只灵兽来增加你和队友的伤害和命中率\n";
        Tabs[0].skills.Add(d05);

        Dat d06 = new Dat();
        d06.id = 237;
        d06.name = "召唤狂狼";
        d06.desc = "召唤一只狂狼吞食尸体来增加伤害\n";
        Tabs[0].skills.Add(d06);

        Dat d07 = new Dat();
        d07.id = 241;
        d07.name = "太阳藤";
        d07.desc = "召唤一条藤蔓吞食尸体来补充你的魔法\n";
        Tabs[0].skills.Add(d07);

        Dat d08 = new Dat();
        d08.id = 246;
        d08.name = "棘灵";
        d08.desc = "召唤灵兽将你和队友受到的伤害反弹回敌人\n";
        Tabs[0].skills.Add(d08);

        Dat d09 = new Dat();
        d09.id = 247;
        d09.name = "召唤灰熊";
        d09.desc = "召唤一只凶猛的大灰熊\n";
        Tabs[0].skills.Add(d09);
    }

    private void InitTab2()
    {
        Dat d10 = new Dat();
        d10.id = 223;
        d10.name = "狼人变化";
        d10.desc = "变形成狼人\n";
        Tabs[1].skills.Add(d10);

        Dat d11 = new Dat();
        d11.id = 224;
        d11.name = "变形术";
        d11.desc = "被动 - 增加变成狼人或灰熊的生命上限和持续时间\n";
        Tabs[1].skills.Add(d11);

        Dat d12 = new Dat();
        d12.id = 228;
        d12.name = "熊人变化";
        d12.desc = "变形成熊人\n";
        Tabs[1].skills.Add(d12);

        Dat d13 = new Dat();
        d13.id = 232;
        d13.name = "野性狂暴";
        d13.desc = "变成狼人后从被攻击的敌人身上偷取生命\n";
        Tabs[1].skills.Add(d13);

        Dat d14 = new Dat();
        d14.id = 233;
        d14.name = "大槌";
        d14.desc = "当变成熊人时不停地槌击敌人\n造成额外的伤害\n";
        Tabs[1].skills.Add(d14);

        Dat d15 = new Dat();
        d15.id = 238;
        d15.name = "狂犬病";
        d15.desc = "当变为狼人时撕咬敌人的同时\n造成疾病伤害并传播到其他怪物\n";
        Tabs[1].skills.Add(d15);

        Dat d16 = new Dat();
        d16.id = 239;
        d16.name = "焰爪";
        d16.desc = "当变成狼人或熊人时\n槌击敌人时并附加焰爪攻击\n";
        Tabs[1].skills.Add(d16);

        Dat d17 = new Dat();
        d17.id = 242;
        d17.name = "饥饿";
        d17.desc = "当变形为狼人或熊人时\n撕咬敌人偷取生命和魔法\n";
        Tabs[1].skills.Add(d17);

        Dat d18 = new Dat();
        d18.id = 243;
        d18.name = "震波";
        d18.desc = "当变成为熊人时\n创造一道震波震晕敌人\n";
        Tabs[1].skills.Add(d18);

        Dat d19 = new Dat();
        d19.id = 248;
        d19.name = "狂怒";
        d19.desc = "当变形为狼人时\n可以攻击多个附近的敌人\n或者攻击多次单个敌人\n";
        Tabs[1].skills.Add(d19);
    }

    private void InitTab3()
    {
        Dat d20 = new Dat();
        d20.id = 225;
        d20.name = "火风暴";
        d20.desc = "释放狂暴混沌灼烧敌人\n";
        Tabs[2].skills.Add(d20);

        Dat d21 = new Dat();
        d21.id = 229;
        d21.name = "熔浆巨岩";
        d21.desc = "放射巨大熔岩击退敌人\n";
        Tabs[2].skills.Add(d21);

        Dat d22 = new Dat();
        d22.id = 230;
        d22.name = "极地风暴";
        d22.desc = "喷射冰霜冻结敌人\n";
        Tabs[2].skills.Add(d22);

        Dat d23 = new Dat();
        d23.id = 234;
        d23.name = "火山爆";
        d23.desc = "在敌人下放开启一道火山出口\n把敌人烧成灰烬\n";
        Tabs[2].skills.Add(d23);

        Dat d24 = new Dat();
        d24.id = 235;
        d24.name = "飓风装甲";
        d24.desc = "保护你免受火、冰、电伤害\n";
        Tabs[2].skills.Add(d24);

        Dat d25 = new Dat();
        d25.id = 240;
        d25.name = "飓风";
        d25.desc = "释放几个小旋风让路经的敌人晕眩并受伤\n";
        Tabs[2].skills.Add(d25);

        Dat d26 = new Dat();
        d26.id = 244;
        d26.name = "火山";
        d26.desc = "召唤一座火山将死亡和毁灭的力量降落到敌人身上\n";
        Tabs[2].skills.Add(d26);

        Dat d27 = new Dat();
        d27.id = 245;
        d27.name = "龙卷风";
        d27.desc = "创造一个龙卷风毁灭敌人\n";
        Tabs[2].skills.Add(d27);

        Dat d28 = new Dat();
        d28.id = 249;
        d28.name = "末日战场";
        d28.desc = "创造一场流星雨毁灭附近的敌人\n";
        Tabs[2].skills.Add(d28);

        Dat d29 = new Dat();
        d29.id = 250;
        d29.name = "暴风";
        d29.desc = "创造一场冰雨狂风撕碎附近的敌人\n";
        Tabs[2].skills.Add(d29);
    }

    public override void FillAttribute(int tab, int index)
    {
        Dat desc = Tabs[tab].skills[index];
        FillAttribute(desc);
    }

    public override void FillAttribute(Dat desc)
    {
        int lvl = desc.lvl; // +附加值            
        desc.attr = "";

        switch (desc.id)
        {
            case 221:
                desc.attr += "伤害:" + dec((ln(lvl, 2, 1) << 8) / 256, 0) + "-" + dec((ln(lvl, 4, 1) << 8) / 256, 0) + "\n";
                desc.attr += "攻击次数:" + dec(ln(lvl, 12, 1), 0) + "\n";
                desc.attr += "数量:" + dec(min(lvl, 5), 0) + "\n";
                desc.attr += "每乌鸦魔法消耗:" + dec(max(ln(lvl, 6, 0) << 8, 1 << 8) / 256, 1) + "\n";

                break;

            case 222:
                desc.attr += "生命:" + dec(42 * (100 + ((lvl - 1) * 25)) / 100 + (0), 0) + "-" + dec(58 * (100 + ((lvl - 1) * 25)) / 100 + (0), 0) + "\n";
                desc.attr += "毒素伤害:" + (dec((dec(ln(lvl, 12, 7, 12, 15, 17, 19), 0)) * (dec(ln(lvl, 100, 0), 0)) / 256, 0)) + "-" + (dec((dec(ln(lvl, 16, 7, 12, 15, 17, 19), 0)) * (dec(ln(lvl, 100, 0), 0)) / 256, 0)) + "\n结束于 " + dec(dec(ln(lvl, 100, 0), 0) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 8, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;

            case 223:
                desc.attr += "命中率:" + sign(dec(ln(lvl, 50, 15), 0)) + "\n";
                desc.attr += "攻击速度:" + sign(dec(dm(lvl, 10, 80), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 15, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "生命:" + sign(dec(25 + ln(blvl(224), 20, 5), 0)) + "\n";
                desc.attr += "耐力加成:" + sign(dec(25, 0)) + "\n";
                desc.attr += "持续时间:" + dec((1000 + ln(blvl(224), 1000, 500)) / 25, 1) + " 秒 \n";
                desc.extr += "[22EE00]狼人变化 由以下技能得到额外加成:[-]\n";
                desc.extr += "变形术\n";
                break;

            case 224:
                desc.attr += "最大生命:" + sign(dec(ln(lvl, 20, 5), 0)) + "\n";
                desc.attr += "持续时间: +" + (ln(lvl, 1000, 500)) / 25 + " 秒 \n";
                break;
            case 225:
                desc.attr += "平均火焰伤害:" + (dec(dec((ln(lvl, 3, 3, 5, 7, 14, 21) << 2) * (100 + ((blvl(229) + blvl(234)) * 23)) / 100, 0) * 75 / 256, 0)) + "-" + (dec(dec((ln(lvl, 6, 3, 6, 8, 15, 23) << 2) * (100 + ((blvl(229) + blvl(234)) * 23)) / 100, 0) * 75 / 256, 0)) + " 每秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]火风暴 由以下技能得到额外加成:[-]\n";
                desc.extr += "熔浆巨岩: +23% 火焰伤害每一技能等级\n";
                desc.extr += "火山爆: +23% 火焰伤害每一技能等级\n";
                break;
            case 226:
                desc.attr += "生命:" + dec(56 * (100 + ((lvl - 1) * 30)) / 100 + (0), 0) + "-" + dec(64 * (100 + ((lvl - 1) * 30)) / 100 + (0), 0) + "\n";
                desc.attr += "生命:" + sign(dec(ln(lvl, 30, 5), 0)) + "\n";
                desc.attr += "半径:" + dec(dec(ln(lvl, 30, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 15, 1) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 227:
                desc.attr += "伤害:" + dec((ln(lvl, 2, 1, 2, 4, 5, 8) << 8) * (100 + ln(blvl(247), 25, 10)) / 25600, 0) + "-" + dec((ln(lvl, 6, 1, 2, 4, 5, 8) << 8) * (100 + ln(blvl(247), 25, 10)) / 25600, 0) + "\n";
                desc.attr += "数量:" + dec(min(lvl, 5), 0) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 50, 25), 0)) + "\n";
                desc.attr += "防御:" + sign(dec(ln(lvl, 50, 10), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 15, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "生命:" + dec(60 * (100 + ((blvl(237) > 0) ? ln(blvl(237), 50, 25) : 0)) / 100 + (0), 0) + "-" + dec(82 * (100 + ((blvl(237) > 0) ? ln(blvl(237), 50, 25) : 0)) / 100 + (0), 0) + "\n";
                desc.extr += "[22EE00]召唤灵狼 由以下技能得到额外加成:[-]\n";
                desc.extr += "召唤狂狼\n";
                desc.extr += "召唤灰熊\n";
                break;
            case 228:
                desc.attr += "伤害:" + sign(dec(ln(lvl, 50, 7), 0)) + "\n";
                desc.attr += "防御:" + sign(dec(ln(lvl, 25, 5), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 15, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "生命:" + sign(dec(50 + ln(blvl(224), 20, 5), 0)) + "\n";
                desc.attr += "持续时间:" + dec((1000 + ln(blvl(224), 1000, 500)) / 25, 1) + " 秒 \n";
                desc.extr += "[22EE00]熊人变化 由以下技能得到额外加成:[-]\n";
                desc.extr += "变形术\n";
                break;
            case 229:
                desc.attr += "伤害:" + dec((ln(lvl, 6, 4, 7, 10, 13, 16) << 8) * (100 + blvl(244) * 10) / 25600, 0) + "-" + dec((ln(lvl, 12, 5, 8, 11, 14, 17) << 8) * (100 + blvl(244) * 10) / 25600, 0) + "\n";
                desc.attr += "火焰伤害:" + (dec(dec((ln(lvl, 6, 4, 7, 10, 13, 16) << 8) * (100 + (blvl(225) * 8)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 12, 5, 8, 11, 14, 17) << 8) * (100 + (blvl(225) * 8)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "平均火焰伤害:" + dec(dec((ln(lvl, 10, 6, 7, 8, 9, 10) << 2) * (100 + (blvl(225) * 8)) / 100, 0) * 75 / 256, 0) + "-" + dec(dec((ln(lvl, 14, 6, 7, 8, 9, 10) << 2) * (100 + (blvl(225) * 8)) / 100, 0) * 25 / 256, 0) * 3 + " 每秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 20, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]熔浆巨岩 由以下技能得到额外加成:[-]\n";
                desc.extr += "火山: +10% 伤害每一技能等级\n";
                desc.extr += "火风暴: +8% 火焰伤害每一技能等级\n";
                break;
            case 230:
                desc.attr += "平均冰冷伤害:" + (dec(dec((ln(lvl, 21, 16, 18, 20, 24, 29) << 2) * (100 + ((blvl(235) + blvl(250)) * 15)) / 100, 0) / 10.24f, 0)) + "-" + (dec(dec((ln(lvl, 40, 16, 19, 21, 25, 31) << 2) * (100 + ((blvl(235) + blvl(250)) * 15)) / 100, 0) / 10.24f, 0)) + " 每秒 \n";
                desc.attr += "冰冻时间:" + dec(dec(ln(lvl, 100, 15), 0) / 25, 1) + " 秒 \n";
                desc.attr += "射程:" + dec(dec(ln(lvl, 35, 2) / 4, 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(dec(max(ln(lvl, 24, 1) << 2, 0) / 20.48f, 0), 0) + " 每秒 \n";
                desc.attr += "最小魔法需求:" + dec(4, 0) + "\n";
                desc.extr += "[22EE00]极地风暴 由以下技能得到额外加成:[-]\n";
                desc.extr += "暴风: +15% 冰冷伤害每一技能等级\n";
                desc.extr += "飓风装甲: +15% 冰冷伤害每一技能等级\n";
                break;

            case 231:
                desc.attr += "生命:" + dec(80 * (100 + ((lvl - 1) * 25)) / 100 + (0), 0) + "-" + dec(110 * (100 + ((lvl - 1) * 25)) / 100 + (0), 0) + "\n";
                desc.attr += "治疗:" + dec(dm(lvl, 3, 12), 0) + "%\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 10, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 232:
                desc.attr += "移动速度: +" + dec(19, 0) + "-" + dec(((110 * (3 + lvl / 2)) * (70 - 10)) / (100 * ((3 + lvl / 2) + 6)) + 10, 0) + "%\n";
                desc.attr += "生命偷取: +" + dec(4, 0) + "-" + dec(4 * lvl + 8, 0) + "%\n";
                desc.attr += "伤害:" + sign(dec(ln(lvl, 50, 5), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 20, 10), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 3, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "持续时间:" + dec((500) / 25, 1) + " 秒 \n";
                break;
            case 233:
                desc.attr += "晕眩时间:" + dec((dm(lvl, 10, 100)) / 25, 1) + " 秒 \n";
                desc.attr += "伤害: +" + dec(20, 0) + "-" + dec(20 * (lvl / 2 + 3), 0) + "%\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 20, 10), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 3, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "持续时间:" + dec((500) / 25, 1) + " 秒 \n";
                break;
            case 234:
                desc.attr += "火焰伤害:" + (dec(dec((ln(lvl, 15, 6, 12, 16, 18, 22) << 8) * (100 + ((blvl(225) + blvl(244)) * 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 25, 6, 12, 16, 19, 23) << 8) * (100 + ((blvl(225) + blvl(244)) * 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 80, 0)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 15, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]火山爆 由以下技能得到额外加成:[-]\n";
                desc.extr += "火风暴: +12% 火焰伤害每一技能等级\n";
                desc.extr += "火山: +12% 火焰伤害每一技能等级\n";
                break;
            case 235:
                desc.attr += "吸收 " + dec((ln(lvl, 40, 12) * (100 + (blvl(240) + blvl(245) + blvl(250)) * 7) / 100), 0) + " 伤害\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 5, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]飓风装甲 由以下技能得到额外加成:[-]\n";
                desc.extr += "飓风: +7% 伤害每一技能等级\n";
                desc.extr += "龙卷风: +7% 伤害每一技能等级\n";
                desc.extr += "暴风: +7% 伤害每一技能等级\n";
                break;
            case 236:
                desc.attr += "生命:" + dec(128 * (100 + ((lvl - 1) * 25)) / 100 + (0), 0) + "-" + dec(144 * (100 + ((lvl - 1) * 25)) / 100 + (0), 0) + "\n";
                desc.attr += "伤害:" + sign(dec(ln(lvl, 20, 7), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 25, 7), 0)) + "\n";
                desc.attr += "半径:" + dec(dec(ln(lvl, 30, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 20, 1) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 237:
                desc.attr += "生命:" + dec(98 * (100 + (ln(lvl, 50, 25))) / 100 + (0), 0) + "-" + dec(130 * (100 + (ln(lvl, 50, 25))) / 100 + (0), 0) + "\n";
                desc.attr += "伤害:" + dec((ln(lvl, 7, 2, 3, 6, 8, 11) << 8) * (100 + ln(blvl(247), 25, 10)) / 25600, 0) + "-" + dec((ln(lvl, 12, 2, 3, 6, 9, 13) << 8) * (100 + ln(blvl(247), 25, 10)) / 25600, 0) + "\n";
                desc.attr += "数量:" + dec(min(lvl, 3), 0) + "\n";
                desc.attr += "生命:" + sign(dec(ln(lvl, 50, 25), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 20, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(blvl(227), 50, 25), 0)) + "\n";
                desc.attr += "防御:" + sign(dec(ln(blvl(227), 50, 10), 0)) + "\n";
                desc.extr += "[22EE00]召唤狂狼 由以下技能得到额外加成:[-]\n";
                desc.extr += "召唤灵狼\n";
                desc.extr += "召唤灰熊\n";
                break;
            case 238:
                desc.attr += "命中率:" + sign(dec(ln(lvl, 50, 7), 0)) + "\n";
                desc.attr += "毒素伤害:" + (dec((dec((ln(lvl, 6, 4, 5, 7, 11, 16) << 3) * (100 + ((blvl(222)) * 18)) / 100, 0)) * (dec(ln(lvl, 100, 10), 0)) / 256, 0)) + "-" + (dec((dec((ln(lvl, 14, 4, 5, 7, 11, 16) << 3) * (100 + ((blvl(222)) * 18)) / 100, 0)) * (dec(ln(lvl, 100, 10), 0)) / 256, 0)) + "\n结束于 " + dec(dec(ln(lvl, 100, 10), 0) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 10, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]狂犬病 由以下技能得到额外加成:[-]\n";
                desc.extr += "毒藤: +18% 毒素伤害每一技能等级\n";
                break;
            case 239:
                desc.attr += "火焰伤害:" + (dec(dec((ln(lvl, 15, 8, 12, 20, 24, 30) << 8) * (100 + ((blvl(225) + blvl(229) + blvl(244) + blvl(234)) * 22)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 20, 8, 12, 22, 26, 34) << 8) * (100 + ((blvl(225) + blvl(229) + blvl(244) + blvl(234)) * 22)) / 100, 0) / 256, 0)) + "\n"; ;
                desc.attr += "命中率:" + sign(dec(ln(lvl, 50, 15), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]焰爪 由以下技能得到额外加成:[-]\n";
                desc.extr += "火风暴: +22% 火焰伤害每一技能等级\n";
                desc.extr += "熔浆巨岩: +22% 火焰伤害每一技能等级\n";
                desc.extr += "火山爆: +22% 火焰伤害每一技能等级\n";
                desc.extr += "火山: +22% 火焰伤害每一技能等级\n";
                break;
            case 240:
                desc.attr += "伤害:" + dec((ln(lvl, 12, 4, 7, 10, 13, 16) << 7) * (100 + (blvl(245) + blvl(250)) * 10) / 25600, 0) + "-" + dec((ln(lvl, 16, 4, 7, 11, 14, 17) << 7) * (100 + (blvl(245) + blvl(250)) * 10) / 25600, 0) + "\n";
                desc.attr += "晕眩时间:" + dec((10) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 7, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]飓风 由以下技能得到额外加成:[-]\n";
                desc.extr += "龙卷风: +10% 伤害每一技能等级\n";
                desc.extr += "暴风: +10% 伤害每一技能等级\n";
                break;
            case 241:
                desc.attr += "生命:" + dec(138 * (100 + ((lvl - 1) * 20)) / 100 + (0), 0) + "-" + dec(192 * (100 + ((lvl - 1) * 20)) / 100 + (0), 0) + "\n";
                desc.attr += "魔法恢复比率:" + dec(dm(lvl, 1, 8), 0) + "%\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 14, 1) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 242:
                desc.attr += "生命偷取:" + dec(dm(lvl, 50, 200), 0) + "%\n";
                desc.attr += "魔法偷取:" + dec(dm(lvl, 50, 200), 0) + "%\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 50, 10), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 3, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "伤害:" + dec(-75, 0) + "%\n";
                break;
            case 243:
                desc.attr += "晕眩时间:" + dec((ln(lvl, 40, 15)) / 25, 1) + " 秒 \n";
                desc.attr += "伤害:" + dec((ln(lvl, 10, 3, 5, 7) << 8) / 256, 0) + "-" + dec((ln(lvl, 20, 3, 5, 7) << 8) / 256, 0) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 7, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 244:
                desc.attr += "伤害:" + dec((ln(lvl, 8, 2, 4, 6, 8, 10) << 8) * (100 + blvl(229) * 12) / 25600, 0) + "-" + dec((ln(lvl, 10, 2, 4, 6, 8, 10) << 8) * (100 + blvl(229) * 12) / 25600, 0) + "\n";
                desc.attr += "火焰伤害:" + (dec(dec((ln(lvl, 8, 2, 4, 6, 8, 11) << 8) * (100 + ((blvl(234) + blvl(249)) * 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 10, 2, 4, 6, 8, 13) << 8) * (100 + ((blvl(234) + blvl(249)) * 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 25, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]火山 由以下技能得到额外加成:[-]\n";
                desc.extr += "熔浆巨岩: +12% 伤害每一技能等级\n";
                desc.extr += "火山爆: +12% 火焰伤害每一技能等级\n";
                desc.extr += "末日战场: +12% 火焰伤害每一技能等级\n";
                break;
            case 245:
                desc.attr += "伤害:" + dec((ln(lvl, 25, 8, 14, 20, 24, 28) << 8) * (100 + (blvl(235) + blvl(240) + blvl(250)) * 9) / 25600, 0) + "-" + dec((ln(lvl, 35, 8, 15, 21, 25, 29) << 8) * (100 + (blvl(235) + blvl(240) + blvl(250)) * 9) / 25600, 0) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 10, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]龙卷风 由以下技能得到额外加成:[-]\n";
                desc.extr += "飓风装甲: +9% 伤害每一技能等级\n";
                desc.extr += "飓风: +9% 伤害每一技能等级\n";
                desc.extr += "暴风: +9% 伤害每一技能等级\n";
                break;
            case 246:
                desc.attr += "生命:" + dec(200 * (100 + ((lvl - 1) * 25)) / 100 + (0), 0) + "-" + dec(226 * (100 + ((lvl - 1) * 25)) / 100 + (0), 0) + "\n";
                desc.attr += dec(ln(lvl, 50, 20), 0) + "% 伤害反弹\n";
                desc.attr += "半径:" + dec(dec(ln(lvl, 30, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 25, 1) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 247:
                desc.attr += "伤害:" + dec((ln(lvl, 30, 10, 15, 20, 26, 30) << 8) * (100 + ln(lvl, 25, 10)) / 25600, 0) + "-" + dec((ln(lvl, 60, 10, 15, 20, 26, 30) << 8) * (100 + ln(lvl, 25, 10)) / 25600, 0) + "\n";
                desc.attr += "伤害:" + sign(dec(ln(lvl, 25, 10), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 40, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "生命:" + dec(550 * (100 + (ln(blvl(237), 50, 25))) / 100 + (0), 0) + "-" + dec(750 * (100 + (ln(blvl(237), 50, 25))) / 100 + (0), 0) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(blvl(227), 50, 25), 0)) + "\n";
                desc.attr += "防御:" + sign(dec(ln(blvl(227), 50, 10), 0)) + "\n";
                desc.extr += "[22EE00]召唤灰熊 由以下技能得到额外加成:[-]\n";
                desc.extr += "召唤灵狼\n";
                desc.extr += "召唤狂狼\n";
                break;
            case 248:
                desc.attr += "攻击次数 " + dec(min((2 + lvl - 1), 5), 0) + "\n";
                desc.attr += "攻击力:" + sign(dec(ln(lvl, 50, 7), 0)) + "\n";
                desc.attr += "伤害:" + sign(dec(ln(lvl, 100, 17), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 249:
                desc.attr += "平均火焰伤害:" + dec(dec((ln(lvl, 10, 6, 7, 8, 9, 10) << 2) * (100 + ((blvl(225) + blvl(229) + blvl(244)) * 7)) / 100, 0) * 75 / 256, 0) + "-" + dec(dec((ln(lvl, 14, 6, 7, 8, 9, 10) << 2) * (100 + ((blvl(225) + blvl(229) + blvl(244)) * 7)) / 100, 0) * 25 / 256, 0) * 3 + " 每秒 \n";
                desc.attr += "火焰伤害:" + (dec(dec((ln(lvl, 25, 15, 20, 25, 31, 38) << 8) * (100 + ((blvl(225) + blvl(229) + blvl(244)) * 14)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 75, 16, 22, 27, 34, 40) << 8) * (100 + ((blvl(225) + blvl(229) + blvl(244)) * 14)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 250, 0) + blvl(234) * 50) / 25, 1) + " 秒 \n";
                desc.attr += "半径:" + dec(8 * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 35, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]末日战场 由以下技能得到额外加成:[-]\n";
                desc.extr += "火山爆: +" + dec(2, 0) + " 秒 每一技能等级\n";
                desc.extr += "火风暴: +14% 火焰伤害每一技能等级\n";
                desc.extr += "熔浆巨岩: +14% 火焰伤害每一技能等级\n";
                desc.extr += "火山: +14% 火焰伤害每一技能等级\n";
                break;
            case 250:
                desc.attr += "冰冷伤害:" + (dec(dec((ln(lvl, 25, 7, 10, 12, 14, 16) << 8) * (100 + ((blvl(240) + blvl(245)) * 9)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 50, 7, 10, 12, 14, 16) << 8) * (100 + ((blvl(240) + blvl(245)) * 9)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 250, 0) + blvl(235) * 50) / 25, 1) + " 秒 \n";
                desc.attr += "半径:" + dec(9 * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 30, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]暴风 由以下技能得到额外加成:[-]\n";
                desc.extr += "飓风装甲: +" + dec(2, 0) + " 秒 每一技能等级\n";
                desc.extr += "飓风: +9% 伤害每一技能等级\n";
                desc.extr += "龙卷风: +9% 伤害每一技能等级\n";
                break;
        }
    }
}

public class SkillNec : SkillInfo
{
    public override void Init()
    {
        base.Init();
        //tabs[0].sID = new byte[10] { 66,71,72,76,77,81,82,86,87,91 };
        Tabs[0].name = "诅咒\n技能";
        //tabs[1].sID = new byte[10] { 67,68,73,74,78,83,84,88,92,93 };
        Tabs[1].name = "毒素\n骨系\n咒语";
        //tabs[2].sID = new byte[10] { 69,70,75,79,80,85,89,90,94,95 };
        Tabs[2].name = "召唤\n咒语";

            InitTab1();
            InitTab2();
            InitTab3();
    }

    private void InitTab1()
    {
        Dat d00 = new Dat();
        d00.id = 66;
        d00.name = "伤害加深";
        d00.desc = "诅咒一组敌人\n加深他们受到的物理伤害\n";
        Tabs[0].skills.Add(d00);

        Dat d01 = new Dat();
        d01.id = 71;
        d01.name = "微弱暗视";
        d01.desc = "诅咒一组怪物\n降低它们的视野范围\n";
        Tabs[0].skills.Add(d01);

        Dat d02 = new Dat();
        d02.id = 72;
        d02.name = "削弱";
        d02.desc = "诅咒一组敌人\n减少他们造成的伤害\n";
        Tabs[0].skills.Add(d02);

        Dat d03 = new Dat();
        d03.id = 76;
        d03.name = "攻击反噬";
        d03.desc = "诅咒一组敌人,  \n使他们造成的伤害返还于本身\n";
        Tabs[0].skills.Add(d03);

        Dat d04 = new Dat();
        d04.id = 77;
        d04.name = "恐惧";
        d04.desc = "诅咒一组怪物,\n使它们因恐惧而逃跑\n";
        Tabs[0].skills.Add(d04);

        Dat d05 = new Dat();
        d05.id = 81;
        d05.name = "迷乱";
        d05.desc = "诅咒一个怪物\n使它进行无差别地攻击\n";
        Tabs[0].skills.Add(d05);

        Dat d06 = new Dat();
        d06.id = 82;
        d06.name = "生命偷取";
        d06.desc = "诅咒一组怪物\n使它们被攻击时生命流向攻击者\n";
        Tabs[0].skills.Add(d06);

        Dat d07 = new Dat();
        d07.id = 86;
        d07.name = "吸引";
        d07.desc = "诅咒一个怪物 \n使它成为附近怪物的攻击目标\n此诅咒不能和其它诅咒叠加\n";
        Tabs[0].skills.Add(d07);

        Dat d08 = new Dat();
        d08.id = 87;
        d08.name = "衰老";
        d08.desc = "诅咒一组敌人 \n使他们变慢、变弱并减少它们造成的伤害\n";
        Tabs[0].skills.Add(d08);

        Dat d09 = new Dat();
        d09.id = 91;
        d09.name = "降低抵抗";
        d09.desc = "诅咒一个敌人使他受到更多的元素伤害 \n降低怪物元素抗性\n降低敌对玩家的最大元素抗性\n";
        Tabs[0].skills.Add(d09);
    }

    private void InitTab2()
    {
        Dat d10 = new Dat();
        d10.id = 67;
        d10.name = "牙";
        d10.desc = "召唤一批利齿攻击敌人\n";
        Tabs[1].skills.Add(d10);

        Dat d11 = new Dat();
        d11.id = 68;
        d11.name = "骨盾";
        d11.desc = "创造一个旋转的白骨护盾\n来吸收近战伤害\n";
        Tabs[1].skills.Add(d11);

        Dat d12 = new Dat();
        d12.id = 73;
        d12.name = "淬毒匕首";
        d12.desc = "在匕首攻击中增加毒素伤害\n";
        Tabs[1].skills.Add(d12);

        Dat d13 = new Dat();
        d13.id = 74;
        d13.name = "尸体爆炸";
        d13.desc = "选中一个怪物尸体,\n让它爆炸伤害附近的敌人\n";
        Tabs[1].skills.Add(d13);

        Dat d14 = new Dat();
        d14.id = 78;
        d14.name = "骨墙";
        d14.desc = "用尸体和残骸建起一道屏障\n";
        Tabs[1].skills.Add(d14);

        Dat d15 = new Dat();
        d15.id = 83;
        d15.name = "毒爆";
        d15.desc = "让选中的怪物尸体生成毒素云雾\n";
        Tabs[1].skills.Add(d15);

        Dat d16 = new Dat();
        d16.id = 84;
        d16.name = "骨矛";
        d16.desc = "召唤一枝致命的骨矛戳穿你的敌人\n";
        Tabs[1].skills.Add(d16);

        Dat d17 = new Dat();
        d17.id = 88;
        d17.name = "骨牢";
        d17.desc = "环绕目标周围建起由石化骨骸构成的屏障\n";
        Tabs[1].skills.Add(d17);

        Dat d18 = new Dat();
        d18.id = 92;
        d18.name = "剧毒新星";
        d18.desc = "放射一个剧毒的新星光环\n";
        Tabs[1].skills.Add(d18);

        Dat d19 = new Dat();
        d19.id = 93;
        d19.name = "骨魂";
        d19.desc = "释放一个急于复仇的亡灵\n攻击你指定的目标或自动选择的目标\n";
        Tabs[1].skills.Add(d19);
    }

    private void InitTab3()
    {
        Dat d20 = new Dat();
        d20.id = 69;
        d20.name = "骷髅掌握";
        d20.desc = "被动 - 提升你召唤的骷髅，\n法师以及重生生物的质量\n";
        Tabs[2].skills.Add(d20);

        Dat d21 = new Dat();
        d21.id = 70;
        d21.name = "骷髅复生";
        d21.desc = "利用怪物尸体，复活一个骷髅战士为你战斗\n";
        Tabs[2].skills.Add(d21);

        Dat d22 = new Dat();
        d22.id = 75;
        d22.name = "粘土石魔";
        d22.desc = "从大地召唤一个石魔\n在你身边作战\n";
        Tabs[2].skills.Add(d22);

        Dat d23 = new Dat();
        d23.id = 79;
        d23.name = "石魔掌握";
        d23.desc = "提升所有石魔的速度和生命\n";
        Tabs[2].skills.Add(d23);

        Dat d24 = new Dat();
        d24.id = 80;
        d24.name = "骷髅法师";
        d24.desc = "利用怪物尸体\n召唤一个骷髅法师为你战斗\n";
        Tabs[2].skills.Add(d24);

        Dat d25 = new Dat();
        d25.id = 85;
        d25.name = "鲜血石魔";
        d25.desc = "创造一个和你共享生命的石魔\n承受它的偷取和伤害\n";
        Tabs[2].skills.Add(d25);

        Dat d26 = new Dat();
        d26.id = 89;
        d26.name = "召唤抵抗";
        d26.desc = "被动 - 增加所有召唤的怪物的元素抗性\n";
        Tabs[2].skills.Add(d26);

        Dat d27 = new Dat();
        d27.id = 90;
        d27.name = "钢铁石魔";
        d27.desc = "从金属武器中召唤石魔\n石魔会获得物品的属性\n";
        Tabs[2].skills.Add(d27);

        Dat d28 = new Dat();
        d28.id = 94;
        d28.name = "火焰石魔";
        d28.desc = "创造一个火焰石魔\n利用火焰伤害来治疗自身\n";
        Tabs[2].skills.Add(d28);

        Dat d29 = new Dat();
        d29.id = 95;
        d29.name = "重生";
        d29.desc = "把一个怪物复活\n让它为你而战\n";
        Tabs[2].skills.Add(d29);
    }

    public override void FillAttribute(int tab, int index)
    {
        Dat desc = Tabs[tab].skills[index];
        FillAttribute(desc);
    }

    public override void FillAttribute(Dat desc)
    {
        int lvl = desc.lvl; // +附加值  
        desc.attr = "";

        switch (desc.id)
        {
            case 66:
                desc.attr += "半径:" + dec(dec(ln(lvl, 3, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 200, 75)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "伤害增加:" + sign(dec(100, 0)) + "\n";

                break;
            case 67:
                desc.attr += "牙齿数量:" + dec(min(ln(lvl, 2, 1), 24), 0) + "\n";
                desc.attr += "魔法伤害:" + (dec(dec((ln(lvl, 4, 2, 2, 3, 4, 5) << 7) * (100 + ((blvl(78) + blvl(88) + blvl(84) + blvl(93)) * 15)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 8, 2, 3, 4, 5, 6) << 7) * (100 + ((blvl(78) + blvl(88) + blvl(84) + blvl(93)) * 15)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 6, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]牙 由以下技能得到额外加成:[-]\n";
                desc.extr += "骨墙: +15% 魔法伤害每一技能等级\n";
                desc.extr += "骨矛: +15% 魔法伤害每一技能等级\n";
                desc.extr += "骨牢: +15% 魔法伤害每一技能等级\n";
                desc.extr += "骨魂: +15% 魔法伤害每一技能等级\n";
                break;
            case 68:
                desc.attr += "吸收 " + dec((ln(lvl, 20, 10) + (blvl(78) + blvl(88)) * 15), 0) + " 伤害\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 11, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]骨盾 由以下技能得到额外加成:[-]\n";
                desc.extr += "骨墙: +" + dec(15, 0) + " 伤害吸收每一技能等级\n";
                desc.extr += "骨牢: +" + dec(15, 0) + " 伤害吸收每一技能等级\n";
                break;
            case 69:
                desc.attr += "骷髅生命: +" + (lvl * 8) + "\n";
                desc.attr += "骷髅伤害: +" + (lvl * 2) + "\n";
                desc.attr += "骷髅法师生命: +" + (lvl * 8) + "\n";
                desc.attr += "骷髅法师投射伤害\n";
                desc.attr += "怪物生命: +" + (lvl * 5) + "%\n";
                desc.attr += "怪物伤害: +" + (lvl * 10) + "%\n";
                break;
            case 70:
                desc.attr += "伤害:" + sign(dec(((lvl < 4) ? 0 : ((lvl - 3) * 7)), 0)) + "\n";
                desc.attr += "命中率:" + dec(5 + (lvl + blvl(69)) * 15, 0) + "\n";
                desc.attr += "防御:" + dec(5 + (lvl + blvl(69)) * 15, 0) + "\n";
                desc.attr += "生命:" + dec(21 * (100 + ((lvl < 4) ? 0 : (50 * (lvl - 3)))) / 100 + (blvl(69) * 8), 0) + "\n生命:" + dec(30 * (100 + ((lvl < 4) ? 0 : (50 * (lvl - 3)))) / 100 + (blvl(69) * 8), 0) + " (噩梦)\n生命:" + dec(42 * (100 + ((lvl < 4) ? 0 : (50 * (lvl - 3)))) / 100 + (blvl(69) * 8), 0) + " (地狱)\n";
                desc.attr += "骷髅总数:" + dec((lvl < 4) ? lvl : (2 + lvl / 3), 0) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 6, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "伤害:" + dec((1 + blvl(69) * 2 + dec(dec((ln(lvl, 0, 0, 1, 2, 3, 4) << 8), 0) / 256, 0)) * (100 + ((lvl < 4) ? 0 : ((lvl - 3) * 7))) / 100, 0) + "-" + dec((2 + blvl(69) * 2 + dec(dec((ln(lvl, 0, 0, 1, 2, 3, 4) << 8), 0) / 256, 0)) * (100 + ((lvl < 4) ? 0 : ((lvl - 3) * 7))) / 100, 0) + "\n";
                desc.extr += "[22EE00]骷髅复生 由以下技能得到额外加成:[-]\n";
                desc.extr += "骷髅掌握\n";
                desc.extr += "召唤抵抗\n";
                break;
            case 71:
                desc.attr += "半径:" + dec(dec(ln(lvl, 4, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 175, 50)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 9, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 72:
                desc.attr += "半径:" + dec(dec(ln(lvl, 9, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 350, 60)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "目标伤害:" + dec(-33, 0) + "%\n";
                break;
            case 73:
                desc.attr += "毒素伤害:" + (dec((dec((ln(lvl, 18, 10, 15, 20, 23, 26) << 1) * (100 + ((blvl(83) + blvl(92)) * 20)) / 100, 0)) * (dec(ln(lvl, 50, 10), 0)) / 256, 0)) + "-" + (dec((dec((ln(lvl, 40, 10, 15, 20, 23, 26) << 1) * (100 + ((blvl(83) + blvl(92)) * 20)) / 100, 0)) * (dec(ln(lvl, 50, 10), 0)) / 256, 0)) + "\n结束于 " + dec(dec(ln(lvl, 50, 10), 0) / 25, 1) + " 秒 \n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 30, 20), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 12, 1) << 6, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]淬毒匕首 由以下技能得到额外加成:[-]\n";
                desc.extr += "毒爆: +20% 毒素伤害每一技能等级\n";
                desc.extr += "剧毒新星: +20% 毒素伤害每一技能等级\n";
                break;
            case 74:
                desc.attr += "半径:" + dec((ln(lvl, 8, 1)) / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 15, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "伤害:" + dec(60, 0) + "-" + dec(100, 0) + "% 尸体生命\n";
                break;
            case 75:
                desc.attr += "生命:" + dec(100 * (100 + ((100 + (35 * (lvl - 1))) * (100 + (blvl(79) * 20) + (blvl(85) * 5)) / 100 - 100)) / 100 + (0), 0) + "\n生命:" + dec(175 * (100 + ((100 + (35 * (lvl - 1))) * (100 + (blvl(79) * 20) + (blvl(85) * 5)) / 100 - 100)) / 100 + (0), 0) + " (噩梦)\n生命:" + dec(275 * (100 + ((100 + (35 * (lvl - 1))) * (100 + (blvl(79) * 20) + (blvl(85) * 5)) / 100 - 100)) / 100 + (0), 0) + " (地狱)\n";
                desc.attr += "伤害:" + dec((2) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + "-" + dec((5) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + "\n伤害:" + dec((2) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + "-" + dec((6) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + " (噩梦)\n伤害:" + dec((3) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + "-" + dec((7) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + " (地狱)\n";
                desc.attr += "攻击准确率: +" + lvl * 20 + "\n";
                desc.attr += "减慢敌人:" + dec(dm(lvl, 0, 75), 0) + "%\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 15, 3) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "命中率:" + dec(40 + (blvl(79) * 25) + (lvl * 20), 0) + "\n";
                desc.attr += "防御:" + dec(100 + blvl(90) * 35, 0) + "\n";
                desc.extr += "[22EE00]粘土石魔 由以下技能得到额外加成:[-]\n";
                desc.extr += "石魔掌握\n";
                desc.extr += "召唤抵抗\n";
                desc.extr += "鲜血石魔: +5% 生命每一技能等级\n";
                desc.extr += "钢铁石魔: +" + dec(35, 0) + " 防御每一技能等级\n";
                desc.extr += "火焰石魔: +6% 伤害每一技能等级\n";
                break;
            case 76:
                desc.attr += dec(ln(lvl, 200, 25), 0) + "% 伤害反噬\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 300, 60)) / 25, 1) + " 秒 \n";
                desc.attr += "半径:" + dec(dec(ln(lvl, 7, 0), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 5, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 77:
                desc.attr += "持续时间:" + dec((ln(lvl, 200, 25)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 7, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "半径:" + dec(dec(ln(lvl, 4, 0), 0) * 2 / 3, 1) + " 码\n";
                break;
            case 78:
                desc.attr += "生命:" + dec(19 * (100 + ((25 * (lvl - 1)) + ((blvl(68) + blvl(88)) * 10))) / 100 + (0), 0) + "\n生命:" + dec(147 * (100 + ((25 * (lvl - 1)) + ((blvl(68) + blvl(88)) * 10))) / 100 + (0), 0) + " (噩梦)\n生命:" + dec(431 * (100 + ((25 * (lvl - 1)) + ((blvl(68) + blvl(88)) * 10))) / 100 + (0), 0) + " (地狱)\n";
                desc.attr += "持续时间:" + dec((600) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 17, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]骨墙 由以下技能得到额外加成:[-]\n";
                desc.extr += "骨盾: +10% 生命每一技能等级\n";
                desc.extr += "骨牢: +10% 生命每一技能等级\n";
                break;
            case 79:
                desc.attr += "生命:" + sign(dec(lvl * 20, 0)) + "\n";
                desc.attr += "攻击准确率: " + (lvl * 25) + "\n";
                desc.attr += "移动速度:" + sign(dec(dm(lvl, 0, 40), 0)) + "\n";
                break;
            case 80:
                desc.attr += "生命:" + dec(61 * (100 + (0)) / 100 + (blvl(69) * 8), 0) + "\n生命:" + dec(88 * (100 + (0)) / 100 + (blvl(69) * 8), 0) + " (噩梦)\n生命:" + dec(123 * (100 + (0)) / 100 + (blvl(69) * 8), 0) + " (地狱)\n";
                desc.attr += "防御:" + dec((lvl + blvl(69)) * 10, 0) + "\n";
                desc.attr += "骷髅法师数量 " + dec((lvl < 4) ? lvl : (2 + lvl / 3), 0) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 8, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]骷髅法师 由以下技能得到额外加成:[-]\n";
                desc.extr += "骷髅掌握\n";
                desc.extr += "召唤抵抗\n";
                break;
            case 81:
                desc.attr += "半径:" + dec(dec(ln(lvl, 6, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 250, 50)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 13, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 82:
                desc.attr += "半径:" + dec(dec(ln(lvl, 4, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 400, 60)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 9, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "偷取生命:" + dec(ln(lvl, 50, 0), 0) + "% 攻击伤害\n";
                break;
            case 83:
                desc.attr += "毒素伤害:" + (dec((dec((ln(lvl, 8, 2, 4, 6, 8, 10) << 4) * (100 + ((blvl(73) + blvl(92)) * 15)) / 100, 0)) * (dec(ln(lvl, 50, 10), 0)) / 256, 0)) + "-" + (dec((dec((ln(lvl, 24, 2, 4, 6, 8, 10) << 4) * (100 + ((blvl(73) + blvl(92)) * 15)) / 100, 0)) * (dec(ln(lvl, 50, 10), 0)) / 256, 0)) + "\n结束于 " + dec(dec(ln(lvl, 50, 10), 0) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 8, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]毒爆 由以下技能得到额外加成:[-]\n";
                desc.extr += "淬毒匕首: +15% 毒素伤害每一技能等级\n";
                desc.extr += "剧毒新星: +15% 毒素伤害每一技能等级\n";
                break;

            case 84:
                desc.attr += "魔法伤害:" + (dec(dec((ln(lvl, 16, 8, 9, 12, 18, 24) << 8) * (100 + ((blvl(78) + blvl(88) + blvl(67) + blvl(93)) * 7)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 24, 8, 9, 13, 19, 25) << 8) * (100 + ((blvl(78) + blvl(88) + blvl(67) + blvl(93)) * 7)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 28, 1) << 6, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]骨矛 由以下技能得到额外加成:[-]\n";
                desc.extr += "牙: +7% 魔法伤害每一技能等级\n";
                desc.extr += "骨墙: +7% 魔法伤害每一技能等级\n";
                desc.extr += "骨牢: +7% 魔法伤害每一技能等级\n";
                desc.extr += "骨魂: +7% 魔法伤害每一技能等级\n";
                break;
            case 85:
                desc.attr += "将 " + dec(dm(lvl, 75, 150), 0) + "% 伤害转化为生命\n";
                desc.attr += "伤害:" + dec((6) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + "-" + dec((16) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + "\n伤害:" + dec((9) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + "-" + dec((23) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + " (噩梦)\n伤害:" + dec((10) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + "-" + dec((27) * (100 + 35 * (lvl - 1) + (blvl(94) * 6)) / 100, 0) + " (地狱)\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 25, 4) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "生命:" + dec(201 * (100 + ((blvl(79) * 20))) / 100 + (0), 0) + "\n生命:" + dec(388 * (100 + ((blvl(79) * 20))) / 100 + (0), 0) + " (噩梦)\n生命:" + dec(637 * (100 + ((blvl(79) * 20))) / 100 + (0), 0) + " (地狱)\n";
                desc.attr += "命中率:" + dec(60 + (blvl(79) * 25) + blvl(75) * 20, 0) + "\n";
                desc.attr += "防御:" + dec(80 + blvl(90) * 35, 0) + "\n";
                desc.extr += "[22EE00]鲜血石魔 由以下技能得到额外加成:[-]\n";
                desc.extr += "石魔掌握\n";
                desc.extr += "召唤抵抗\n";
                desc.extr += "粘土石魔: +" + dec(20, 0) + " 攻击命中率每一技能等级\n";
                desc.extr += "钢铁石魔: +" + dec(35, 0) + " 防御每一技能等级\n";
                desc.extr += "火焰石魔: +6% 伤害每一技能等级\n";
                break;
            case 86:
                desc.attr += "持续时间:" + dec((ln(lvl, 300, 90)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 17, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "半径:" + dec(dec(ln(lvl, 9, 0), 0) * 2 / 3, 1) + " 码\n";
                break;
            case 87:
                desc.attr += "持续时间:" + dec((ln(lvl, 100, 15)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 11, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "半径:" + dec(dec(ln(lvl, 6, 0), 0) * 2 / 3, 1) + " 码\n";
                break;
            case 88:
                desc.attr += "生命:" + dec(19 * (100 + ((25 * (lvl - 1)) + ((blvl(68) + blvl(78)) * 8))) / 100 + (0), 0) + "\n生命:" + dec(147 * (100 + ((25 * (lvl - 1)) + ((blvl(68) + blvl(78)) * 8))) / 100 + (0), 0) + " (噩梦)\n生命:" + dec(431 * (100 + ((25 * (lvl - 1)) + ((blvl(68) + blvl(78)) * 8))) / 100 + (0), 0) + " (地狱)\n";
                desc.attr += "持续时间:" + dec((600) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 27, -1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]骨牢 由以下技能得到额外加成:[-]\n";
                desc.extr += "骨盾: +8% 生命每一技能等级\n";
                desc.extr += "骨墙: +8% 生命每一技能等级\n";
                break;
            case 89:
                desc.attr += "所有抗性:" + sign(dec(dm(lvl, 20, 75), 0)) + "\n";
                break;
            case 90:
                desc.attr += "反刺\n";
                desc.attr += dec(ln(lvl, 150, 15), 0) + "% 伤害反噬\n";
                desc.attr += "防御加成: +" + lvl * 35 + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 35, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "生命:" + dec(306 * (100 + ((blvl(79) * 20) + (blvl(85) * 5))) / 100 + (0), 0) + "\n生命:" + dec(595 * (100 + ((blvl(79) * 20) + (blvl(85) * 5))) / 100 + (0), 0) + " (噩梦)\n生命:" + dec(980 * (100 + ((blvl(79) * 20) + (blvl(85) * 5))) / 100 + (0), 0) + " (地狱)\n";
                desc.attr += "伤害:" + dec((7) * (100 + (blvl(94) * 6)) / 100, 0) + "-" + dec((19) * (100 + (blvl(94) * 6)) / 100, 0) + "\n伤害:" + dec((11) * (100 + (blvl(94) * 6)) / 100, 0) + "-" + dec((30) * (100 + (blvl(94) * 6)) / 100, 0) + " (噩梦)\n伤害:" + dec((12) * (100 + (blvl(94) * 6)) / 100, 0) + "-" + dec((33) * (100 + (blvl(94) * 6)) / 100, 0) + " (地狱)\n";
                desc.attr += "命中率:" + dec(80 + (blvl(79) * 25) + blvl(75) * 20, 0) + "\n";
                desc.attr += "防御:" + dec(140 + lvl * 35, 0) + "\n";
                desc.extr += "[22EE00]钢铁石魔 由以下技能得到额外加成:[-]\n";
                desc.extr += "石魔掌握\n";
                desc.extr += "召唤抵抗\n";
                desc.extr += "粘土石魔: +" + dec(20, 0) + " 攻击命中率每一技能等级\n";
                desc.extr += "鲜血石魔: +5% 生命每一技能等级\n";
                desc.extr += "火焰石魔: +6% 伤害每一技能等级\n";
                break;
            case 91:
                desc.attr += "半径:" + dec(dec(ln(lvl, 7, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 500, 50)) / 25, 1) + " 秒 \n";
                desc.attr += "所有抗性:" + dec(-dm(lvl, 25, 70), 0) + "%\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 22, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 92:
                desc.attr += "毒素伤害:" + (dec((dec((ln(lvl, 14, 4, 5, 8, 12, 14) << 4) * (100 + ((blvl(73) + blvl(83)) * 10)) / 100, 0)) * (dec(ln(lvl, 50, 0), 0)) / 256, 0)) + "-" + (dec((dec((ln(lvl, 25, 4, 5, 8, 12, 14) << 4) * (100 + ((blvl(73) + blvl(83)) * 10)) / 100, 0)) * (dec(ln(lvl, 50, 0), 0)) / 256, 0)) + "\n结束于 " + dec(dec(ln(lvl, 50, 0), 0) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 20, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]剧毒新星 由以下技能得到额外加成:[-]\n";
                desc.extr += "淬毒匕首: +10% 毒素伤害每一技能等级\n";
                desc.extr += "毒爆: +10% 毒素伤害每一技能等级\n";
                break;
            case 93:
                desc.attr += "魔法伤害:" + (dec(dec((ln(lvl, 20, 16, 17, 18, 19, 20) << 8) * (100 + ((blvl(78) + blvl(88) + blvl(67) + blvl(84)) * 6)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 30, 17, 18, 19, 20, 21) << 8) * (100 + ((blvl(78) + blvl(88) + blvl(67) + blvl(84)) * 6)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 24, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]骨魂 由以下技能得到额外加成:[-]\n";
                desc.extr += "牙: +6% 魔法伤害每一技能等级\n";
                desc.extr += "骨墙: +6% 魔法伤害每一技能等级\n";
                desc.extr += "骨矛: +6% 魔法伤害每一技能等级\n";
                desc.extr += "骨牢: +6% 魔法伤害每一技能等级\n";
                break;
            case 94:
                desc.attr += "吸收 " + dec(dm(lvl, 25, 100), 0) + "% 火焰伤害\n";
                desc.attr += "伤害:" + dec((10) * (100 + 0) / 100, 0) + "-" + dec((27) * (100 + 0) / 100, 0) + "\n伤害:" + dec((15) * (100 + 0) / 100, 0) + "-" + dec((39) * (100 + 0) / 100, 0) + " (噩梦)\n伤害:" + dec((18) * (100 + 0) / 100, 0) + "-" + dec((47) * (100 + 0) / 100, 0) + " (地狱)\n" + "火焰伤害:" + (dec(dec((ln(lvl, 10, 9, 10, 11, 12, 13) << 8), 0) / 256, 0) + dec(dec((ln(ln(lvl, 8, 1), 2, 1, 2, 3, 5, 7) << 7) * (100 + (blvl(100) * 18 + blvl(125) * 6)) / 100, 0) / 256, 0) * 6) + "-" + (dec(dec((ln(lvl, 27, 10, 11, 12, 13, 14) << 8), 0) / 256, 0) + dec(dec((ln(ln(lvl, 8, 1), 6, 1, 2, 3, 5, 7) << 7) * (100 + (blvl(100) * 18 + blvl(125) * 6)) / 100, 0) / 256, 0) * 6) + "\n";
                desc.attr += "圣火 " + (dec(dec((ln(ln(lvl, 8, 1), 2, 1, 2, 3, 5, 7) << 7) * (100 + (blvl(100) * 18 + blvl(125) * 6)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(ln(lvl, 8, 1), 6, 1, 2, 3, 5, 7) << 7) * (100 + (blvl(100) * 18 + blvl(125) * 6)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 50, 10) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "生命:" + dec(328 * (100 + ((blvl(79) * 20) + (blvl(85) * 5))) / 100 + (0), 0) + "\n生命:" + dec(643 * (100 + ((blvl(79) * 20) + (blvl(85) * 5))) / 100 + (0), 0) + " (噩梦)\n生命:" + dec(1063 * (100 + ((blvl(79) * 20) + (blvl(85) * 5))) / 100 + (0), 0) + " (地狱)\n";
                desc.attr += "命中率:" + dec(120 + (blvl(79) * 25) + blvl(75) * 20, 0) + "\n";
                desc.attr += "防御:" + dec(200 + blvl(90) * 35, 0) + "\n";
                desc.extr += "[22EE00]火焰石魔 由以下技能得到额外加成:[-]\n";
                desc.extr += "石魔掌握\n";
                desc.extr += "召唤抵抗\n";
                desc.extr += "粘土石魔: +" + dec(20, 0) + " 攻击命中率每一技能等级\n";
                desc.extr += "鲜血石魔: +5% 生命每一技能等级\n";
                desc.extr += "钢铁石魔: +" + dec(35, 0) + " 防御每一技能等级\n";
                break;
            case 95:
                desc.attr += "怪物数:" + dec(lvl, 0) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 45, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 4500, 0)) / 25, 1) + " 秒 \n";
                desc.attr += "生命:" + sign(dec(200 + tlvl(69) * 5, 0)) + "\n";
                desc.attr += "伤害:" + sign(dec(tlvl(69) * 10, 0)) + "\n";
                desc.extr += "[22EE00]重生 由以下技能得到额外加成:[-]\n";
                desc.extr += "骷髅掌握\n";
                desc.extr += "召唤抵抗\n";
                break;
        }
    }
}

public class SkillPal : SkillInfo
{
    public override void Init()
    {
        base.Init();
        //tabs[0].sID = new byte[10] { 96,97,101,106,107,111,112,116,117,121 };
        Tabs[0].name = "格斗\n技能";
        //tabs[1].sID = new byte[10] { 98,102,103,108,113,114,118,119,122,123 };
        Tabs[1].name = "攻击\n灵气";
        //tabs[2].sID = new byte[10] { 99,100,104,105,109,110,115,120,124,125 };
        Tabs[2].name = "防御\n灵气";

        InitTab1();
        InitTab2();
        InitTab3();
    }

    private void InitTab1()
    {
        Dat d00 = new Dat();
        d00.id = 96;
        d00.name = "牺牲";
        d00.desc = "以生命为代价增加伤害和准确性\n";
        Tabs[0].skills.Add(d00);

        Dat d01 = new Dat();
        d01.id = 97;
        d01.name = "重击";
        d01.desc = "用盾牌打击敌人\n使其短暂昏迷\n";
        Tabs[0].skills.Add(d01);

        Dat d02 = new Dat();
        d02.id = 101;
        d02.name = "圣光弹";
        d02.desc = "发射神圣光弹打击不死的敌人\n或治疗你的同盟者\n";
        Tabs[0].skills.Add(d02);

        Dat d03 = new Dat();
        d03.id = 106;
        d03.name = "热诚";
        d03.desc = "可以让一次攻击打击多个敌人\n";
        Tabs[0].skills.Add(d03);

        Dat d04 = new Dat();
        d04.id = 107;
        d04.name = "冲锋";
        d04.desc = "高速冲击一个敌人\n";
        Tabs[0].skills.Add(d04);

        Dat d05 = new Dat();
        d05.id = 111;
        d05.name = "复仇";
        d05.desc = "在每次成功的攻击中加上火、电和冰的元素伤害\n";
        Tabs[0].skills.Add(d05);

        Dat d06 = new Dat();
        d06.id = 112;
        d06.name = "祝福之槌";
        d06.desc = "召唤一个飞舞的神槌\n向外旋转打击敌人\n对不死的敌人造成150%的伤害\n";
        Tabs[0].skills.Add(d06);

        Dat d07 = new Dat();
        d07.id = 116;
        d07.name = "转化";
        d07.desc = "有效的攻击有一定比率转化魔物为你作战\n";
        Tabs[0].skills.Add(d07);

        Dat d08 = new Dat();
        d08.id = 117;
        d08.name = "神圣护盾";
        d08.desc = "用神圣的力量增强你的护盾\n";
        Tabs[0].skills.Add(d08);

        Dat d09 = new Dat();
        d09.id = 121;
        d09.name = "天堂之拳";
        d09.desc = "强力的闪电攻击目标\n并释放圣光弹攻击附近的敌人\n";
        Tabs[0].skills.Add(d09);
    }

    private void InitTab2()
    {
        Dat d10 = new Dat();
        d10.id = 98;
        d10.name = "力量";
        d10.desc = "启用时，增加你和对友的攻击伤害\n";
        Tabs[1].skills.Add(d10);

        Dat d11 = new Dat();
        d11.id = 102;
        d11.name = "圣火";
        d11.desc = "启用时, 周期性地发出火焰伤害周围的敌人\n";
        Tabs[1].skills.Add(d11);

        Dat d12 = new Dat();
        d12.id = 103;
        d12.name = "荆棘";
        d12.desc = "启用时, 灵气将敌人的伤害反弹回去\n";
        Tabs[1].skills.Add(d12);

        Dat d13 = new Dat();
        d13.id = 108;
        d13.name = "祝福瞄准";
        d13.desc = "启用时, 增加你和队友的命中率\n";
        Tabs[1].skills.Add(d13);

        Dat d14 = new Dat();
        d14.id = 113;
        d14.name = "专注";
        d14.desc = "启用时, 增加伤害并降低攻击被打断的概率\n";
        Tabs[1].skills.Add(d14);

        Dat d15 = new Dat();
        d15.id = 114;
        d15.name = "圣洁寒冰";
        d15.desc = "启用时, 增加伤害并降低攻击被打断的概率\n";
        Tabs[1].skills.Add(d15);

        Dat d16 = new Dat();
        d16.id = 118;
        d16.name = "神圣冲击";
        d16.desc = "启用时, 周期性地对半径内的敌人造成电系伤害\n在攻击上增加闪电伤害\n";
        Tabs[1].skills.Add(d16);

        Dat d17 = new Dat();
        d17.id = 119;
        d17.name = "庇护所";
        d17.desc = "启用时, 伤害并击退不死系怪物\n";
        Tabs[1].skills.Add(d17);

        Dat d18 = new Dat();
        d18.id = 122;
        d18.name = "狂热";
        d18.desc = "启用时, 增加你和队友的的攻击速度、伤害和命中率\n";
        Tabs[1].skills.Add(d18);

        Dat d19 = new Dat();
        d19.id = 123;
        d19.name = "审判";
        d19.desc = "启用时, 降低敌人的防御和元素抗性\n";
        Tabs[1].skills.Add(d19);
    }

    private void InitTab3()
    {
        Dat d20 = new Dat();
        d20.id = 99;
        d20.name = "祈祷";
        d20.desc = "启用时, 慢慢地恢复你和队友的生命\n";
        Tabs[2].skills.Add(d20);

        Dat d21 = new Dat();
        d21.id = 100;
        d21.name = "火焰抵抗";
        d21.desc = "启用时, 降低你和队友受到的火焰伤害\n";
        Tabs[2].skills.Add(d21);

        Dat d22 = new Dat();
        d22.id = 104;
        d22.name = "蔑视";
        d22.desc = "启用时, 增加你和队友的防御等级\n";
        Tabs[2].skills.Add(d22);

        Dat d23 = new Dat();
        d23.id = 105;
        d23.name = "冰冷抵抗";
        d23.desc = "启用时, 降低你和队友受到的冰冷伤害\n";
        Tabs[2].skills.Add(d23);

        Dat d24 = new Dat();
        d24.id = 109;
        d24.name = "净化";
        d24.desc = "启用时, 降低你和队友中毒或被诅咒的时间\n";
        Tabs[2].skills.Add(d24);

        Dat d25 = new Dat();
        d25.id = 110;
        d25.name = "闪电抵抗";
        d25.desc = "启用时, 降低你和队友受到的闪电伤害\n";
        Tabs[2].skills.Add(d25);

        Dat d26 = new Dat();
        d26.id = 115;
        d26.name = "活力";
        d26.desc = "启用时, 增加你和队友的移动速度\n并提高耐力恢复速度及最大值\n";
        Tabs[2].skills.Add(d26);

        Dat d27 = new Dat();
        d27.id = 120;
        d27.name = "冥想";
        d27.desc = "启用时, 提高你和队友的魔法恢复速度\n";
        Tabs[2].skills.Add(d27);

        Dat d28 = new Dat();
        d28.id = 124;
        d28.name = "救赎";
        d28.desc = "启用时, 尝试救赎死去敌人的灵魂\n来增加生命和魔法\n";
        Tabs[2].skills.Add(d28);

        Dat d29 = new Dat();
        d29.id = 125;
        d29.name = "拯救";
        d29.desc = "启用时, 增加你和队友的火、电和冰的抗性\n";
        Tabs[2].skills.Add(d29);
    }

    public override void FillAttribute(int tab, int index)
    {
        Dat desc = Tabs[tab].skills[index];
        FillAttribute(desc);
    }

    public override void FillAttribute(Dat desc)
    {
        int lvl = desc.lvl; // +附加值            
        desc.attr = "";

        switch (desc.id)
        {
            case 96:
                desc.attr += "命中率:" + sign(dec(ln(lvl, 20, 7), 0)) + "\n";
                desc.attr += "伤害:" + sign(dec(ln(lvl, 180, 15) + blvl(124) * 15 + blvl(122) * 5, 0)) + "\n";
                desc.attr += "+8% 自身伤害\n";
                desc.extr += "[22EE00]牺牲 由以下技能得到额外加成:[-]\n";
                desc.extr += "救赎: +15% 伤害每一技能等级\n";
                desc.extr += "狂热: +5% 伤害每一技能等级\n";
                break;
            case 97:
                desc.attr += "伤害:" + sign(dec((lvl * 15), 0)) + "\n";
                desc.attr += "眩晕时间:" + dec((min(250, ln(lvl, 15, 5))) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 2, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 98:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "伤害:" + sign(dec(ln(lvl, 40, 10), 0)) + "\n";
                break;
            case 99:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "治疗:" + dec(dec((ln(lvl, 2, 1, 1, 2, 2, 3) << 8), 0) / 256, 0) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 16, 3) << 4, 1 << 8) / 256, 1) + "\n";
                break;
            case 100:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "火焰抵抗:" + sign(dec(dm(lvl, 35, 150), 0)) + "\n";
                break;
            case 101:
                desc.attr += "魔法伤害:" + (dec(dec((ln(lvl, 8, 8, 10, 13, 16, 20) << 8) * (100 + ((blvl(112) + blvl(121)) * 50)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 16, 8, 11, 15, 18, 23) << 8) * (100 + ((blvl(112) + blvl(121)) * 50)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "治疗:" + dec(ln(lvl, 1, 2) * (100 + blvl(99) * 15) / 100, 0) + "-" + dec(ln(lvl, 6, 4) * (100 + blvl(99) * 15) / 100, 0) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 32, 1) << 4, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]圣光弹 由以下技能得到额外加成:[-]\n";
                desc.extr += "祝福之槌: +50% 魔法伤害每一技能等级\n";
                desc.extr += "天堂之拳: +50% 魔法伤害每一技能等级\n";
                desc.extr += "祈祷: +15% 生命恢复每一技能等级\n";
                break;
            case 102:
                desc.attr += "半径:" + dec(dec(ln(lvl, 6, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "火焰伤害:" + dec(dec((ln(lvl, 2, 1, 2, 3, 5, 7) << 7) * (100 + (blvl(100) * 18 + blvl(125) * 6)) / 100, 0) * 6 / 256, 0) + "-" + dec(dec((ln(lvl, 6, 1, 2, 3, 5, 7) << 7) * (100 + (blvl(100) * 18 + blvl(125) * 6)) / 100, 0) * 6 / 256, 0) + " 在攻击上\n";
                desc.attr += "火焰伤害:" + dec(dec((ln(lvl, 2, 1, 2, 3, 5, 7) << 7) * (100 + (blvl(100) * 18 + blvl(125) * 6)) / 100, 0) / 256, 1) + "-" + dec(dec((ln(lvl, 6, 1, 2, 3, 5, 7) << 7) * (100 + (blvl(100) * 18 + blvl(125) * 6)) / 100, 0) / 256, 1) + "\n";
                desc.extr += "[22EE00]圣火 由以下技能得到额外加成:[-]\n";
                desc.extr += "火焰抵抗: +18% 火焰伤害每一技能等级\n";
                desc.extr += "拯救: +6% 火焰伤害每一技能等级\n";
                break;
            case 103:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += dec(ln(lvl, 250, 40), 0) + "% 伤害反噬\n";
                break;
            case 104:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "防御加成:" + sign(dec(ln(lvl, 70, 10), 0)) + "\n";
                break;
            case 105:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "冰冷抵抗:" + sign(dec(dm(lvl, 35, 150), 0)) + "\n";
                break;
            case 106:
                desc.attr += "攻击力:" + sign(dec((lvl * 10), 0)) + "\n";
                desc.attr += "伤害:" + sign(dec(((lvl < 5) ? 0 : ((lvl - 4) * 6)) + blvl(96) * 12, 0)) + "\n";
                desc.attr += dec(min((2 + lvl - 1), 5), 0) + " 次攻击\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 2, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]热诚 由以下技能得到额外加成:[-]\n";
                desc.extr += "牺牲: +12% 伤害每一技能等级\n";
                break;
            case 107:
                desc.attr += "伤害:" + sign(dec(ln(lvl, 100, 25) + (blvl(115) + blvl(98)) * 20, 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 50, 15), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 9, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]冲锋 由以下技能得到额外加成:[-]\n";
                desc.extr += "活力: +20% 伤害每一技能等级\n";
                desc.extr += "力量: +20% 伤害每一技能等级\n";
                break;
            case 108:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 75, 15), 0)) + "\n";
                break;
            case 109:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "抵消时间 " + dec(dm(lvl, 30, 90), 0) + "%\n";
                desc.attr += "治疗: " + dec(dec((ln(tlvl(99), 2, 1, 1, 2, 2, 3) << 8), 0) / 256, 0) + "\n";
                desc.attr += "祈祷: +" + dec(dec(dec((ln(tlvl(99), 2, 1, 1, 2, 2, 3) << 8), 0) / 256, 0), 0) + " 生命恢复每2秒 \n";
                desc.extr += "[22EE00]净化 由以下技能得到额外加成:[-]\n";
                desc.extr += "祈祷\n";
                break;
            case 110:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "闪电抵抗:" + sign(dec(dm(lvl, 35, 150), 0)) + "\n";
                break;
            case 111:
                desc.attr += "冰冻时间:" + dec(dec(ln(lvl, 30, 15), 0) / 25, 1) + " 秒 \n";
                desc.attr += "火焰伤害:" + sign(dec(ln(lvl, 70, 6) + blvl(100) * 10 + blvl(125) * 2, 0)) + "\n";
                desc.attr += "冰冷伤害:" + sign(dec(ln(lvl, 70, 6) + blvl(105) * 10 + blvl(125) * 2, 0)) + "\n";
                desc.attr += "闪电伤害:" + sign(dec(ln(lvl, 70, 6) + blvl(110) * 10 + blvl(125) * 2, 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 20, 10), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 16, 1) << 6, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]复仇 由以下技能得到额外加成:[-]\n";
                desc.extr += "火焰抵抗: +10% 火焰伤害每一技能等级\n";
                desc.extr += "冰冷抵抗: +10% 冰冷伤害每一技能等级\n";
                desc.extr += "闪电抵抗: +10% 闪电伤害每一技能等级\n";
                desc.extr += "拯救: +2% 元素伤害每一技能等级\n";
                break;
            case 112:
                desc.attr += "魔法伤害:" + (dec(dec((ln(lvl, 12, 8, 10, 12, 13, 14) << 8) * (100 + ((blvl(115) + blvl(108)) * 14)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 16, 8, 10, 12, 13, 14) << 8) * (100 + ((blvl(115) + blvl(108)) * 14)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 20, 1) << 6, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]祝福之槌 由以下技能得到额外加成:[-]\n";
                desc.extr += "祝福瞄准: +14% 魔法伤害每一技能等级\n";
                desc.extr += "活力: +14% 魔法伤害每一技能等级\n";
                break;
            case 113:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "打断概率 " + dec(20, 0) + "%\n";
                desc.attr += "伤害:" + sign(dec(ln(lvl, 60, 15), 0)) + "\n";
                break;
            case 114:
                desc.attr += "半径:" + dec(dec(ln(lvl, 6, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "冰冷伤害:" + dec(dec((ln(lvl, 2, 1, 2, 3, 4, 5) << 8) * (100 + (blvl(105) * 15 + blvl(125) * 7)) / 100, 0) * 5 / 256, 0) + "-" + dec(dec((ln(lvl, 3, 1, 2, 3, 4, 5) << 8) * (100 + (blvl(105) * 15 + blvl(125) * 7)) / 100, 0) * 5 / 256, 0) + " 在攻击上\n";
                desc.attr += "冰冷伤害:" + (dec(dec((ln(lvl, 2, 1, 2, 3, 4, 5) << 8) * (100 + (blvl(105) * 15 + blvl(125) * 7)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 3, 1, 2, 3, 4, 5) << 8) * (100 + (blvl(105) * 15 + blvl(125) * 7)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "敌人减慢 " + dec(dm(lvl, 25, 60), 0) + "%\n";
                desc.extr += "[22EE00]圣洁寒冰 由以下技能得到额外加成:[-]\n";
                desc.extr += "冰冷抵抗: +15% 冰冷伤害每一技能等级\n";
                desc.extr += "拯救: +7% 冰冷伤害每一技能等级\n";
                break;
            case 115:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 3), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "移动速度:" + sign(dec(dm(lvl, 7, 50), 0)) + "\n";
                desc.attr += "耐力加成:" + sign(dec(ln(lvl, 50, 25), 0)) + "\n";
                desc.attr += "耐力恢复速度:" + sign(dec(ln(lvl, 50, 25), 0)) + "\n";
                break;
            case 116:
                desc.attr += "转化概率:" + dec(dm(lvl, 0, 50), 0) + "%\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 400, 0)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 4, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 117:
                desc.attr += "重击伤害: +" + (dec((ln(lvl, 3, 2, 3, 4) << 8) * (100 + (0)) / 25600, 0)) + "-" + (dec((ln(lvl, 6, 2, 3, 4) << 8) * (100 + (0)) / 25600, 0)) + "\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 750, 625)) / 25, 1) + " 秒 \n";
                desc.attr += "防御加成:" + sign(dec(ln(lvl, 25, 15) + blvl(104) * 15, 0)) + "\n";
                desc.attr += "格挡成功率:" + sign(dec(dm(lvl, 10, 40), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 35, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]神圣护盾 由以下技能得到额外加成:[-]\n";
                desc.extr += "蔑视: +15% 防御每一技能等级\n";
                break;
            case 118:
                desc.attr += "半径:" + dec(dec(ln(lvl, 6, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "闪电伤害:" + dec(1, 0) + "-" + dec(dec((ln(lvl, 10, 6, 8, 10, 12, 15) << 8) * (100 + (blvl(110) * 12 + blvl(125) * 4)) / 100, 0) * 6 / 256, 0) + " 在攻击上\n";
                desc.attr += "闪电伤害:" + (dec(dec((ln(lvl, 1, 0) << 8) * (100 + (blvl(110) * 12 + blvl(125) * 4)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 10, 6, 8, 10, 12, 15) << 8) * (100 + (blvl(110) * 12 + blvl(125) * 4)) / 100, 0) / 256, 0)) + "\n";
                desc.extr += "[22EE00]神圣冲击 由以下技能得到额外加成:[-]\n";
                desc.extr += "闪电抵抗: +12% 闪电伤害每一技能等级\n";
                desc.extr += "拯救: +4% 闪电伤害每一技能等级\n";
                break;
            case 119:
                desc.attr += "半径:" + dec(dec(ln(lvl, 5, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "对不死系伤害: +" + dec(ln(lvl, 150, 30), 0) + " %\n";
                desc.attr += "魔法伤害:" + (dec(dec((ln(lvl, 8, 4, 4, 5, 5, 6) << 8) * (100 + (blvl(109) * 7)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 16, 4, 5, 6, 6, 7) << 8) * (100 + (blvl(109) * 7)) / 100, 0) / 256, 0)) + "\n";
                desc.extr += "[22EE00]庇护所 由以下技能得到额外加成:[-]\n";
                desc.extr += "净化: +7% 魔法伤害每一技能等级\n";
                break;
            case 120:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法恢复比率:" + sign(dec(ln(lvl, 300, 25), 0)) + "\n";
                desc.attr += "治疗: +" + dec(dec((ln(tlvl(99), 2, 1, 1, 2, 2, 3) << 8), 0) / 256, 0) + "\n";
                desc.attr += "祈祷: +" + dec(dec(dec((ln(tlvl(99), 2, 1, 1, 2, 2, 3) << 8), 0) / 256, 0), 0) + " 生命恢复每2秒 \n";
                desc.extr += "[22EE00]冥想 由以下技能得到额外加成:[-]\n";
                desc.extr += "祈祷\n";
                break;
            case 121:
                desc.attr += "圣光弹 伤害:" + (dec(dec((ln(lvl, 40, 6, 10, 16, 32, 48) << 8) * (100 + (blvl(101) * 15)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 50, 6, 10, 16, 32, 48) << 8) * (100 + (blvl(101) * 15)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "闪电伤害:" + (dec(dec((ln(lvl, 150, 15, 30, 45, 55, 65) << 8) * (100 + (blvl(118) * 7)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 200, 15, 30, 45, 55, 65) << 8) * (100 + (blvl(118) * 7)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 25, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]天堂之拳 由以下技能得到额外加成:[-]\n";
                desc.extr += "圣光弹: +15% 圣光弹 伤害每一技能等级\n";
                desc.extr += "神圣冲击: +7% 闪电伤害每一技能等级\n";
                break;
            case 122:
                desc.attr += "半径:" + dec(dec(ln(lvl, 11, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "队友伤害:" + sign(dec(ln(lvl, 50, 17) / 2, 0)) + "\n";
                desc.attr += "自身伤害:" + sign(dec(ln(lvl, 50, 17), 0)) + "\n";
                desc.attr += "攻击速度:" + sign(dec(dm(lvl, 10, 40), 0)) + "\n";
                desc.attr += "命中率:" + sign(dec(ln(lvl, 40, 5), 0)) + "\n";
                break;
            case 123:
                desc.attr += "半径:" + dec(dec(ln(lvl, 20, 0), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "防御:" + sign(dec(-dm(lvl, 40, 100), 0)) + "\n";
                desc.attr += "抗性:" + dec(-min(ln(lvl, 30, 5), 150), 0) + "%\n";
                break;
            case 124:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 0), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "救赎几率:" + dec(dm(lvl, 10, 100), 0) + "%\n";
                desc.attr += "生命/魔法恢复:" + dec(ln(lvl, 25, 5), 0) + " 点\n";
                break;
            case 125:
                desc.attr += "半径:" + dec(dec(ln(lvl, 16, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "火电冰抗性:" + sign(dec(dm(lvl, 50, 120), 0)) + "\n";
                break;
        }
    }
}

public class SkillSor : SkillInfo
{
    public override void Init()
    {
        base.Init();
        //tabs[0].sID = new byte[10] { 36,37,41,46,47,51,52,56,61,62 };
        Tabs[0].name = "火焰系\n咒语";
        //tabs[1].sID = new byte[10] { 38,42,43,48,49,53,54,57,58,63 };
        Tabs[1].name = "闪电系\n咒语";
        //tabs[2].sID = new byte[10] { 39,40,44,45,50,55,59,60,64,65 };
        Tabs[2].name = "冰冻系\n咒语";

        InitTab1();
        InitTab2();
        InitTab3();
    }

    private void InitTab1()
    {
        Dat d00 = new Dat();
        d00.id = 36;
        d00.name = "火弹";
        d00.desc = "创造一个火焰小球\n";
        Tabs[0].skills.Add(d00);

        Dat d01 = new Dat();
        d01.id = 37;
        d01.name = "温暖";
        d01.desc = "被动 - 提高魔法恢复的速率\n";
        Tabs[0].skills.Add(d01);

        Dat d02 = new Dat();
        d02.id = 41;
        d02.name = "地狱火";
        d02.desc = "发出一束连续喷射的火焰\n烧焦你的敌人\n";
        Tabs[0].skills.Add(d02);

        Dat d03 = new Dat();
        d03.id = 46;
        d03.name = "烈焰之径";
        d03.desc = "沿着你的移动路线建立一道燃烧的火墙\n";
        Tabs[0].skills.Add(d03);

        Dat d04 = new Dat();
        d04.id = 47;
        d04.name = "火球";
        d04.desc = "创造一个爆炸的火球吞没你的敌人\n";
        Tabs[0].skills.Add(d04);

        Dat d05 = new Dat();
        d05.id = 51;
        d05.name = "火墙";
        d05.desc = "创造一道火墙阻燃敌人\n";
        Tabs[0].skills.Add(d05);

        Dat d06 = new Dat();
        d06.id = 52;
        d06.name = "火焰强化";
        d06.desc = "给近战武器附上火焰伤害\n在远程武器上加上1/3火焰伤害\n";
        Tabs[0].skills.Add(d06);

        Dat d07 = new Dat();
        d07.id = 56;
        d07.name = "陨石";
        d07.desc = "召唤一个从天而降的陨石压碎并烧焦敌人\n";
        Tabs[0].skills.Add(d07);

        Dat d08 = new Dat();
        d08.id = 61;
        d08.name = "火系掌握";
        d08.desc = "被动 - 增加火焰系咒语的伤害\n";
        Tabs[0].skills.Add(d08);

        Dat d09 = new Dat();
        d09.id = 62;
        d09.name = "九头海蛇";
        d09.desc = "召唤一只多头的火焰兽\n向敌人发射火焰弹\n";
        Tabs[0].skills.Add(d09);
    }

    private void InitTab2()
    {
        Dat d10 = new Dat();
        d10.id = 38;
        d10.name = "闪电弹";
        d10.desc = "发射多个自己寻找目标的闪电\n";
        Tabs[1].skills.Add(d10);

        Dat d11 = new Dat();
        d11.id = 42;
        d11.name = "静电力场";
        d11.desc = "创造一个静电力场降低所有附近敌人的生命\n";
        Tabs[1].skills.Add(d11);

        Dat d12 = new Dat();
        d12.id = 43;
        d12.name = "心灵遥感";
        d12.desc = "使用精神力量拾取物品,\n 触发机关,或打击敌人\n";
        Tabs[1].skills.Add(d12);

        Dat d13 = new Dat();
        d13.id = 48;
        d13.name = "新星";
        d13.desc = "创造一个扩大的电环电击敌人\n";
        Tabs[1].skills.Add(d13);

        Dat d14 = new Dat();
        d14.id = 49;
        d14.name = "闪电";
        d14.desc = "释放一道强烈的闪电蹂躏敌人\n";
        Tabs[1].skills.Add(d14);

        Dat d15 = new Dat();
        d15.id = 53;
        d15.name = "连锁闪电";
        d15.desc = "创造一道连锁闪电\n贯穿多个目标\n";
        Tabs[1].skills.Add(d15);

        Dat d16 = new Dat();
        d16.id = 54;
        d16.name = "连锁闪电";
        d16.desc = "创造一道连锁闪电\n贯穿多个目标\n";
        Tabs[1].skills.Add(d16);

        Dat d17 = new Dat();
        d17.id = 57;
        d17.name = "雷暴";
        d17.desc = "召唤一个致命的雷暴打击敌人\n";
        Tabs[1].skills.Add(d17);

        Dat d18 = new Dat();
        d18.id = 58;
        d18.name = "能量护盾";
        d18.desc = "创造一个能量护盾\n用魔法来代替生命受到的伤害\n";
        Tabs[1].skills.Add(d18);

        Dat d19 = new Dat();
        d19.id = 63;
        d19.name = "电系掌握";
        d19.desc = "被动 - 增强闪电咒语的伤害\n";
        Tabs[1].skills.Add(d19);
    }

    private void InitTab3()
    {
        Dat d20 = new Dat();
        d20.id = 39;
        d20.name = "冰弹";
        d20.desc = "发射一个冰弹伤害并减慢敌人\n";
        Tabs[2].skills.Add(d20);

        Dat d21 = new Dat();
        d21.id = 40;
        d21.name = "冰封装甲";
        d21.desc = "提升防御等级并冻结打击你的敌人\n";
        Tabs[2].skills.Add(d21);

        Dat d22 = new Dat();
        d22.id = 44;
        d22.name = "霜之新星";
        d22.desc = "创造一道扩展的冰环\n伤害并减慢敌人\n";
        Tabs[2].skills.Add(d22);

        Dat d23 = new Dat();
        d23.id = 45;
        d23.name = "冰风暴";
        d23.desc = "发射一个冰球伤害并冻结敌人\n";
        Tabs[2].skills.Add(d23);

        Dat d24 = new Dat();
        d24.id = 50;
        d24.name = "碎冰甲";
        d24.desc = "提升防御等级\n冻结并伤害打击你的敌人\n";
        Tabs[2].skills.Add(d24);

        Dat d25 = new Dat();
        d25.id = 55;
        d25.name = "冰矛";
        d25.desc = "发射一个坚冰彗星冻结或杀死附近的敌人\n";
        Tabs[2].skills.Add(d25);

        Dat d26 = new Dat();
        d26.id = 59;
        d26.name = "暴风雪";
        d26.desc = "召唤大量厚重的冰块毁灭敌人\n";
        Tabs[2].skills.Add(d26);

        Dat d27 = new Dat();
        d27.id = 60;
        d27.name = "寒冰装甲";
        d27.desc = "提升防御等级\n并向远程攻击者发射冰弹报复\n";
        Tabs[2].skills.Add(d27);

        Dat d28 = new Dat();
        d28.id = 64;
        d28.name = "冰封球";
        d28.desc = "创造一个冰球向周围发射冰弹\n蹂躏你的敌人\n";
        Tabs[2].skills.Add(d28);

        Dat d29 = new Dat();
        d29.id = 65;
        d29.name = "冰冷掌握";
        d29.desc = "被动 - 通过降低敌人的冰冷抗性来提高伤害\n";
        Tabs[2].skills.Add(d29);
    }

    public override void FillAttribute(int tab, int index)
    {
        Dat desc = Tabs[tab].skills[index];
        FillAttribute(desc);
    }

    public override void FillAttribute(Dat desc)
    {
        int lvl = desc.lvl; // +附加值            
        desc.attr = "";

        switch (desc.id)
        {
            case 36:
                desc.attr += "火焰伤害:" + (dec(dec(dec((ln(lvl, 6, 3, 4, 8, 18, 54) << 7) * (100 + ((blvl(47) + blvl(56)) * 16)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 12, 3, 6, 10, 20, 56) << 7) * (100 + ((blvl(47) + blvl(56)) * 16)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 5, 0) << 7, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]火弹 由以下技能得到额外加成:[-]\n";
                desc.extr += "火球: +16% 火焰伤害每一技能等级\n";
                desc.extr += "陨石: +16% 火焰伤害每一技能等级\n";
                break;
            case 37:
                desc.attr += dec(ln(lvl, 30, 12), 0) + "%\n";
                break;
            case 38:
                desc.attr += "闪电伤害:" + (dec(dec(dec((ln(lvl, 4, 1, 1, 2, 3, 4) << 7) * (100 + ((blvl(49)) * 6)) / 100, 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 8, 1, 1, 2, 3, 4) << 7) * (100 + ((blvl(49)) * 6)) / 100, 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "数量 " + dec(min(24, ln(lvl, 3, 1)), 0) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 24, 4) << 5, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]闪电弹 由以下技能得到额外加成:[-]\n";
                desc.extr += "闪电: +6% 闪电伤害每一技能等级\n";
                break;
            case 39:
                desc.attr += "冰冷伤害:" + (dec(dec((ln(lvl, 6, 2, 4, 6, 8, 10) << 7) * (100 + ((blvl(44) + blvl(45) + blvl(55) + blvl(59) + blvl(64)) * 15)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 10, 3, 5, 7, 9, 11) << 7) * (100 + ((blvl(44) + blvl(45) + blvl(55) + blvl(59) + blvl(64)) * 15)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "冰冻时间:" + dec(dec(ln(lvl, 150, 35), 0) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 3, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]冰弹 由以下技能得到额外加成:[-]\n";
                desc.extr += "霜之新星: +15% 冰冷伤害每一技能等级\n";
                desc.extr += "冰风暴: +15% 冰冷伤害每一技能等级\n";
                desc.extr += "冰矛: +15% 冰冷伤害每一技能等级\n";
                desc.extr += "暴风雪: +15% 冰冷伤害每一技能等级\n";
                desc.extr += "冰封球: +15% 冰冷伤害每一技能等级\n";
                break;
            case 40:
                desc.attr += "防御加成:" + dec(ln(lvl, 30, 5), 0) + "%\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 3000, 300) + (blvl(50) + blvl(60)) * 250) / 25, 1) + " 秒 \n";
                desc.attr += "冰冻时间 " + dec((ln(lvl, 30, 3) * (100 + ((blvl(50) + blvl(60)) * 5)) / 100) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 7, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]冰封装甲 由以下技能得到额外加成:[-]\n";
                desc.extr += "碎冰甲: +" + dec((250 + 12) / 25, 0) + " 秒 每一技能等级\n";
                desc.extr += "碎冰甲: +5% 冰冻时间每一技能等级\n";
                desc.extr += "寒冰装甲: +" + dec((250 + 12) / 25, 0) + " 秒 每一技能等级\n";
                desc.extr += "寒冰装甲: +5% 冰冻时间每一技能等级\n";
                break;
            case 41:
                desc.attr += "平均火焰伤害:" + (dec(dec(dec((ln(lvl, 32, 24, 26, 28, 32, 36) << 2) * (100 + ((blvl(37)) * 13)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 10.24f, 0)) + "-" + (dec(dec(dec((ln(lvl, 64, 24, 27, 29, 33, 37) << 2) * (100 + ((blvl(37)) * 13)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 10.24f, 0)) + " 每秒 \n";
                desc.attr += "射程:" + dec(dec(ln(lvl, 20, 3) / 4, 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(dec(max(ln(lvl, 36, 1) << 2, 0) / 20.48f, 0), 0) + " 每秒 \n";
                desc.attr += "最低魔法需求:" + dec(6, 0) + "\n";
                desc.extr += "[22EE00]地狱火 由以下技能得到额外加成:[-]\n";
                desc.extr += "温暖: +13% 火焰伤害每一技能等级\n";
                break;
            case 42:
                desc.attr += "半径:" + dec(dec(ln(lvl, 5, 1), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 9, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.attr += "削弱敌人生命 " + dec(25, 0) + "%\n";
                break;
            case 43:
                desc.attr += "闪电伤害:" + (dec(dec(dec((lvl << 8), 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 2, 1) << 8), 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 7, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 44:
                desc.attr += "冰冷伤害:" + (dec(dec((ln(lvl, 4, 4, 6, 8, 10, 12) << 7) * (100 + ((blvl(59) + blvl(64)) * 10)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 8, 5, 7, 9, 11, 13) << 7) * (100 + ((blvl(59) + blvl(64)) * 10)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "冰冻时间:" + dec(dec(ln(lvl, 200, 25), 0) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 9, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]霜之新星 由以下技能得到额外加成:[-]\n";
                desc.extr += "暴风雪: +10% 冰冷伤害每一技能等级\n";
                desc.extr += "冰封球: +10% 冰冷伤害每一技能等级\n";
                break;
            case 45:
                desc.attr += "冰冷伤害:" + (dec(dec((ln(lvl, 16, 14, 28, 42, 56, 70) << 7) * (100 + ((blvl(39) + blvl(59) + blvl(64)) * 8)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 24, 15, 29, 43, 57, 71) << 7) * (100 + ((blvl(39) + blvl(59) + blvl(64)) * 8)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "冰冻时间 " + dec((dec(ln(lvl, 75, 5) * (100 + ((blvl(55)) * 10)) / 100, 0)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 12, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]冰风暴 由以下技能得到额外加成:[-]\n";
                desc.extr += "冰弹: +8% 冰冷伤害每一技能等级\n";
                desc.extr += "冰矛: +10% 冰冻时间每一技能等级\n";
                desc.extr += "暴风雪: +8% 冰冷伤害每一技能等级\n";
                desc.extr += "冰封球: +8% 冰冷伤害每一技能等级\n";
                break;
            case 46:
                desc.attr += "火焰持续时间:" + dec((ln(lvl, 90, 25)) / 25, 1) + " 秒 \n";
                desc.attr += "平均火焰伤害:" + (dec(dec(dec((ln(lvl, 4, 2, 3, 4, 6, 9) << 4), 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) * 75 / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 8, 2, 3, 4, 6, 9) << 4), 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) * 75 / 256, 0)) + " 每秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 22, 1) << 7, 1 << 8) / 256, 1) + "\n";
                break;
            case 47:
                desc.attr += "火焰伤害:" + (dec(dec(dec((ln(lvl, 12, 13, 23, 28, 33, 38) << 7) * (100 + ((blvl(36) + blvl(56)) * 14)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 28, 15, 25, 30, 35, 40) << 7) * (100 + ((blvl(36) + blvl(56)) * 14)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 10, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.attr += "半径: 1 码\n";
                desc.extr += "[22EE00]火球 由以下技能得到额外加成:[-]\n";
                desc.extr += "火弹: +14% 火焰伤害每一技能等级\n";
                desc.extr += "陨石: +14% 火焰伤害每一技能等级\n";
                break;
            case 48:
                desc.attr += "闪电伤害:" + (dec(dec(dec((ln(lvl, 1, 6, 7, 8, 9, 10) << 8), 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 20, 8, 9, 10, 11, 12) << 8), 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 15, 1) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 49:
                desc.attr += "闪电伤害:" + (dec(dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(38) + blvl(53) + blvl(48)) * 8)) / 100, 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 40, 8, 12, 20, 28, 36) << 8) * (100 + ((blvl(38) + blvl(53) + blvl(48)) * 8)) / 100, 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 16, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]闪电 由以下技能得到额外加成:[-]\n";
                desc.extr += "闪电弹: +8% 闪电伤害每一技能等级\n";
                desc.extr += "新星: +8% 闪电伤害每一技能等级\n";
                desc.extr += "连锁闪电: +8% 闪电伤害每一技能等级\n";
                break;
            case 50:
                desc.attr += "持续时间:" + dec((ln(lvl, 3000, 300) + (blvl(40) + blvl(60)) * 250) / 25, 1) + " 秒 \n";
                desc.attr += "防御加成:" + dec(ln(lvl, 45, 6), 0) + "%\n";
                desc.attr += "冰冷伤害:" + (dec(dec((ln(lvl, 12, 4, 6, 8, 10, 12) << 7) * (100 + ((blvl(40) + blvl(60)) * 9)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 16, 5, 7, 9, 11, 13) << 7) * (100 + ((blvl(40) + blvl(60)) * 9)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "冰冻时间:" + dec(dec(ln(lvl, 100, 0, 25, 50), 0) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 11, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]碎冰甲 由以下技能得到额外加成:[-]\n";
                desc.extr += "冰封装甲: +" + dec((250 + 12) / 25, 0) + " 秒 每一技能等级\n";
                desc.extr += "冰封装甲: +9% 冰冷伤害每一技能等级\n";
                desc.extr += "寒冰装甲: +" + dec((250 + 12) / 25, 0) + " 秒 每一技能等级\n";
                desc.extr += "寒冰装甲: +9% 冰冷伤害每一技能等级\n";
                break;
            case 51:
                desc.attr += "火焰持续时间:" + dec(ln(lvl, 90, 0) / 25, 1) + " 秒 \n";
                desc.attr += "平均火焰伤害:" + (dec(dec(dec((ln(lvl, 15, 9, 14, 21) << 4), 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) * 75 / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 20, 9, 14, 21) << 4), 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) * 75 / 256, 0)) + " 每秒 \n";
                desc.attr += dec(dec(ln(lvl, 7, 2), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 22, 1) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 52:
                desc.attr += "持续时间:" + dec((ln(lvl, 3600, 600)) / 25, 1) + " 秒 \n";
                desc.attr += "火焰伤害:" + (dec(dec(dec((ln(lvl, 16, 3, 7, 11, 15, 19) << 7) * (100 + ((blvl(37)) * 9)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 20, 5, 9, 13, 17, 21) << 7) * (100 + ((blvl(37)) * 9)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "攻击力:" + sign(dec(ln(lvl, 20, 9), 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 25, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]火焰强化 由以下技能得到额外加成:[-]\n";
                desc.extr += "温暖: +9% 火焰伤害每一技能等级\n";
                break;
            case 53:
                desc.attr += "电击次数:" + dec(ln(lvl, 26, 1) / 5, 0) + "\n";
                desc.attr += "闪电伤害:" + (dec(dec(dec((ln(lvl, 1, 0) << 8) * (100 + ((blvl(38) + blvl(49) + blvl(48)) * 4)) / 100, 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 40, 11, 13, 15) << 8) * (100 + ((blvl(38) + blvl(49) + blvl(48)) * 4)) / 100, 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 9, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]连锁闪电 由以下技能得到额外加成:[-]\n";
                desc.extr += "闪电弹: +4% 闪电伤害每一技能等级\n";
                desc.extr += "新星: +4% 闪电伤害每一技能等级\n";
                desc.extr += "闪电: +4% 闪电伤害每一技能等级\n";
                break;
            case 54:
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 24, -1) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 55:
                desc.attr += "冰冷伤害:" + (dec(dec((ln(lvl, 32, 14, 26, 28, 30, 32) << 7) * (100 + ((blvl(39) + blvl(45) + blvl(64)) * 5)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 48, 15, 27, 29, 31, 33) << 7) * (100 + ((blvl(39) + blvl(45) + blvl(64)) * 5)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "冰冻时间 " + dec((ln(lvl, 50, 3) * (100 + blvl(59) * 3) / 100) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 20, 1) << 7, 1 << 8) / 256, 1) + "\n";
                desc.attr += "半径:" + dec(dec(ln(lvl, 4, 0), 0) * 2 / 3, 1) + " 码\n";
                desc.extr += "[22EE00]冰矛 由以下技能得到额外加成:[-]\n";
                desc.extr += "冰弹: +5% 冰冷伤害每一技能等级\n";
                desc.extr += "冰风暴: +5% 冰冷伤害每一技能等级\n";
                desc.extr += "暴风雪: +3% 冰冻时间每一技能等级\n";
                desc.extr += "冰封球: +5% 冰冷伤害每一技能等级\n";
                break;
            case 56:
                desc.attr += "火焰伤害:" + (dec(dec(dec((ln(lvl, 80, 23, 39, 79, 81, 83) << 8) * (100 + ((blvl(36) + blvl(47)) * 5)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 100, 25, 41, 81, 83, 85) << 8) * (100 + ((blvl(36) + blvl(47)) * 5)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "半径:" + dec(dec(ln(lvl, 6, 0), 0) * 2 / 3, 1) + " 码\n";
                desc.attr += "平均火焰伤害:" + dec(dec(dec((ln(lvl, 15, 4, 5, 6) << 3) * (100 + (blvl(41) * 3)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) * 75 / 256, 0) + "-" + dec(dec(dec((ln(lvl, 25, 4, 5, 6) << 3) * (100 + (blvl(41) * 3)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) * 25 / 256, 0) * 3 + " 每秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 34, 1) << 7, 1 << 8) / 256, 0) + "\n";
                desc.extr += "[22EE00]陨石 由以下技能得到额外加成:[-]\n";
                desc.extr += "火弹: +5% 火焰伤害每一技能等级\n";
                desc.extr += "火球: +5% 火焰伤害每一技能等级\n";
                desc.extr += "地狱火: +3% 平均火焰伤害 每秒 每一技能等级\n";
                break;
            case 57:
                desc.attr += "持续时间:" + dec((ln(lvl, 800, 200)) / 25, 1) + " 秒 \n";
                desc.attr += "闪电伤害:" + (dec(dec(dec((ln(lvl, 1, 10, 10, 11) << 8), 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 100, 10, 10, 11) << 8), 0) * (100 + ln(tlvl(63), 50, 12)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 19, 0) << 8, 1 << 8) / 256, 1) + "\n";
                break;
            case 58:
                desc.attr += "持续时间:" + dec((ln(lvl, 3600, 1500)) / 25, 1) + " 秒 \n";
                desc.attr += "吸收 " + dec(min(dec(dec((ln(lvl, 20, 5, 2, 1) << 8), 0) / 256, 0), 95), 0) + "%\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 5, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]能量护盾 由以下技能得到额外加成:[-]\n";
                desc.extr += "心灵遥感\n";
                break;
            case 59:
                desc.attr += "冰冷伤害:" + (dec(dec((ln(lvl, 45, 15, 30, 45, 55, 65) << 8) * (100 + ((blvl(39) + blvl(45) + blvl(55)) * 5)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 75, 16, 31, 46, 56, 66) << 8) * (100 + ((blvl(39) + blvl(45) + blvl(55)) * 5)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 100, 0)) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 23, 1) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]暴风雪 由以下技能得到额外加成:[-]\n";
                desc.extr += "冰弹: +5% 冰冷伤害每一技能等级\n";
                desc.extr += "冰风暴: +5% 冰冷伤害每一技能等级\n";
                desc.extr += "冰矛: +5% 冰冷伤害每一技能等级\n";
                break;
            case 60:
                desc.attr += "防御加成:" + dec(ln(lvl, 45, 5), 0) + "%\n";
                desc.attr += "持续时间:" + dec((ln(lvl, 3600, 150) + (blvl(40) + blvl(50)) * 250) / 25, 1) + " 秒 \n";
                desc.attr += "冰冷伤害:" + (dec(dec((ln(lvl, 8, 2, 4, 6, 8, 10) << 7) * (100 + ((blvl(40) + blvl(50)) * 7)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 12, 3, 5, 7, 9, 11) << 7) * (100 + ((blvl(40) + blvl(50)) * 7)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 17, 0) << 8, 1 << 8) / 256, 1) + "\n";
                desc.extr += "[22EE00]寒冰装甲 由以下技能得到额外加成:[-]\n";
                desc.extr += "冰封装甲: +" + dec((250 + 12) / 25, 0) + " 秒 每一技能等级\n";
                desc.extr += "冰封装甲: +7% 冰冷伤害每一技能等级\n";
                desc.extr += "碎冰甲: +" + dec((250 + 12) / 25, 0) + " 秒 每一技能等级\n";
                desc.extr += "碎冰甲: +7% 冰冷伤害每一技能等级\n";
                break;
            case 61:
                desc.attr += "火焰伤害:" + sign(dec(ln(lvl, 30, 7), 0)) + "\n";
                break;
            case 62:
                desc.attr += "持续时间:" + dec((ln(lvl, 250, 0)) / 25, 1) + " 秒 \n";
                desc.attr += "九头海蛇 火焰伤害:" + (dec(dec(dec((ln(lvl, 24, 9, 13, 17, 21, 25) << 7) * (100 + ((blvl(36) + blvl(47)) * 3)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 256, 0)) + "-" + (dec(dec(dec((ln(lvl, 34, 11, 15, 19, 23, 27) << 7) * (100 + ((blvl(36) + blvl(47)) * 3)) / 100, 0) * (100 + ln(tlvl(61), 30, 7)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 40, 1) << 7, 1 << 8) / 256, 0) + "\n";
                desc.extr += "[22EE00]九头海蛇 由以下技能得到额外加成:[-]\n";
                desc.extr += "火弹: +3% 火焰伤害每一技能等级\n";
                desc.extr += "火球: +3% 火焰伤害每一技能等级\n";
                break;
            case 63:
                desc.attr += "闪电伤害:" + sign(dec(ln(lvl, 50, 12), 0)) + "\n";
                break;
            case 64:
                desc.attr += "冰冷伤害:" + (dec(dec((ln(lvl, 80, 20, 24, 28, 29, 30) << 7) * (100 + ((blvl(39)) * 2)) / 100, 0) / 256, 0)) + "-" + (dec(dec((ln(lvl, 90, 21, 25, 29, 30, 31) << 7) * (100 + ((blvl(39)) * 2)) / 100, 0) / 256, 0)) + "\n";
                desc.attr += "冰冻时间:" + dec(dec(ln(lvl, 200, 25), 0) / 25, 1) + " 秒 \n";
                desc.attr += "魔法消耗:" + dec(max(ln(lvl, 50, 1) << 7, 1 << 8) / 256, 0) + "\n";
                desc.extr += "[22EE00]冰封球 由以下技能得到额外加成:[-]\n";
                desc.extr += "冰弹: +2% 冰冷伤害每一技能等级\n";
                break;
            case 65:
                desc.attr += dec(ln(lvl, 20, 5), 0) + "%\n";
                break;
        }
    }
}



