using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text deckCount;

    public static bool isAnimating;

    private static UIManager instance;

    private bool isWaitingForCardChoice;
    private int chosenCard;
    private bool callYS;

    void Awake()
    {
        instance = this;
        isWaitingForCardChoice = false;
    }

    void Update()
    {
        if (isWaitingForCardChoice && Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.transform.tag == "Card")
                {
                    chosenCard = hit.transform.GetComponent<CardAdapter>().Card.ID;
                    isWaitingForCardChoice = false;
                }
                else if (hit.transform.tag == "CallYS")
                {
                    callYS = true;
                    isWaitingForCardChoice = false;
                }
            }
        }
    }

    public static IEnumerator PlayerCardChoice(List<int> hand)
    {
        instance.chosenCard = -1;

        GameObject[] cos = new GameObject[3];
        for (int i = 0; i < hand.Count; i++)
        {
            cos[i] = Instantiate(CardIndexManager.CardIndex[hand[i]]);
            cos[i].transform.position = new Vector3(-5f + i * 5f, 0f, -1f);
        }

        instance.isWaitingForCardChoice = true;
        while (instance.isWaitingForCardChoice)
            yield return null;

        for (int i = 0; i < hand.Count; i++)
        {
            Destroy(cos[i]);
        }
    }

    public static int GetChosenCard(out bool call_YS)
    {
        call_YS = instance.callYS;
        return instance.chosenCard;
    }

    public static void PlayerCardTargetChoice(int count, out List<Character> targets)
    {
        targets = new List<Character>();
    }

    public static void UpdateDeckCount(int count)
    {
        instance.deckCount.text = "" + count;
    }
}
