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

    void Awake()
    {
        turnCount = 0;
        hand = new List<int>(3);
        prayPool = new List<IPray>();
    }

    void Start()
    {
        deck = DeckManager.GetPlayDeck();
        StartCoroutine(PlayerTurn());
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
                Card card = CardIndexManager.GetCardByID(cardid);
                int target_count = card.TargetCount();
                if (target_count > 0)
                {
                    List<Character> targets;
                    UIManager.PlayerCardTargetChoice(target_count, out targets);
                    List<IPray> pray;
                    card.ResolveEffect(this, targets, out pray);
                    prayPool.AddRange(pray);
                }
                else
                {
                    List<IPray> pray;
                    card.ResolveEffect(this, null, out pray);
                    prayPool.AddRange(pray);
                }
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

    public void Buff(Character target, Buff buff)
    {
        target.AddBuff(buff);
    }

    public void Heal(Character target, int heal)
    {
        target.health += heal;
    }

    public void Attack(Character from, Character to)
    {
        int buffed_damage = from.damage;
        int buffed_evasion = to.evasion;
        int buffed_defence = to.defence;
        foreach (Buff b in to.Buffs)
        {
            buffed_defence += b.deltaDefence;
            buffed_evasion += b.deltaEvasion;
        }
        foreach (Buff b in from.Buffs)
            buffed_damage += b.deltaDamage;
        if (Random.Range(1, 100) > buffed_evasion)
        {
            to.health -= buffed_damage - buffed_defence;
            if (to.health <= 0)
                Die(to);
        }
    }

    public void Die(Character c)
    {
        // TODO: DIE
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
}
