using UnityEngine;
using System.Collections;


public class EnvSound : MonoBehaviour
{
    public AudioClip[] audios;
    public float minDelay = 0;
    public float maxDelay = 1;

    public float destroyDelay = 0;
    public bool loop = true;

    void Start()
    {
        StartCoroutine(Play());

        if (destroyDelay > 0)
            GameObject.Destroy(this, destroyDelay);
    }

    IEnumerator Play()
    {
        float t = Random.Range(minDelay, maxDelay); // 6.3  17
        yield return new WaitForSeconds(t);

        if (Game.ActiveScene != "Battle")
        {
            int index = Random.Range(0, audios.Length);

            NGUITools.PlaySound(audios[index]);
        }

        if (loop)
            StartCoroutine(Play());
    }
}
