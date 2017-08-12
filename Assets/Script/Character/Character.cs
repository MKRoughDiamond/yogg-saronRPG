using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int maxHealth;
    [HideInInspector]
    public int health;
    public int damage;
    public int defence;
    public int evasion;
    public int experience;
    public int level;
    public GameObject check;

    private List<Buff> buffs;
    public List<Buff> Buffs
    {
        get
        {
            return buffs;
        }
    }

    void Awake()
    {
        health = maxHealth;
        buffs = new List<Buff>();
    }

    public void AddBuff(Buff buff)
    {
        buffs.Add(buff);
    }

    public void CheckBuff(int turn_count)
    {
        buffs.RemoveAll(b => turn_count - b.turnstamp >= b.duration - 1);
    }
}
