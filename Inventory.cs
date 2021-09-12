using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public List<Item> items;

    void Start()
    {
        items = new List<Item>();
        items.Add(new Item("Meal 1", 20));
        items.Add(new Item("Meal 2", 20));
        items.Add(new Item("Meal 3", 20));
        items.Add(new Item("Water Bottle", 20));
        items.Add(new Item("Blankets", 20));
    }




    public bool RequestItem(string item)
    {
        for(int i = 0; i < items.Count; i++)
        {
            if (item == items[i].GetName())
            {
                if (items[i].GetNum() > 0)
                {
                    items[i].Decrease();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }
}
