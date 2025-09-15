using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager instance;
    public AudioSource audioSource;
    public Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>();
    

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        SetSounds();
    }


    void SetSounds()
    {
        AudioClip[] sfxs = Resources.LoadAll<AudioClip>("Sfx");
        foreach(AudioClip audio in sfxs)
        {
            Sounds.Add(audio.name, audio);
            Debug.Log($"{audio.name}�߰�");
        }
    }

    public void PlaySfx(string name)
    {
        if(!Sounds.ContainsKey(name))
        {
            Debug.LogWarning("�ش� ����� �����ϴ�.");
            return;
        }

        AudioClip sfx = Sounds[name];
        audioSource.PlayOneShot(sfx);
    }


}
