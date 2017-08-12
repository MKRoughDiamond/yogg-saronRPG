using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceUp : ICard
{
    public static int id = 0;

    public static int targetCount = 0;

    public static string description = "효과 : 1턴동안 방어력 1 증가\n기도: \"" + DamageAny.description + "\" x1";

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
        b.confused = false;
        b.deltaDamage = 0;
        b.deltaDefence = 1;
        b.deltaEvasion = 0;
        b.duration = 1;
        b.turnstamp = logic.TurnCount;
        logic.Buff(b, logic.player);

        pray = new IPray[] { new DamageAny() };
    }

    public override GameObject GetSpritePrefab()
    {
        return CardIndexManager.CardIndex[id];
    }

    public class DamageAny : IPray
    {
        public static string description = "4의 데미지";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            logic.Damage(4, logic.RandomAny());
        }
    }
}
