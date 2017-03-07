using UnityEngine;
using System.Collections;


public class LgDarePvP : MonoBehaviour
{
    public float progressSpeed = 1;
    public AudioClip womenDeathAudio;
    public AudioClip manDeathAudio;
    public AudioClip winAudio;

    private UISlider tarHpProgress;
    private UISlider myHpProgress;

    private string[] roleAnims = { "", "ama/ama_01", "ass/ass_01", "bar/bar_01", "dru/Werebear[01]", "nec/nec_ani[01]", "pal/paladin_ani[01]", "sor/sorcerer_ani[01]"};
    private string roundHost = "";

    private UISpriteAnimation iconAnim;
    private UISpriteAnimation tarIconAnim;


    void Start()
    {
        UISprite sp = transform.FindChild("Camera/Anchor/Panel/Battle/MyInfo/Icon").GetComponent<UISprite>();
        sp.spriteName = roleAnims[(int)Global.LocalHero.charactor.profession];
        iconAnim = sp.transform.GetComponent<UISpriteAnimation>();
        iconAnim.namePrefix = sp.spriteName.Split('/')[0];

        sp = transform.FindChild("Camera/Anchor/Panel/Battle/TargetInfo/Icon").GetComponent<UISprite>();
        sp.spriteName = roleAnims[(int)Global.OtherHero.charactor.profession];
        tarIconAnim = sp.transform.GetComponent<UISpriteAnimation>();
        tarIconAnim.namePrefix = sp.spriteName.Split('/')[0];

        roundHost = Global.LocalHero.charactor.name;
    }

    void OnClose()
    {
        Game.ChangeScene("Rank");
    }

    public void OnAnimFinish()
    {
        Transform trans = transform.FindChild("Camera/Anchor/Panel/Splash/Node");

        UISpriteAnimation anim = trans.GetComponent<UISpriteAnimation>();
        anim.enabled = true;

        GameObject.Destroy(trans.parent.gameObject, 1.6f);

        Invoke("OnStart", 1.6f);
    }

    void OnStart()
    {
        Game.PlaySound<Battle>();

        Transform trans = transform.FindChild("Camera/Anchor/Panel/Battle");
        trans.gameObject.SetActive(true);

        /////////////////////////////////////////////////////////////
        InitUIComponent();

        //
        StartCoroutine(OnFighting());
    }

    IEnumerator OnFighting()
    {
        int atkRating = Config.CharAttribute.CaleAttackRating(Global.LocalHero.charactor);
        int defence = (int)Config.CharAttribute.Defence(Global.LocalHero.charactor);
        int damage = Config.CharAttribute.DamagePhyx(Global.LocalHero.charactor);
        int maxHp = Config.CharAttribute.HP(Global.LocalHero.charactor);
        int hp = maxHp;

        int tarAtkRating = Config.CharAttribute.CaleAttackRating(Global.OtherHero.charactor);
        int tarDefence = (int)Config.CharAttribute.Defence(Global.OtherHero.charactor);
        int tarDamage = Config.CharAttribute.DamagePhyx(Global.OtherHero.charactor);
        int tarMaxHp = Config.CharAttribute.HP(Global.OtherHero.charactor);
        int tarHp = tarMaxHp;

        while (tarHp > 0 && hp > 0)
        {
            yield return new WaitForSeconds(0.8f);

            if (roundHost == Global.LocalHero.charactor.name)
            {
                float rating = (atkRating - tarDefence * 2.2f) / (atkRating + tarDefence * 2.2f) * 0.15f + 0.85f;
                if (Random.Range(0, 1.0f) < rating)
                {
                    tarHp -= (int)(damage * (1 - (float)tarDefence / (damage + tarDefence)) * (0.6f * (1- 1.0f / Global.LocalHero.charactor.level)));
                }
                roundHost = Global.OtherHero.charactor.name;
            }
            else
            {
                float rating = (tarAtkRating - defence * 2.2f) / (tarAtkRating + defence * 2.2f) * 0.15f + 0.85f;
                if (Random.Range(0, 1.0f) < rating)
                {
                    hp -= (int)(tarDamage * (1 - (float)defence / (tarDamage + defence)) * (0.6f * (1 - 1.0f / Global.LocalHero.charactor.level)));
                }
                roundHost = Global.LocalHero.charactor.name;
            }

            tarHpProgress.value = (float)tarHp / tarMaxHp;
            myHpProgress.value = (float)hp / maxHp;
            if (tarHp <= 0 && hp <= 0)
            {
                break;
            }
        }

        OnEnd(hp > 0);
    }

    void OnEnd(bool win)
    {
        iconAnim.enabled = false;
        tarIconAnim.enabled = false;

        Transform trans = transform.FindChild("Camera/Anchor/Panel/Battle/Mark");
        trans.gameObject.SetActive(true);
        UILabel mark = trans.FindChild("Label").GetComponent<UILabel>();
        mark.text = win ? "胜  \n  利" : "失  \n  败";

        mark = trans.FindChild("Rank").GetComponent<UILabel>();
        mark.text = win ? "排名提升：[00EE00]" + (Global.OtherHero.charactor.rank - Global.LocalHero.charactor.rank) : "再接再厉";


        if (win)
        {
            NGUITools.PlaySound(winAudio);

            int myRank = Global.LocalHero.charactor.rank;
            Global.LocalHero.charactor.rank = Global.OtherHero.charactor.rank;
            Global.OtherHero.charactor.rank = myRank;

            if (Global.SolePlayerMode)
            {
                ParseAgent.StorageToLocal();

                Game.ChangeScene("Rank", 5);
            }
            else
            {                
                Global.LocalHero.SaveAsyc("Rank");
                Global.OtherHero.SaveAsyc("", true);
            }
        }
        else
        {
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

            Game.ChangeScene("Rank", 5);
        }
    }

    private void InitUIComponent()
    {
        string root = "Camera/Anchor/Panel/Battle/TargetInfo/";
        SetCharProperty(Global.OtherHero.charactor, root);

        Transform node = transform.FindChild(root + "HPBar");
        tarHpProgress = node.GetComponent<UISlider>();

        root = "Camera/Anchor/Panel/Battle/MyInfo/";
        SetCharProperty(Global.LocalHero.charactor, root);

        node = transform.FindChild(root + "HPBar");
        myHpProgress = node.GetComponent<UISlider>();
    }

    private void SetCharProperty(RemoteChar charactor, string root)
    {
        Transform node = transform.FindChild(root + "Name");
        UILabel nameLab = node.GetComponent<UILabel>();
        nameLab.text = charactor.name;

        node = transform.FindChild(root + "Level");
        UILabel lvlLab = node.GetComponent<UILabel>();
        lvlLab.text = "等级: "+charactor.level.ToString();

        node = transform.FindChild(root + "Prof");
        UILabel proLab = node.GetComponent<UILabel>();
        proLab.text = "职业: "+Global.Profession(charactor.profession);
    }
}