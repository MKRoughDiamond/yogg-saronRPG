using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCard : Card
{
    void Awake()
    {
        id = 0;
    }

    public override string GetDescription()
    {
        return "Sample Card. Effect: 5 Defence and Evasion boost for 2 turn. Pray: 10 Heal.";
    }

    public override void ResolveEffect(BattleLogic logic, List<Character> targets, out List<IPray> pray)
    {
        Buff b;
        b.cannotAttack = false;
        b.deltaDamage = 0;
        b.deltaDefence = 5;
        b.deltaEvasion = 5;
        b.duration = 2;
        b.turnstamp = logic.TurnCount;
        logic.Buff(logic.player, b);

        pray = new List<IPray> { this, this };
    }

    public override void ResolvePray(BattleLogic logic)
    {
        logic.Heal(logic.player, 10);
    }
}
