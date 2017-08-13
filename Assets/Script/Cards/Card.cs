using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPray
{
    public abstract string GetDescription();

    public abstract void ResolvePray(BattleLogic logic);
}

public abstract class ICard : MonoBehaviour
{
    public abstract string GetName();
    public abstract int GetID();
    public abstract int GetTargetCount();
    public abstract string GetDescription();

    public abstract void ResolveEffect(BattleLogic logic, Character[] targets);

    public abstract IPray[] GetPray();

    public abstract GameObject GetSpritePrefab();
}
