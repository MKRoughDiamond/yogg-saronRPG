using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weaken : ICard
{
    public static int id = 2;

    public static int targetCount = 2;

    public static string description = "효과 : 2턴동안 2명의 적에게 '약화'를 부여함(공격력 -1)\n기도: \"" + SlowSelf.description + "\" x1, \"" + DamageAny.description + "\" x1";

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
        b.name = "약화";
        b.cannotAttack = false;
        b.confused = false;
        b.deltaDamage = -1;
        b.deltaDefence = 0;
        b.deltaEvasion = 0;
        b.duration = 2;
        b.turnstamp = logic.TurnCount;
        logic.Buff(b, targets);

        pray = new IPray[] { new SlowSelf(), new DamageAny() };
    }

    public class SlowSelf : IPray
    {
        public static string description = "자신에게 1턴간 둔화";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            Buff b;
            b.name = "둔화";
            b.cannotAttack = false;
            b.confused = false;
            b.deltaDamage = 0;
            b.deltaDefence = 0;
            b.deltaEvasion = -100;
            b.duration = 1;
            b.turnstamp = logic.TurnCount;
            logic.Buff(b, logic.player);
        }
    }

    public override GameObject GetSpritePrefab()
    {
        return CardIndexManager.CardIndex[id];
    }

    public class DamageAny : IPray
    {
        public static string description = "3의 데미지";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            logic.Damage(3, logic.RandomAny());
        }
    }
}
