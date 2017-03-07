using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public sealed class CharAttribute
    {
        public enum Type : byte
        {
            Str,
            Dex,
            Vit,
            Eng,
            HP,
            Stamina,
            Erg,
            Def,
            Atk,
            LvlDef,
            LvlAtk,
        }

        public static ushort[,] baseAttrMap = {  {20, 25, 20, 15, 50, 84, 15},
                                            {20, 20, 20, 25, 50, 95, 25},
                                            {30, 20, 25, 10, 55, 92, 10},
                                            {15, 20, 25, 20, 55, 84, 20},
                                            {15, 25, 15, 25, 45, 79, 25},
                                            {10, 25, 10, 35, 40, 74, 35},
                                            {25, 20, 25, 15, 55, 89, 15} };

        public static float[,] incAttrMap = { {2, 1, 1.5f},
                                         {2, 1.25f, 1.5f},
                                         {2, 1, 1},
                                         {1.5f, 1, 2},
                                         {1.5f, 1, 2},
                                         {1, 1, 2},
                                         {2, 1, 1.5f} };

        public static uint[] expAttrMap = { 500, 1000, 2250, 4125, 6300, 8505, 10206, 11510, 13319, 14429,
                                       18036, 22545, 28181, 35226, 44033, 55042, 68801, 86002, 107503, 134378,
                                       167973, 209966, 262457, 328072, 410090, 512612, 640765, 698434, 761293, 829810,
                                       904492, 985897, 1074627, 1171344,1276765,1391674,1516924,1653448,1802257,1964461,
                                       2141263, 2333976, 2544034, 2772997,3022566,3294598,3591112,3914311,4266600,4650593,
                                       5069147, 5525370, 6022654,6564692,7155515,7799511,8501467,9266598,10100593,11009646,
                                       12000515, 13080560, 14257811,15541015,16939705,18464279,20126064,21937409,23911777,26063836,
                                       28409582, 30966444, 33753424,36791232,40102443,43711663,47645713,51933826,56607872,61702579,
                                       67255812, 73308835, 79906630,87098226,94937067,103481403,112794729,122946255,134011418,146072446,
                                       159218965, 173548673, 189168053,206193177,224750564,244978115,267026144,291058498
                                     };

        

        public static ushort CaleAttr(RemoteChar charactor, Type type)
        {
            if ((int)type < (int)Type.HP)
                return (ushort)(baseAttrMap[(int)charactor.profession-1, (int)type] + charactor.level);

            return (ushort)(baseAttrMap[(int)charactor.profession - 1, (int)type] + charactor.level * incAttrMap[(int)charactor.profession - 1, (int)type - (int)Type.HP]);
        }

        public static uint CaleExp(int mineLevel, int enemyLevel, uint exp)
        {
            int subLvl = Mathf.Abs(mineLevel - enemyLevel);

            if (mineLevel < 25)
            {
                if (subLvl <= 5)
                    return exp;
                else if (subLvl <= 10)
                    return (uint)(exp * Mathf.Min(0.05f, (1 - subLvl / 10.0f)));
                else
                    return (uint)(exp * 0.05f);
            }
            else if (mineLevel >= 25 && mineLevel < 100)
            {
                if (mineLevel < enemyLevel)
                    return (uint)(exp * (mineLevel / enemyLevel));
                else if (subLvl <= 5)
                    return exp;
                else if (subLvl > 5 && subLvl < 10)
                    return (uint)(exp * Mathf.Min(0.05f, (1 - subLvl / 10.0f)));
                else
                    return (uint)(exp * 0.05f);
            }

            return 0;
        }

        // 计算当前等级
        public static byte CaleLevel(int currentLevel, ref uint currentExp, uint addExp)
        {
            currentExp += addExp;

            uint needExp = CaleNeedExp(currentLevel);
            if (currentExp >= needExp)
            {
                currentLevel = Mathf.Min(99, currentLevel+1);
                currentExp = 0;
            }

            return (byte)currentLevel;
        }

        public static uint CaleNeedExp(int currentLevel)
        {
            //uint needExp = uint.MaxValue;

            //for (int i = 0; i < currentLevel - 1; i++)
            //{
            //    uint needExp += expAttrMap[currentLevel - 1];
            //}

            return expAttrMap[currentLevel - 1];
        }

        public static int GetSetsInfo(RemoteChar charactor, ushort setsID, Config.CharAttribute.Type t)
        {
            if (setsID == 0 || !Config.DataLoader.setsMaps.ContainsKey(setsID))
                return 0;

            Config.Sets s = Config.DataLoader.setsMaps[setsID];
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
                if (t == Config.CharAttribute.Type.Str)
                    return s.str;
                else if (t == Config.CharAttribute.Type.Dex)
                    return s.dex;
                else if (t == Config.CharAttribute.Type.HP)
                    return s.hp;
                else if (t == Config.CharAttribute.Type.Def)
                    return s.def;
                else if (t == Config.CharAttribute.Type.Atk)
                    return s.atk;
                else if (t == Config.CharAttribute.Type.LvlDef)
                    return (int)(s.lvlDef * charactor.level);
                else if (t == Config.CharAttribute.Type.LvlAtk)
                    return (int)(s.lvlAtk * charactor.level);
            }

            return 0;
        }

        public static int GetAttrOfEquipment(RemoteChar charactor, Config.CharAttribute.Type type, ushort equip, ref List<int> setsArr)
        {
            int str = 0;
            if (Config.DataLoader.equipmentMaps.ContainsKey(equip))
            {
                Config.Equipment eq = Config.DataLoader.equipmentMaps[equip];

                if (type == Config.CharAttribute.Type.Str)
                    str += eq.strAddi;
                else if (type == Config.CharAttribute.Type.Dex)
                    str += eq.dexAddi;
                else if (type == Config.CharAttribute.Type.HP)
                    str += eq.hpAddi;
                else if (type == Config.CharAttribute.Type.Def)
                    str += (eq.minDefense + eq.maxDefense) / 2 + eq.defAddi;
                else if (type == Config.CharAttribute.Type.Atk)
                    str += (eq.minDamage + eq.maxDamage) / 2 + eq.damageAddi;

                if (!setsArr.Contains(eq.setsID))
                {
                    int setsValue = GetSetsInfo(charactor, eq.setsID, type);
                    if (setsValue > 0)
                    {
                        str += setsValue;
                        setsArr.Add(eq.setsID);
                    }
                }
            }
            //else
            //    Debug.Log("equipment not find " + equip);

            return str;
        }

        public static int DamagePhyx(RemoteChar charactor)
        {

                int atk = 0;
                List<int> setsArr = new List<int>();

                atk += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Atk, charactor.headgear, ref setsArr);
                atk += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Atk, charactor.armor, ref setsArr);
                atk += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Atk, charactor.belts, ref setsArr);
                atk += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Atk, charactor.boots, ref setsArr);
                atk += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Atk, charactor.gloves, ref setsArr);
                atk += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Atk, charactor.ring, ref setsArr);
                atk += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Atk, charactor.amulets, ref setsArr);

                int str = Str(charactor);
                int dex = Dex(charactor);
                int atk1 = GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Atk, charactor.weaponL, ref setsArr);
                if (DataLoader.equipmentMaps.ContainsKey(charactor.weaponL))
                {
                    Equipment weapon = DataLoader.equipmentMaps[charactor.weaponL];
                    if (weapon.weaponType == WeaponType.Bows || weapon.weaponType == WeaponType.Crossbows || 
                        weapon.weaponType == WeaponType.Spear || weapon.weaponType == WeaponType.Daggers)
                    {
                        atk1 = (int)(atk1 * (dex + 100) / 100.0f);
                    }
                    else if (weapon.weaponType == WeaponType.Axes || weapon.weaponType == WeaponType.Maces || 
                        weapon.weaponType == WeaponType.Polearms || weapon.weaponType == WeaponType.Sword)
                    {
                        atk1 = (int)(atk1 * (str + 100) / 100.0f);
                    }
                }

                int atk2 = GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Atk, charactor.weaponR, ref setsArr);
                if (DataLoader.equipmentMaps.ContainsKey(charactor.weaponR))
                {
                    Equipment weapon = DataLoader.equipmentMaps[charactor.weaponR];
                    if (weapon.weaponType == WeaponType.Bows || weapon.weaponType == WeaponType.Crossbows ||
                        weapon.weaponType == WeaponType.Spear || weapon.weaponType == WeaponType.Daggers)
                    {
                        atk2 = (int)(atk2 * (dex + 100) / 100.0f);
                    }
                    else if (weapon.weaponType == WeaponType.Axes || weapon.weaponType == WeaponType.Maces ||
                        weapon.weaponType == WeaponType.Polearms || weapon.weaponType == WeaponType.Sword)
                    {
                        atk2 = (int)(atk2 * (str + 100) / 100.0f);
                    }
                }

                return atk + atk1 + atk2 + str;
        }

        // 计算防御率
        public static int Defence(RemoteChar charactor)
        {
            int def = (int)(Dex(charactor) / 4);
                List<int> setsArr = new List<int>();

                def += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Def, charactor.headgear, ref setsArr);
                def += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Def, charactor.armor, ref setsArr);
                def += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Def, charactor.belts, ref setsArr);
                def += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Def, charactor.boots, ref setsArr);
                def += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Def, charactor.gloves, ref setsArr);
                def += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Def, charactor.ring, ref setsArr);
                def += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Def, charactor.weaponL, ref setsArr);
                def += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Def, charactor.weaponR, ref setsArr);
                def += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Def, charactor.amulets, ref setsArr);

                return def;
        }

        public static int StrOfEquipment(RemoteChar charactor)
        {
                int str = 0;
                List<int> setsArr = new List<int>();

                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Str, charactor.headgear, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Str, charactor.armor, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Str, charactor.belts, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Str, charactor.boots, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Str, charactor.gloves, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Str, charactor.ring, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Str, charactor.weaponL, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Str, charactor.weaponR, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Str, charactor.amulets, ref setsArr);

                return str;
        }

        public static int DexOfEquipment(RemoteChar charactor)
        {
                int str = 0;
                List<int> setsArr = new List<int>();

                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Dex, charactor.headgear, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Dex, charactor.armor, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Dex, charactor.belts, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Dex, charactor.boots, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Dex, charactor.gloves, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Dex, charactor.ring, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Dex, charactor.weaponL, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Dex, charactor.weaponR, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.Dex, charactor.amulets, ref setsArr);

                return str;
        }

        public static int HPOfEquipment(RemoteChar charactor)
        {
                int str = 0;
                List<int> setsArr = new List<int>();

                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.HP, charactor.headgear, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.HP, charactor.armor, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.HP, charactor.belts, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.HP, charactor.boots, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.HP, charactor.gloves, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.HP, charactor.ring, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.HP, charactor.weaponL, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.HP, charactor.weaponR, ref setsArr);
                str += GetAttrOfEquipment(charactor, Config.CharAttribute.Type.HP, charactor.amulets, ref setsArr);

                return str;
        }

        public static int Str(RemoteChar charactor)
        {
                int val = charactor.str + StrOfEquipment(charactor);
                return val + Config.CharAttribute.CaleAttr(charactor, Config.CharAttribute.Type.Str);
        }

        public static int Dex(RemoteChar charactor)
        {
            int val = charactor.dex + DexOfEquipment(charactor);
            return val + CaleAttr(charactor, Config.CharAttribute.Type.Dex);
        }

        public static int Vit(RemoteChar charactor)
        {
            return charactor.vit + CaleAttr(charactor, Config.CharAttribute.Type.Vit);
        }

        public static int Eng(RemoteChar charactor)
        {
            return charactor.eng + CaleAttr(charactor,Config.CharAttribute.Type.Eng);
        }

        public static int HP(RemoteChar charactor)
        {
            int h = CaleAttr(charactor,Config.CharAttribute.Type.HP);
                h += HPOfEquipment(charactor);

                if (charactor.profession == Config.Profession.Barbarian)
                    h += charactor.vit * 5;
                else if (charactor.profession == Config.Profession.Paladin ||
                         charactor.profession == Config.Profession.Amazon ||
                         charactor.profession == Config.Profession.Assassin)
                {
                    h += charactor.vit * 3;
                }
                else
                    h += charactor.vit * 2;

                return Vit(charactor) + h;
        }

        // 计算命中率 
        public static int CaleAttackRating(RemoteChar charactor)
        {
            return Dex(charactor) * 4 - 28;
        }

        // 计算最大金钱
        public static uint CaleMaxGold(uint level)
        {
            if (level < 10) return 50000;
            else if (level < 20) return 100000;
            else if (level < 30) return 150000;
            else if (level == 30) return 200000;
            else if (level == 31) return 800000;
            else
            {
                return 800000 + ((level - 30) / 2) * 50000;
            }
        }

        // 计算总得格挡几率
        public static float CaleBlocking(RemoteChar charactor)
        {
            int blocking = 33; // add eq pro

            ushort dex = CaleAttr(charactor,Type.Dex);
            // add eq pro
            
            return System.Math.Max((blocking / 100.0f * (dex - 15)) / (charactor.level * 2), 0.75f);
        }

        public static string ConvertDifficulty(Scene.Difficulty dif)
        {
            if (dif == Scene.Difficulty.Nightmare)
                return "噩梦难度";
            else if (dif == Scene.Difficulty.Hell)
                return "地狱难度";

            return "一般难度";
        }

        public static string ConvertCHNum(int n)
        {
            if (n == 1)
                return "一";
            else if (n == 2)
                return "二";
            else if (n == 3)
                return "三";
            else if (n == 4)
                return "四";
            else if (n == 5)
                return "五";
            else if (n == 6)
                return "六";
            else if (n == 7)
                return "七";
            else if (n == 8)
                return "八";
            else if (n == 9)
                return "九";

            return n.ToString();
        }
    }


}
