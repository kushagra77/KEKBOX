using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Item
{
    public string name;
    public int num;

    // Item initialiser
    public Item(string _name, int _num)
    {
        name = _name;
        num = _num;
    }

    public void Decrease()
    {
        num--;
    }

    public int GetNum()
    {
        return num;
    }
    public string GetName()
    {
        return name;
    }


}
