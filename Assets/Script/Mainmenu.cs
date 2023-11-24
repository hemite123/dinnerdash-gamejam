using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mainmenu : MonoBehaviour
{
    public GameObject image;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        StartCoroutine(blackFade());
    }

    IEnumerator blackFade()
    {
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            image.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null;
        }
        SceneManager.LoadScene(1);
    }
}
