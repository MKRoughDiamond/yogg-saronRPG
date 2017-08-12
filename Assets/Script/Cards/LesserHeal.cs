using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LesserHeal : ICard
{
    public static int id = 3;

    public static string cardname = "응급 처치";

    public static int targetCount = 0;

    public static string description = "자신의 체력을 3 회복시킵니다.";

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
             + "기도: \"" + HealSelf.description + "\", "
                   + "\"" + HealAllEnemy.description + "\"";
    }

    public override void ResolveEffect(BattleLogic logic, Character[] targets)
    {
        logic.Heal(3, logic.player);
    }

    public override IPray[] GetPray()
    {
        return new IPray[] { new HealSelf(), new HealAllEnemy() };
    }

    public class HealSelf : IPray
    {
        public static string description = "플레이어의 체력을 3 회복시킵니다.";

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

    public class HealAllEnemy : IPray
    {
        public static string description = "적 모두의 체력을 1 회복시킵니다.";

        public override string GetDescription()
        {
            return description;
        }

        public override void ResolvePray(BattleLogic logic)
        {
            logic.Heal(1, logic.AllEnemy());
        }
    }
}
