using System;
using System.Collections.Generic;
using UnityEngine;


public static class Global
{
    // 是否为单人模式
    public static bool SolePlayerMode = false;

    public static Hero LocalHero;
    //public static Parse.ParseObject CurrentRole;

    public static List<Hero> MyHeros = new List<Hero>();

    public static ushort SelectBattle = 0;
    public static Hero OtherHero;

    public static string[] CharIcon = {"", "amaIc", "assIc", "barIc", "druIc", "necIc", "palIc", "sorIc" };

    public static string ConvertToColor(Config.Equipment.Quality quality)
    {
        string color = "[FFFFFF]";

        if (quality == Config.Equipment.Quality.Normal)
            color = "[FFFFFF]";
        else if (quality == Config.Equipment.Quality.Poor)
            color = "[BEBEBE]";
        else if (quality == Config.Equipment.Quality.Magic)
            color = "[4850B8]";
        else if (quality == Config.Equipment.Quality.Rare)
            color = "[FFFF00]";
        else if (quality == Config.Equipment.Quality.Crafted)
            color = "[908858]";
        else if (quality == Config.Equipment.Quality.Sets)
            color = "[00c400]";

        return color;
    }

    // 组合装备提示
    public static string CombEquipmentTips(RemoteChar charactor, ushort id, bool mine, bool depot)
    {
        string tips = "";

        if (Config.DataLoader.equipmentMaps.ContainsKey(id))
        {
            Config.Equipment equipment = Config.DataLoader.equipmentMaps[id];

            string color = Global.ConvertToColor(equipment.quality);

            if (!string.IsNullOrEmpty(equipment.nickname))
                tips += color + equipment.nickname + "[-]\n";

            tips += color + equipment.name + "[-]\n";

            if (!string.IsNullOrEmpty(equipment.setsName))
                tips += "[00c400]" + equipment.setsName + "[-]\n";

            if (equipment.minDamage > 0 && equipment.maxDamage > 0)
            {
                if (equipment.type == Config.Equipment.Type.Weapon)
                {
                    if (equipment.weaponType == Config.WeaponType.Polearms ||
                        equipment.weaponType == Config.WeaponType.Bows ||
                        equipment.weaponType == Config.WeaponType.Crossbows ||
                        equipment.weaponType == Config.WeaponType.Spear)
                    {
                        tips += "[FFFFFF]双手伤害 : " + equipment.minDamage + " 到 " + equipment.maxDamage + "[-]\n";
                    }
                    else
                    {
                        tips += "[FFFFFF]单手伤害 : " + equipment.minDamage + " 到 " + equipment.maxDamage + "[-]\n";
                    }
                }
                else
                {
                    tips += "[FFFFFF]伤害 : " + equipment.minDamage + " 到 " + equipment.maxDamage + "[-]\n";
                }
            }

            if (equipment.minDefense > 0 && equipment.maxDefense > 0)
            {
                if (equipment.minDefense != equipment.maxDefense)
                    tips += "[FFFFFF]防御 : " + equipment.minDefense + " 到 " + equipment.maxDefense + "[-]\n";
                else
                    tips += "[FFFFFF]防御 : " + equipment.minDefense + "[-]\n";
            }

            if (equipment.type == Config.Equipment.Type.Chest)
            {
                tips += "[FFFF00]打开箱子有几率获得魔法装备[-]\n";
            }

            if (equipment.profession != Config.Profession.None)
            {
                color = "[FFFFFF]";
                if (equipment.profession != charactor.profession)
                    color = "[FF0000]";

                tips += color + "(限 " + Profession(equipment.profession)+ " 使用)[-]\n";
            }

            if (equipment.dexRequest > 0)
            {
                color = "[FFFFFF]";
                if (equipment.dexRequest > Config.CharAttribute.Dex(charactor))
                    color = "[FF0000]";

                tips += color + "需要敏捷点数 : " + equipment.dexRequest + "[-]\n";
            }

            if (equipment.strRequest > 0)
            {
                color = "[FFFFFF]";
                if (equipment.strRequest > Config.CharAttribute.Str(charactor))
                    color = "[FF0000]";

                tips += color + "需要力量点数 : " + equipment.strRequest + "[-]\n";
            }

            if (equipment.lvlRequest > 0)
            {
                color = "[FFFFFF]";
                if (equipment.lvlRequest > charactor.level)
                    color = "[FF0000]";

                tips += color + "需要等级 : " + equipment.lvlRequest + "[-]\n";
            }

            if (equipment.damageAddi > 0)
            {
                tips += "[4850B8]+" + equipment.damageAddi + " 最大伤害值[-]\n";
            }

            if (equipment.strAddi > 0)
            {
                tips += "[2820B8]+" + equipment.strAddi + " 力量[-]\n";
            }

            if (equipment.dexAddi > 0)
            {
                tips += "[2820B8]+" + equipment.dexAddi + " 敏捷[-]\n";
            }

            if (equipment.defAddi > 0)
            {
                tips += "[2820B8]+" + equipment.defAddi + " 防御[-]\n";
            }

            if (equipment.hpAddi > 0)
            {
                tips += "[2820B8]+" + equipment.hpAddi + " 生命[-]\n";
            }

            if (equipment.poisonResist > 0)
            {
                tips += "[2820B8]+" + equipment.poisonResist + "% 抗毒[-]\n";
            }

            if (equipment.coldResist > 0)
            {
                tips += "[2820B8]+" + equipment.coldResist + "% 抗冰[-]\n";
            }

            if (equipment.lightningResist > 0)
            {
                tips += "[2820B8]+" + equipment.lightningResist + "% 抗闪电[-]\n";
            }

            if (equipment.fireResist > 0)
            {
                tips += "[2820B8]+" + equipment.fireResist + "% 抗火[-]\n";
            }

            if (equipment.setsID > 0 && Config.DataLoader.setsMaps.ContainsKey(equipment.setsID))
            {
                Config.Sets s = Config.DataLoader.setsMaps[equipment.setsID];

                if ((s.weaponL == 0 || s.weaponL == charactor.weaponL || s.weaponL == charactor.weaponR) &&
                    (s.weaponR == 0 || s.weaponR == charactor.weaponL || s.weaponR == charactor.weaponR) &&
                    (s.armor == 0 || s.armor == charactor.armor) &&
                    (s.headgear == 0 || s.headgear == charactor.headgear) &&
                    (s.gloves == 0 || s.gloves == charactor.gloves) &&
                    (s.amulets == 0 || s.amulets == charactor.amulets) &&
                    (s.belts == 0 || s.belts == charactor.belts) &&
                    (s.boots == 0 || s.boots == charactor.boots) &&
                    (s.ring == 0 || s.ring == charactor.ring))
                {
                    tips += "[000000] [-]\n";
                    if (s.hp > 0)
                    tips += "[00c400]+" + s.hp + " 生命[-]\n";
                    if (s.dex > 0)
                    tips += "[00c400]+" + s.dex + " 敏捷[-]\n";
                    if (s.str > 0)
                    tips += "[00c400]+" + s.str + " 力量[-]\n";
                    if (s.atk > 0)
                    tips += "[00c400]+" + s.atk + " 伤害[-]\n";
                    if (s.def > 0)
                    tips += "[00c400]+" + s.def + " 防御[-]\n";
                    if (s.lvlDef > 0)
                        tips += "[00c400]+" + (int)(s.lvlDef * charactor.level) + " 防御(由角色等级决定)[-]\n";
                    if (s.lvlAtk > 0)
                        tips += "[00c400]+" + (int)(s.lvlAtk * charactor.level) + " 伤害(由角色等级决定)[-]\n";
                    if (s.resist > 0)
                        tips += "[00c400]所有抗性 +" + s.resist + "[-]\n";
                }

                tips += "[000000] [-]\n";
                tips += "[908858]" + s.name + "[-]\n";

                if (s.headgear > 0)
                {
                    Config.Equipment eq = Config.DataLoader.equipmentMaps[s.headgear];

                    if ((depot && charactor.backpack.Contains(s.headgear)) || (!depot && s.headgear == charactor.headgear))
                        tips += "[00c400]" + eq.setsName + "[-]\n";
                    else
                        tips += "[c40000]" + eq.setsName + "[-]\n";
                }

                if (s.armor > 0)
                {
                    Config.Equipment eq = Config.DataLoader.equipmentMaps[s.armor];

                    if ((depot && charactor.backpack.Contains(s.armor)) || (!depot && s.armor == charactor.armor))
                        tips += "[00c400]" + eq.setsName + "[-]\n";
                    else
                        tips += "[c40000]" + eq.setsName + "[-]\n";
                }

                if (s.gloves > 0)
                {
                    Config.Equipment eq = Config.DataLoader.equipmentMaps[s.gloves];

                    if ((depot && charactor.backpack.Contains(s.gloves)) || (!depot && s.gloves == charactor.gloves))
                        tips += "[00c400]" + eq.setsName + "[-]\n";
                    else
                        tips += "[c40000]" + eq.setsName + "[-]\n";
                }

                if (s.boots > 0)
                {
                    Config.Equipment eq = Config.DataLoader.equipmentMaps[s.boots];

                    if ((depot && charactor.backpack.Contains(s.boots)) || (!depot && s.boots == charactor.boots))
                        tips += "[00c400]" + eq.setsName + "[-]\n";
                    else
                        tips += "[c40000]" + eq.setsName + "[-]\n";
                }

                if (s.belts > 0)
                {
                    Config.Equipment eq = Config.DataLoader.equipmentMaps[s.belts];

                    if ((depot && charactor.backpack.Contains(s.belts)) || (!depot && s.belts == charactor.belts))
                        tips += "[00c400]" + eq.setsName + "[-]\n";
                    else
                        tips += "[c40000]" + eq.setsName + "[-]\n";
                }

                if (s.amulets > 0)
                {
                    Config.Equipment eq = Config.DataLoader.equipmentMaps[s.amulets];

                    if ((depot && charactor.backpack.Contains(s.amulets)) || (!depot && s.amulets == charactor.amulets))
                        tips += "[00c400]" + eq.setsName + "[-]\n";
                    else
                        tips += "[c40000]" + eq.setsName + "[-]\n";
                }

                if (s.ring > 0)
                {
                    Config.Equipment eq = Config.DataLoader.equipmentMaps[s.ring];

                    if ((depot && charactor.backpack.Contains(s.ring)) || (!depot && (s.ring == charactor.ring)))
                        tips += "[00c400]" + eq.setsName + "[-]\n";
                    else
                        tips += "[c40000]" + eq.setsName + "[-]\n";
                }

                if (s.weaponR > 0)
                {
                    Config.Equipment eq = Config.DataLoader.equipmentMaps[s.weaponR];

                    if ((depot && charactor.backpack.Contains(s.weaponR)) ||
                        (!depot && ((s.weaponR == charactor.weaponL && s.weaponR > 0) || s.weaponR == charactor.weaponR)))
                        tips += "[00c400]" + eq.setsName + "[-]\n";
                    else
                        tips += "[c40000]" + eq.setsName + "[-]\n";
                }

                if (s.weaponL > 0)
                {
                    Config.Equipment eq = Config.DataLoader.equipmentMaps[s.weaponL];

                    if ((depot && charactor.backpack.Contains(s.weaponL)) ||
                        (!depot && (s.weaponL == charactor.weaponL || (s.weaponR == charactor.weaponR && s.weaponR > 0))))
                        tips += "[00c400]" + eq.setsName + "[-]\n";
                    else
                        tips += "[c40000]" + eq.setsName + "[-]\n";
                }
            }

            if (mine)
            {
                if (depot)
                {
                    tips += "[000000] [-]\n";
                    tips += "[FFFF00] 出售价格 " + Cost(equipment) + " \n";
                }
            }
        }

        return tips;
    }

    public static int Cost(Config.Equipment equipment)
    {
        return (equipment.qualityLevel + 1) * (equipment.qualityLevel + 13) + equipment.lvlRequest * 3;
    }

    public static string Profession(Config.Profession pro)
    {
        string str = "";

        if (pro == Config.Profession.Amazon)
            str = "亚马逊";
        else if (pro == Config.Profession.Assassin)
            str = "刺客";
        else if (pro == Config.Profession.Barbarian)
            str = "野蛮人";
        else if (pro == Config.Profession.Druid)
            str = "德鲁伊";
        else if (pro == Config.Profession.Necromancer)
            str = "死灵法师";
        else if (pro == Config.Profession.Paladin)
            str = "圣骑士";
        else if (pro == Config.Profession.Sorceress)
            str = "法师";

        return str;
    }

    // 排名匹配的角色
    public static List<Hero> RankHeros = new List<Hero>();
    // 生成排名角色
    public static void GenRankHeros()
    {
        RankHeros.Clear();

        ushort[] weaponL = { 1430, 1431, 1432, 1433, 1434 };
        ushort[] weaponR = { 1367, 1368, 1369, 1370, 1371 };
        ushort[] amulet = { 1001, 1002, 1003, 1004, 1005 };
        ushort[] ring = { 1347, 1348, 1349, 1350, 1351 };
        ushort[] headgear = { 1236, 1237, 1238, 1239, 1240 };
        ushort[] belt = { 1079, 1080, 1081, 1082, 1083 };
        ushort[] armor = { 1094, 1095, 1096, 1097, 1098 };
        ushort[] boot = { 1139, 1140, 1141, 1142, 1143 };
        ushort[] glove = { 1223, 1224, 1225, 1226, 1227 };
        for (uint i = 0; i < 5; i++)
        {
            int rank = (int)(Global.LocalHero.charactor.rank - 5 + i);
            if (rank <= 1)
                continue;

            Hero hero = new Hero();
            hero.charactor = new RemoteChar();
            hero.charactor.name = "我是电脑" + i;
            hero.charactor.level = (byte)(Global.LocalHero.charactor.level + 5 - i);
            hero.charactor.str = (ushort)(4 * hero.charactor.level);
            hero.charactor.dex = (ushort)(hero.charactor.level);
            hero.charactor.rank = rank;
            hero.charactor.profession = (Config.Profession)UnityEngine.Random.Range(1, (int)Config.Profession.Monster);

            hero.charactor.weaponL = weaponL[UnityEngine.Random.Range(0, weaponL.Length)];
            hero.charactor.weaponR = weaponR[UnityEngine.Random.Range(0, weaponR.Length)];
            hero.charactor.amulets = amulet[UnityEngine.Random.Range(0, amulet.Length)];
            hero.charactor.ring = ring[UnityEngine.Random.Range(0, ring.Length)];
            hero.charactor.headgear = headgear[UnityEngine.Random.Range(0, headgear.Length)];
            hero.charactor.belts = belt[UnityEngine.Random.Range(0, belt.Length)];
            hero.charactor.armor = armor[UnityEngine.Random.Range(0, armor.Length)];
            hero.charactor.boots = boot[UnityEngine.Random.Range(0, boot.Length)];
            hero.charactor.gloves = glove[UnityEngine.Random.Range(0, glove.Length)];

            RankHeros.Add(hero);
        }
    }
}

