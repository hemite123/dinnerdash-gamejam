using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class ButtonHandling : MonoBehaviour
{
    Gamemanager gamemanager;
    public static ButtonHandling instance;
    Dictionary<string, bool> buttonstype = new Dictionary<string, bool>();
    public List<Image> images = new List<Image>();
    public string firstText;
    int MaxDesk = 0;
    int openImage = 1;
    bool buttonselectRecipe;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        gamemanager = Gamemanager.instance;
        foreach (Image img in images)
        {
            img.GetComponent<Button>().onClick.AddListener(delegate { UIActionHandling(img.name); });
            buttonstype.Add(img.name, buttonstype.Count == 0 ? true : false);
        }
    }

    private void Update()
    {
        foreach (Image img in images)
        {
            bool actived;
            buttonstype.TryGetValue(img.name, out actived);
            if (actived)
            {
                img.color = new Color32(190, 190, 190, 255);
            }
            else
            {
                img.color = new Color32(101, 101, 101, 255);
            }
        }
    }

    public void ActionButton(ScriptableObject Data)
    {
        if (Data.GetType().Equals(typeof(Utensil)))
        {
            Utensil utensil = (Utensil)Data;
            if(gamemanager.currency < utensil.utensil_price)
            {
                StartCoroutine(gamemanager.notifDisplay("Not Enough Money"));
                return;
            }
            float additionalscale = gamemanager.additionscale;
            if (utensil.utensil_type.Equals(UtensilType.Desk))
            {
                MaxDesk = utensil.utensil_max_use;
            }
            gamemanager.buyingobject = true;
            GameObject gameObject = Instantiate(utensil.utensil_gameobject, new Vector3(0, 0, 0), Quaternion.identity);
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            gamemanager.draggingbuying = gameObject;
            GameObject go_verification = Instantiate(gamemanager.verification);
            gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 60);
            foreach (Transform childObj in gameObject.transform)
            {
                if (childObj.tag.Equals("Chair"))
                {
                    childObj.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 60);
                }
            }
            go_verification.transform.localPosition = gameObject.transform.localPosition + new Vector3(0, 1.5f, 0);
            go_verification.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { AcceptBuy(gameObject,utensil); });
            go_verification.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { DeclineBuy(gameObject); });
            gamemanager.spawnVerification = go_verification;
            gamemanager.charge = utensil.utensil_price;
            gamemanager.rotate_image.SetActive(true);
        }
    }


    public void UIActionHandling(string button_type)
    {
        List<string> converttolist = new List<string>(buttonstype.Keys);
        foreach (string button in converttolist)
        {
            if (button == button_type)
            {
                buttonstype[button] = true;
            }
            else
            {
                buttonstype[button] = false;
            }
        }
    }

    public void AcceptBuy(GameObject object_to_buy,ScriptableObject data)
    {
        Collider2D collider = object_to_buy.GetComponent<Collider2D>();
        if (collider.IsTouchingLayers())
        {
            StartCoroutine(gamemanager.notifDisplay("Cant Place Here"));
            return;
        }
        gamemanager.buyingobject = false;
        gamemanager.draggingbuying = null;
        gamemanager.updatingMap = true;
        gamemanager.addUtensil = false;
        QueueDialog que = QueueDialog.instance;
        if(!que.selectedquest.HasValue.Equals(default(Quest))) 
        {
            List<QuestData> qdata = que.selectedquest.Value.questdata;
            if(qdata != null)
            {
                int index = 0;
                foreach (QuestData questdata in qdata)
                {
                    if (questdata.name_quest.Equals("desk") && ((Utensil)data).utensil_type.Equals(UtensilType.Desk))
                    {
                        que.quests[que.indexquest].questdata[index] = new QuestData(questdata.name_quest, true);
                        break;
                    }
                    else if (questdata.name_quest.Equals("refrigene") && ((Utensil)data).utensil_type.Equals(UtensilType.Fridge))
                    {
                        que.quests[que.indexquest].questdata[index] = new QuestData(questdata.name_quest, true);
                        break;
                    }
                    else if (questdata.name_quest.Equals("stove") && ((Utensil)data).utensil_type.Equals(UtensilType.Stove))
                    {
                        que.quests[que.indexquest].questdata[index] = new QuestData(questdata.name_quest, true);
                        break;
                    }
                    else if (questdata.name_quest.Equals("service_desk") && ((Utensil)data).utensil_type.Equals(UtensilType.Service_Desk))
                    {
                        que.quests[que.indexquest].questdata[index] = new QuestData(questdata.name_quest, true);
                        break;
                    }
                    else if (questdata.name_quest.Equals("trash_can") && ((Utensil)data).utensil_type.Equals(UtensilType.Trash))
                    {
                        que.quests[que.indexquest].questdata[index] = new QuestData(questdata.name_quest, true);
                        break;
                    }
                    index++;
                }
            }
            
        }
        DestroyImmediate(GameObject.FindGameObjectWithTag("Notif"));
        object_to_buy.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        object_to_buy.GetComponent<SpriteRenderer>().sortingOrder = 0;
        foreach (Transform childObj in object_to_buy.transform)
        {
            if (childObj.tag.Equals("Chair"))
            {
                childObj.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            }
        }
        if(gamemanager.maxchair <= MaxDesk)
        {
            gamemanager.maxchair = MaxDesk;
            MaxDesk = 0;
        }
        gamemanager.currency -= gamemanager.charge;
        gamemanager.charge = 0;
        gamemanager.rotate_image.SetActive(false);
    }

    public void DeclineBuy(GameObject object_to_buy)
    {
        gamemanager.buyingobject = false;
        gamemanager.draggingbuying = null;
        gamemanager.addUtensil = false;
        gamemanager.rotate_image.SetActive(false);
        gamemanager.charge = 0;
        DestroyImmediate(GameObject.FindGameObjectWithTag("Notif"));
        DestroyImmediate(object_to_buy);
    }

    public void AcceptChange(GameObject object_to_buy)
    {
        Collider2D collider = object_to_buy.GetComponent<Collider2D>();
        if (collider.IsTouchingLayers())
        {
            StartCoroutine(gamemanager.notifDisplay("Cant Place Here"));
            return;
        }
        gamemanager.changePlace = false;
        gamemanager.updatingMap = true;
        gamemanager.draggingbuying = null;
        gamemanager.addUtensil = false;
        gamemanager.rotate_image.SetActive(false);
        object_to_buy.GetComponent<SpriteRenderer>().sortingOrder = 0;
        DestroyImmediate(GameObject.FindGameObjectWithTag("Notif"));
        object_to_buy.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        foreach (Transform childObj in object_to_buy.transform)
        {
            if (childObj.tag.Equals("Chair"))
            {
                childObj.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    public void DeleteChange(GameObject object_to_buy)
    {
        gamemanager.changePlace = false;
        gamemanager.draggingbuying = null;
        gamemanager.addUtensil = false;
        DestroyImmediate(GameObject.FindGameObjectWithTag("Notif"));
        DestroyImmediate(object_to_buy);
        gamemanager.rotate_image.SetActive(false);
    }

    public void ExpandUi()
    {
        if (!gamemanager.uiExpandPlace.active)
        {
            gamemanager.uiExpandPlace.SetActive(true);
        }
        gamemanager.changedSize = true;
        Text textdata = gamemanager.uiExpandPlace.transform.Find("textExpand").GetComponent<Text>();
        string gettext = textdata.text;
        firstText = gettext;
        textdata.text = string.Format(gettext, gamemanager.buyingExpandingPrice.ToString());

    }

    public void AcceptExpandUI()
    {
        if(gamemanager.currency >= gamemanager.buyingExpandingPrice)
        {
            if (gamemanager.uiExpandPlace.active)
            {
                gamemanager.uiExpandPlace.SetActive(false);
            }
            gamemanager.currency -= gamemanager.buyingExpandingPrice;
            gamemanager.buyingExpandingPrice += 1000;
            gamemanager.camera.m_Lens.OrthographicSize += 0.05f;
            gamemanager.maxCustomer += 1;
            gamemanager.calculate = false;
            gamemanager.changedSize = false;
            gamemanager.uiExpandPlace.transform.Find("textExpand").GetComponent<Text>().text = firstText;
        }
        else
        {
            StartCoroutine(gamemanager.notifDisplay("Not Enough Money"));
            //Error buying
        }
    }

    public void DeclineExpandUI()
    {
        if (gamemanager.uiExpandPlace.active)
        {
            gamemanager.uiExpandPlace.SetActive(false);
        }
        gamemanager.changedSize = false;
        gamemanager.uiExpandPlace.transform.Find("textExpand").GetComponent<Text>().text = firstText;
    }

    public void StartGame()
    {
        if (!gamemanager.isTutorial)
        {
            if (gamemanager.firstrunning)
            {
                gamemanager.firstrunning = false;
            }
            gamemanager.gameStarting = true;
            gamemanager.customerodds = gamemanager.normalodds;
            if(gamemanager.current_customer_serving >= 30)
            {
                int multiple = gamemanager.current_customer_serving / gamemanager.one_star;
                gamemanager.customerodds += 0.01f * multiple;
            }
            GameObject.Find("daynight").GetComponent<TMPro.TextMeshProUGUI>().text = "Day";
            foreach(FoodMenu food_select in gamemanager.foodSelect)
            {
                GameObject instance = Instantiate(gamemanager.image_recipes_fridge,gamemanager.content_recipeS_fridge.transform,false);
                instance.transform.Find("imgFood").GetComponent<Image>().sprite = food_select.food_img;
                instance.transform.localScale = new Vector3(1, 1, 1);
                Transform parenttospawn = instance.transform.Find("ingred").Find("viewport").Find("content_ingredient").transform;
                foreach (Ingredient ingredient in food_select.ingredient_food)
                {
                    GameObject clondata = Instantiate(gamemanager.fill_ui_recipe,parenttospawn,false);
                    clondata.transform.localScale = new Vector3(1, 1, 1);
                    clondata.transform.Find("ImgData").GetComponent<Image>().sprite = ingredient.ingredient_img;
                    clondata.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = ingredient.ingredient_name;

                }
            }
        }
        else
        {
            StartCoroutine(gamemanager.notifDisplay("You'r In Tutorial"));
        }
        
    }

    public void SpawningFridge(Ingredient ingredspawn)
    {
        SpriteHandler handle = gamemanager.refrigenerator.GetComponent<SpriteHandler>();
        Transform selectedSpawn = null;
        foreach(Transform trans in gamemanager.refrigenerator.transform)
        {
            if(trans.childCount <= 0)
            {
                selectedSpawn = trans;
                break;
            }
        }
        if(selectedSpawn != null)
        {
            GameObject insntate = Instantiate(gamemanager.sprite_food_and_ingredient, selectedSpawn, false);
            insntate.transform.localScale = new Vector3(2, 2, 2);
            insntate.transform.position = selectedSpawn.transform.position;
            handle.refriitem.Add(insntate);
            FoodHandling data = insntate.GetComponent<FoodHandling>();
            data.refrigenerator = gamemanager.refrigenerator;
            data.isIngredient = true;
            data.food_data = ingredspawn;
            gamemanager.chunk_food.Add(insntate);
        }
        

    }

    public void CloseAndOpen()
    {
       if(openImage == 1)
        {
            openImage = 0;
            StartCoroutine(closePopup());
        }
        else
        {
            openImage = 1;
            StartCoroutine(openPopup());
        }
    }

    IEnumerator closePopup()
    {
        for(float i = 151f; i >= -60f; i--)
        {
            gamemanager.image_buy.transform.localPosition += new Vector3(0, -1,0);
        }
        yield return null;
    }

    IEnumerator openPopup()
    {
        for (float i = -60f; i <= 151f; i++)
        {
            gamemanager.image_buy.transform.localPosition += new Vector3(0, 1, 0);
        }
        yield return null;
    }

    public void NextDialog()
    {
        QueueDialog dialog = QueueDialog.instance;
        if (!dialog.nextInteraction)
        {
            dialog.nextInteraction = true;
            StartCoroutine(dialog.NextTutorial());
        }
        else if (!dialog.skipText)
        {
            dialog.skipText = true;
        }
    }

    public void selectRecipe(FoodMenu recipeData)
    {
        if (!buttonselectRecipe)
        {
            gamemanager.ui_Recipe.SetActive(true);
            buttonselectRecipe = true;
            gamemanager.ui_Recipe.transform.Find("ImgFood").GetComponent<Image>().sprite = recipeData.food_img;
            foreach(Ingredient ingred in recipeData.ingredient_food)
            {
                GameObject clondata = Instantiate(gamemanager.fill_ui_recipe, gamemanager.content_recipe.transform, false);
                clondata.transform.localScale = new Vector3(1, 1, 1);
                clondata.transform.Find("ImgData").GetComponent<Image>().sprite = ingred.ingredient_img;
                clondata.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = ingred.ingredient_name;
            }
            gamemanager.content_recipe.transform.parent.parent.GetComponent<ScrollRect>().horizontalNormalizedPosition = 0f;
        }
    }

    public void CloseRecipe()
    {
        gamemanager.ui_Recipe.SetActive(false);
        StartCoroutine(startRemoveRecipe());
        buttonselectRecipe = false;
    }

    IEnumerator startRemoveRecipe()
    {
        while (gamemanager.content_recipe.transform.childCount > 0)
        {
            foreach (Transform transform in gamemanager.content_recipe.transform)
            {
                DestroyImmediate(transform.gameObject);
            }
            yield return null;
        }
    }

    public void NextDay()
    {
        if (gamemanager.nextday)
        {
            StartCoroutine(gamemanager.CloseSumamry());
        }
        
    }

    public void CloseRefrigenerator()
    {
        gamemanager.uiRefrigenerator.SetActive(false);
        gamemanager.refrigenerator = null;
    }
}
