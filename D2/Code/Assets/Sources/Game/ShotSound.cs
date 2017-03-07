using UnityEngine;
using System.Collections;


public class ShotSound : MonoBehaviour
{
    public AudioClip audioClip;
    public float delay = 0;
    public bool autoDestroy = false;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);

        NGUITools.PlaySound(audioClip);

        GameObject.Destroy(this);
    }
}
