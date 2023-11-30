using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "UtensilData", menuName = "Data/Utensil", order = 4)]
public class Utensil : ScriptableObject
{
    public string utensil_name;
    public Sprite utensil_img;
    public int utensil_price;
    public float utensil_mutliple;
    public UtensilType utensil_type;
    public int utensil_max_use;
    public Utensil utensil_placing;
    public bool isGround;
    public List<Ingredient> cookStep = new List<Ingredient>();
    public GameObject utensil_gameobject;
}

public enum UtensilType
{
    Stove,Chopper,Pan,Desk,Chair,Pot,Wok,Fridge,Kitchen_Desk,Trash,Service_Desk,Coffee_maker
}