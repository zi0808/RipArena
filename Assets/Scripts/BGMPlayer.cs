using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : Singleton<BGMPlayer>
{
    public AudioClip NormalBGM;
    public AudioClip GameOver;
    AudioSource AS;
    private void Start()
    {
        AS = GetComponent<AudioSource>();
        AS.clip = NormalBGM;
    }
    public void Play(float volume = 0.6f)
    {
        AS.volume = volume;
        AS.Play();
    }
    public void Fadeout()
    {
        StartCoroutine(CRFadeout());
    }
    IEnumerator CRFadeout(float duration = 2f)
    {
        float c_vol = AS.volume;
        float timer = duration;
        while (timer > 0)
        {
            AS.volume = Mathf.Lerp(0, c_vol, timer / duration);
            yield return new WaitForEndOfFrame();
            timer -= Time.deltaTime;
        }
    }
}
