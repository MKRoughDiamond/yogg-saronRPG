using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text deckCount;
    public Button hideButton;
    public Button yoggButton;
    public Transform shownCardRoot;

    private static UIManager instance;

    private bool isWaitingForCardChoice;
    private int chosenCard;
    private bool callYS;

    private bool isWaitingForTargetChoice;
    private Character[] chosenTarget;
    private int chooseCount;

    void Awake()
    {
        instance = this;
        isWaitingForCardChoice = false;
    }

    void Update()
    {
        if ((isWaitingForCardChoice || isWaitingForTargetChoice) && Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (isWaitingForCardChoice)
                {
                    if (hit.transform.tag == "Card")
                    {
                        chosenCard = hit.transform.GetComponent<CardAdapter>().Card.GetID();
                        isWaitingForCardChoice = false;
                    }
                    else if (hit.transform.tag == "CallYS")
                    {
                        callYS = true;
                        isWaitingForCardChoice = false;
                    }
                }
                else if (isWaitingForTargetChoice)
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        Character t = hit.transform.GetComponent<Character>();
                        if (!t.check.activeSelf)
                        {
                            chosenTarget[chooseCount++] = t;
                            t.check.SetActive(true);
                            if (chooseCount == chosenTarget.Length)
                                isWaitingForTargetChoice = false;
                        }
                        else
                        {
                            chosenTarget[chooseCount--] = null;
                            t.check.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public static IEnumerator PlayerCardChoice(List<int> hand)
    {
        instance.chosenCard = -1;
        instance.callYS = false;
        CanvasAdapter.InfoBarRoot.gameObject.SetActive(false);
        instance.hideButton.gameObject.SetActive(true);
        instance.yoggButton.gameObject.SetActive(true);

        GameObject[] cos = new GameObject[3];
        for (int i = 0; i < hand.Count; i++)
            cos[i] = Instantiate(CardIndexManager.CardIndex[hand[i]], new Vector3(-5f + i * 5f, 0f, -1f), new Quaternion(), instance.shownCardRoot);

        instance.isWaitingForCardChoice = true;
        while (instance.isWaitingForCardChoice)
            yield return null;

        for (int i = 0; i < hand.Count; i++)
        {
            Destroy(cos[i]);
        }
        CanvasAdapter.InfoBarRoot.gameObject.SetActive(true);
        instance.hideButton.gameObject.SetActive(false);
        instance.yoggButton.gameObject.SetActive(false);
    }

    public static int GetChosenCard(out bool call_YS)
    {
        call_YS = instance.callYS;
        return instance.chosenCard;
    }

    public void OnHideButtonClick()
    {
        shownCardRoot.gameObject.SetActive(!shownCardRoot.gameObject.activeSelf);
        CanvasAdapter.InfoBarRoot.gameObject.SetActive(!shownCardRoot.gameObject.activeSelf);
    }

    public void OnYoggButtonClick()
    {
        callYS = true;
        isWaitingForCardChoice = false;
    }

    public static IEnumerator PlayerCardTargetChoice(int count)
    {
        instance.chosenTarget = new Character[count];
        instance.chooseCount = 0;
        instance.isWaitingForTargetChoice = true;
        while (instance.isWaitingForTargetChoice)
            yield return null;
    }

    public static Character[] GetChosenTarget()
    {
        foreach (Character t in instance.chosenTarget)
            t.check.SetActive(false);
        return instance.chosenTarget;
    }

    public static void UpdateDeckCount(int count)
    {
        instance.deckCount.text = "" + count;
    }
}
