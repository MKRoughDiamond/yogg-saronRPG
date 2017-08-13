using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainLogic : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public GameObject battleLogicPrefab;
    public List<GameObject> enemyPrefabs;
    public Character player;

    public GameObject chestPrefab;

    private GameObject currentBackground;
    private BattleLogic currentBattle;

    private int roomCount;

    void Start()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        currentBackground = Instantiate(backgroundPrefab);
        currentBackground.transform.position = new Vector3(0f, 0f, 5f);

        Instantiate(chestPrefab).transform.parent = currentBackground.transform;

        yield return StartCoroutine(PlayerWalkInAnimation());

        if (Input.GetAxis("Horizontal") == 0f)
        {
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(FirstCardLoot());
        }

        while (true)
        {
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(MakeRoom());

            currentBattle.StartBattle();
            while (!currentBattle.battleFinished)
                yield return null;

            if (player.health > 0)
            {
                yield return StartCoroutine(ScreenShowAnimation(CanvasAdapter.VictoryScreen));
                while (!Input.GetMouseButtonDown(0))
                    yield return null;
                CanvasAdapter.VictoryScreen.gameObject.SetActive(false);
            }
            else
            {
                CanvasAdapter.RateText.text = "#" + (roomCount - 1);
                yield return StartCoroutine(ScreenShowAnimation(CanvasAdapter.DefeatScreen));
                while (!Input.GetMouseButtonDown(0))
                    yield return null;
                CanvasAdapter.DefeatScreen.gameObject.SetActive(false);
                break;
            }
        }

        Application.Quit();
    }

    private IEnumerator FirstCardLoot()
    {
        CanvasAdapter.Deck.gameObject.SetActive(true);
        int card_count = 0;
        for (int i = 0; i < 5; i++)
        {
            ICard[] three_card = new ICard[3];
            for (int j = 0; j < 3; j++)
            {
                three_card[j] = Instantiate(CardIndexManager.CardIndex[i]).GetComponent<CardAdapter>().Card;
                three_card[j].transform.position = new Vector3(0f, 0f, -0.5f);
                three_card[j].transform.localScale = new Vector3(0.1f, 0.1f, -0.5f);
                yield return null;
            }
            StartCoroutine(CardAnimation(three_card[0], new Vector3(-5f, 0f, -0.4f), new Vector3(1f, 1f, 1f)));
            StartCoroutine(CardAnimation(three_card[2], new Vector3(5f, 0f, -0.5f), new Vector3(1f, 1f, 1f)));
            yield return StartCoroutine(CardAnimation(three_card[1], new Vector3(0f, 0f, -0.4f), new Vector3(1.5f, 1.5f, 1f)));

            yield return new WaitForFixedUpdate();
            while (!Input.GetMouseButtonDown(0))
                yield return null;

            Vector3 right_lower = new Vector3(8.1f, -4.4f, -0.5f);
            StartCoroutine(CardAnimation(three_card[0], right_lower, new Vector3(0.6f, 0.6f, 1f)));
            StartCoroutine(CardAnimation(three_card[1], right_lower, new Vector3(0.6f, 0.6f, 1f)));
            yield return StartCoroutine(CardAnimation(three_card[2], right_lower, new Vector3(0.6f, 0.6f, 1f)));

            yield return new WaitForFixedUpdate();

            Destroy(three_card[0].gameObject);
            Destroy(three_card[1].gameObject);
            Destroy(three_card[2].gameObject);

            card_count += 3;
            UIManager.UpdateDeckCount(card_count);
        }
        yield return new WaitForFixedUpdate();
        while (!Input.GetMouseButtonDown(0))
            yield return null;
        yield return new WaitForSeconds(0.5f);
        CanvasAdapter.Deck.gameObject.SetActive(false);
    }

    private IEnumerator MakeRoom()
    {
        roomCount++;
        CanvasAdapter.BattleStart.text = "방 " + roomCount;
        GameObject old_bg = currentBackground;
        currentBackground = Instantiate(backgroundPrefab);
        currentBackground.transform.position = new Vector3(19.2f, 0f, 5f);

        GameObject blo = Instantiate(battleLogicPrefab);
        currentBattle = blo.GetComponent<BattleLogic>();
        currentBattle.player = player;

        Character[] enemies = new Character[3];
        for (int i = 0; i < 3; i++)
        {
            enemies[i] = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count - 1)]).GetComponent<Character>();
            enemies[i].maxHealth = 2 + roomCount;
            enemies[i].health = enemies[i].maxHealth;
            enemies[i].transform.parent = currentBackground.transform;
            enemies[i].transform.localPosition = new Vector3(3.5f, -3f + 3f * i, -0.2f);
        }
        currentBattle.enemies = new List<Character>(enemies);

        yield return StartCoroutine(BackgroundChangeAnimation(old_bg, currentBackground));

        Destroy(old_bg);
    }

    private IEnumerator PlayerWalkInAnimation()
    {
        float t = 0f;
        Vector3 original = player.transform.position;

        while (player.transform.position.x < -5.1f)
        {
            t += 0.5f * Time.deltaTime;
            player.transform.position = Vector3.Lerp(original, new Vector3(-5.1f, 0f, 0f), t);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator BackgroundChangeAnimation(GameObject before, GameObject current)
    {
        float t = 0f;
        while (current.transform.position.x > 0f)
        {
            t += 0.5f * Time.deltaTime;
            before.transform.position = Vector3.Lerp(new Vector3(0f, 0f, 5f), new Vector3(-19.2f, 0f, 5f), t);
            current.transform.position = Vector3.Lerp(new Vector3(19.2f, 0f, 5f), new Vector3(0f, 0f, 5f), t);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator CardAnimation(ICard card, Vector3 target_pos, Vector3 target_scale)
    {
        float t = 0f;
        Vector3 original_pos = card.transform.position;
        Vector3 original_scale = card.transform.localScale;
        while (card.transform.position != target_pos || card.transform.localScale != target_scale)
        {
            t += 2f * Time.deltaTime;
            card.transform.position = Vector3.Lerp(original_pos, target_pos, t);
            card.transform.localScale = Vector3.Lerp(original_scale, target_scale, t);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator ScreenShowAnimation(Image screen)
    {
        screen.gameObject.SetActive(true);
        Color original = screen.color;
        screen.color = Color.clear;

        float t = 0f;
        while (screen.color.a != 1)
        {
            t += 1f * Time.deltaTime;
            screen.color = Color.Lerp(screen.color, original, t);
            yield return new WaitForEndOfFrame();
        }
        t = 0f;
    }
}
