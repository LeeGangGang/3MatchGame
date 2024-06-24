using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEventPool : MonoBehaviour
{
    [SerializeField] AttackEventPrefab atkEventPrefab;

    Dictionary<AttackEvent, AttackEventPrefab> atkEventLinkList = new Dictionary<AttackEvent, AttackEventPrefab>();
    Queue<AttackEventPrefab> atkObjQueue = new Queue<AttackEventPrefab>();

    // Object Pool용 프리팹 모음
    List<AttackEventPrefab> atkEventObjs = new List<AttackEventPrefab>();

    public void Enter()
    {

    }

    public void Exit()
    {
        atkEventObjs.ForEach((obj) =>
        {
            obj.Exit(null);
        });

        atkEventLinkList.Clear();
        atkObjQueue.Clear();
    }

    public void AddAttackEventObject(AttackEvent data)
    {
        AttackEventPrefab obj = null;
        for (int i = 0; i < atkEventObjs.Count; i++)
        {
            if (atkEventObjs[i].isActive() == false)
                obj = atkEventObjs[i];
        }

        if (obj == null)
            obj = CreateAttackEventObject();

        obj.Enter(data);

        atkEventLinkList.Add(data, obj);
        atkObjQueue.Enqueue(obj);

        Relocation();
    }

    public void RemoveAttackEventObject(AttackEvent data)
    {
        if (atkEventLinkList.ContainsKey(data))
        {
            atkEventLinkList[data].Exit(() =>
            {
                atkEventLinkList.Remove(data);
                atkObjQueue.Dequeue();
                Relocation();
            });
        }
    }

    public void RemoveDieAttackEventObject(List<AttackEvent> curDatas, List<AttackEvent> dieDatas)
    {
        foreach (var data in dieDatas)
        {
            if (atkEventLinkList.ContainsKey(data))
            {
                atkEventLinkList[data].Exit(null);
                atkEventLinkList.Remove(data);
            }
        }

        atkObjQueue.Clear();
        foreach (var data in curDatas)
        {
            if (atkEventLinkList.ContainsKey(data))
                atkObjQueue.Enqueue(atkEventLinkList[data]);
        }

        Relocation();
    }

    AttackEventPrefab CreateAttackEventObject()
    {
        AttackEventPrefab obj = Instantiate(atkEventPrefab, this.transform);
        atkEventObjs.Add(obj);
        return obj;
    }

    void Relocation()
    {
        int idx = 0;
        foreach (var obj in atkObjQueue)
        {
            Vector3 newPos = new Vector3(70f + (140f * idx), 0f, 0f);
            obj.gameObject.transform.localPosition = newPos;
            idx++;
        }
    }
}
