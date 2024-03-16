using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitSpawnBlockController : MonoBehaviour
{
    [SerializeField] Transform _blockRootTr;
    [SerializeField] Transform _spwanRootTr = null;
    [SerializeField] GameObject _spwanObj = null;

    List<SpawnPos> _allSpawnPos = new List<SpawnPos>();
    Queue<ABlock> spawnQueue = new Queue<ABlock>();

    public SpawnPos CreateSpawnPos(Vector3 pos)
    {
        SpawnPos spawnPos = Instantiate(_spwanObj, pos, Quaternion.identity, _spwanRootTr).GetComponent<SpawnPos>();
        _allSpawnPos.Add(spawnPos);

        return spawnPos;
    }

    public ABlock SetFallBlock(SpawnPos spawn)
    {
        ABlock fallBlock = GetBlock();
        if (fallBlock != null)
        {
            fallBlock.transform.position = new Vector3(spawn.transform.position.x, spawn.transform.position.y + 1f, 0f);
            fallBlock.Init(spawn.Pos.x, spawn.Pos.y + 1, Match3Manager.Inst.StepIdx);
            fallBlock.ChangeItemSprite(fallBlock._item, true);
        }

        fallBlock.transform.SetParent(Match3Manager.Inst.BlockCtrl.BlockRoot);
        fallBlock.SetActive(true);

        return fallBlock;
    }

    public void AddBlock(ABlock block)
    {
        if (spawnQueue.Contains(block))
            return;

        spawnQueue.Enqueue(block);
        block.SetActive(false);
        block.transform.SetParent(_blockRootTr);
    }

    public void Clear()
    {
        foreach (SpawnPos spawnPos in _allSpawnPos)
            Destroy(spawnPos.gameObject);

        _allSpawnPos.Clear();
        spawnQueue.Clear();
    }

    ABlock GetBlock()
    {
        if (spawnQueue.Count == 0)
            Match3Manager.Inst.BlockCtrl.CreateSupplementBlock();

        return spawnQueue.Dequeue();
    }
}
