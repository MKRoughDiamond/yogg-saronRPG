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
        infoBar.Defence = character.defence + character.Buffs.Sum(b => b.deltaDefence);
        int ev = character.evasion + character.Buffs.Sum(b => b.deltaEvasion);
        infoBar.Evasion = ev > 0 ? ev : 0;
        Vector3 pos = character.transform.position;
        pos *= 100f;
        pos.z = 0f;
        (infoBar.transform as RectTransform).localPosition = pos;
    }
}
