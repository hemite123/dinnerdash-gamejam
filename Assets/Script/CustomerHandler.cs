using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerHandler : MonoBehaviour
{
    public List<Customer> customer_data = new List<Customer>();
    public int customer_list = 0;
    public float totalCustomerWaitAngry;
    public float currentCustomerWaitAngry;
    public string color = "green";
    bool allset = false;
    Gamemanager gamemanager;

    private void Start()
    {
        gamemanager = Gamemanager.instance;
        //generate new customer
    }

    private void Update()
    {
        if(totalCustomerWaitAngry >= 0 && allset)
        {
            currentCustomerWaitAngry -= Time.deltaTime;
            if(currentCustomerWaitAngry <= totalCustomerWaitAngry / 2 && color == "green")
            {
                color = "yellow";
                transform.Find("emot_sprite").GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else if (currentCustomerWaitAngry <= totalCustomerWaitAngry / 4 && color == "yellow")
            {
                color = "red";
                transform.Find("emot_sprite").GetComponent<SpriteRenderer>().color = Color.red;
            }
            if (currentCustomerWaitAngry <= 0)
            {
                gamemanager.angrycustomer += 1;
                gamemanager.totalcustomerinfield -= 1;
                gamemanager.currentCustomer -= 1;
                for (int i = 0; i < gamemanager.QueuePosition.Count; i++)
                {
                    (Vector3, bool) data = gamemanager.QueuePosition[i];
                    gamemanager.QueuePosition[i] = (data.Item1, false);
                }
                gamemanager.reindexing = true;
                DestroyImmediate(this.gameObject);
            }
            
        }
        if(customer_data.Count <= 0)
        {
            for (int i = 0; i < customer_list; i++)
            {
                float odds = 0;
                foreach (Customer customer in gamemanager.customer_data)
                {
                    odds += customer.CustomerSpawnRarity;
                }
                float random = Random.Range(1f, odds);
                float top = 0f;
                foreach (Customer customer in gamemanager.customer_data)
                {
                    top += customer.CustomerSpawnRarity;
                    if (random <= top)
                    {
                        customer_data.Add(customer);
                        totalCustomerWaitAngry += customer.CustomerAngryWaitingTime;
                        break;
                    }
                }
            }
            currentCustomerWaitAngry = totalCustomerWaitAngry;
            allset = true;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.GetComponent<SpriteHandler>())
        {
            SpriteHandler spritehand = collision.transform.GetComponent<SpriteHandler>();
            Utensil utensil = (Utensil) spritehand.dataSprite;
            if(customer_list <= utensil.utensil_max_use && spritehand.chairSetup)
            {
                spritehand.customer_sit = customer_list;
                //put the customer at point
            }
            else
            {
                
                //change customer color to red 
            }
        }
    }
}
