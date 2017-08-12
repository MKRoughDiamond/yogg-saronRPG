using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confuse : ICard
{
    public static int id = 4;

    public static string cardname = "정신 공격";

    public static int targetCount = 1;

    public static string description = "1턴 동안 1명의 적에게 \"혼란\"을 부여합니다.";

    public override int GetID()
    {
        return id;
    }

    public override string GetName()
    {
        return cardname;
    }

    public override int GetTargetCount()
    {
        return targetCount;
    }

    public override string GetDescription()
    {
        return "효과: " + description + "\n"
             + "기도: \"" + ConfuseAny.description + "\"";
    }

    public override void ResolveEffect(BattleLogic logic, Character[] targets)
    {
        Buff b;
        b.name = cardname;
        b.icon = BuffIcon.Confuse;
        b.cannotAttack = false;
        b.confused = true;
        b.deltaDamage = 0;
        b.deltaDefence = 0;
        b.deltaEvasion = 0;
        b.duration = 1;
        b.turnstamp = logic.TurnCount;
        logic.Buff(b, targets);
    }

    public override IPray[] GetPray()
    {
        return new IPray[] { new ConfuseAny(), new ConfuseAny() };
    }

    public override GameObject GetSpritePrefab()
    {
        return CardIndexManager.CardIndex[id];
    }

    public class ConfuseAny : IPray
    {
        public static string description = "2턴 동안 대상 하나에게 '혼란'을 부여합니다.";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            Buff b;
            b.name = "혼란";
            b.icon = BuffIcon.Confuse;
            b.cannotAttack = false;
            b.confused = true;
            b.deltaDamage = 0;
            b.deltaDefence = 0;
            b.deltaEvasion = 0;
            b.duration = 2;
            b.turnstamp = logic.TurnCount;
            logic.Buff(b, logic.RandomAny());
        }
    }
}
