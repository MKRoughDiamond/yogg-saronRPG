using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleLogic : MonoBehaviour
{
    public Character player;
    public List<Character> enemies;
    public GameObject tempEffectPrefab;

    private int turnCount;
    public int TurnCount
    {
        get
        {
            return turnCount;
        }
    }
    private List<int> deck;
    private List<int> hand;
    private List<IPray> prayPool;
    private int prayStack;

    private bool pause;

    private bool _kill = false;

    void Awake()
    {
        turnCount = 0;
        pause = false;
        hand = new List<int>(3);
        prayPool = new List<IPray>();
    }

    void Start()
    {
        deck = DeckManager.GetPlayDeck();
        StartCoroutine(TurnRunner());
    }

    private IEnumerator TurnRunner()
    {
        while (!_kill)
        {
            if (!pause)
                yield return StartCoroutine(Turn());
            else
                yield return null;
        }
    }

    private IEnumerator Turn()
    {
        turnCount++;
        BuffCheck();
        print("Turn: " + turnCount);
        print(player.name + " " + player.health);
        foreach (Character c in enemies)
            print(c.name + " " + c.health);
        yield return StartCoroutine(PlayerTurn());
        yield return StartCoroutine(EnemyTurn());
        yield return new WaitForSeconds(3f);
    }

    private void BuffCheck()
    {
        player.CheckBuff(turnCount);
        foreach (Character c in enemies)
            c.CheckBuff(turnCount);
    }

    private IEnumerator PlayerTurn()
    {
        hand.Clear();
        if (deck.Count == 0)
            deck = DeckManager.GetPlayDeck();
        for (int i = 0; i < 3; i++)
        {
            int c = deck[Random.Range(0, deck.Count - 1)];
            hand.Add(c);
            deck.Remove(c);
        }
        UIManager.UpdateDeckCount(deck.Count);

        yield return StartCoroutine(UIManager.PlayerCardChoice(hand));

        bool call_YS;
        int cardid = UIManager.GetChosenCard(out call_YS);

        if (!call_YS)
        {
            if (cardid != -1)
            {
                ICard card = CardIndexManager.GetCardByID(cardid);
                int target_count = card.GetTargetCount();
                if (target_count > 0)
                {
                    yield return StartCoroutine(UIManager.PlayerCardTargetChoice(target_count));

                    Character[] targets = UIManager.GetChosenTarget(); ;
                    IPray[] pray;
                    card.ResolveEffect(this, targets, out pray);
                    prayPool.AddRange(pray);
                }
                else
                {
                    IPray[] pray;
                    card.ResolveEffect(this, null, out pray);
                    prayPool.AddRange(pray);
                }
                print(card);
                print(card.GetDescription());
            }
        }
        else
        {
            yield return StartCoroutine(CallYS());
        }
    }

    private IEnumerator CallYS()
    {
        foreach (IPray p in prayPool)
        {
            p.ResolvePray(this);
            yield return null;
        }
    }

    private IEnumerator EnemyTurn()
    {
        foreach (Character e in enemies)
        {
            Attack(e, player);
            yield return null;
        }
    }

    public void Buff(Buff buff, Character to)
    {
        to.AddBuff(buff);
        print("Buff " + buff.name + " to " + to.name);
        StartCoroutine(DestroyTempEffect(Instantiate(tempEffectPrefab, to.transform.position + new Vector3(0f, 0f, -0.2f), new Quaternion())));
    }

    public void Buff(Buff buff, Character[] to)
    {
        foreach (Character c in to)
            Buff(buff, c);
    }

    public void Heal(int heal, Character to)
    {
        int before = to.health;
        to.health += heal;
        if (to.health > to.maxHealth)
            to.health = to.maxHealth;
        print("Heal " + heal + " to " + to.name + " " + before + " -> " + to.health);
        StartCoroutine(DestroyTempEffect(Instantiate(tempEffectPrefab, to.transform.position + new Vector3(0f, 0f, -0.2f), new Quaternion())));
    }

    public void Heal(int heal, Character[] to)
    {
        foreach (Character c in to)
            Heal(heal, c);
    }

    public bool Attack(Character from, Character to)
    {
        int buffed_damage = from.damage;
        bool cannot_attack = false;
        bool confused = false;

        print(from.name + " tries to attack " + to.name);

        if (cannot_attack)
        {
            print(from.name + " cannot attack");
            return false;
        }
        foreach (Buff b in from.Buffs)
        {
            buffed_damage += b.deltaDamage;
            if (b.cannotAttack)
                cannot_attack = true;
            if (b.confused)
                confused = true;
        }
        if (buffed_damage < 0)
            buffed_damage = 0;

        if (!confused)
        {
            print(from.name + " is attacking " + to.name);
            return Damage(buffed_damage, to);
        }
        else
        {
            Character t = RandomAny();
            print(from.name + " is attacking " + t.name);
            return Damage(buffed_damage, t);
        }
    }

    public bool[] Attack(Character from, Character[] to)
    {
        bool[] bs = new bool[to.Length];
        for (int i = 0; i < to.Length; i++)
            bs[i] = Attack(from, to[i]);
        return bs;
    }

    public bool Damage(int damage, Character to)
    {
        int buffed_evasion = to.evasion + to.Buffs.Sum(b => b.deltaEvasion);
        int buffed_defence = to.defence + to.Buffs.Sum(b => b.deltaDefence);
        if (buffed_defence < 0)
            buffed_defence = 0;
        if (Random.Range(1, 100) > buffed_evasion)
        {
            int before = to.health;
            to.health -= damage - buffed_defence;
            print("Damage " + (damage - buffed_defence) + " to " + to.name + " " + before + " -> " + to.health);
            if (to.health <= 0)
                Die(to);
            StartCoroutine(DestroyTempEffect(Instantiate(tempEffectPrefab, to.transform.position + new Vector3(0f, 0f, -0.2f), new Quaternion())));
            return true;
        }
        else
            return false;
    }

    public bool[] Damage(int damage, Character[] to)
    {
        bool[] bs = new bool[to.Length];
        for (int i = 0; i < to.Length; i++)
            bs[i] = Damage(damage, to[i]);
        return bs;
    }

    public void Die(Character c)
    {
        // TODO: DIE
        print(c.name + " died");
        if (player != c)
            enemies.Remove(c);
    }

    public Character[] RandomAny(int count)
    {
        Character[] l = new Character[count];
        List<int> il = new List<int>(count);
        if (count >= enemies.Count + 1)
        {
            il = new List<int>(enemies.Count + 1);
            for (int i = 0; i < enemies.Count; i++)
                l[i] = enemies[i];
            l[enemies.Count] = player;
            return l;
        }
        for (int i = 0; i < count; i++)
        {
            int j;
            do
            {
                j = Random.Range(0, enemies.Count);
            } while (il.IndexOf(j) == -1);
            il.Add(j);

            if (j == enemies.Count)
                l[i] = player;
            else
                l[i] = enemies[j];
        }
        return l;
    }

    public Character RandomAny()
    {
        int i = Random.Range(0, enemies.Count);
        if (i == enemies.Count)
            return player;
        else
            return enemies[i];
    }

    public Character[] RandomEnemy(int count)
    {
        Character[] l = new Character[count];
        List<int> il = new List<int>(count);
        if (count >= enemies.Count)
        {
            il = new List<int>(enemies.Count);
            for (int i = 0; i < enemies.Count; i++)
                l[i] = enemies[i];
            return l;
        }
        for (int i = 0; i < count; i++)
        {
            int j;
            do
            {
                j = Random.Range(0, enemies.Count - 1);
            } while (il.IndexOf(j) == -1);
            il.Add(j);

            l[i] = enemies[j];
        }
        return l;
    }

    public Character RandomEnemy()
    {
        return enemies[Random.Range(0, enemies.Count - 1)];
    }

    public Character[] All()
    {
        Character[] l = new Character[enemies.Count + 1];
        for (int i = 0; i < enemies.Count; i++)
            l[i] = enemies[i];
        l[enemies.Count - 1] = player;
        return l;
    }

    public Character[] AllEnemy()
    {
        Character[] l = new Character[enemies.Count];
        for (int i = 0; i < enemies.Count; i++)
            l[i] = enemies[i];
        return l;
    }

    private IEnumerator DestroyTempEffect(GameObject effect)
    {
        yield return new WaitForSeconds(3f);
        Destroy(effect);
    }
}

public struct Buff
{
    public string name;
    public int turnstamp;
    public int duration;
    public int deltaEvasion;
    public int deltaDefence;
    public int deltaDamage;
    public bool cannotAttack;
    public bool confused;
}
