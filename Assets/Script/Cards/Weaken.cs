using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weaken : ICard
{
    public static int id = 2;

    public static string cardname = "허약의 물약";

    public static int targetCount = 2;

    public static string description = "1턴 동안 2명의 적에게 \"약화\"를 부여합니다. (대상의 공격력이 1만큼 감소합니다.)";

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
             + "기도: \"" + SlowSelf.description + "\", "
                   + "\"" + DamageAny.description + "\"";
    }

    public override void ResolveEffect(BattleLogic logic, Character[] targets)
    {
        Buff b;
        b.name = cardname;
        b.icon = BuffIcon.DamageDown;
        b.cannotAttack = false;
        b.confused = false;
        b.deltaDamage = -1;
        b.deltaDefence = 0;
        b.deltaEvasion = 0;
        b.duration = 2;
        b.turnstamp = logic.TurnCount;
        logic.Buff(b, targets);
    }

    public override IPray[] GetPray()
    {
        return new IPray[] { new SlowSelf(), new DamageAny() };
    }

    public class SlowSelf : IPray
    {
        public static string description = "플레이어에게 1턴간 '둔화'를 부여합니다.";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            Buff b;
            b.name = "둔화";
            b.icon = BuffIcon.EvasionDown;
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
        public static string description = "대상 하나에게 3의 대미지를 줍니다.";

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
