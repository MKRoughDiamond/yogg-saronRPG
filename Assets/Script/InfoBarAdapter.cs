using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InfoBarAdapter : MonoBehaviour
{
    public Character character;
    [HideInInspector]
    public CharacterInfoBar infoBar;
    public GameObject infoBarPrefab;

    private GameObject instance;

    void Start()
    {
        Vector3 pos = character.transform.position;
        pos *= 100f;
        pos.z = 0f;
        pos.y -= 120f;
        instance = Instantiate(infoBarPrefab, new Vector3(), new Quaternion(), CanvasAdapter.InfoBarRoot);
        (instance.transform as RectTransform).localPosition = pos;
        infoBar = instance.GetComponent<CharacterInfoBar>();
        if (character.damage == -1)
            infoBar.DisableDamageIcon();
    }

    void Update()
    {
        infoBar.Health = (float)character.health / (float)character.maxHealth;
        infoBar.Damage = character.damage + character.Buffs.Sum(b => b.deltaDamage);
    }
}
