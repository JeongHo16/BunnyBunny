using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : G_Singleton<SoundMgr>
{
    [HideInInspector] public AudioSource AudioSrc = null;
    Dictionary<string, AudioClip> adClipDict = new Dictionary<string, AudioClip>();

    protected override void Init()
    {
        base.Init();
        AudioSrc = GetComponent<AudioSource>();
    }

    void Start()
    {
        AudioClipLoad();
    }

    void AudioClipLoad()
    {
        AudioClip audioClip = null;
        object[] temp = Resources.LoadAll("Sounds");
        for (int i = 0; i < temp.Length; i++)
        {
            audioClip = temp[i] as AudioClip;
            if (adClipDict.ContainsKey(audioClip.name))
                continue;
            adClipDict.Add(audioClip.name, audioClip);
        }
    }

    public void PlayBGM(string fileName)
    {
        AudioClip clip = null;
        if (adClipDict.ContainsKey(fileName))
            clip = adClipDict[fileName] as AudioClip;

        if (clip == null)
        {
            Debug.Log("CLIP IS NULL");
            return;
        }

        if (AudioSrc.clip != null && AudioSrc.clip.name == fileName)
            return;

        AudioSrc.clip = clip;
        AudioSrc.volume = 1.0f;
        AudioSrc.loop = true;
        AudioSrc.Play();
    }

    public void PlayGUISound(string fileName)
    {
        if (!AllSceneMgr.Instance.user.Sfx) return;

        AudioClip clip = null;
        if (adClipDict.ContainsKey(fileName))
            clip = adClipDict[fileName] as AudioClip;

        if (clip == null)
        {
            Debug.Log("CLIP IS NULL");
            return;
        }

        AudioSrc.PlayOneShot(clip, 1.0f);
    }
}