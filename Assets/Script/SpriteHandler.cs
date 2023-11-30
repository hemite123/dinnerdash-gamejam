using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

//Fix the scaling
public class SpriteHandler : MonoBehaviour
{
    public ScriptableObject dataSprite;
    Gamemanager gamemanager;
    public int customer_sit = 0;
    public float waitingtimeinsecond = 1500;
    public bool processTime;
    Camera cam;
    public Dictionary<string,bool> customerInteraction = new Dictionary<string, bool>() { {"order_time",false }, { "order_wait", false },{ "order_eat", false }, { "order_finish", false } };
    public Dictionary<string, bool> cookInteraction = new Dictionary<string, bool>() {{ "cook_time", false }};
    public string current_do;
    public float current_waiting_time = 0f;
    public bool start_waiting_time;
    public GameObject waiting_time;
    public List<(FoodMenu,bool)> food_order = new List<(FoodMenu, bool)>();
    public List<Customer> customer_data = new List<Customer>();
    public GameObject popup;
    public int total_this_sprite = 0;
    public GameObject salary_get;
    public bool chairSetup = false;
    public Dictionary<Transform, bool> chairData = new Dictionary<Transform, bool>();
    public Sprite defaultSprite;
    public bool isCookingUtensil;
    public bool cookProseess;
    public ScriptableObject ingredientOn;
    public List<ScriptableObject> storeIngredient = new List<ScriptableObject>();
    public ScriptableObject finalProduct;
    public int cookStep = 0;
    public Image image_food;
    public bool setImage,pauseUpdateSprite, runningTimerCorut;
    Vector3 lastlocalscale;
    public float lastscale;
    public List<GameObject> refriitem = new List<GameObject>();
    List<GameObject> imageOrder = new List<GameObject>();
    public GameObject stove_ingredient;
    public bool onproc, useable = false;
    public int maxingredientout = 2;
    public string color = "based";
    bool reset= false;
    Animator anim;
    private void Start()
    {
        lastlocalscale = transform.localScale;
        gamemanager = Gamemanager.instance;
        foreach (Transform transform in gameObject.transform)
        {
            if (transform.tag == "Chair")
            {
                chairData.Add(transform, false);
            }
        }
        cam = gamemanager.cam;
    }

    private void Update()
    {
        if (!gamemanager.gameStarting && !reset && !gamemanager.firstrunning && ((Utensil)dataSprite).utensil_type.Equals(UtensilType.Stove))
        {
            reset = true;
            finalProduct = null;
            StartCoroutine(startRemoveStoveIngredient());
        }else if(gamemanager.gameStarting && reset)
        {
            reset = false;
        }
        ActionToDO();
        if (start_waiting_time && !isCookingUtensil)
        {
            current_waiting_time += Time.deltaTime;
            waiting_time.GetComponent<Image>().fillAmount -= (1 / waitingtimeinsecond) * Time.deltaTime;
            if (((Utensil)dataSprite).utensil_type.Equals(UtensilType.Coffee_maker))
            {
                waiting_time.SetActive(true);
                if(current_waiting_time >= waitingtimeinsecond)
                {
                    useable = true;
                    start_waiting_time = false;
                    current_waiting_time = 0;
                    waiting_time.GetComponent<Image>().fillAmount = 1;
                    waiting_time.SetActive(false);
                }
                return;
            }
            if (!pauseUpdateSprite)
            {
                if (color.Equals("based"))
                {
                    color = "green";
                    StartCoroutine(sitWithOrderUp());
                }
                if (current_waiting_time >= waitingtimeinsecond / 2 && color.Equals("green"))
                {
                    StopCoroutine(sitWithOrderUp());
                    color = "yellow";
                    StartCoroutine(halfAngry());
                }
                if (current_waiting_time >= waitingtimeinsecond * 0.75f && color.Equals("yellow"))
                {
                    StopCoroutine(halfAngry());
                    color = "red";
                    StartCoroutine(fullAngry());
                }
            }
            else if(pauseUpdateSprite && !runningTimerCorut)
            {
                runningTimerCorut = true;
                StartCoroutine(resetTimer());
            }
            
            if(food_order.Count <= 0)
            {
                if (current_waiting_time >= waitingtimeinsecond)
                {
                    gamemanager.angrycustomer += 1;
                    RefreshCustomer();
                }
            }
            else
            {
                bool isdone = true;
                int index = 0;
                foreach ((FoodMenu, bool) menu in food_order)
                {
                    if (!menu.Item2)
                    {
                        isdone = false;
                    }
                    else
                    {
                        imageOrder[index].SetActive(false);

                    }
                    index++;
                }
                if (isdone && current_waiting_time <= waitingtimeinsecond)
                {
                    start_waiting_time = false;
                    current_waiting_time = 0;
                    waiting_time.GetComponent<Image>().fillAmount = 1;
                    customerInteraction["order_wait"] = true;
                    waiting_time.SetActive(false);
                    current_do = "";
                    processTime = false;
                }
                else if (current_waiting_time >= waitingtimeinsecond && !isdone)
                {
                    gamemanager.angrycustomer += 1;
                    RefreshCustomer();
                }
            }
            
        }

        if(isCookingUtensil && cookProseess && start_waiting_time)
        {
            
            stove_ingredient.transform.parent.parent.gameObject.SetActive(false);
            if (!waiting_time.active)
            {
                waiting_time.SetActive(true);
            }
            current_waiting_time += Time.deltaTime;
            waiting_time.GetComponent<Image>().fillAmount -= (1 / waitingtimeinsecond) * Time.deltaTime;
            if (current_waiting_time >= waitingtimeinsecond && !onproc)
            {
                onproc = true;
                Ingredient ingredient = (Ingredient)ingredientOn;
                if (ingredient.finish_ingredient_object == null)
                {
                    storeIngredient.Add(ingredient);
                }
                else
                {
                    storeIngredient.Add(ingredient.finish_ingredient_object);
                }
                if(storeIngredient.Count > 0 && !stove_ingredient.active)
                {
                    stove_ingredient.transform.parent.parent.gameObject.SetActive(true);
                    stove_ingredient.transform.parent.parent.GetComponent<ScrollRect>().horizontalNormalizedPosition = 0f;
                }
                cookStep++;
                ingredientOn = null;
                current_waiting_time = 0;
                cookProseess = false;
                waiting_time.SetActive(false);
                start_waiting_time = false;
                foreach(FoodMenu ingredient_cook in gamemanager.foodSelect)
                {
                    //check if 
                    bool wrong = false;
                    List<Ingredient> listof_store = new List<Ingredient>(ingredient_cook.ingredient_food);
                    int index = 0;
                    foreach(Ingredient ingred in storeIngredient)
                    {
                        if (listof_store[index].Equals(ingred) || listof_store[index].finish_ingredient_object.Equals(ingred))
                        {
                            index++;
                            continue;
                        }
                        wrong = true;
                        break;
                    }
                    if (!wrong && storeIngredient.Count == listof_store.Count)
                    {
                        Transform spawnhere = null;
                        foreach(GameObject food_place in gamemanager.food_placer)
                        {
                            if(food_place.transform.GetChild(0).childCount == 0)
                            {
                                spawnhere = food_place.transform.GetChild(0);
                                StartCoroutine(startRemoveStoveIngredient());
                                storeIngredient = new List<ScriptableObject>();
                                
                            }
                        }
                        if(spawnhere == null)
                        {
                            finalProduct = ingredient_cook;
                            StartCoroutine(startRemoveStoveIngredient());
                            setImage = false;
                            storeIngredient = new List<ScriptableObject>();
                            return;
                        }
                        cookStep = 0;
                        storeIngredient = new List<ScriptableObject>();
                        GameObject food_instantiate = Instantiate(gamemanager.sprite_food_and_ingredient, spawnhere, true);
                        food_instantiate.GetComponent<FoodHandling>().food_data = ingredient_cook;
                        food_instantiate.transform.localScale = new Vector3(1.3f, 1f, 1f);
                        food_instantiate.GetComponent<BoxCollider2D>().size = new Vector2(1.3f, 1f);
                        food_instantiate.transform.localPosition = new Vector3(0, spawnhere.localPosition.y, 0);
                        gamemanager.chunk_food.Add(food_instantiate);
                        break;
                    }
                    
                }
                GameObject cloneimage = Instantiate(gamemanager.stove_ingredient, stove_ingredient.transform, false);
                cloneimage.transform.localScale = new Vector3(1, 1, 1   );
                cloneimage.GetComponent<Image>().sprite = ingredient.ingredient_img;
                onproc = false;
                waiting_time.GetComponent<Image>().fillAmount = 1;
                //check food to make
            }

        }

        if (!setImage && finalProduct != null)
        {
            setImage = true;
            image_food.gameObject.SetActive(true);
            image_food.GetComponent<Image>().sprite = ((FoodMenu)finalProduct).food_img;
        }
        else if (finalProduct == null && setImage)
        {
            setImage = false;
            image_food.gameObject.SetActive(false);
        }

        if (customer_sit > 0 && !chairSetup)
        {
            int index = 0;
            bool ishaveexclusive = false;
            chairSetup = true;
            foreach (Customer custcheck in customer_data)
            {
                CustomerType ctype = custcheck.customerType;
                if (ctype.Equals(CustomerType.VIP) || ctype.Equals(CustomerType.BRANDED) || ctype.Equals(CustomerType.CRAZYRICH))
                {
                    ishaveexclusive = true;
                }
            }
            foreach (Customer cust in customer_data)
            {
                List<KeyValuePair<Transform, bool>> loopChair = new List<KeyValuePair<Transform, bool>>(chairData);
                foreach(KeyValuePair<Transform,bool> dataChair in loopChair)
                {
                    if (!dataChair.Value)
                    {
                        Transform transform = dataChair.Key;
                        transform.GetComponent<SpriteRenderer>().sprite = cust.customer_img.Find(x => x.action =="sit").img_sprite;
                        chairData[dataChair.Key] = true;
                        if (ishaveexclusive)
                        {
                            if (cust.customerType.Equals(CustomerType.NORMAL))
                            {
                                waitingtimeinsecond += cust.CustomerAngryTime / 2;
                                index++;
                                break;
                            }
                        }
                        waitingtimeinsecond += cust.CustomerAngryTime;
                        break;

                    }
                }
            }
            //initialize the chair
        }

        if(imageOrder.Count == 0)
        {
            if(dataSprite != null && (((Utensil)dataSprite).utensil_type.Equals(UtensilType.Desk)))
            {
                foreach (Transform tran in transform.GetComponentInChildren<Canvas>().transform)
                {
                    if (tran.tag.Equals("ChairPopup"))
                    {
                        imageOrder.Add(tran.gameObject);
                    }

                }
            }
            //error here
           
        }

        

    }

    void ActionToDO()
    {
        if (customer_sit > 0 && !processTime && current_do.Equals("") && gamemanager.gameStarting)
        {
            StartCoroutine(fillAction());
        }
    }

    IEnumerator startRemoveStoveIngredient()
    {
        while(stove_ingredient.transform.childCount > 0)
        {
            foreach (Transform transform in stove_ingredient.transform)
            {
                DestroyImmediate(transform.gameObject);
            }
            yield return null;
        }
        stove_ingredient.transform.parent.parent.gameObject.SetActive(false);
    }

    IEnumerator fillAction()
    {
        processTime = true;
        if (dataSprite.GetType().Equals(typeof(Utensil)))
        {
            Utensil utensil = (Utensil)dataSprite;
            if (utensil.utensil_type.Equals(UtensilType.Desk))
            {
                //Customer Behaviour
                List<KeyValuePair<string, bool>> spliting = new List<KeyValuePair<string, bool>>(customerInteraction);
                foreach (KeyValuePair<string, bool> action in spliting)
                {
                    if (!action.Value)
                    {
                        string temp_current_do = action.Key;
                        if (temp_current_do.Equals("order_time"))
                        {
                            yield return new WaitForSeconds(1f);
                            //popup.SetActive(true);
                            SpriteHandlingCustomer("order_up");
                            current_do = action.Key;
                            start_waiting_time = true;
                            break;
                        }
                        else if (temp_current_do.Equals("order_wait"))
                        {
                            SpriteHandlingCustomer("sit");
                            popup.SetActive(false);
                            if (!waiting_time.active)
                            {
                                waiting_time.SetActive(true);
                            }
                            int index = 0;
                            foreach (GameObject imgord in imageOrder)
                            {
                                if (index == food_order.Count)
                                {
                                    break;
                                }
                                imgord.SetActive(true);
                                imgord.GetComponent<Image>().sprite = food_order[index].Item1.food_img;
                                index++;
                            }
                            current_do = action.Key;
                            break;
                        }
                        else if (temp_current_do.Equals("order_eat"))
                        {
                            SpriteHandlingCustomer("eating");
                            yield return new WaitForSeconds(5f);
                            List<KeyValuePair<Transform, bool>> loopChair = new List<KeyValuePair<Transform, bool>>(chairData);
                            foreach (KeyValuePair<Transform, bool> dataChair in loopChair)
                            {
                                dataChair.Key.GetComponent<Animator>().runtimeAnimatorController = null;
                                dataChair.Key.GetComponent<SpriteRenderer>().sprite = defaultSprite;
                                chairData[dataChair.Key] = false;
                            }
                            popup.SetActive(true);
                            foreach (GameObject imgord in imageOrder)
                            {
                                imgord.SetActive(false);
                            }
                            current_do = action.Key;
                            break;

                        }
                        else if (temp_current_do.Equals("order_finish"))
                        {
                            popup.SetActive(false);
                            //calculate the customer give
                            foreach (Customer customer in customer_data)
                            {
                                total_this_sprite += UnityEngine.Random.Range(customer.minSalary, customer.maxSalary);
                            }
                            gamemanager.todayEarning += total_this_sprite;
                            salary_get.GetComponent<Text>().text = "$ " + total_this_sprite.ToString();
                            salary_get.SetActive(true);
                            gamemanager.customereat += 1;
                            RefreshCustomer();
                            yield return new WaitForSeconds(0.6f);
                            total_this_sprite = 0;
                            salary_get.SetActive(false); 
                            break;
                            //reset all stats
                        }

                    }
                }
            }
        }
    }

    public void RefreshCustomer()
    {
        customer_sit = 0;
        color = "green";
        processTime = false;
        popup.SetActive(false);
        current_waiting_time = 0;
        waitingtimeinsecond = 0;
        start_waiting_time = false;
        chairSetup = false;
        customer_data = new List<Customer>();
        waiting_time.GetComponent<Image>().fillAmount = 1;
        waiting_time.SetActive(false);
        List<KeyValuePair<Transform, bool>> loopChair = new List<KeyValuePair<Transform, bool>>(chairData);
        foreach (KeyValuePair<Transform, bool> dataChair in loopChair)
        {
            dataChair.Key.GetComponent<SpriteRenderer>().sprite = defaultSprite;
            chairData[dataChair.Key] = false;
        }
        List<string> keytolist = new List<string>(customerInteraction.Keys);
        foreach (string keyname in keytolist)
        {
            customerInteraction[keyname] = false;
        }
        foreach (GameObject imgord in imageOrder)
        {
            imgord.SetActive(false);
        }
        current_do = "";
        gamemanager.totalcustomerinfield -= 1;
        food_order = new List<(FoodMenu, bool)>();
    }


    public void SpriteHandlingCustomer(string action)
    {
        List<Transform> keyslist = new List<Transform>(chairData.Keys);
        int index = 0;
        foreach (Customer cust in customer_data)
        {
            Transform transform = keyslist[index];
            SpriteHandling getdata = cust.customer_img.Find(x => x.action == action);
            if (getdata.isAnimation)
            {
                transform.GetComponent<Animator>().runtimeAnimatorController = getdata.animation;
            }
            else
            {
                transform.GetComponent<SpriteRenderer>().sprite = getdata.img_sprite;
            }
            index++;
        }
    }

    IEnumerator sitWithOrderUp()
    {
        SpriteHandlingCustomer("sit");
        yield return new WaitForSeconds(1f);
        if (current_do.Equals("order_time"))
        {
            SpriteHandlingCustomer("order_up");
        }
        StopCoroutine(sitWithOrderUp());
    }

    IEnumerator halfAngry()
    {
        while (color.Equals("yellow"))
        {
            SpriteHandlingCustomer("angsithalf");
            yield return new WaitForSeconds(1f);
            if (current_do.Equals("order_time"))
            {
                SpriteHandlingCustomer("order_up");
            }
            yield return new WaitForSeconds(5f);
            yield return null;
        }
        StopCoroutine(halfAngry());
    }

    IEnumerator fullAngry()
    {
        while (color.Equals("red"))
        {
            SpriteHandlingCustomer("angsit");
            yield return new WaitForSeconds(0.5f);
            if (current_do.Equals("order_time"))
            {
                SpriteHandlingCustomer("order_up");
            }
            yield return new WaitForSeconds(3f);
            yield return null;
        }
        StopCoroutine(fullAngry());
    }


    IEnumerator resetTimer()
    {
        if (pauseUpdateSprite)
        {
            yield return new WaitForSeconds(2f);
            pauseUpdateSprite = false;
        }
        runningTimerCorut = false;
        StopAllCoroutines();
    }

}
