using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvasionUp : ICard
{
    public static int id = 1;

    public static string cardname = "수호의 바람";

    public static int targetCount = 0;

    public static string description = "2턴 동안 회피율이 20 증가합니다.";

    public override int GetID()
    {
        return id;
    }

    public override string GetName()
    {
        return name;
    }

    public override int GetTargetCount()
    {
        return targetCount;
    }

    public override string GetDescription()
    {
        return "효과: " + description + "\n"
             + "기도: \"" + DamageAll.description + "\", "
                   + "\"" + HealAll.description + "\"";
    }

    public override void ResolveEffect(BattleLogic logic, Character[] targets)
    {
        Buff b;
        b.name = cardname;
        b.icon = BuffIcon.EvasionUp;
        b.cannotAttack = false;
        b.confused = false;
        b.deltaDamage = 0;
        b.deltaDefence = 0;
        b.deltaEvasion = 20;
        b.duration = 2;
        b.turnstamp = logic.TurnCount;
        logic.Buff(b, logic.player);
    }

    public override IPray[] GetPray()
    {
        return new IPray[] { new DamageAll(), new HealAll() };
    }

    public override GameObject GetSpritePrefab()
    {
        return CardIndexManager.CardIndex[id];
    }

    public class DamageAll : IPray
    {
        public static string description = "모두에게 2의 대미지를 줍니다.";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            logic.Damage(2, logic.All());
        }
    }

    public class HealAll : IPray
    {
        public static string description = "모두의 체력을 1 회복시킵니다.";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            logic.Heal(1, logic.All());
        }
    }
}
