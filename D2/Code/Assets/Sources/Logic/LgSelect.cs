using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LgSelect : MonoBehaviour
{

    void Start()
    {        
        if (Global.SolePlayerMode)
        {
            Global.MyHeros.Clear();
            if (PlayerPrefs.HasKey("MyHeros"))
            {
                string hero = PlayerPrefs.GetString("MyHeros");
                try
                {
                    List<RemoteChar> heros = LitJson.JsonMapper.ToObject<List<RemoteChar>>(hero);
                    for (int i = 0; heros != null && i < heros.Count; i++)
                    {
                        Hero h = new Hero();
                        h.charactor = heros[i];
                        Global.MyHeros.Add(h);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning(ex.ToString());
                }
            }
        }

        for (int i = 0; i < Global.MyHeros.Count; i++)
        {
            string root = "Camera/Anchor/Panel/Player"+(i+1)+"/Root";

            Transform ts = transform.FindChild(root);
            ts.gameObject.SetActive(true);

            ts = transform.FindChild(root + "/Level");
            UILabel lab = ts.GetComponent<UILabel>();
            lab.text = "等级:" + Global.MyHeros[i].charactor.level; //

            ts = transform.FindChild(root + "/Name");
            lab = ts.GetComponent<UILabel>();
            lab.text = Global.MyHeros[i].charactor.name;

            ts = transform.FindChild(root + "/Prof");
            lab = ts.GetComponent<UILabel>();
            lab.text = "职业:" + Global.Profession(Global.MyHeros[i].charactor.profession);

            ts = transform.FindChild(root + "/Rank");
            lab = ts.GetComponent<UILabel>();
            lab.text = "排名:[EAEE00]" + Global.MyHeros[i].charactor.rank + "[-]";

            ts = transform.FindChild(root + "/Player");
            UISprite spr = ts.GetComponent<UISprite>();
            spr.spriteName = Global.CharIcon[(int)Global.MyHeros[i].charactor.profession];
        }
    }

    void OnCreateChar()
    {
        if (Global.MyHeros.Count >= 5)
        {
            LgMessageBox.Show("只能创建五名角色", UIWidget.Pivot.Center, null);
            return;
        }

        Game.ChangeScene("SpawnChar");
    }

    void OnDeleteChar()
    {
        for (int i = 0; i < Global.MyHeros.Count; i++)
        {
            string root = "Camera/Anchor/Panel/Player" + (i + 1) + "/Root/Toggle";

            Transform ts = transform.FindChild(root);
            UIToggle tog = ts.GetComponent<UIToggle>();
            if (tog.value)
            {
                LgMessageBox.Show("是否删除角色【" + Global.MyHeros[i].charactor.name + "】", UIWidget.Pivot.Center, OnOK, i);
                return;
            }
        }

        LgMessageBox.Show("请选择一名要删除的角色", UIWidget.Pivot.Center, null);
    }

    void OnOK(params object[] args)
    {
        int i = (int)args[0];
        string root = "Camera/Anchor/Panel/Player" + (i + 1) + "/Root";

        Transform ts = transform.FindChild(root);
        ts.gameObject.SetActive(false);

        if (Global.SolePlayerMode)
        {            
            ParseAgent.StorageToLocal();
        }
        else
        {
            ParseAgent.handle.DeleteRole(Global.MyHeros[i].charactor.name);
        }

        Global.MyHeros.RemoveAt(i);

        LgMessageBox.Hide();
    }

    void OnSelectPlayer(GameObject arg)
    {
        string str = arg.transform.parent.parent.name;
        int index = int.Parse(str.Replace("Player", ""));

        Global.LocalHero = Global.MyHeros[index-1];

        Game.ChangeScene("Main");
    }
}