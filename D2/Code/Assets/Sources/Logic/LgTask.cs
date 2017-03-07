using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LgTask : MonoBehaviour
{
    private Transform tabNode;

    void Start()
    {
        tabNode = this.transform.FindChild("Camera/Anchor/Panel/Tab");

        for (int i = 0; i < 5; i++)
        {
            Transform node = transform.FindChild("Camera/Anchor/Panel/Tg" + (i + 1));
            node.gameObject.SetActive(Global.LocalHero.charactor.sceneID / 100 >= i + 1);

            UIToggle tog = node.GetComponent<UIToggle>();
            tog.value = Global.LocalHero.charactor.sceneID / 100 == i + 1;
        }
    }

    private void SelectTab(int tab)
    {
        List<Config.Task> tasks = Config.DataLoader.taskMaps.FindAll(delegate(Config.Task t) { return t.actNum == tab; });

        int i = 0;
        for (; i < tasks.Count; i++)
        {
            Transform t = tabNode.FindChild("T" + (i + 1));
            t.gameObject.SetActive(true);
            UIImageButton btn = t.GetComponent<UIImageButton>();

            int index = Config.DataLoader.taskMaps.IndexOf(tasks[i]) + 1;

            string act = index < 10 ? "0" + index : index.ToString();
            string ast = act;
            if (Global.LocalHero.charactor.sceneID < tasks[i].activeScene)
                ast += "2";
            else if (Global.LocalHero.charactor.sceneID == tasks[i].activeScene)
                ast += "1";
            else
                ast += "3";

            btn.normalSprite = "copyAni" + ast;
            btn.hoverSprite = btn.normalSprite;
            btn.pressedSprite = btn.normalSprite;

            UserData d = btn.gameObject.GetComponent<UserData>();
            if ( d == null)
                d = btn.gameObject.AddComponent<UserData>();
            d.data = tasks[i].name;

            t = btn.transform.FindChild("Background");
            UISprite sp = t.GetComponent<UISprite>();
            sp.spriteName = btn.normalSprite;
            sp.MakePixelPerfect();
        }

        for (; i < 6; i++)
        {
            Transform t = tabNode.FindChild("T" + (i + 1));
            t.gameObject.SetActive(false);
        }
    }

    public void OnToggle1()
    {
        Transform node = transform.FindChild("Camera/Anchor/Panel/Tg1");
        UIToggle tog = node.GetComponent<UIToggle>();
        if (!tog.value)
            return;

        SelectTab(1);
    }

    public void OnToggle2()
    {
        Transform node = transform.FindChild("Camera/Anchor/Panel/Tg2");
        UIToggle tog = node.GetComponent<UIToggle>();
        if (!tog.value)
            return;

        SelectTab(2);
    }

    public void OnToggle3()
    {
        Transform node = transform.FindChild("Camera/Anchor/Panel/Tg3");
        UIToggle tog = node.GetComponent<UIToggle>();
        if (!tog.value)
            return;

        SelectTab(3);
    }

    public void OnToggle4()
    {
        Transform node = transform.FindChild("Camera/Anchor/Panel/Tg4");
        UIToggle tog = node.GetComponent<UIToggle>();
        if (!tog.value)
            return;

        SelectTab(4);
    }

    public void OnToggle5()
    {
        Transform node = transform.FindChild("Camera/Anchor/Panel/Tg5");
        UIToggle tog = node.GetComponent<UIToggle>();
        if (!tog.value)
            return;

        SelectTab(5);
    }

    void OnSelectTaskBox(UIImageButton btn)
    {
        Transform t = btn.transform.FindChild("Sprite");
        t.gameObject.SetActive(true);

        for (int i = 1; i < 7; i++)
        {
            Transform tm = btn.transform.parent.FindChild("T" + i + "/Sprite");
            if (tm != t)
                tm.gameObject.SetActive(false);
        }
    }

    void OnSetContent(GameObject o)
    {
        UIImageButton btn = o.GetComponent<UIImageButton>();
        OnSelectTaskBox(btn);

        UserData d = o.transform.GetComponent<UserData>();

        Config.Task task = Config.DataLoader.taskMaps.Find(delegate(Config.Task t1) { return t1.name == d.data; });

        Transform t = transform.FindChild("Camera/Anchor/Panel/Text/Label");
        UILabel lab = t.GetComponent<UILabel>();

        t = transform.FindChild("Camera/Anchor/Panel/PassedButton");

        UserData dp = t.GetComponent<UserData>();
        if (dp == null)
            dp = t.gameObject.AddComponent<UserData>();
        dp.data = d.data;

        if (btn.normalSprite[btn.normalSprite.Length-1] == '3')
        {
            lab.text = "你在先前的游戏之中完成这个任务了。";
            t.gameObject.SetActive(true);
        }
        else
        {
            lab.text = task.name + "\n\n" + task.content;
            t.gameObject.SetActive(false);
        }
    }

    void OnClose()
    {
        Game.ChangeScene("Main");
    }

    void OnPassedInfo(GameObject o)
    {
        UserData d = o.transform.GetComponent<UserData>();

        Config.Task task = Config.DataLoader.taskMaps.Find(delegate(Config.Task t1) { return t1.name == d.data; });

        if (!string.IsNullOrEmpty(task.passed))
        {
            Transform t = transform.FindChild("Camera/Anchor/Panel/Text/Label");
            UILabel lab = t.GetComponent<UILabel>();

            lab.text = task.passed;

            TypewriterEffect te = t.GetComponent<TypewriterEffect>();
            te.UpdateText(lab);
        }
    }
}