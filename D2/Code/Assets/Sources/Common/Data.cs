using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public abstract class IData
    {
    }

    public class Sets : IData
    {
        public ushort ID;
        public string name = "";

        public ushort weaponL;
        public ushort weaponR;
        public ushort headgear;
        public ushort armor;
        public ushort gloves;
        public ushort boots;
        public ushort belts;
        public ushort ring;
        public ushort amulets;

        public ushort hp;
        public ushort def;
        public ushort atk;
        public ushort str;
        public ushort dex;
        public ushort resist;
        public float lvlDef;
        public float lvlAtk;
    }

    public class Equipment : IData
    {
        public enum Type : byte
        {
            Armor,
            Weapon,
            Belts,
            Boots,
            Headgear,
            Gloves,
            Ring,
            Amulets,
            
            Chest,
        }

        public enum Quality : byte
        {
            Poor,       // bebebe 190 190 190
            Normal,     // FFFFFF
            Magic,      // 4850B8 72 80 184
            Rare,       // ffff00 255 255 0
            Crafted,    // 908858 144 136 88
            Sets,       // 00c400 00ff00
        }

        public ushort ID;
        public string name = "";
        public string setsName = "";
        public ushort setsID;           // 套装ID
        public string nickname = "";
        public string icon = "";
        public Type type;
        public WeaponType weaponType;
        public Quality quality;
        public Profession profession;

        public ushort minDamage;
        public ushort maxDamage;
        public ushort minDefense;
        public ushort maxDefense;

        public ushort strRequest;
        public ushort dexRequest;
        public short durability;        // 装备耐久度
        public byte lvlRequest;
        public byte qualityLevel;

        public byte poisonResist;
        public byte coldResist;
        public byte lightningResist;
        public byte fireResist;

        public ushort damageAddi;		// 伤害加成

        public ushort strAddi;          // 力量加成
        public ushort dexAddi;          // 敏捷加成
        public ushort vitAddi;          // 体力加成
        public ushort engAddi;          // 精力加成
		public ushort hpAddi;			// 生命加成
		public ushort defAddi;			// 防御加成

        public byte costSpace;
    }


    public enum WeaponType
    {
        None,
        Sword,      // 剑
        Axes,       // 斧头
        Daggers,    // 匕首
        Maces,      // 锤
        Wand,       // 棍
        Polearms,   // 长柄武器(长斧、镰刀等）
        Spear,      // 矛、枪

        Scepters,   // 权杖
        Staves,     // 法杖

        Shields,    // 盾牌

        Bows,       // 弓
        Crossbows,  // 弩
    }


    public enum Profession
    {
        None,
        Amazon,
        Assassin,
        Barbarian,
        Druid,
        Necromancer,
        Paladin,
        Sorceress,

        Monster,
        Demons,
        Undead,
    }

    public class Scene : IData
    {
        public enum Difficulty
        {
            Normal,
            Nightmare,
            Hell,
        }

        public ushort ID;
        public string name = "";
        public string mod = "";
        
        public byte level_1;
        public byte level_2;
        public byte level_3;

        // 根据难度筛选怪物
        public string monster = "";
        
        //public List<byte> monster = new List<byte>();
        //public ushort awardExp;     // 奖励经验
        //public byte awardDot;       // 奖励技能点
    }

    public sealed class Char : IData
    {
        public enum Quality
        {
            Normal,
            Champion,       // mlvl +2
            Minion,         // mlvl +3
            Unique,         // mlvl +3
            Super,          // mlvl +4
            Boss,           // mlvl +5
        }

        public ushort ID;
        public string name = "";
        public string mod = "";

        public Quality quality;

        public byte actNum;
        public ushort scene;
            
        public byte level = 1;
        public ushort exp;
        public ushort gold;

        public int hp;
        public int damage;
        // 六种抗性未加 冰 火 毒 电 魔 物

        public byte phyxResist;
        public byte magicResist;
        public byte fireResist;
        public byte lightningResist;
        public byte coldResist;
        public byte poisonResist;
    }

    public class Magic : IData
    {
        public ushort ID;
        public string name;

        public byte lvlRequest;

        public ushort parentA;
        public ushort parentB;
        public ushort parentC;

        public ushort minDamageAddi;
        public ushort maxDamageAddi;

        public ushort incFireDamage;
        public ushort incLightningDamage;
        public ushort incColdDamage;
        public ushort incPoisonDamage;
    }

    public class Task : IData
    {
        public string name = "";
        public byte actNum;
        public ushort activeScene;
        public string content = "";
        public string passed = "";
    }


}
