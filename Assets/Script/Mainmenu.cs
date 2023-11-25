using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mainmenu : MonoBehaviour
{
    public GameObject image, Creditobject,Settingobject;
    public float volume;
    public Slider sliderVolume;
    public AudioSource audio;
    public TMPro.TextMeshProUGUI textVolume;
    // Start is called before the first frame update

    public void StartGame()
    {
        StartCoroutine(blackFade());
    }

    IEnumerator blackFade()
    {
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            image.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null;
        }
        var sceneload = SceneManager.LoadSceneAsync(1);
        sceneload.completed += (x) =>
        {
            AudioHandling.instance.volume = volume;
        };
    }

    private void Update()
    {
        volume = sliderVolume.value;
        if(audio.volume != volume)
        {
            textVolume.text = ((int)((volume/100f)*10000)).ToString();
            audio.volume = volume;
        }
    }

    public void OpenCredit(bool action)
    {
        Creditobject.SetActive(action);
    }

    public void OpenSetting(bool action)
    {
        Settingobject.SetActive(action);
    }
}
