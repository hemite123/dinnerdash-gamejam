using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;
using UnityEngine.Rendering.Universal;


public class Gamemanager : MonoBehaviour
{
    //clean up variable into one line
    public static Gamemanager instance;
    public float timer = 180, currenttimer = 0;
    public Transform tick;
    bool isDay = true;
    public bool gameStarting = false;
    public DragAndDrop DragAndDrop_Instance;
    public GameObject button_utensil, content_utensi;
    public int days = 1, currency = 0;
    public bool buyingobject = false;
    public int charge = 0;
    public bool updatingMap = true;
    ButtonHandling button_hand;
    public Camera cam;
    public List<FoodMenu> foodSelect = new List<FoodMenu>();
    public int todayEarning = 0;
    public List<Customer> customer_data = new List<Customer>();
    public List<GameObject> food_placer = new List<GameObject>();
    public GameObject food_list_ui;
    public bool addImage;
    public bool addUtensil;
    public GameObject Image_food;
    public GameObject hover_food;
    EventSystem event_system;
    GameObject event_sys_go;
    public GameObject sprite_food_and_ingredient;
    public GameObject image_buy;
    public bool changePlace;
    public GameObject verification;
    public GameObject spawnVerification;
    public float  additionscale = 0;
    public float lastProjectionsize = 0;
    public GameObject uiExpandPlace;
    public int buyingExpandingPrice;
    public bool changedSize = true;
    public bool calculate = false;
    public List<(Vector2,bool)> QueuePosition = new List<(Vector2, bool)>();
    public int maxCustomer = 5;
    public float positionsize = 8f;
    public int maxchair = 0;
    public int currentCustomer = 0;
    public float customerodds = 1;
    float timertowait = 0f;
    public GameObject customerSpawning;
    public GameObject draggingbuying;
    public GameObject verificationEdit;
    public GameObject content_food;
    public GameObject food_button;
    public bool reindexing;
    public GameObject refrigenerator;
    public GameObject uiRefrigenerator;
    public GameObject refrigeneratorSpawn;
    public GameObject itemrefrigenerator;
    public bool closingtime;
    public TMPro.TextMeshProUGUI currenyUI;
    public bool isTutorial = true;
    public GameObject notificaiton_error;
    public GameObject ui_Recipe;
    public GameObject fill_ui_recipe;
    public GameObject content_recipe;
    public GameObject stove_ingredient;
    public int totalcustomerinfield = 0;
    public float additional;
    public CinemachineVirtualCamera camera;
    public List<GameObject> chunk_food= new List<GameObject>();
    public List<CustomerHandler> customer_spawn = new List<CustomerHandler>();
    public bool firstrunning = true;
    public QueueDialog quedialog;
    public GameObject displayEarning;
    public float normalodds;
    float timetospawnchar = 0;
    public bool nextday = false;
    public int one_star = 30;
    public int current_customer_serving = 0;
    public int angrycustomer;
    public int customereat;
    public Slider slider;
    public GameObject floor;
    public GameObject image_recipes_fridge;
    public GameObject content_recipeS_fridge;
    public GameObject rotate_image;
    public List<FoodMenu> ownedFood = new List<FoodMenu>();
    public GameObject fadeImage;
    public AudioClip clipNight;
    public AudioClip clipDays;
    public Light2D daynight;
    public GameObject lightCollection;
    public List<Item> itemOwned = new List<Item>();
    public GameObject content_item;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currency += 1000000;
        StartCoroutine(lightFate());
        event_system = GameObject.FindObjectOfType<EventSystem>(); 
        cam = Camera.main;
        lastProjectionsize = cam.orthographicSize;
        quedialog = QueueDialog.instance;
        button_hand = ButtonHandling.instance;
        LoadedGameUIAndAsset();
        
    }

    private void LoadedGameUIAndAsset()
    {
        //load data Util
        ScriptableObject[] data = Resources.LoadAll<ScriptableObject>("Game_Data");
        foreach(ScriptableObject obj in data)
        {
            if (obj.GetType().Equals(typeof(Utensil)))
            {
                Utensil uten = obj as Utensil;
                GameObject clone = Instantiate(button_utensil,content_utensi.transform,true);
                clone.GetComponent<Button>().onClick.AddListener(delegate { button_hand.ActionButton(obj); });
                foreach(Transform go in clone.transform)
                {
                    if(go.GetComponent<Image>())
                    {
                        go.GetComponent<Image>().sprite = uten.utensil_img;
                    }else if(go.GetComponent<TMPro.TextMeshProUGUI>() && go.name.Equals("price"))
                    {
                        go.GetComponent<TMPro.TextMeshProUGUI>().text = uten.utensil_price.ToString();
                    }else if(go.GetComponent<TMPro.TextMeshProUGUI>() && go.name.Equals("UtensilName"))
                    {
                        go.GetComponent<TMPro.TextMeshProUGUI>().text = uten.utensil_name;
                    }
                }
                clone.transform.localScale = new Vector3(1, 1, 1);
            }else if (obj.GetType().Equals(typeof(Customer)))
            {
                customer_data.Add((Customer)Instantiate(obj));
               
            }
            else if (obj.GetType().Equals(typeof(FoodMenu)))
            {
                FoodMenu uten = obj as FoodMenu;
                GameObject clone = Instantiate(food_button, content_food.transform, true);
                clone.GetComponent<Button>().onClick.AddListener(delegate { button_hand.SelectFood(uten,clone); });
                foreach (Transform go in clone.transform)
                {
                    if (go.GetComponent<Image>() && !go.name.Equals("ImgChecked"))
                    {
                        go.GetComponent<Image>().sprite = uten.food_img;
                    }else if (go.name.Equals("ImgChecked"))
                    {
                        if (ownedFood.Contains((FoodMenu)obj))
                        {
                            go.GetComponent<Image>().sprite = button_hand.own_sprite;
                        }
                        else
                        {
                            go.GetComponent<Image>().sprite = button_hand.unonw_sprite;
                        }
                    }
                    else if (go.GetComponent<TMPro.TextMeshProUGUI>() && go.name.Equals("FoodName"))
                    {
                        go.GetComponent<TMPro.TextMeshProUGUI>().text = uten.food_name;
                    }
                    else if (go.GetComponent<TMPro.TextMeshProUGUI>() && go.name.Equals("PriceFood"))
                    {
                        go.GetComponent<TMPro.TextMeshProUGUI>().text = uten.foodPrice.ToString();
                    }
                }
                clone.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (obj.GetType().Equals(typeof(Item)))
            {
                Item uten = obj as Item;
                GameObject clone = Instantiate(food_button, content_item.transform, true);
                clone.GetComponent<Button>().onClick.AddListener(delegate { button_hand.BuyItem(uten, clone); });
                foreach (Transform go in clone.transform)
                {
                    if (go.GetComponent<Image>() && !go.name.Equals("ImgChecked"))
                    {
                        go.GetComponent<Image>().sprite = uten.item_img;
                    }
                    else if (go.name.Equals("ImgChecked"))
                    {
                        go.GetComponent<Image>().sprite = button_hand.unonw_sprite;
                    }
                    else if (go.GetComponent<TMPro.TextMeshProUGUI>() && go.name.Equals("FoodName"))
                    {
                        go.GetComponent<TMPro.TextMeshProUGUI>().text = uten.itemname;
                    }
                    else if (go.GetComponent<TMPro.TextMeshProUGUI>() && go.name.Equals("PriceFood"))
                    {
                        go.GetComponent<TMPro.TextMeshProUGUI>().text = uten.price_item.ToString();
                    }
                }
                clone.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (obj.GetType().Equals(typeof(Ingredient)))
            {
                Ingredient uten = obj as Ingredient;
                if (!uten.isCookingIngredient)
                {
                    continue;
                }
                GameObject clone = Instantiate(itemrefrigenerator, refrigeneratorSpawn.transform, true);
                clone.GetComponent<Button>().onClick.AddListener(delegate { button_hand.SpawningFridge((Ingredient)obj); });
                clone.GetComponent<Image>().sprite = uten.ingredient_img;
                clone.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = uten.ingredient_name;
                clone.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        customer_data.Sort();

    }

    // Update is called once per frame
    void Update()
    {
        currenyUI.text = currency.ToString();
        if (gameStarting)
        {
            //event system detect ui need to find
            if(timertowait > 0f)
            {
                timertowait -= Time.deltaTime;
            }
            if (image_buy.active)
            {
                image_buy.SetActive(false);
            }
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.transform.tag.Equals("Utensil"))
                {
                    SpriteHandler sprithandl = hit.transform.GetComponent<SpriteHandler>();
                    string getcur = sprithandl.current_do;
                    Utensil uten = (Utensil)sprithandl.dataSprite;
                    if (uten.utensil_type.Equals(UtensilType.Fridge))
                    {
                        if(sprithandl.refriitem.Count >= sprithandl.maxingredientout)
                        {
                            return;
                        }
                        refrigenerator = hit.transform.gameObject;
                        quedialog.DisplayingTutorial("openFridge");
                        uiRefrigenerator.SetActive(true);
                        return;
                    }
                    if (!getcur.Equals(""))
                    {
                        if (getcur.Equals("order_time"))
                        {
                            for (int i = 0; i < sprithandl.customer_sit; i++)
                            {
                                int random = UnityEngine.Random.Range(0, foodSelect.Count);
                                sprithandl.food_order.Add((foodSelect[random], false));
                            }
                            sprithandl.customerInteraction["order_time"] = true;
                            sprithandl.current_do = "";
                            sprithandl.processTime = false;
                        }
                        else if (getcur.Equals("order_eat"))
                        {
                            sprithandl.customerInteraction["order_eat"] = true;
                            sprithandl.current_do = "";
                            sprithandl.processTime = false;
                        }
                    }else if(sprithandl.finalProduct != null)
                    {
                        Transform spawnhere = null;
                        foreach (GameObject food_place in food_placer)
                        {
                            if (food_place.transform.GetChild(0).childCount == 0)
                            {
                                spawnhere = food_place.transform.GetChild(0);
                            }
                        }
                        if (spawnhere != null)
                        {
                            sprithandl.cookStep = 0;
                            sprithandl.storeIngredient = new List<ScriptableObject>();
                            GameObject food_instantiate = Instantiate(sprite_food_and_ingredient, spawnhere, true);
                            food_instantiate.GetComponent<FoodHandling>().food_data = sprithandl.finalProduct;
                            food_instantiate.transform.localScale = new Vector3(1.3f, 1f, 1f);
                            food_instantiate.transform.localPosition = new Vector3(0, spawnhere.localPosition.y, 0);
                            sprithandl.finalProduct = null;
                            sprithandl.onproc = false;
                            chunk_food.Add(food_instantiate);
                        }
                        else
                        {
                            //notif cant pickup
                        }
                        //instantiate the food to place 
                    }
                }
                else
                {
                    if (refrigenerator != null && !event_system.IsPointerOverGameObject())
                    {
                        uiRefrigenerator.SetActive(false);
                        refrigenerator = null;
                    }
                }
            }
           
            if(food_list_ui.transform.childCount <= foodSelect.Count && !addImage)
            {
                addImage = true;
                if(food_list_ui.transform.childCount > 0)
                {
                    foreach(Transform child in food_list_ui.transform)
                    {
                        DestroyImmediate(child);
                    }
                }
                foreach(FoodMenu food in foodSelect)
                {
                    GameObject instantiate = Instantiate(Image_food, food_list_ui.transform, false);
                    instantiate.GetComponent<Button>().onClick.AddListener(delegate { ButtonHandling.instance.selectRecipe(food); });
                    instantiate.GetComponent<Image>().sprite = food.food_img;
                }
            }

            if (reindexing)
            {
                reindexing = false;
                int index = maxCustomer;
                foreach (CustomerHandler customer in customer_spawn)
                {
                    foreach(SpriteRenderer srend in customer.sr)
                    {
                        srend.sortingOrder = index; 
                    }
                    for (int i = 0; i < QueuePosition.Count; i++)
                    {
                        (Vector3, bool) data = QueuePosition[i];
                        if (!data.Item2)
                        {
                            customer.transform.position = data.Item1;
                            data.Item2 = true;
                            QueuePosition[i] = data;
                            break;
                        }

                    }
                    index--;
                }
                
            }
           

            if (currentCustomer < maxCustomer && timertowait <= 0f && !closingtime)
            {
                float[] chance = new float[2] { customerodds, 100.0f - customerodds};
                float random = UnityEngine.Random.Range(0f, 100f);
                float top = 0;
                for(int i = 0; i < chance.Length; i++)
                {
                    top += chance[i];
                    if(random < top)
                    {
                        if(i == 0)
                        {
                            SpawnCustomer();
                        }
                       
                        //spawning 
                    }
                }
            }

        }
        else
        {
            if(food_placer.Count <= GameObject.FindGameObjectsWithTag("Food_Place").Length && !addUtensil)
            {
                food_placer = new List<GameObject>();
                addUtensil = true;
                foreach(GameObject go in GameObject.FindGameObjectsWithTag("Food_Place"))
                {
                    food_placer.Add(go);
                }
                
            }

            if (!image_buy.active && !buyingobject && !changePlace)
            {
                if (nextday)
                {
                    return;
                }
                image_buy.SetActive(true);
            }else if (buyingobject || changePlace)
            {
                image_buy.SetActive(false);
            }



            //changeday and edit mode
        }
        
    }

    void LateUpdate()
    {
        if (gameStarting)
        {
            if (currenttimer < timer)
            {
                timetospawnchar += Time.deltaTime;
                if (currenttimer >= timer / 2 && isDay)
                {
                    isDay = false;
                    GameObject.Find("daynight").GetComponent<TMPro.TextMeshProUGUI>().text = "Night";
                    StartCoroutine(changeDayToNight("night"));
                    AudioHandling adhand = AudioHandling.instance;
                    adhand.audiochange = clipNight;
                    adhand.forcechange = true;
                    customerodds += 2.5f;
                }else if(timetospawnchar >= timer/8)
                {
                    timetospawnchar = 0;
                    SpawnCustomer();
                }
                tick.Rotate(Vector3.back, 360/timer * Time.deltaTime);
                currenttimer += Time.deltaTime;
            }
            else
            {
                if (!closingtime)
                {
                    closingtime = true;
                }
                if (totalcustomerinfield <= 0)
                {
                    nextday = true;
                    foreach(GameObject food_chunks in chunk_food)
                    {
                        DestroyImmediate(food_chunks);
                    }
                    StartCoroutine(ShowSummary());
                    uiRefrigenerator.SetActive(false);
                    refrigenerator = null;
                    displayEarning.transform.Find("Content").Find("type1").Find("earing").GetComponent<TMPro.TextMeshProUGUI>().text = "$ " + todayEarning;
                    displayEarning.transform.Find("Content").Find("type2").Find("earing").GetComponent<TMPro.TextMeshProUGUI>().text = customereat.ToString();
                    displayEarning.transform.Find("Content").Find("type3").Find("earing").GetComponent<TMPro.TextMeshProUGUI>().text = angrycustomer.ToString();
                    displayEarning.transform.Find("Content").Find("days").GetComponent<TMPro.TextMeshProUGUI>().text = "Days " + days;
                    currenttimer = 0;
                    gameStarting = false;
                    isDay = true;
                    closingtime = false;
                    customerodds = 0;
                    StartCoroutine(removeallRecipeFridge());
                }
                
                //displaying UI detailed gold and stars
            }
        }
       
        
    }

    public IEnumerator notifDisplay(string text,string texttype)
    {
        notificaiton_error.SetActive(true);
        if (texttype.Equals("error"))
        {
            notificaiton_error.GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            notificaiton_error.GetComponent<TMPro.TextMeshProUGUI>().color = Color.green;
        }
        notificaiton_error.GetComponent<TMPro.TextMeshProUGUI>().text = text;
        yield return new WaitForSeconds(1);
        notificaiton_error.SetActive(false);
    }

    public IEnumerator ShowSummary()
    {
        displayEarning.SetActive(true);
        float elapsetime = 0;
        while(elapsetime <= 5f)
        {
            elapsetime += Time.deltaTime;
            displayEarning.transform.localPosition = Vector2.Lerp(displayEarning.transform.localPosition, new Vector2(0, 0), elapsetime/ 5f);
            yield return null;
        }
    }


    public IEnumerator CloseSumamry()
    {
        nextday = false;
        current_customer_serving += customereat - angrycustomer;
        customereat = 0;
        angrycustomer = 0;
        slider.value = current_customer_serving / 150f;
        float elapsetime = 0;
        while (elapsetime <= 5f)
        {
            elapsetime += Time.deltaTime;
            displayEarning.transform.localPosition = Vector2.Lerp(displayEarning.transform.localPosition, new Vector2(0, -1187), elapsetime / 5f);
            yield return null;
        }
        currency += todayEarning;
        days += 1;
        todayEarning = 0;
        displayEarning.SetActive(false);
    }

    void SpawnCustomer() 
    {
        currentCustomer += 1;
        timertowait += 5f;
        GameObject go = Instantiate(customerSpawning);
        int CustomerRandom = UnityEngine.Random.Range(1, maxchair+1);
        go.GetComponent<CustomerHandler>().customer_list = CustomerRandom;
        go.transform.localScale = new Vector3(1, 1, 0);
        for (int j = 0; j < QueuePosition.Count; j++)
        {
            if (!QueuePosition[j].Item2)
            {
                QueuePosition[j] = (QueuePosition[j].Item1, true);
                go.transform.position = QueuePosition[j].Item1;
                break;

            }
        }
        totalcustomerinfield += 1;
        for (int i = 0; i < QueuePosition.Count; i++)
        {
            (Vector3, bool) data = QueuePosition[i];
            QueuePosition[i] = (data.Item1, false);
        }
        customer_spawn.Add(go.GetComponent<CustomerHandler>());
    }

    IEnumerator removeallRecipeFridge()
    {
        while (content_recipeS_fridge.transform.childCount > 0)
        {
            foreach (Transform trf in content_recipeS_fridge.transform)
            {
                DestroyImmediate(trf.gameObject);
            }
            yield return null;
        }
        while (food_list_ui.transform.childCount > 0)
        {
            foreach (Transform trf in food_list_ui.transform)
            {
                DestroyImmediate(trf.gameObject);
            }
            yield return null;
        }
        addImage = false;
    }

    IEnumerator lightFate()
    {
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            fadeImage.GetComponent<Image>().color = new Color(0, 0, 0, i);
            yield return null;
        }
    }

    public IEnumerator changeDayToNight(string types)
    {
        if (types == "days")
        {
            for (float i = 0.4f; i <= 1; i += Time.deltaTime)
            {
                daynight.intensity = i;
                yield return null;
            }
            lightCollection.SetActive(false);
        }
        else
        {
            for (float i = 1f; i >= 0.4f; i -= Time.deltaTime)
            {
                daynight.intensity = i;
                yield return null;
            }
            lightCollection.SetActive(true);
        }
    }
}
