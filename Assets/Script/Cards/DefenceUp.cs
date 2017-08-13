using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceUp : ICard
{
    public static int id = 0;

    public static string cardname = "얼음 방패";

    public static int targetCount = 0;

    public static string description = "2턴 동안 방어도가 1 증가합니다.";

    public override string GetName()
    {
        return cardname;
    }

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
        return "효과: " + description + "\n기도: \"" + DamageAny.description + "\" x2";
    }

    public override void ResolveEffect(BattleLogic logic, Character[] targets)
    {
        Buff b;
        b.name = cardname;
        b.icon = BuffIcon.DefenceUp;
        b.cannotAttack = false;
        b.confused = false;
        b.deltaDamage = 0;
        b.deltaDefence = 1;
        b.deltaEvasion = 0;
        b.duration = 2;
        b.turnstamp = logic.TurnCount;
        logic.Buff(b, logic.player);
    }

    public override IPray[] GetPray()
    {
        return new IPray[] { new DamageAny() };
    }

    public override GameObject GetSpritePrefab()
    {
        return CardIndexManager.CardIndex[id];
    }

    public class DamageAny : IPray
    {
        public static string description = "대상 하나에게 4의 대미지를 줍니다.";

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
