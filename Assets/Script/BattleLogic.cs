using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLogic : MonoBehaviour
{
    public Character player;
    public List<Character> enemies;

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

    public IEnumerator TurnRunner()
    {
        while (!_kill)
        {
            if (!pause)
                yield return StartCoroutine(PlayerTurn());
            else
                yield return null;
        }
    }

    public IEnumerator PlayerTurn()
    {
        hand.Clear();
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
                    Character[] targets;
                    UIManager.PlayerCardTargetChoice(target_count, out targets);
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
        yield return StartCoroutine(EnemyTurn());
    }

    public IEnumerator CallYS()
    {
        yield return null;
    }

    public IEnumerator EnemyTurn()
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
    }

    public void Buff(Buff buff, Character[] to)
    {
        foreach (Character c in to)
            Buff(buff, c);
    }

    public void Heal(int heal, Character to)
    {
        to.health += heal;
        if (to.health > to.maxHealth)
            to.health = to.maxHealth;
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
        if (cannot_attack)
            return false;
        foreach (Buff b in from.Buffs)
        {
            buffed_damage += b.deltaDamage;
            if (b.cannotAttack)
                cannot_attack = true;
            if (b.confused)
                confused = true;
        }

        if (!confused)
            return Damage(buffed_damage, to);
        else
            return Damage(buffed_damage, RandomAny());
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
        int buffed_evasion = to.evasion;
        int buffed_defence = to.defence;
        foreach (Buff b in to.Buffs)
        {
            buffed_defence += b.deltaDefence;
            buffed_evasion += b.deltaEvasion;
        }
        if (Random.Range(1, 100) > buffed_evasion)
        {
            to.health -= damage - buffed_defence;
            if (to.health <= 0)
                Die(to);
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
    }

    public Character[] RandomAny(int count)
    {
        Character[] l = new Character[count];
        List<int> il = new List<int>(count);
        if (count >= enemies.Count + 1)
        {
            for (int i = 0; i < count - 1; i++)
                l[i] = enemies[i];
            l[count - 1] = player;
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
            for (int i = 0; i < count; i++)
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
}

public struct Buff
{
    public int turnstamp;
    public int duration;
    public int deltaEvasion;
    public int deltaDefence;
    public int deltaDamage;
    public bool cannotAttack;
    public bool confused;
}
