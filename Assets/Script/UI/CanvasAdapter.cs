using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAdapter : MonoBehaviour
{
    [SerializeField]
    private Transform infoBarRoot;
    public static Transform InfoBarRoot
    {
        get
        {
            return instance.infoBarRoot;
        }
    }

    [SerializeField]
    private Image victoryScreen;
    public static Image VictoryScreen { get { return instance.victoryScreen; } }
    [SerializeField]
    private Image defeatScreen;
    public static Image DefeatScreen { get { return instance.defeatScreen; } }

    [SerializeField]
    private Image deck;
    public static Image Deck { get { return instance.deck; } }

    [SerializeField]
    private Text battleStart;
    public static Text BattleStart { get { return instance.battleStart; } }
    [SerializeField]
    private Text turnCounter;
    public static Text TurnCounter { get { return instance.turnCounter; } }
    [SerializeField]
    private Text selectTarget;
    public static Text SelectTarget { get { return instance.selectTarget; } }
    [SerializeField]
    private Text rateText;
    public static Text RateText { get { return instance.rateText; } }

    public static Transform Transform
    {
        get
        {
            return instance.transform;
        }
    }

    private static CanvasAdapter instance;

    void Awake()
    {
        instance = this;
    }
}
