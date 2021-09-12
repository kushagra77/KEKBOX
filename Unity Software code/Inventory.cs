using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public List<Item> items;

    void Start()
    {
        // Initialise storage with 20 of each item type
        items = new List<Item>();
        items.Add(new Item("Meal 1", 20));
        items.Add(new Item("Meal 2", 20));
        items.Add(new Item("Meal 3", 20));
        items.Add(new Item("Water Bottle", 20));
        items.Add(new Item("Blankets", 20));
    }



    // Check if an item is currently in stock
    public bool RequestItem(string item)
    {
        // Linear search through all the items
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
