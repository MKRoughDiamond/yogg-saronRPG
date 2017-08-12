using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confuse : ICard
{
    public static int id = 4;

    public static int targetCount = 1;

    public static string description = "효과 : 1턴동안 1명의 적에게 '혼란'을 부여함\n기도: \"" + ConfuseAny.description + "\" x2";

    public override int GetID()
    {
        return id;
    }

    public override int GetTargetCount()
    {
        return targetCount;
    }

    public override string GetDescription()
    {
        return description;
    }

    public override void ResolveEffect(BattleLogic logic, Character[] targets, out IPray[] pray)
    {
        Buff b;
        b.cannotAttack = false;
        b.confused = true;
        b.deltaDamage = 0;
        b.deltaDefence = 0;
        b.deltaEvasion = 0;
        b.duration = 1;
        b.turnstamp = logic.TurnCount;
        logic.Buff(b, targets);

        pray = new IPray[] { new ConfuseAny(), new ConfuseAny() };
    }

    public override GameObject GetSpritePrefab()
    {
        return CardIndexManager.CardIndex[id];
    }

    public class ConfuseAny : IPray
    {
        public static string description = "2턴간 혼란";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            Buff b;
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
