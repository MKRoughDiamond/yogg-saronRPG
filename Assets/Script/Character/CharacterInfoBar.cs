using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoBar : MonoBehaviour
{
    public Image healthBar;
    public Text damageText;
    public GameObject damageIcon;
    public Text defenceText;
    public Text evasionText;

    public float Health
    {
        set
        {
            healthBar.transform.localScale = new Vector3(value, 1f, 1f);
        }
    }

    public int Damage
    {
        set
        {
            if (damageIcon.activeSelf)
                damageText.text = "" + value;
        }
    }

    public int Defence
    {
        set
        {
            defenceText.text = "" + value;
        }
    }

    public int Evasion
    {
        set
        {
            evasionText.text = "" + value + "%";
        }
    }

    public void DisableDamageIcon()
    {
        damageIcon.SetActive(false);
    }
}
