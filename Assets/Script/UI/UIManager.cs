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
    public Text yoggStack;
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

    public static UIManager GetInstance()
    {
        return instance;
    }

    public IEnumerator PlayerCardChoice(List<int> hand)
    {
        instance.chosenCard = -1;
        instance.callYS = false;
        CanvasAdapter.InfoBarRoot.gameObject.SetActive(false);
        instance.hideButton.gameObject.SetActive(true);
        instance.yoggButton.gameObject.SetActive(true);

        GameObject[] cos = new GameObject[3];
        Vector3 right_lower = new Vector3(8.1f, -4.4f, -0.5f);
        for (int i = 0; i < hand.Count; i++)
        {
            cos[i] = Instantiate(CardIndexManager.CardIndex[hand[i]]);
            cos[i].transform.parent = instance.shownCardRoot;
            cos[i].transform.position = right_lower;
            cos[i].transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        }
        StartCoroutine(CardAnimation(cos[0], new Vector3(-5f, 0f, -0.4f), new Vector3(1f, 1f, 1f)));
        StartCoroutine(CardAnimation(cos[2], new Vector3(5f, 0f, -0.5f), new Vector3(1f, 1f, 1f)));
        yield return StartCoroutine(CardAnimation(cos[1], new Vector3(0f, 0f, -0.4f), new Vector3(1f, 1f, 1f)));

        instance.isWaitingForCardChoice = true;
        while (instance.isWaitingForCardChoice)
            yield return null;

        Vector3 left_lower = new Vector3(-9f, -5f, -0.5f);
        StartCoroutine(CardAnimation(cos[0], left_lower, new Vector3(0.6f, 0.6f, 1f)));
        StartCoroutine(CardAnimation(cos[2], left_lower, new Vector3(0.6f, 0.6f, 1f)));
        yield return StartCoroutine(CardAnimation(cos[1], left_lower, new Vector3(0.6f, 0.6f, 1f)));

        for (int i = 0; i < hand.Count; i++)
            Destroy(cos[i]);

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
        CanvasAdapter.SelectTarget.gameObject.SetActive(true);
        while (instance.isWaitingForTargetChoice)
            yield return null;
        CanvasAdapter.SelectTarget.gameObject.SetActive(false);
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

    public static void UpdateYoggStack(int count)
    {
        instance.yoggStack.text = "" + count + " / 5";
    }

    private IEnumerator CardAnimation(GameObject card, Vector3 target_pos, Vector3 target_scale)
    {
        float t = 0f;
        Vector3 original_pos = card.transform.position;
        Vector3 original_scale = card.transform.localScale;
        while (card.transform.position != target_pos || card.transform.localScale != target_scale)
        {
            t += 8f * Time.deltaTime;
            card.transform.position = Vector3.Lerp(original_pos, target_pos, t);
            card.transform.localScale = Vector3.Lerp(original_scale, target_scale, t);
            yield return new WaitForEndOfFrame();
        }
    }
}
