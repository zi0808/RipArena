using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlaySounds : MonoBehaviour
{
    public enum PlaySoundsTriggerCondition
    {
        OnStart,
        OnManualActivation
    }

    [Header("Audio Clips")]
    public AudioClip[] clips;
    [Header("Trigger Condition")]
    public PlaySoundsTriggerCondition condition = PlaySoundsTriggerCondition.OnStart;
    #region internal_fields
    Dictionary<string, AudioClip> clips_dict;
    const string random_key = "##RANDOM##";
    #endregion

    AudioSource AS;
    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
        clips_dict = new Dictionary<string, AudioClip>();
        foreach (AudioClip ac in clips)
            clips_dict.Add(ac.name, ac);

        if (condition == PlaySoundsTriggerCondition.OnStart)
            PlaySound();
    }

    public void PlaySound(string clip_name = random_key)
    {
        AudioClip Clip = null;
        if (clip_name == random_key)
            Clip = clips[Random.Range(0, clips.Length)];
        else
        {
            if (clips_dict.ContainsKey(clip_name))
                Clip = clips_dict[clip_name];
            else
                return;
        }
        AS.clip = Clip;
        AS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
