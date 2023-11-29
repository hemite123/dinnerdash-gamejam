using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueDialog : MonoBehaviour
{

    public List<string> textData = new List<string>();
    public List<ImageHandling> image = new List<ImageHandling>();
    public List<GoldGiver> GoldGiver = new List<GoldGiver> ();
    public List<Quest> quests = new List<Quest> ();
    public Queue<string> queue = new Queue<string>();
    public Queue<Image> queueimages = new Queue<Image>();
    public List<TutorialActionDo> tutorialAction = new List<TutorialActionDo>();
    public TMPro.TextMeshProUGUI textChat;
    public bool nextInteraction;
    public static QueueDialog instance;
    int index = 0;
    Gamemanager gamemanager;
    bool isquest = false;
    public Quest? selectedquest = new Quest();
    public int indexquest = 0;
    public GameObject imagetutorial;
    public bool skipText = false;
    public bool skipActionTutoral = false;
    public List<Sprite> randomselect = new List<Sprite>();
    public GameObject imageCharaRandom;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        gamemanager = Gamemanager.instance;
        foreach (string strin in textData)
        {
            queue.Enqueue(strin);
        }
        StartCoroutine(NextTutorial());
        nextInteraction = true;
    }

    public IEnumerator NextTutorial()
    {
        skipText = false;
        imageCharaRandom.GetComponent<Image>().sprite = randomselect[UnityEngine.Random.Range(0, randomselect.Count - 1)];
        imagetutorial.SetActive(false);
        if(queue.Count == 0)
        {
            textChat.transform.parent.parent.gameObject.SetActive(false);
            gamemanager.isTutorial = false;
            yield break;
        }
        foreach (GoldGiver goldive in GoldGiver)
        {
            if (goldive.index == index)
            {
                gamemanager.currency += goldive.goldgiver;
                break;
            }
        }
        textChat.text = "";
        foreach (ImageHandling imghand in image)
        {
            if(imghand.index == index)
            {
                imagetutorial.SetActive(true);
                imagetutorial.GetComponent<Image>().sprite = imghand.imageToDisplay;
                break;
            }
        }
        Quest? selected = null;
        int questindex = 0;
        foreach (Quest quest in quests)
        {
            if (quest.index == index)
            {
                selected = quest;
                selectedquest = quest;
                indexquest = questindex;
                break;
            }
            questindex++;
        }
        if (selected.HasValue)
        {
            textChat.transform.parent.parent.gameObject.SetActive(false);
            bool alldone = false;
            while (!alldone)
            {
                bool checker = true;
                foreach (QuestData qdata in selected.Value.questdata)
                {
                    if (!qdata.isdone)
                    {
                        checker = false;
                    }
                }
                if (checker)
                {
                    alldone = true;
                }
                yield return null;
            }

            textChat.transform.parent.parent.gameObject.SetActive(true);
        }
        foreach (char chardata in queue.Peek())
        {
            textChat.text += chardata;
            if (!skipText)
            {
                yield return new WaitForSeconds(0.001f);
            }
        }
        queue.Dequeue();
        nextInteraction = false;
        index++;    
        yield break;
    }

    public void DisplayingTutorial(string actioname)
    {
        //check here
        int index = tutorialAction.FindIndex(x => x.actionname.Equals(actioname));
        TutorialActionDo changed = tutorialAction[index];
        TutorialActionDo? display = changed;
        if(!display.HasValue || display.Value.istrigger || skipActionTutoral)
        {
            return;
        }
        if (changed.isNeedPause) 
        {
            Time.timeScale = 0;
        }
        changed.istrigger = true;
        changed.displaying.SetActive(true);
        changed.displaying.transform.GetComponentInChildren<Button>().onClick.AddListener(delegate { ButtonHandling.instance.CloseActionTutorial(changed); });
        tutorialAction[index] = changed;
    }
}

[System.Serializable]
public struct ImageHandling
{
    public int index;
    public Sprite imageToDisplay;
}

[System.Serializable]
public struct GoldGiver{
    public int index;
    public int goldgiver;
}

[System.Serializable]
public struct Quest
{
    public int index;
    public List<QuestData> questdata;
}

[System.Serializable]
public struct QuestData
{
    public string name_quest;
    public bool isdone;
    public QuestData(string name, bool isdones)
    {
        this.name_quest = name;
        this.isdone = isdones;
    }
}

[System.Serializable]
public struct TutorialActionDo
{
    public string actionname;
    public bool istrigger;
    public GameObject displaying;
    public bool isNeedPause;
}