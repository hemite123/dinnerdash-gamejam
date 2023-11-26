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
    public List<SpriteRenderer> sr = new List<SpriteRenderer>();
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
                int index = 0;
                foreach (Customer customer in customer_data)
                {
                    sr[index].sprite = customer.customer_img.Find(x => x.action == "halfangidle").img_sprite;
                    index++;
                }
                
            }
            else if (currentCustomerWaitAngry <= totalCustomerWaitAngry / 4 && color == "yellow")
            {
                color = "red";
                int index = 0;
                foreach (Customer customer in customer_data)
                {
                    sr[index].sprite = customer.customer_img.Find(x => x.action == "angidle").img_sprite;
                    index++;
                }
                
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
                gamemanager.customer_spawn.Remove(this);
                DestroyImmediate(this.gameObject);
                gamemanager.reindexing = true;
                
            }
            
        }
        if(customer_data.Count <= 0)
        {
            //make mapping for grid
            StartCoroutine(findCustomer());
        }
    }

    IEnumerator findCustomer()
    {
        float firstspawn = 0;
        if (customer_list > 1)
        {
            firstspawn = -(0.3f * (float)(customer_list - 1));
        }
        for (int i = 0; i < customer_list; i++)
        {
            float odds = 0;
            foreach (Customer customer in gamemanager.customer_data)
            {
                odds += customer.CustomerSpawnRarity;
            }
            bool get = false;
            while (!get)
            {
                float random = Random.Range(1f, odds);
                float top = 0f;
                foreach (Customer customer in gamemanager.customer_data)
                {
                    top += customer.CustomerSpawnRarity;
                    if (random <= top && gamemanager.current_customer_serving / gamemanager.one_star >= customer.ratingJoin)
                    {
                        customer_data.Add(customer);
                        GameObject spritespawn = new GameObject();
                        spritespawn.transform.parent = transform;
                        SpriteRenderer srclone = spritespawn.AddComponent<SpriteRenderer>();
                        srclone.sprite = customer.customer_img.Find(x => x.action == "idle").img_sprite;
                        sr.Add(srclone);
                        spritespawn.transform.localScale = new Vector3(1, 1, 1);
                        spritespawn.transform.localPosition = new Vector3(firstspawn, 0, 0);
                        firstspawn += 0.6f;
                        totalCustomerWaitAngry += customer.CustomerAngryWaitingTime;
                        get = true;
                        break;
                    }
                }
                yield return null;
            }
            
        }
        currentCustomerWaitAngry = totalCustomerWaitAngry;
        allset = true;
        gamemanager.reindexing = true;
    }

}
