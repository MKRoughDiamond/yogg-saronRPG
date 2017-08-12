using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLogic : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public GameObject battleLogicPrefab;
    public List<GameObject> enemyPrefabs;
    public Character player;

    private GameObject currentBackground;
    private BattleLogic currentBattle;

    void Start()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        currentBackground = Instantiate(backgroundPrefab);
        currentBackground.transform.position = new Vector3(0f, 0f, 5f);

        yield return StartCoroutine(PlayerWalkInAnimation());

        while (true)
        {
            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(MakeRoom());

            currentBattle.StartBattle();
            while (!currentBattle.battleFinished)
                yield return null;

            if (player.health <= 0)
                break;
        }
    }

    private IEnumerator MakeRoom()
    {
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
            enemies[i].transform.parent = currentBackground.transform;
            enemies[i].transform.localPosition = new Vector3(3.5f, -3f + 3f * i, 0f);
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
}
