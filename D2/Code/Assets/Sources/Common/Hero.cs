using System;
using System.Collections.Generic;


public class Hero
{
    public RemoteChar charactor = null;
    private Parse.ParseObject obj = null;

    public Hero(Parse.ParseObject obj)
    {
        charactor = new RemoteChar();
        this.obj = obj;

        Read();
    }

    public Hero(){}

    public void SaveAsyc(CompletedEventHandle callback)
    {
        if (obj == null)
            obj = new Parse.ParseObject("Roles");

        Write();
        obj.SaveAsync().ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                ParseHelper.Result = Result.Faulted;
                string error = t.Exception.ToString();
                if (error.Contains("api.parse.com; Host not found"))
                {
                    ParseHelper.Result = Result.Disconnect;
                }

                callback();
            }
            else if (t.IsCanceled)
            {
                ParseHelper.Result = Result.Canceled;
                callback();
            }
            else
            {
                ParseHelper.Result = Result.Completed;
                callback(obj);
            }
        });
    }

    public void SaveAsyc(string nextScene, bool background = false)
    {
        Write();
        ParseAgent.handle.UpdateRoleProperty(obj, nextScene, background);
    }

    private void Write()
    {
        obj["user"]         = charactor.user;
        obj["name"]         = charactor.name;
        obj["profession"]   = (byte)charactor.profession;
        obj["level"]        = charactor.level;
        obj["rank"]         = charactor.rank;

        obj["sceneID"]      = charactor.sceneID;
        obj["difficulty"]   = (byte)charactor.difficulty;
        obj["exp"]          = charactor.exp;
        obj["gold"]         = charactor.gold;

        obj["str"]          = charactor.str;
        obj["dex"]          = charactor.dex;
        obj["vit"]          = charactor.vit;
        obj["eng"]          = charactor.eng;

        obj["armor"]        = charactor.armor;
        obj["weaponL"]      = charactor.weaponL;
        obj["weaponR"]      = charactor.weaponR;
        obj["belts"]        = charactor.belts;

        obj["boots"]        = charactor.boots;
        obj["headgear"]     = charactor.headgear;
        obj["gloves"]       = charactor.gloves;
        obj["ring"]         = charactor.ring;

        obj["amulets"]      = charactor.amulets;
        obj["durability"]   = charactor.durability;
        obj["backpack"]     = charactor.backpack;
        obj["skilltree"]    = charactor.skilltree;
    }

    private void Read()
    {
        charactor.user          = obj.Get<string>("user");
        charactor.name          = obj.Get<string>("name");
        charactor.profession    = (Config.Profession)obj.Get<byte>("profession");
        charactor.level         = obj.Get<byte>("level");
        charactor.rank          = obj.Get<int>("rank");

        charactor.sceneID       = obj.Get<ushort>("sceneID");
        charactor.difficulty    = (Config.Scene.Difficulty)obj.Get<byte>("difficulty");
        charactor.exp           = obj.Get<uint>("exp");
        charactor.gold          = obj.Get<uint>("gold");

        charactor.str           = obj.Get<ushort>("str");
        charactor.dex           = obj.Get<ushort>("dex");
        charactor.vit           = obj.Get<ushort>("vit");
        charactor.eng           = obj.Get<ushort>("eng");

        charactor.armor         = obj.Get<ushort>("armor");
        charactor.weaponL       = obj.Get<ushort>("weaponL");
        charactor.weaponR       = obj.Get<ushort>("weaponR");
        charactor.belts         = obj.Get<ushort>("belts");

        charactor.boots         = obj.Get<ushort>("boots");
        charactor.headgear      = obj.Get<ushort>("headgear");
        charactor.gloves        = obj.Get<ushort>("gloves");
        charactor.ring          = obj.Get<ushort>("ring");

        charactor.amulets       = obj.Get<ushort>("amulets");
        charactor.durability    = obj.Get<short>("durability");

        IList<ushort> bp        = obj.Get<IList<ushort>>("backpack");

        charactor.backpack.AddRange(bp);

        IList<int> st           = obj.Get<IList<int>>("skilltree");
        charactor.skilltree.AddRange(st);
    }
}

public class RemoteChar : Config.IData
{
    public string user = "";
    public string name = "";            // 唯一
    public Config.Profession profession = Config.Profession.Amazon;// 职业

    public byte level = 1;              // 角色等级
    
    public int rank = 5000000;          // 排名

    public ushort sceneID;              // 通过的场景ID
    public Config.Scene.Difficulty difficulty = Config.Scene.Difficulty.Normal;

    public uint exp;                    // 经验
    public uint gold;                   // 金币

    public ushort ski;

    public ushort str;                  // 力量
    public ushort dex;                  // 敏捷
    public ushort vit;                  // 体力
    public ushort eng;                  // 精力

    public ushort armor;                // 铠甲
    public ushort weaponL;              // 左手武器
    public ushort weaponR;              // 右手武器
    public ushort belts;                // 腰带
    public ushort boots;                // 靴子
    public ushort headgear;             // 帽子
    public ushort gloves;               // 手套
    public ushort ring;                 // 戒指
    public ushort amulets;              // 护身符

    public short durability = 100;      // 装备总的耐久度

    // 背包
    public List<ushort> backpack = new List<ushort>();
    // 技能
    public List<int> skilltree = new List<int>();
}