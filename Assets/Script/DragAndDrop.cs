using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour
{
    // Start is called before the first frame update
    
    public Transform draggedObject;
    public Vector2 lastPosition;
    public bool objectOutside;
    public Vector2 screenSize;
    public Vector3 currentMousePosition;
    Camera cam;
    EdgeCollider2D edgeCollider;
    Gamemanager gamemanager;
    public GameObject setCustomer;
    public GameObject colliderforcustomer;
    bool changescreensize = false;
    GameObject currentcollider;
    bool customerDrag = false;
    float aditionalx = 0f;
    float aditionaly = 0f;
    bool singleexecute = false;
    public LayerMask mask;
    public bool delayclick = false;

    void Start()
    {
       
        gamemanager = Gamemanager.instance;
        cam = gamemanager.cam;
        edgeCollider = cam.GetComponent<EdgeCollider2D>(); 
        UpdateSizeCanvas();
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamemanager.quedialog.imagetutorial.transform.parent.parent.gameObject.active)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                DragAndDropFunction();
            }
          
        }
        if (screenSize != (Vector2)cam.ScreenToWorldPoint(cam.pixelRect.size))
        {
            UpdateSizeCanvas();
        }
        if(draggedObject != null)
        {
            CheckOutside();
        }
        if (Input.GetKeyDown(KeyCode.R) && (gamemanager.buyingobject || !gamemanager.gameStarting))
        {
            gamemanager.draggingbuying.transform.Rotate(new Vector3(0, 0, 90), 90);
            //rotate 90 deggre
        }

    }

    void DragAndDropFunction()
    {
        if (Input.GetMouseButton(0))
        {
            if (draggedObject == null)
            {
                if (delayclick)
                {
                    delayclick = false;
                    return;
                }
                RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,0f,mask);
                if (hit.collider != null)
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.tag.Equals("Notif") || hit.transform.gameObject.layer.Equals(5))
                    {
                        return;
                    }
                    if (gamemanager.gameStarting && (hit.transform.tag.Equals("Utensil") || hit.transform.tag.Equals("Food_Place")) || gamemanager.changedSize)
                    {
                        return;
                    }
                    if(!gamemanager.changePlace && !gamemanager.buyingobject && !gamemanager.gameStarting)
                    {
                        gamemanager.changePlace = true;
                        GameObject go_verification = Instantiate(gamemanager.verificationEdit);
                        go_verification.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { ButtonHandling.instance.AcceptChange(hit.transform.gameObject); });
                        go_verification.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { ButtonHandling.instance.DeleteChange(hit.transform.gameObject); });
                        gamemanager.spawnVerification = go_verification;
                        go_verification.transform.localPosition = hit.transform.localPosition + new Vector3(0, 0, 0);
                        gamemanager.draggingbuying = hit.transform.gameObject;
                        gamemanager.draggingbuying.GetComponent<SpriteRenderer>().sortingOrder = 3;
                        gamemanager.draggingbuying.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 60);
                        foreach (Transform childObj in gamemanager.draggingbuying.transform)
                        {
                            if (childObj.tag.Equals("Chair"))
                            {
                                childObj.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 60);
                            }
                        }
                        gamemanager.rotate_image.SetActive(true);
                        return;
                    }
                    if ((gamemanager.buyingobject || gamemanager.changePlace) && hit.transform.gameObject != gamemanager.draggingbuying)
                    {
                        return;
                    }
                    if (gamemanager.spawnVerification != null)
                    {
                        gamemanager.spawnVerification.SetActive(false);
                    }
                    draggedObject = hit.transform;
                    lastPosition = (Vector2)hit.transform.position;
                }
            }
            else
            {
                if (gamemanager.spawnVerification != null)
                {
                    gamemanager.spawnVerification.transform.localPosition = new Vector3(draggedObject.localPosition.x, draggedObject.localPosition.y, draggedObject.localPosition.z);
                }
            }
            
        }else if (Input.GetMouseButtonUp(0) && draggedObject != null)
        { 
            if(!objectOutside)
            {
                if (gamemanager.buyingobject || gamemanager.changePlace)
                {
                    draggedObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 60);
                    foreach (Transform childObj in draggedObject.transform)
                    {
                        if (childObj.tag.Equals("Chair"))
                        {
                            childObj.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 60);
                        }
                    }
                    if (gamemanager.spawnVerification != null)
                    {
                        gamemanager.spawnVerification.transform.localPosition = new Vector3(draggedObject.localPosition.x, draggedObject.localPosition.y, draggedObject.localPosition.z);
                        gamemanager.spawnVerification.SetActive(true);
                    }
                    draggedObject = null;
                    return;
                }
                else if (gamemanager.gameStarting)
                {
                    if (setCustomer != null)
                    {
                        //set chair and set all desk to prepare
                        SpriteHandler spritehand = setCustomer.GetComponent<SpriteHandler>();
                        CustomerHandler custHand = draggedObject.GetComponent<CustomerHandler>();
                        FoodHandling foodHand = draggedObject.GetComponent<FoodHandling>();
                        if (custHand != null)
                        {
                            spritehand.customer_sit = custHand.customer_list;
                            spritehand.customer_data = custHand.customer_data;
                            gamemanager.currentCustomer -= 1;
                            //reindexing customer
                            for (int i = 0; i < gamemanager.QueuePosition.Count; i++)
                            {
                                (Vector3, bool) data = gamemanager.QueuePosition[i];
                                gamemanager.QueuePosition[i] = (data.Item1, false);
                            }
                            gamemanager.reindexing = true;
                        }
                        else if (foodHand != null)
                        {
                            if (foodHand.isIngredient && ((Utensil)spritehand.dataSprite).utensil_type.Equals(UtensilType.Stove))
                            {
                                //set for cooking time
                                Ingredient ingredient = (Ingredient)foodHand.food_data;
                                spritehand.ingredientOn = ingredient;
                                spritehand.waitingtimeinsecond = ingredient.ingredient_cook_time;
                                spritehand.start_waiting_time = true;
                                spritehand.cookProseess = true;

                            }
                            else if (!foodHand.isIngredient && ((Utensil)spritehand.dataSprite).utensil_type.Equals(UtensilType.Desk))
                            {
                                for (int i = 0; i < spritehand.food_order.Count; i++)
                                {
                                    (FoodMenu, bool) datafromarray = spritehand.food_order[i];
                                    if (datafromarray.Item1.Equals(foodHand.food_data) && !datafromarray.Item2)
                                    {
                                        spritehand.food_order[i] = (datafromarray.Item1, true);
                                        break;
                                    }
                                }
                            }
                            if (foodHand.refrigenerator != null)
                            {
                                SpriteHandler refriFoodHandling = foodHand.refrigenerator.GetComponent<SpriteHandler>();
                                List<GameObject> gorefitem = refriFoodHandling.refriitem;
                                foreach (GameObject go in gorefitem)
                                {
                                    if (go == draggedObject.gameObject)
                                    {
                                        gorefitem.Remove(go);
                                        break;
                                    }
                                }
                                refriFoodHandling.refriitem = gorefitem;
                            }
                        }
                        setCustomer = null;
                        DestroyImmediate(draggedObject.gameObject);
                        draggedObject = null;
                        return;
                    }
                }
            }
            objectOutside = false; 
            draggedObject.transform.position = lastPosition;
            draggedObject.GetComponent<SpriteRenderer>().color = Color.white;
            lastPosition = Vector2.zero;
            foreach (Transform childObj in draggedObject.transform)
            {
                if (childObj.tag.Equals("Chair"))
                {
                    childObj.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            draggedObject = null;
        }
    }

    void UpdateSizeCanvas()
    {
        screenSize = cam.ScreenToWorldPoint(cam.pixelRect.size);
        gamemanager.floor.transform.localScale = new Vector3(1, 1, 1);
        //camera bound
        Vector2 lefttop = new Vector2(-screenSize.x,screenSize.y);
        Vector2 leftbottom = new Vector2(-screenSize.x, -screenSize.y);
        Vector2 righttop = new Vector2(screenSize.x, screenSize.y);
        Vector2 rightbottom = new Vector2(screenSize.x, -screenSize.y);
        edgeCollider.points = new[] { lefttop,leftbottom ,lefttop,righttop,righttop,rightbottom,leftbottom,rightbottom};
        if(currentcollider != null)
        {
            DestroyImmediate(currentcollider);
        }
        GameObject go = Instantiate(colliderforcustomer);
        float height = cam.orthographicSize * 2f;
        float width = height * screenSize.x / screenSize.y;
        go.transform.position = new Vector2(-screenSize.x + 2f, 0);
        go.GetComponent<BoxCollider2D>().size += new Vector2(0,height);
        var topRightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        var worldSpaceWidth = topRightCorner.x * 2;
        var worldSpaceHeight = topRightCorner.y * 2;

        var spriteSize = gamemanager.floor.GetComponent<SpriteRenderer>().bounds.size;
        var scaleFactorX = worldSpaceWidth / spriteSize.x;
        var scaleFactorY = worldSpaceHeight / spriteSize.y;

        gamemanager.floor.transform.localScale = new Vector3(scaleFactorX - 0.1f, scaleFactorY, 1);
        currentcollider = go;
        gamemanager.QueuePosition = new List<(Vector2, bool)>();
        Vector2 firstpos = new Vector2(-screenSize.x + 1, -screenSize.y + 1);
        aditionalx += 0.1f;
        aditionaly += 0.12f;
        for (int i = 0; i <= gamemanager.maxCustomer; i++)
        {
            gamemanager.QueuePosition.Add((firstpos + new Vector2(0.15f, (0.2f * i)) * (gamemanager.positionsize), false));

        }
    }

    void CheckOutside()
    {
        draggedObject.position = (Vector2)cam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D objectCollision = draggedObject.GetComponent<Collider2D>();
        if (objectCollision.IsTouchingLayers())
        {
            if (gamemanager.gameStarting)
            {
                try
                {
                    RaycastHit2D[] result = new RaycastHit2D[1];
                    objectCollision.Raycast(Vector2.zero, result);
                    if (result.Length > 0 && result[0] != null)
                    {
                        SpriteHandler spritehand = result[0].transform.GetComponent<SpriteHandler>();
                        if (spritehand != null)
                        {
                            if (spritehand.dataSprite.GetType().Equals(typeof(Utensil)))
                            {
                                Utensil data = (Utensil)spritehand.dataSprite;
                                CustomerHandler customehandler = draggedObject.transform.GetComponent<CustomerHandler>();
                                FoodHandling foodHandler = draggedObject.transform.GetComponent<FoodHandling>();
                                if (customehandler != null)
                                {
                                    customerDrag = true;
                                    if (customehandler.customer_list > data.utensil_max_use || spritehand.chairSetup)
                                    {
                                        objectOutside = true;
                                        draggedObject.GetComponent<SpriteRenderer>().color = Color.red;
                                    }
                                    else
                                    {
                                        objectOutside = false;
                                        setCustomer = result[0].transform.gameObject;
                                        draggedObject.GetComponent<SpriteRenderer>().color = Color.green;
                                    }
                                }else if(foodHandler != null)
                                {
                                    #region Cooking Process
                                    if (foodHandler.isIngredient && data.utensil_type.Equals(UtensilType.Stove))
                                    {
                                        if(spritehand.finalProduct == null)
                                        {
                                            if (!spritehand.cookProseess)
                                            {
                                                List<FoodMenu> selectedRecipe = new List<FoodMenu>();
                                                bool firstCook = false;
                                                foreach (FoodMenu listfood in gamemanager.foodSelect)
                                                {
                                                    if (spritehand.storeIngredient.Count == 0)
                                                    {
                                                        if (foodHandler.food_data.Equals(listfood.ingredient_food[0]))
                                                        {
                                                            selectedRecipe.Add(listfood);
                                                            firstCook = true;
                                                            break;
                                                        }   
                                                    }
                                                    else
                                                    {
                                                        List<Ingredient> temporaryCheck = new List<Ingredient>();
                                                        int index = 0;
                                                        //list of store find convert all
                                                        List<ScriptableObject> listof_store = spritehand.storeIngredient;
                                                        foreach (Ingredient ingredient in listfood.ingredient_food)
                                                        {
                                                            if(index == listof_store.Count)
                                                            {
                                                                break;
                                                            }
                                                            if (listof_store[index] == ingredient || listof_store[index] == ingredient.finish_ingredient_object)
                                                            {
                                                                temporaryCheck.Add(ingredient);
                                                                index++;
                                                            }
                                                            else
                                                            {
                                                                break;
                                                            }
                                                        }
                                                        if (temporaryCheck.Count == spritehand.storeIngredient.Count)
                                                        {
                                                            selectedRecipe.Add(listfood);
                                                        }
                                                    }

                                                }
                                                if (selectedRecipe.Count > 0)
                                                {
                                                    if (!firstCook)
                                                    {
                                                        FoodMenu fixRecipe = null;
                                                        foreach (FoodMenu listfood in selectedRecipe)
                                                        {
                                                            if (foodHandler.food_data.Equals(listfood.ingredient_food[spritehand.cookStep]))
                                                            {
                                                                fixRecipe = listfood;
                                                                break;
                                                            }
                                                        }
                                                        if (fixRecipe == null)
                                                        {
                                                            draggedObject.GetComponent<SpriteRenderer>().color = Color.red;
                                                            return;
                                                        }

                                                    }
                                                    setCustomer = result[0].transform.gameObject;
                                                    draggedObject.GetComponent<SpriteRenderer>().color = Color.green;
                                                    objectOutside = false;
                                                    return;
                                                }
                                                else
                                                {
                                                    objectOutside = true;
                                                    draggedObject.GetComponent<SpriteRenderer>().color = Color.red;
                                                    return;
                                                }

                                            }
                                            else
                                            {
                                                objectOutside = true;
                                                draggedObject.GetComponent<SpriteRenderer>().color = Color.red;
                                            }
                                        }
                                        else
                                        {
                                            objectOutside = true;
                                            draggedObject.GetComponent<SpriteRenderer>().color = Color.red;
                                        }

                                        return;
                                    }
                                    #endregion
                                    #region Trash can
                                    else if (data.utensil_type.Equals(UtensilType.Trash))
                                    {
                                        setCustomer = result[0].transform.gameObject;
                                        objectOutside = false;
                                        draggedObject.GetComponent<SpriteRenderer>().color = Color.green;
                                        return;
                                    }
                                    else if(!foodHandler.isIngredient && data.utensil_type.Equals(UtensilType.Desk))    
                                    {
                                        #region Serving Food Customer
                                        bool containfood = false;
                                        foreach ((FoodMenu, bool) foodmenu in spritehand.food_order)
                                        {
                                            if (foodmenu.Item1 == foodHandler.food_data && !foodmenu.Item2)
                                            {
                                                containfood = true;
                                                break;
                                            }
                                        }
                                        if (containfood)
                                        {
                                            objectOutside = false;
                                            setCustomer = result[0].transform.gameObject;
                                            draggedObject.GetComponent<SpriteRenderer>().color = Color.green;
                                        }
                                        else
                                        {
                                            objectOutside = true;
                                            draggedObject.GetComponent<SpriteRenderer>().color = Color.red;
                                        }
                                        #endregion
                                    }
                                    #endregion

                                }
                            }
                        }
                    }
                }catch(Exception e)
                {
                }
                return;
            }else if (objectCollision.callbackLayers.value.Equals(5))
            {
                return;
            }
            else
            {
                objectOutside = true;
                draggedObject.GetComponent<SpriteRenderer>().color = Color.red;
                foreach (Transform childObj in objectCollision.transform)
                {
                    if (childObj.tag.Equals("Chair"))
                    {
                        childObj.GetComponent<SpriteRenderer>().color = Color.red;
                    }
                }
            }
        }
        else
        {
            currentMousePosition = Input.mousePosition;
            objectOutside = false;
            setCustomer = null;
            if (gamemanager.gameStarting)
            {
                draggedObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
            else
            {
                draggedObject.GetComponent<SpriteRenderer>().color = Color.green;
            }
            if(objectCollision.transform.childCount > 0)
            {
                foreach (Transform childObj in objectCollision.transform)
                {
                    if (childObj.tag.Equals("Chair"))
                    {
                        childObj.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                }
            }
           
        }

    }
}
