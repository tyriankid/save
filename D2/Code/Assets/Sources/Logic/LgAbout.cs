using UnityEngine;
using System.Collections;


public class LgAbout : MonoBehaviour
{
    private const string version = "版本：1.1.6b";
    private static bool showed = false;
    private static LgAbout handle = null;


    void Start()
    {
        handle = this;
    }

    void OnDestroy()
    {
        showed = false;
        handle = null;
    }

    void OnCancel()
    {
        GameObject.DestroyImmediate(gameObject);
    }

    public static void Hide()
    {
        handle.OnCancel();
    }

    public static void Show()//string text, UIWidget.Pivot pivot, bool okAndCancel)
    {
        GameObject o = GameObject.Instantiate(Resources.Load("About_UI")) as GameObject;

        Transform t = GameObject.Find("Camera").transform;
        o.transform.parent = t;

        o.transform.localPosition = Vector3.zero;
        o.transform.localScale = Vector3.one;

        // 设置内容
        UILabel lab = o.transform.FindChild("Root/Version").GetComponent<UILabel>();//.GetComponentInChildren<UILabel>();
        lab.text = version;
        //lab.text = text;
        //lab.pivot = pivot;

        showed = true;
    }

    public static bool Visible
    {
        get
        {
            return showed;
        }
    }
}