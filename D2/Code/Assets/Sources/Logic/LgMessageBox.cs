using UnityEngine;
using System.Collections;


public class LgMessageBox : MonoBehaviour
{
    private static bool showed = false;
    private static LgMessageBox handle = null;

    public delegate void OK(params object[] args);
    private OK ok;
    private object[] args;


    void OnDestroy()
    {
        showed = false;
    }

    void OnCancel()
    {
        GameObject.DestroyImmediate(gameObject);
    }

    void OnOK()
    {
        ok(args);
    }

    public static void Hide()
    {
        handle.OnCancel();
    }

    public static void Show(string text, UIWidget.Pivot pivot, OK func, params object[] args)
    {
        GameObject o = GameObject.Instantiate(Resources.Load("Message_UI")) as GameObject;
        LgMessageBox lmb = o.GetComponent<LgMessageBox>();
        handle = lmb;

        Transform t = GameObject.Find("Camera").transform;
        o.transform.parent = t;

        o.transform.localPosition = Vector3.zero;
        o.transform.localScale = Vector3.one;

        // 设置内容
        UILabel lab = o.transform.FindChild("Root/Label").GetComponent<UILabel>();//.GetComponentInChildren<UILabel>();
        lab.text = text;
        lab.pivot = pivot;

        t = o.transform.FindChild("Root/OKButton");
        t.gameObject.SetActive(func != null);

        handle.ok = func;
        handle.args = args;
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