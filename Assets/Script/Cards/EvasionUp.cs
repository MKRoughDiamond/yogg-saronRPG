using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvasionUp : ICard
{
    public static int id = 1;

    public static int targetCount = 0;

    public static string description = "효과 : 2턴동안 회피율 20 증가\n기도: \"" + DamageAll.description + "\" x1, \"" + HealAll.description + "\" x1";

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
        b.name = "회피 증가";
        b.cannotAttack = false;
        b.confused = false;
        b.deltaDamage = 0;
        b.deltaDefence = 0;
        b.deltaEvasion = 20;
        b.duration = 2;
        b.turnstamp = logic.TurnCount;
        logic.Buff(b, logic.player);

        pray = new IPray[] { new DamageAll(), new HealAll() };
    }

    public override GameObject GetSpritePrefab()
    {
        return CardIndexManager.CardIndex[id];
    }

    public class DamageAll : IPray
    {
        public static string description = "모두에게 2의 데미지";

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
        public static string description = "모두에게 1의 회복";

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
