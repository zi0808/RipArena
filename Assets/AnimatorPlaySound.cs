using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Rendering;

public class AnimatorPlaySound : MonoBehaviour
{
    public AudioClip[] clips;
    Dictionary<string, AudioClip> clip_dict;
    public AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        if (source == null)
            source = GetComponent<AudioSource>();
        if (clips.Length > 0)
        {
            clip_dict = new Dictionary<string, AudioClip>();
            foreach (AudioClip AC in clips)
                clip_dict.Add(AC.name, AC);
        }
    }

    private void Play(string clip_name)
    {
        if (source == null)
            return;
        try
        {
            if (clip_name != "##RANDOM##")
                source.clip = clip_dict[clip_name];
            else
                source.clip = clips[Random.Range(0,clips.Length)];
            source.Play();
        }
        catch (System.Exception E)
        {
            Debug.LogWarning("Animator Play Sound : Can't Find " + clip_name + " | " + E.Message);
        }
    }
    public void PlaySound(string clip_name = "##RANDOM##")
    {
        source.loop = false;
        Play(clip_name);
    }
    public void StartSound(string clip_name = "##RANDOM##")
    {
        source.loop = true;
        Play(clip_name);
    }
    public void StopSound()
    {
        source.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
