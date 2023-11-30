using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "FoodData", menuName = "Data/FoodMenu", order = 2)]
public class FoodMenu : ScriptableObject
{
    public string food_name;
    public Sprite food_img;
    public int foodPrice;
    public int foodGachaOdds;
    public List<Ingredient> ingredient_food = new List<Ingredient>();
    public float timerReduce;
    public bool isCraftable;

}
