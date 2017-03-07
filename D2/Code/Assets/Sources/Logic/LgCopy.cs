using UnityEngine;
using System.Collections;


public class LgCopy : MonoBehaviour
{
    public GameObject sceneCard;
    private UIDraggablePanel scrollView;

    void Start()
    {
        Transform actNode = transform.FindChild("Camera/Panel");
        scrollView = actNode.GetComponent<UIDraggablePanel>();

        Transform node = transform.FindChild("Camera/Anchor/Panel/Difficulty");
        UILabel difLab = node.GetComponent<UILabel>();
        difLab.text = Config.CharAttribute.ConvertDifficulty(Global.LocalHero.charactor.difficulty);

        int sceneID = Global.LocalHero.charactor.sceneID;
        if (sceneID == 0)
            sceneID = Global.LocalHero.charactor.sceneID = 101;

        for (int i = 0; i < 5; i++)
        {
            node = transform.FindChild("Camera/Anchor/Panel/Tg" + (i + 1));
            node.gameObject.SetActive(sceneID / 100 >= i + 1);

            UIToggle tog = node.GetComponent<UIToggle>();
            tog.value = (Global.SelectBattle > 0 ? Global.SelectBattle : sceneID) / 100 == i + 1;
        }

        sceneCard.SetActive(false);
    }

    private void SelectActTab(int act)
    {
        int n = 0;
        for (int i = 0; i < Config.DataLoader.sceneMaps.Count; i++)
        {
            Config.Scene s = Config.DataLoader.sceneMaps[i];

            if (s.ID % 100 != 0 && act == s.ID / 100)
            {
                if (n == 0)
                {
                    Config.Scene yd = Config.DataLoader.sceneMaps.Find(delegate(Config.Scene sc) { return sc.ID == act * 100; });

                    Transform actNode = transform.FindChild("Camera/Anchor/Panel/ActNum");
                    UILabel actLab = actNode.GetComponent<UILabel>();
                    actLab.text = "第" + Config.CharAttribute.ConvertCHNum(act) + "幕 " + yd.name;
                }

                Transform tm = sceneCard.transform.parent.FindChild("Scene" + (n + 1));
                if (tm == null)
                {
                    GameObject o = NGUITools.AddChild(sceneCard.transform.parent.gameObject, sceneCard);
                    o.name = "Scene" + (n + 1);
                    UserData d = o.AddComponent<UserData>();
                    d.data = s.ID.ToString();
                    tm = o.transform;
                }
                else
                {
                    UserData d = tm.GetComponent<UserData>();
                    d.data = s.ID.ToString();
                }
                tm.gameObject.SetActive(true);

                float y = sceneCard.transform.localPosition.y - n * 70;
                tm.localPosition = new Vector3(0, y, 0);

                Transform trans = tm.FindChild("Root/Icon");
                UISprite icon = trans.GetComponent<UISprite>();
                icon.spriteName = s.mod + ".gif";

                trans = tm.FindChild("Root/Name");
                UILabel lab = trans.GetComponent<UILabel>();
                lab.text = s.name;

                trans = tm.FindChild("Root/Level");
                lab = trans.GetComponent<UILabel>();
                if (Global.LocalHero.charactor.difficulty == Config.Scene.Difficulty.Normal)
                    lab.text = "等级:" + s.level_1.ToString();
                else if (Global.LocalHero.charactor.difficulty == Config.Scene.Difficulty.Nightmare)
                    lab.text = "等级:" + s.level_2.ToString();
                else if (Global.LocalHero.charactor.difficulty == Config.Scene.Difficulty.Hell)
                    lab.text = "等级:" + s.level_3.ToString();

                trans = tm.FindChild("Root/EnterButton");
                if (s.ID > Global.LocalHero.charactor.sceneID)
                {
                    lab.color = Color.red;
                    trans.gameObject.SetActive(false);
                }
                else
                {
                    lab.color = Color.white;
                    trans.gameObject.SetActive(true);
                }

                n++;
            }
        }

        for (; n < 25; n++)
        {
            Transform tm = sceneCard.transform.parent.FindChild("Scene" + (n + 1));
            if (tm != null)
                GameObject.DestroyImmediate(tm.gameObject);
        }
    }

    void OnCancel()
    {
        Game.ChangeScene("Main");
    }

    void OnEnterCopy(GameObject o)
    {
        UserData d = o.transform.parent.parent.GetComponent<UserData>();
        Global.SelectBattle = ushort.Parse(d.data);

        Game.ChangeScene("Battle", 0, false);
    }

    public void OnChangeAct1()
    {
        Transform node = transform.FindChild("Camera/Anchor/Panel/Tg1");
        UIToggle tog = node.GetComponent<UIToggle>();
        if (!tog.value)
            return;

        SelectActTab(1);

        scrollView.ResetPosition();
    }

    public void OnChangeAct2()
    {
        Transform node = transform.FindChild("Camera/Anchor/Panel/Tg2");
        UIToggle tog = node.GetComponent<UIToggle>();
        if (!tog.value)
            return;

        SelectActTab(2);

        scrollView.ResetPosition();
    }

    public void OnChangeAct3()
    {
        Transform node = transform.FindChild("Camera/Anchor/Panel/Tg3");
        UIToggle tog = node.GetComponent<UIToggle>();
        if (!tog.value)
            return;

        SelectActTab(3);

        scrollView.ResetPosition();
    }

    public void OnChangeAct4()
    {
        Transform node = transform.FindChild("Camera/Anchor/Panel/Tg4");
        UIToggle tog = node.GetComponent<UIToggle>();
        if (!tog.value)
            return;

        SelectActTab(4);

        scrollView.ResetPosition();
    }

    public void OnChangeAct5()
    {
        Transform node = transform.FindChild("Camera/Anchor/Panel/Tg5");
        UIToggle tog = node.GetComponent<UIToggle>();
        if (!tog.value)
            return;

        SelectActTab(5);

        scrollView.ResetPosition();
    }
}