using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "IngredientData", menuName = "Data/Ingredient", order = 3)]
public class Ingredient : ScriptableObject
{
    public string ingredient_name;
    public Sprite ingredient_img;
    public float ingredient_cook_time;
    public ScriptableObject finish_ingredient_object;
    public bool isCookingIngredient;
    //list utensil(for step final cook)
}
