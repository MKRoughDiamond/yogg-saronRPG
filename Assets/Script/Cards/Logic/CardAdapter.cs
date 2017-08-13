using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAdapter : MonoBehaviour
{
    [SerializeField]
    private ICard card;
    public ICard Card
    {
        get
        {
            return card;
        }
    }
}
