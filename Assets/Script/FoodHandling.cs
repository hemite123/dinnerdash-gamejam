using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodHandling : MonoBehaviour
{
    public ScriptableObject food_data;
    public bool isIngredient = false;
    bool settted = false;
    public GameObject refrigenerator;

    private void Update()
    {
        if(food_data != null && !settted)
        {
            settted = true;
            if (food_data.GetType().Equals(typeof(FoodMenu)))
            {
                gameObject.GetComponent<SpriteRenderer>().sprite =((FoodMenu)food_data).food_img;
            }else if (food_data.GetType().Equals(typeof(Ingredient)))
            {
                isIngredient = true;
                gameObject.GetComponent<SpriteRenderer>().sprite = ((Ingredient)food_data).ingredient_img;
            }
            
        }
        
    }

}
