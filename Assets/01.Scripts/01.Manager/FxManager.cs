using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eFxID
{
    BlockMove,

    BlockRemove,

    Lightning,
    MultiColorAllRemove,
    Package3x3,
    Package5x5,
    Liner,

    WireRemove,
    SpiralRemove,
}

public class FxManager : MonoBehaviour
{
    public static FxManager Inst;

    Dictionary<eFxID, Transform> _fxRoot = new Dictionary<eFxID, Transform>();
    Dictionary<eFxID, ObjectPoolQueue<FxBase>> _fxPool = new Dictionary<eFxID, ObjectPoolQueue<FxBase>>();
    Dictionary<eFxID, GameObject> _fxPrefabs = new Dictionary<eFxID, GameObject>();

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        Init();
    }

    public void Init()
    {
        // 하위 Root 잡기 _fxRoot.Add();
        foreach (eFxID id in Enum.GetValues(typeof(eFxID)))
        {
            GameObject root = new GameObject();
            root.name = $"Fx_{id} Pool";
            root.transform.SetParent(this.transform);
            _fxRoot.Add(id, root.transform);
        }

        // 프리팹 매칭 시키기 _fxPrefabs.Add();
        foreach (eFxID id in Enum.GetValues(typeof(eFxID)))
        {
            string fullPath = $"Fx/{id}";
            GameObject prefab = Resources.Load<GameObject>(fullPath);
            if (prefab)
            {
                _fxPool.Add(id, new ObjectPoolQueue<FxBase>());
                _fxPrefabs.Add(id, prefab);
            }
        }

        // 미리 생성하기 _fxPool[id].Enqueue(obj);
        foreach (eFxID id in Enum.GetValues(typeof(eFxID)))
        {
            for (int i = 0; i < 10; i++)
                CreateFx(id, eColor.None);
        }
    }

    public void Release()
    {
        foreach (var pool in _fxPool)
            pool.Value.DestroyAll();

        _fxRoot.Clear();
        _fxPrefabs.Clear();
        _fxPool.Clear();

        Resources.UnloadUnusedAssets();
    }

    public void EnterLineRenderFx(eFxID _id, Vector3 _startPos, Vector3 _endPos, float _lifeTime = 0f)
    {
        if (!_fxPool.ContainsKey(_id))
            return;

        var fx = _fxPool[_id].GetObject();

        if (fx == null)
            fx = CreateFx(_id, eColor.None);

        fx.transform.position = Vector3.back;
        fx.transform.rotation = Quaternion.identity;
        fx.gameObject.SetActive(true);
        fx.Enter(_startPos, _endPos, _lifeTime, true);
    }

    public void EnterFx(eFxID _id, Vector3 _targetPos, Quaternion _rotation = default, float _lifeTime = 0f)
    {
        if (!_fxPool.ContainsKey(_id))
            return;

        var fx = _fxPool[_id].GetObject();

        if (fx == null)
            fx = CreateFx(_id, eColor.None);

        fx.transform.position = _targetPos + Vector3.back;
        fx.transform.rotation = _rotation;
        fx.gameObject.SetActive(true);
        fx.Enter(_lifeTime, true);
    }

    public void EnterFx(eFxID _id, eColor _color, Vector3 _targetPos, float _lifeTime = 0f)
    {
        if (!_fxPool.ContainsKey(_id))
            return;

        var fx = _fxPool[_id].GetObject();

        if (fx == null)
            fx = CreateFx(_id, _color);
        
        fx.Init(_id, _color);

        fx.transform.position = _targetPos + Vector3.back;
        fx.transform.rotation = Quaternion.identity;
        fx.gameObject.SetActive(true);
        fx.Enter(_lifeTime, true);
    }

    public void ExitFx(eFxID _id, FxBase _fx, bool _changeParent = false)
    {
        if (!_fxPool.ContainsKey(_id))
            return;

        if (_changeParent)
            _fx.transform.SetParent(_fxRoot[_id]);

        _fx.transform.localPosition = Vector3.zero;
        _fx.transform.localRotation = Quaternion.identity;
        _fx.gameObject.SetActive(false);
        _fxPool[_id].Enqueue(_fx);
    }

    public void EnterBoardFx(eFxID _id, Vector3 _scale)
    {
        if (!_fxPool.ContainsKey(_id))
            return;

        var fx = _fxPool[_id].GetObject();

        if (fx == null)
            fx = CreateFx(_id, eColor.None);

        fx.Init(_id, eColor.None);
        fx.transform.localScale = _scale;
        fx.transform.rotation = Quaternion.identity;
        fx.gameObject.SetActive(true);
        fx.Enter(1, true);
    }

    FxBase CreateFx(eFxID _id, eColor _color)
    {
        var fx = Instantiate(_fxPrefabs[_id], _fxRoot[_id]).GetComponent<FxBase>();
        fx.Init(_id, _color);
        _fxPool[_id].Enqueue(fx);

        return fx;
    }
}