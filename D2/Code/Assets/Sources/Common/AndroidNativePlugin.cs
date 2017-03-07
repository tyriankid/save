using System.Collections.Generic;
using UnityEngine;

//#if UNITY_ANDROID

public class AndroidNativePlugin
{

    public static string getPhotosDir()
    {
        if (Application.platform != RuntimePlatform.Android)
            return "";

        using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.ben.util.Utility"))
        {
            using (AndroidJavaObject plugin = pluginClass.CallStatic<AndroidJavaObject>("Instance"))
            {
                return plugin.Call<string>("getPhotosDir");
            }
        }
    } 

    public static string getPhoneNumber()
    {
        if (Application.platform != RuntimePlatform.Android)
            return "";

        //AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");

        using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.ben.util.Utility"))
        {
            using (AndroidJavaObject plugin = pluginClass.CallStatic<AndroidJavaObject>("Instance"))
            {
                return plugin.Call<string>("getPhoneNumber");
            }
        }
    }

    public static void ShowAlertBox(string title, string message)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;

        using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.ben.util.Utility"))
        {
            using (AndroidJavaObject plugin = pluginClass.CallStatic<AndroidJavaObject>("Instance"))
            {
                plugin.Call("showAlert", title, message);
            }
        }
    }
}

//#endif