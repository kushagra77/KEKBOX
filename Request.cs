using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Request
{
    GameObject destination;
    string item;

    // Initialiser for Request
    public Request(GameObject _destination, string _item)
    {
        destination = _destination;
        item = _item;
    }

    public GameObject GetDestination()
    {
        return destination;
    }

    public string GetItem()
    {
        return item;
    }
}
