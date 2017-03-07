using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LgFighting : MonoBehaviour
{
    private List<Config.Char> monsters = new List<Config.Char>();
    private List<Config.Equipment> normalEquipment = new List<Config.Equipment>();

    private UILabel lvlLab;
    private UISlider mHpProgress;
    private UILabel nameLab;
    private UISprite iconTex;
    private UITextList content;
    private UISlider expProgress;
    private UISlider pHpProgress;
    private UILabel sumGoldLab;

    private float tarHpPerc = 1;
    public float progressSpeed = 1;
    private int sumGold;
    private ItemManager depot = new ItemManager();
    private UIAtlas atlasRoot;

    private float accFightingSpeed = 0.8f;

    public AudioClip womenDeathAudio;
    public AudioClip manDeathAudio;
    public AudioClip monsterDeathAudio;


    void Start()
    {
        depot.Start();

        atlasRoot = transform.FindChild("Camera/Anchor/Panel/Tile").GetComponent<UISprite>().atlas;

        for (int i = 0; i < Global.LocalHero.charactor.backpack.Count; i++)
        {
            Config.Equipment eq = Config.DataLoader.equipmentMaps[Global.LocalHero.charactor.backpack[i]];

            if (eq != null && eq.costSpace == 0)
                eq.costSpace = LgDepot.GetAbjustTile(eq.icon, atlasRoot);

            if (!depot.AddItem(eq))
            {
                Debug.Log("start 空间不足");
            }
        }

        GetNormalEquipment();

        Transform t = transform.FindChild("Camera/Anchor/Panel/Battle/AccBtn");
        t.gameObject.SetActive(Global.LocalHero.charactor.gold >= 50);
    }

    void Update()
    {
        if (mHpProgress != null && tarHpPerc < mHpProgress.value)
        {
            mHpProgress.value -= Time.deltaTime * progressSpeed;
            if (mHpProgress.value < tarHpPerc)
                mHpProgress.value = tarHpPerc;
        }
    }

    void OnClose()
    {
        Game.ChangeScene("GameCopy");
    }

    public void OnAnimFinish()
    {
        Transform trans = transform.FindChild("Camera/Anchor/Panel/Splash/Node");

        UISpriteAnimation anim = trans.GetComponent<UISpriteAnimation>();
        anim.enabled = true;

        GameObject.Destroy(trans.parent.gameObject, 1.6f);

        Invoke("OnStart", 1.7f);
    }

    void OnStart()
    {
        StartCoroutine(Game.PlaySound<Battle>());

        Transform trans = transform.FindChild("Camera/Anchor/Panel/Battle");
        trans.gameObject.SetActive(true);

        Config.Scene scene = Config.DataLoader.sceneMaps.Find(delegate(Config.Scene s) { return s.ID == Global.SelectBattle; });
        if (scene == null)
        {
            Debug.Log("not find scene " + Global.SelectBattle);
            return;
        }

        trans = transform.FindChild("Camera/Anchor/Panel/Battle/SenceName");
        UILabel lab = trans.GetComponent<UILabel>();
        lab.text = "当前场景: " + scene.name;

        /////////////////////////////////////////////////////////////
        int slvl = 1;
        if (Global.LocalHero.charactor.difficulty == Config.Scene.Difficulty.Normal)
            slvl = scene.level_1;
        else if (Global.LocalHero.charactor.difficulty == Config.Scene.Difficulty.Nightmare)
            slvl = scene.level_2;
        else if (Global.LocalHero.charactor.difficulty == Config.Scene.Difficulty.Hell)
            slvl = scene.level_3;

        List<ushort> ms = SearchMonster();

        for (ushort i = 0; i < ms.Count; i++)
        {
            Config.Char c = Config.DataLoader.monsterMaps[ms[i]];
            c.level = (byte)(slvl + 1 + c.quality);

            c.gold = (ushort)(Random.Range(1, c.level * 2 + 5));
            c.exp = (ushort)(c.level * 13 + Random.Range(3, c.level * 7 + 5));

            c.hp = (int)c.level * 93;
            c.damage = (int)(c.level * 3.1f);

            monsters.Add(c);
        }

        InitUIComponent();
        expProgress.value = Global.LocalHero.charactor.exp / (float)Config.CharAttribute.CaleNeedExp(Global.LocalHero.charactor.level);
        pHpProgress.value = 1;
        //
        StartCoroutine(OnFighting(slvl));
    }

    IEnumerator OnFighting(int slvl)
    {
        //uint ar = Config.CharAttribute.CaleAttackRating(Global.LocalChar);
        int dr = (int)Config.CharAttribute.Defence(Global.LocalHero.charactor);

        int damage = Config.CharAttribute.DamagePhyx(Global.LocalHero.charactor);
        int maxHp = Config.CharAttribute.HP(Global.LocalHero.charactor);
        int hp = maxHp;

        for (ushort i = 0; i < monsters.Count; i++)
        {
            if (Random.Range(0, 100) > 68 && monsters[i].quality < Config.Char.Quality.Unique)
                continue;

            float mMaxHp = monsters[i].hp;

            UpdateUI(monsters[i].name, monsters[i].mod, monsters[i].level, 1, monsters[i].quality, true);
            yield return new WaitForSeconds(accFightingSpeed);

            while (monsters[i].hp > 0)
            {
                if (Random.Range(0, dr) <= dr / 7)
                    hp -= monsters[i].damage;
                else
                    hp -= (monsters[i].damage + dr) / dr;

                if (Random.Range(0, 100) < 9)
                    Global.LocalHero.charactor.durability--;

                pHpProgress.value = (float)hp / maxHp;
                if (hp <= 0)
                {
                    content.Add("[FF0000]你掛了，被[-][F00000]" + monsters[i].name + "[-][FF0000]杀死，损失金钱" + (int)(sumGold * 0.8f) + "[-]");
                    break;
                }

                monsters[i].hp -= damage;
                if (monsters[i].hp <= 0)
                {
                    hp += (int)(maxHp * 0.6f);
                    hp = Mathf.Min(hp, maxHp);
                    pHpProgress.value = (float)hp / maxHp;

                    sumGold += monsters[i].gold;

                    byte oldLevel = Global.LocalHero.charactor.level;
                    uint exp = Config.CharAttribute.CaleExp(oldLevel, monsters[i].level, monsters[i].exp);
                    Global.LocalHero.charactor.level = Config.CharAttribute.CaleLevel(oldLevel, ref Global.LocalHero.charactor.exp, exp);

                    if (Global.LocalHero.charactor.level > oldLevel)
                    {
                        // 升级
                        content.Add("[00FF00]等级提升 当前等级" + Global.LocalHero.charactor.level + "[-]");
                    }

                    // 掉落物品
                    OnThrow(monsters[i].quality, slvl);

                    content.Add("[FF0000]杀死 " + monsters[i].name+"[-]");
                    content.Add("获得经验 " + exp);
                    content.Add("获得金钱 " + monsters[i].gold);
                    content.Add(" ");

                    sumGoldLab.text = "累计获得金钱: "+sumGold.ToString();
                    expProgress.value = Global.LocalHero.charactor.exp / (float)Config.CharAttribute.CaleNeedExp(Global.LocalHero.charactor.level);
                }

                UpdateUI(monsters[i].name, monsters[i].mod, monsters[i].level, monsters[i].hp / mMaxHp, monsters[i].quality, false);

                yield return new WaitForSeconds(accFightingSpeed);
            }

            if (hp <= 0)
                break;
        }

        OnEnd(hp > 0);
    }

    void OnEnd(bool win)
    {
        if (win)
        {
            Global.LocalHero.charactor.gold += (uint)sumGold;
            if (Global.SelectBattle > Global.LocalHero.charactor.sceneID)
                Global.LocalHero.charactor.sceneID = Global.SelectBattle;

            content.Add("[FFFF00]恭喜，成功杀死该场景Boss " + monsters[monsters.Count - 1].name + "[-]");

            NGUITools.PlaySound(monsterDeathAudio);
        }
        else
        {
            Global.LocalHero.charactor.gold += (uint)(sumGold * 0.2f);

            switch (Global.LocalHero.charactor.profession)
            {
                case Config.Profession.Amazon:
                case Config.Profession.Assassin:
                case Config.Profession.Sorceress:
                    NGUITools.PlaySound(womenDeathAudio);
                    break;
                case Config.Profession.Barbarian:
                case Config.Profession.Druid:
                case Config.Profession.Necromancer:
                case Config.Profession.Paladin:
                    NGUITools.PlaySound(manDeathAudio);
                    break;
            }
        }

        if (Global.SolePlayerMode)
        {
            ParseAgent.StorageToLocal();
            Game.ChangeScene("GameCopy", 3);
        }
        else
        {
            Global.LocalHero.SaveAsyc("GameCopy");
        }

        monsters.Clear();
    }

    void OnSetFightingAcc()
    {
        if (Global.LocalHero.charactor.gold > 90)
        {
            accFightingSpeed *= 0.5f;
            if (accFightingSpeed < 0.3)
            {
                Transform t = transform.FindChild("Camera/Anchor/Panel/Battle/AccBtn");
                t.gameObject.SetActive(false);
            }

            Global.LocalHero.charactor.gold -= 90;
            if (Global.SolePlayerMode)
            {
                ParseAgent.StorageToLocal();
            }
        }
    }

    void GetNormalEquipment()
    {
        normalEquipment.Clear();
        foreach (Config.Equipment eq in Config.DataLoader.equipmentMaps.Values)
        {
            if (eq.quality == Config.Equipment.Quality.Normal && eq.lvlRequest > Global.LocalHero.charactor.level + 3)
            {
                normalEquipment.Add(eq);
            }
        }
    }

    void OnThrow(Config.Char.Quality quality, int level)
    {
        Config.Equipment eq = null;
        bool taken = false;
        ushort id = (ushort)Random.Range(1001, 1001 + Config.DataLoader.equipmentMaps.Count);
        if (Config.DataLoader.equipmentMaps.ContainsKey(id))
        {
            eq = Config.DataLoader.equipmentMaps[id];
            taken = (byte)eq.quality <= (byte)quality && eq.lvlRequest < level + 23;
        }

        if (!taken && quality > Config.Char.Quality.Champion)
        {
            //int index = Random.Range(0, normalEquipment.Count + 8);
            //taken = index < normalEquipment.Count;
            //if (taken)
            //    eq = normalEquipment[index];
            eq = Config.DataLoader.equipmentMaps[1563];
            taken = true;
        }

        if (taken)
        {
            if (eq != null && eq.costSpace == 0)
                eq.costSpace = LgDepot.GetAbjustTile(eq.icon, atlasRoot);

            if (depot.AddItem(eq))
            {
                content.Add("获得物品 " + Global.ConvertToColor(eq.quality) + eq.name + "[-]");
                Global.LocalHero.charactor.backpack.Add(eq.ID);
            }
            else
            {
                content.Add("[FF0000]仓库已满，无法捡起物品[-] " + Global.ConvertToColor(eq.quality) + eq.name + "[-]");
            }
        }
    }

    List<ushort> SearchMonster()
    {
        ushort boss = 0;
        List<ushort> ms = new List<ushort>();
        foreach (Config.Char c in Config.DataLoader.monsterMaps.Values)
        {
            if (Global.SelectBattle / 100 == c.actNum)
            {
                if (c.scene == 0)
                    ms.Add(c.ID);
                else if (c.scene == Global.SelectBattle)
                    boss = c.ID;
            }
        }

        if (boss != 0)
            ms.Add(boss);

        return ms;
    }

    void UpdateUI(string name, string icon, int level, float hp, Config.Char.Quality quality, bool update)
    {
        
        if (update)
        {
            nameLab.text = name;
            if (quality == Config.Char.Quality.Normal)
                nameLab.color = Color.white;
            else if (quality == Config.Char.Quality.Champion)
                nameLab.color = new Color(72.0f/255, 80.0f/255, 184.0f/255);
            else if (quality == Config.Char.Quality.Unique)
                nameLab.color = new Color(1, 1, 0);
            else if (quality == Config.Char.Quality.Super)
                nameLab.color = new Color(144.0f / 255, 136.0f / 255, 88.0f / 255);
            else if (quality == Config.Char.Quality.Boss)
                nameLab.color = Color.yellow;

            lvlLab.text = "等级: " + level;
            iconTex.spriteName = icon + ".gif";
            iconTex.transform.localScale = new Vector2(iconTex.GetAtlasSprite().width / (float)iconTex.GetAtlasSprite().height, 1);

            mHpProgress.value = hp;
        }

        tarHpPerc = hp;
    }

    private void InitUIComponent()
    {
        if (nameLab == null)
        {
            string root = "Camera/Anchor/Panel/Battle/MonsterInfo/";
            Transform node = transform.FindChild(root + "MName");
            nameLab = node.GetComponent<UILabel>();

            node = transform.FindChild(root + "MLevel");
            lvlLab = node.GetComponent<UILabel>();

            node = transform.FindChild(root + "HPBar");
            mHpProgress = node.GetComponent<UISlider>();

            node = transform.FindChild(root + "Monster");
            iconTex = node.GetComponent<UISprite>();

            node = transform.FindChild("Camera/Anchor/Panel/Battle/FightingMsg");
            content = node.GetComponent<UITextList>();

            node = transform.FindChild("Camera/Anchor/Panel/Battle/ExpBar");
            expProgress = node.GetComponent<UISlider>();

            node = transform.FindChild("Camera/Anchor/Panel/Battle/HpBar");
            pHpProgress = node.GetComponent<UISlider>();

            node = transform.FindChild("Camera/Anchor/Panel/Battle/SumGold");
            sumGoldLab = node.GetComponent<UILabel>();
        }
    }
}