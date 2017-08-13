using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBadgeManager : MonoBehaviour
{
    public List<GameObject> buffBadgePrefabs;
    public static List<GameObject> BuffBadgePrefabs { get { return instance.buffBadgePrefabs; } }

    public GameObject healBadgePrefab;
    public static GameObject HealBadgePrefab { get { return instance.healBadgePrefab; } }

    public GameObject damageBadgePrefab;
    public static GameObject DamageBadgePrefab { get { return instance.damageBadgePrefab; } }

    private static EffectBadgeManager instance;

    void Awake()
    {
        instance = this;
    }
}
