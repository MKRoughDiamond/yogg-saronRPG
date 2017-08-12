using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LesserHeal : ICard
{
    public static int id = 3;

    public static int targetCount = 0;

    public static string description = "효과 : 체력을 2 회복\n기도: \"" + HealSelf.description + "\" x1, \"" + HealAny.description + "\" x1";

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
        logic.Heal(2, logic.player);

        pray = new IPray[] { new HealSelf(), new HealAny() };
    }

    public class HealSelf : IPray
    {
        public static string description = "자신에게 3의 회복";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            logic.Heal(3, logic.player);
        }
    }

    public override GameObject GetSpritePrefab()
    {
        return CardIndexManager.CardIndex[id];
    }

    public class HealAny : IPray
    {
        public static string description = "2의 회복";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            logic.Heal(2, logic.RandomAny());
        }
    }
}
