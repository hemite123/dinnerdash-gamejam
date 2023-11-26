using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
[CreateAssetMenu(fileName ="CustomerData",menuName ="Data/Customer",order =1)]
public class Customer : ScriptableObject,IComparable<Customer>
{
    public string customer_name;
    public List<SpriteHandling> customer_img = new List<SpriteHandling>();
    public CustomerType customerType;
    public int minSalary;
    public int maxSalary;
    public float CustomerSpawnRarity;
    public float CustomerAngryTime;
    public float CustomerAngryWaitingTime;
    public float ratingJoin;

   

    public int CompareTo(Customer other)
    {
        if (this.CustomerSpawnRarity < other.CustomerSpawnRarity)
        {
            return 0;
        }
        else if (this.CustomerSpawnRarity > other.CustomerSpawnRarity)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}

public enum CustomerType
{
    VIP,NORMAL,BRANDED,CRAZYRICH
}

[System.Serializable]
public struct SpriteHandling
{
    public string action;
    public Sprite img_sprite;
    public bool isAnimation;
    public Animation animation;
}