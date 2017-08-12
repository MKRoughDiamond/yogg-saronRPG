using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<int> startDeck;

    private List<int> deck;

    private static DeckManager instance;

    void Awake()
    {
        instance = this;
        deck = new List<int>(5);
        deck.AddRange(startDeck);
    }

    public static List<int> GetPlayDeck()
    {
        List<int> d = new List<int>(15);
        foreach (int c in instance.deck)
        {
            d.Add(c);
            d.Add(c);
            d.Add(c);
        }
        return d;
    }
}
