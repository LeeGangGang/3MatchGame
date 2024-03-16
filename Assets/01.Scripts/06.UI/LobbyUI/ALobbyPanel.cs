using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ALobbyPanel : MonoBehaviour
{
    public eLobbyPanel type;
    protected bool isEnter;
    
    public abstract void Init();
    public abstract void Enter();
    public abstract void Exit();
    public abstract void UpdateUI();
}
