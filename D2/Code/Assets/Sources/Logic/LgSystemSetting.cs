using UnityEngine;
using System.Collections;


public class LgSystemSetting : MonoBehaviour
{
    private static bool showed = false;
    private static LgSystemSetting handle = null;

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

    //void OnReturn()
    //{
    //    OnCancel();

    //    Game.ChangeScene("SelectChar");
    //}

    void OnTakenPhoto()
    {
        OnCancel();
        Helper.TakeScreenshot();
    }

    void OnExitApp()
    {
        Application.Quit();
    }

    public static void Hide()
    {
        handle.OnCancel();
    }

    public static void Show()
    {
        GameObject o = GameObject.Instantiate(Resources.Load("System_UI")) as GameObject;

        Transform t = GameObject.Find("Camera").transform;
        o.transform.parent = t;

        o.transform.localPosition = Vector3.zero;
        o.transform.localScale = Vector3.one;

        UISlider lab = o.transform.FindChild("Root/Volume").GetComponent<UISlider>();
        lab.value = NGUITools.soundVolume;

        showed = true;
    }

    public void OnChangeVolume()
    {
        UISlider lab = transform.FindChild("Root/Volume").GetComponent<UISlider>();
        NGUITools.soundVolume = lab.value;

        PlayerPrefs.SetFloat("SoundVolume", NGUITools.soundVolume);

        Object[] audios = GameObject.FindObjectsOfType(typeof(AudioSource));
        for (int i = 0; i < audios.Length; i++)
        {
            AudioSource src = audios[i] as AudioSource;
            if (src != null)
            {
                src.volume = NGUITools.soundVolume;
            }
        }
    }

    public static bool Visible
    {
        get
        {
            return showed && handle != null;
        }
    }
}