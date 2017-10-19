using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleLogic : MonoBehaviour
{
    public Character player;
    public List<Character> enemies;

    public bool battleFinished = false;

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

    private bool ended = false;

    private int animateStack = 0;
    private bool isAnimating
    {
        get
        {
            if (animateStack < 0)
                animateStack = 0;
            return animateStack != 0;
        }
    }

    void Awake()
    {
        turnCount = 0;
        prayStack = 0;
        pause = false;
        hand = new List<int>(3);
        prayPool = new List<IPray>();
    }

    public void StartBattle()
    {
        deck = DeckManager.GetPlayDeck();
        player.health = player.maxHealth;
        UIManager.UpdateDeckCount(15);
        UIManager.UpdateYoggStack(0);
        CanvasAdapter.InfoBarRoot.gameObject.SetActive(true);
        CanvasAdapter.Deck.gameObject.SetActive(true);
        StartCoroutine(TurnRunner());
    }

    private IEnumerator TurnRunner()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TextShowAnimation(CanvasAdapter.BattleStart));
        while (!ended)
        {
            if (!pause)
                yield return StartCoroutine(Turn());
            else
                yield return null;
        }
        CanvasAdapter.InfoBarRoot.gameObject.SetActive(false);
        CanvasAdapter.Deck.gameObject.SetActive(false);
        battleFinished = true;
    }

    private IEnumerator Turn()
    {
        turnCount++;
        CanvasAdapter.TurnCounter.text = turnCount + " 턴";
        yield return StartCoroutine(TextShowAnimation(CanvasAdapter.TurnCounter));
        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(PlayerTurn());
        while (isAnimating)
            yield return null;
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(EnemyTurn());
        while (isAnimating)
            yield return null;
        yield return new WaitForSeconds(0.2f);
        CheckBuff();
        CheckDied();
        yield return new WaitForSeconds(0.2f);
        if (CheckEnd())
        {
            ended = true;
            yield break;
        }
    }

    private void CheckDied()
    {
        List<Character> died = enemies.Where(e => e.health <= 0).ToList();
        if (died.Count > 0)
            foreach (Character c in died)
                Die(c);
    }

    private bool CheckEnd()
    {
        return player.health <= 0 || enemies.Count == 0;
    }

    private void CheckBuff()
    {
        player.CheckBuff(turnCount);
        foreach (Character c in enemies)
            c.CheckBuff(turnCount);
    }

    private IEnumerator ReplenishDeck()
    {
        deck = DeckManager.GetPlayDeck();

        for (int i = 0; i < deck.Count; i++)
        {
            ICard card = Instantiate(CardIndexManager.CardIndex[deck[i]]).GetComponent<CardAdapter>().Card;
            StartCoroutine(ReplenishAnimation(card, i + 1));
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator ReplenishAnimation(ICard card, int my_count)
    {
        Vector3 right_lower = new Vector3(8.1f, -4.4f, -0.5f);
        Vector3 left_lower = new Vector3(-10f, -4.4f, -0.5f);
        card.transform.position = left_lower;
        card.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
        float t = 0f;

        while (card.transform.position != right_lower)
        {
            t += 8f * Time.deltaTime;
            card.transform.position = Vector3.Lerp(left_lower, right_lower, t);
            yield return new WaitForEndOfFrame();
        }
        UIManager.UpdateDeckCount(my_count);
        Destroy(card.gameObject);
    }

    private IEnumerator PlayerTurn()
    {
        hand.Clear();
        if (deck.Count == 0)
        {
            yield return StartCoroutine(ReplenishDeck());
            Buff b;
            b.name = "시간의 강화";
            b.icon = BuffIcon.DamageUp;
            b.cannotAttack = false;
            b.confused = false;
            b.deltaDamage = 1;
            b.deltaDefence = 0;
            b.deltaEvasion = 5;
            b.duration = -1;
            b.turnstamp = -1;
            Buff(b, RandomEnemy());
            while (isAnimating)
                yield return null;
        }
        for (int i = 0; i < 3; i++)
        {
            int c = deck[Random.Range(0, deck.Count)];
            hand.Add(c);
            deck.Remove(c);
        }
        UIManager.UpdateDeckCount(deck.Count);

        yield return StartCoroutine(UIManager.GetInstance().PlayerCardChoice(hand));

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
                    if (enemies.Count > target_count)
                    {
                        yield return StartCoroutine(UIManager.PlayerCardTargetChoice(target_count));

                        Character[] targets = UIManager.GetChosenTarget();
                        card.ResolveEffect(this, targets);
                    }
                    else
                        card.ResolveEffect(this, AllEnemy());
                    while (isAnimating)
                        yield return null;
                    prayPool.AddRange(card.GetPray());
                }
                else
                {
                    card.ResolveEffect(this, null);
                    while (isAnimating)
                        yield return null;
                    prayPool.AddRange(card.GetPray());
                }
                prayStack++;
                if (prayStack > 5)
                    prayStack = 5;
                UIManager.UpdateYoggStack(prayStack);
            }
        }
        else
        {
            yield return StartCoroutine(CallYS());
        }
    }

    private IEnumerator CallYS()
    {
        for (int i = 0; i < prayStack; i++)
        {
            IPray p = prayPool[Random.Range(0, prayPool.Count)];
            p.ResolvePray(this);
            prayPool.Remove(p);
            while (isAnimating)
                yield return null;
            yield return new WaitForSeconds(0.5f);
        }
        prayPool.Clear();
        prayStack = 0;
        UIManager.UpdateYoggStack(prayStack);
    }

    private IEnumerator EnemyTurn()
    {
        Character[] es = enemies.ToArray();
        foreach (Character e in es)
        {
            if (e != null && enemies.IndexOf(e) != -1 && player.health > 0 && e.health > 0)
            {
                Attack(e, player);
                while (isAnimating)
                    yield return null;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    public void Buff(Buff buff, Character to)
    {
        if (to == null)
            return;
        to.AddBuff(buff);
        StartCoroutine(ShowEffectIcon(Instantiate(EffectBadgeManager.BuffBadgePrefabs[(int)buff.icon], to.transform.position + new Vector3(0f, 0f, -0.2f), new Quaternion())));
    }

    public void Buff(Buff buff, Character[] to)
    {
        foreach (Character c in to)
            Buff(buff, c);
    }

    public void Heal(int heal, Character to)
    {
        if (to == null)
            return;
        to.health += heal;
        if (to.health > to.maxHealth)
            to.health = to.maxHealth;
        StartCoroutine(ShowEffectIcon(Instantiate(EffectBadgeManager.HealBadgePrefab, to.transform.position + new Vector3(0f, 0f, -0.2f), new Quaternion())));
    }

    public void Heal(int heal, Character[] to)
    {
        foreach (Character c in to)
            Heal(heal, c);
    }

    public void Attack(Character from, Character to)
    {
        if (to == null)
            return;
        bool cannot_attack = !from.Buffs.TrueForAll(b => !b.cannotAttack);
        if (cannot_attack)
        {
            StartCoroutine(CannotAttackAnimation(from));
            return;
        }
        bool confused = !from.Buffs.TrueForAll(b => !b.confused);
        int buffed_damage = from.damage + from.Buffs.Sum(b => b.deltaDamage);

        if (buffed_damage < 0)
            buffed_damage = 0;

        Character t;
        if (!confused)
            t = to;
        else
            t = RandomAny();
        StartCoroutine(AttackAnimation(from, t, buffed_damage));
    }

    public void Attack(Character from, Character[] to)
    {
        for (int i = 0; i < to.Length; i++)
            Attack(from, to[i]);
    }

    public void Damage(int damage, Character to)
    {
        if (to == null)
            return;
        int buffed_evasion = to.evasion + to.Buffs.Sum(b => b.deltaEvasion);
        int buffed_defence = to.defence + to.Buffs.Sum(b => b.deltaDefence);
        int real_damage = damage - buffed_defence;
        if (Random.Range(1, 101) > buffed_evasion)
        {
            to.health -= (real_damage > 0) ? real_damage : 0;
            if (to.health <= 0)
                to.health = 0;
            GameObject ic = Instantiate(EffectBadgeManager.DamageBadgePrefab, to.transform.position + new Vector3(0f, 0f, -0.2f), new Quaternion());
            ic.GetComponent<DamageBadge>().text.text = "-" + real_damage;
            StartCoroutine(ShowEffectIcon(ic));
        }
        else
            StartCoroutine(EvaseAnimation(to, (player == to) ? -1 : 1));
    }

    public void Damage(int damage, Character[] to)
    {
        for (int i = 0; i < to.Length; i++)
            Damage(damage, to[i]);
    }

    public void Die(Character c)
    {
        if (player != c)
        {
            enemies.Remove(c);
            StartCoroutine(DeathAnimation(c));
        }
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
                j = Random.Range(0, enemies.Count + 1);
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
        int i = Random.Range(0, enemies.Count + 1);
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
                j = Random.Range(0, enemies.Count);
            } while (il.IndexOf(j) == -1);
            il.Add(j);

            l[i] = enemies[j];
        }
        return l;
    }

    public Character RandomEnemy()
    {
        return enemies[Random.Range(0, enemies.Count)];
    }

    public Character[] All()
    {
        Character[] l = new Character[enemies.Count + 1];
        for (int i = 0; i < enemies.Count; i++)
            l[i] = enemies[i];
        l[enemies.Count] = player;
        return l;
    }

    public Character[] AllEnemy()
    {
        Character[] l = new Character[enemies.Count];
        for (int i = 0; i < enemies.Count; i++)
            l[i] = enemies[i];
        return l;
    }

    public IEnumerator ShowEffectIcon(GameObject icon)
    {
        animateStack++;
        SpriteRenderer sprite = icon.GetComponent<SpriteRenderer>();
        float t = 0f;
        Color original = sprite.color;

        sprite.color = Color.clear;
        while (sprite.color.a != 1)
        {
            t += 2f * Time.deltaTime;
            sprite.color = Color.Lerp(sprite.color, original, t);
            yield return new WaitForEndOfFrame();
        }
        t = 0f;

        yield return new WaitForSeconds(1f);

        while (sprite.color.a != 0)
        {
            t += 1.5f * Time.deltaTime;
            sprite.color = Color.Lerp(sprite.color, Color.clear, t);
            yield return new WaitForEndOfFrame();
        }
        Destroy(icon);
        animateStack--;
    }

    public IEnumerator AttackAnimation(Character from, Character to, int damage)
    {
        animateStack++;
        Vector3 original = from.transform.position;
        float t = 0f;

        while (from.transform.position.x - original.x < 1f)
        {
            t += 3f * Time.deltaTime;
            from.transform.position = Vector3.Lerp(original, original + new Vector3(1f, 0f, 0f), t);
            yield return new WaitForEndOfFrame();
        }
        t = 0f;

        yield return new WaitForSeconds(0.4f);

        while (from.transform.position.x - original.x > -0.5f)
        {
            t += 8f * Time.deltaTime;
            from.transform.position = Vector3.Lerp(original + new Vector3(1f, 0f, 0f), original + new Vector3(-0.5f, 0f, 0f), t);
            yield return new WaitForEndOfFrame();
        }
        t = 0f;

        Damage(damage, to);

        while (from.transform.position.x != original.x)
        {
            t += 2f * Time.deltaTime;
            from.transform.position = Vector3.Lerp(original + new Vector3(-0.5f, 0f, 0f), original, t);
            yield return new WaitForEndOfFrame();
        }
        animateStack--;
    }

    public IEnumerator CannotAttackAnimation(Character c)
    {
        animateStack++;
        Vector3 original = c.transform.position;
        float t = 0f;

        while (c.transform.position.x - original.x < 0.5f)
        {
            t += 4f * Time.deltaTime;
            c.transform.position = Vector3.Lerp(original, original + new Vector3(0.5f, 0f, 0f), t);
            yield return new WaitForEndOfFrame();
        }
        t = 0f;

        yield return new WaitForSeconds(0.5f);

        while (c.transform.position.x - original.x > -0.5f)
        {
            t += 8f * Time.deltaTime;
            c.transform.position = Vector3.Lerp(original + new Vector3(0.5f, 0f, 0f), original + new Vector3(-0.5f, 0f, 0f), t);
            yield return new WaitForEndOfFrame();
        }
        t = 0f;

        while (c.transform.position.x - original.x < 0.5f)
        {
            t += 8f * Time.deltaTime;
            c.transform.position = Vector3.Lerp(original + new Vector3(-0.5f, 0f, 0f), original + new Vector3(0.5f, 0f, 0f), t);
            yield return new WaitForEndOfFrame();
        }
        t = 0f;

        while (c.transform.position.x - original.x < 0.5f)
        {
            t += 4f * Time.deltaTime;
            c.transform.position = Vector3.Lerp(original + new Vector3(0.5f, 0f, 0f), original, t);
            yield return new WaitForEndOfFrame();
        }
        t = 0f;
        animateStack--;
    }

    public IEnumerator EvaseAnimation(Character c, int direction)
    {
        animateStack++;
        Vector3 original = c.transform.position;
        float t = 0f;

        while (Mathf.Abs(c.transform.position.x - original.x) != 0.5f)
        {
            t += 8f * Time.deltaTime;
            c.transform.position = Vector3.Lerp(original, original + new Vector3(0.5f, 0f, 0f) * direction, t);
            yield return new WaitForEndOfFrame();
        }
        t = 0f;

        while (c.transform.position.x != original.x)
        {
            t += 8f * Time.deltaTime;
            c.transform.position = Vector3.Lerp(original + new Vector3(0.5f, 0f, 0f) * direction, original, t);
            yield return new WaitForEndOfFrame();
        }
        animateStack--;
    }

    public IEnumerator DeathAnimation(Character c)
    {
        animateStack++;
        SpriteRenderer sprite = c.GetComponent<SpriteRenderer>();
        float t = 0f;

        while (sprite.color.a != 0)
        {
            t += 4f * Time.deltaTime;
            sprite.color = Color.Lerp(sprite.color, Color.clear, t);
            yield return new WaitForEndOfFrame();
        }

        c.GetComponent<InfoBarAdapter>().infoBar.gameObject.SetActive(false);
        c.gameObject.SetActive(false);
        animateStack--;
    }

    public IEnumerator TextShowAnimation(Text text)
    {
        text.gameObject.SetActive(true);
        float t = 0f;
        Color original = text.color;

        text.color = Color.clear;
        while (text.color.a != 1)
        {
            t += 2f * Time.deltaTime;
            text.color = Color.Lerp(text.color, original, t);
            yield return new WaitForEndOfFrame();
        }
        t = 0f;

        yield return new WaitForSeconds(1f);

        while (text.color.a != 0)
        {
            t += 1f * Time.deltaTime;
            text.color = Color.Lerp(text.color, Color.clear, t);
            yield return new WaitForEndOfFrame();
        }
        text.color = original;
        text.gameObject.SetActive(false);
    }
}

public struct Buff
{
    public string name;
    public BuffIcon icon;
    public int turnstamp;
    public int duration;
    public int deltaEvasion;
    public int deltaDefence;
    public int deltaDamage;
    public bool cannotAttack;
    public bool confused;
}

public enum BuffIcon
{
    None,
    Confuse,
    DamageDown,
    DamageUp,
    DefenceUp,
    EvasionDown,
    EvasionUp
}
