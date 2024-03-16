using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitController<T> : MonoBehaviour
{
    protected Transform _unitRoot;

    protected List<Unit> _unitList;

    public abstract void Enter();
    public abstract void Exit();

    public abstract bool ChackExtermination();

    public abstract void InsetUnit(int idx, string unitName, int level);
    public abstract void RemoveUnit(string unitName);

    public abstract List<Unit> FindFullStackUnit();
    public abstract List<Unit> FindLiveUnit();

    public abstract void AddCardCollectionData(T unit);
}
