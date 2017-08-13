using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardIndexManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> cardIndex;
    public static List<GameObject> CardIndex
    {
        get
        {
            if (instance != null)
                return instance.cardIndex;
            else
                return null;
        }
    }

    private static CardIndexManager instance;

    void Awake()
    {
        instance = this;
    }

    public static ICard GetCardByID(int id)
    {
        return instance.cardIndex[id].GetComponent<CardAdapter>().Card;
    }
}
