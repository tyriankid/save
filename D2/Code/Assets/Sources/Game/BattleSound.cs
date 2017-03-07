using UnityEngine;
using System.Collections;


public class BattleSound : MonoBehaviour
{
    public AudioClip menDead;
    public AudioClip womenDead;
    public AudioClip levelup;


    //  §¿˚…˘“Ù

    private static BattleSound handle;

    void Start()
    {
        handle = this;
    }

    public static void PlayDeadSound(bool women)
    {
        if (women)
            NGUITools.PlaySound(handle.womenDead);
        else
            NGUITools.PlaySound(handle.menDead);
    }

    public static void PlayLevelupSound()
    {
        NGUITools.PlaySound(handle.levelup);
    }
}
