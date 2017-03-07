using UnityEngine;
using System.Collections;


public class LgLoading : MonoBehaviour
{
    private static LgLoading handle = null;
    private UILabel label;

    public static void Hide()
    {
        //GameObject.DestroyImmediate(handle.gameObject);
        //handle = null;
        handle.gameObject.SetActive(false);
    }

    public static void Show(string text = "")
    {
        if (handle == null)
        {
            GameObject o = GameObject.Instantiate(Resources.Load("Loading_UI")) as GameObject;
            handle = o.GetComponent<LgLoading>();

            //Transform t = GameObject.Find("Camera").transform.FindChild("Anchor/Panel");
            //o.transform.parent = t;

            //o.transform.localPosition = Vector3.zero;
            //o.transform.localScale = Vector3.one;
        }
        else
        {
            if (!handle.gameObject.activeSelf)
                handle.gameObject.SetActive(true);
        }

        if (handle.label == null)
        {
            Transform t = handle.transform.FindChild("Camera/Anchor/Panel/Label");
            handle.label = t.GetComponent<UILabel>();
        }

        handle.label.text = text;
    }
}