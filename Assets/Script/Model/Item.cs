using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item", order = 5)]
public class Item : ScriptableObject
{
    public string itemname;
    public int price_item;
    public Sprite item_img;
    public float increase_chance_spawn;
    public float increase_chance_customer;

}
