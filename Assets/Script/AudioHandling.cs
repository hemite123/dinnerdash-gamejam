using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandling : MonoBehaviour
{
    public AudioSource audio;
    public bool runningaudio = false;
    public bool playing;
    public bool forcechange = false;
    public static AudioHandling instance;
    public AudioClip audiochange;
    public float volume;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    IEnumerator FadeOut()
    {
        float currentVolume = audio.volume;
        while (currentVolume > 0){
            audio.volume -= 0.01f;
            currentVolume = audio.volume;
            yield return null;
        }
        audio.Stop();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        if(audiochange != null)
        {
            audio.clip = audiochange;
            audiochange = null;
        }
        audio.Play();
        runningaudio = false;
        playing = true;
        float currentVolume = audio.volume;
        while (currentVolume < volume)
        {
            audio.volume += 0.1f / 1f * Time.deltaTime / 1f;
            currentVolume = audio.volume;
            yield return null;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    // Update is called once per frame
    void Update()
    {
        if(audio.time >= audio.clip.length - 4f && !runningaudio && playing || forcechange)
        {
            if (forcechange)
            {
                StopAllCoroutines();
                forcechange = false;
            }
            else
            {
                runningaudio = true;
            }
            StartCoroutine(FadeOut());

        }
    }
}
