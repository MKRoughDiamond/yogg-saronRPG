using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAdapter : MonoBehaviour
{
    [SerializeField]
    private Card card;
    public Card Card
    {
        get
        {
            return card;
        }
    }
}
