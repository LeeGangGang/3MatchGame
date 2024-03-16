using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxBase : MonoBehaviour
{
    [SerializeField] float _lifeTime = 0f;

    protected eFxID _id;
    protected float _timer;
    protected bool _isEnter;
    protected bool _isParent;

    public void Init(eFxID _id, eColor _color)
    {
        this._id = _id;
        _timer = 0f;
        _isEnter = false;

        OnInit(_color);
        this.gameObject.SetActive(false);
    }

    protected virtual void OnInit(eColor _color)
    {
    }

    public void Enter(float _lifeTime = 0f, bool _isParent = false)
    {
        _isEnter = true;
        this._isParent = _isParent;
        
        if (_lifeTime == 0)
            Invoke(nameof(Exit), this._lifeTime);
        else
            Invoke(nameof(Exit), _lifeTime);

        OnEnter();
    }

    protected virtual void OnEnter(Vector3 startPos, Vector3 endPos)
    {
    }

    public void Enter(Vector3 startPos, Vector3 endPos, float _lifeTime = 0f, bool _isParent = false)
    {
        _isEnter = true;
        this._isParent = _isParent;

        if (_lifeTime == 0)
            Invoke(nameof(Exit), this._lifeTime);
        else
            Invoke(nameof(Exit), _lifeTime);

        OnEnter(startPos, endPos);
    }

    protected virtual void OnEnter()
    {
    }

    public void Exit()
    {
        if (!_isEnter)
            return;

        _isEnter = false;
        FxManager.Inst.ExitFx(_id, this, _isParent);
        OnExit();
    }

    protected virtual void OnExit()
    {
    }
}