using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPray
{
    void ResolvePray(BattleLogic logic);
}

public abstract class Card : MonoBehaviour, IPray
{
    protected int id;
    public int ID
    {
        get
        {
            return id;
        }
    }
    protected int targetCount = 0;

    public abstract string GetDescription();

    public abstract void ResolveEffect(BattleLogic logic, List<Character> targets, out List<IPray> pray);

    public abstract void ResolvePray(BattleLogic logic);

    public int TargetCount()
    {
        return targetCount;
    }

    public GameObject GetSpritePrefab()
    {
        return CardIndexManager.CardIndex[id];
    }
}
