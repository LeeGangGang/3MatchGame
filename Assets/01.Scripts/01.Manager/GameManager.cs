using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;

    [SerializeField] public GameTextPool TextPool;

    public eGameMode _mode = eGameMode.None;
    private Dictionary<eGameMode, AMode> _modeList = new Dictionary<eGameMode, AMode>();
    public AMode CurrMode => _modeList[_mode];

    public MyUnitController _myUnitCtrl;
    public EnemyController _enemyCtrl;

    void Awake()
    {
        Inst = GetComponent<GameManager>();
    }

    private void Start()
    {
        _modeList = FindObjectsOfType<AMode>(true).ToDictionary(x => x.Mode);

        _myUnitCtrl = FindObjectOfType<MyUnitController>(true);
        _enemyCtrl = FindObjectOfType<EnemyController>(true);

        TextPool.Init();
    }

    public void GameStart(int num)
    {
        CurrMode.Init(num);
        CurrMode.Enter();

        _myUnitCtrl.Enter();
        _enemyCtrl.Enter();

        var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
        foreach (var myUnitType in udm.MyUnit.GetMySelectUnitDataList())
        {
            int level = udm.MyUnit.GetMyUnitData(myUnitType.Key)[0];
            _myUnitCtrl.InsetUnit(myUnitType.Value - 1, myUnitType.Key.ToString(), level);
        }

        var sdm = (StageInfoDataModel)DataModelController.Inst.GetDataModel(eDataModel.StageInfoDataModel);
        int idx = 0;
        foreach (var enemyList in sdm.GetStageEnemyInfo(num))
        {
            string enemyName = enemyList.Key.ToString();
            int lastIdx = idx;
            foreach (var enemy in enemyList.Value)
            {
                for (int i = idx; i < enemy.Value + lastIdx; i++)
                {
                    _enemyCtrl.InsetUnit(i, enemyName, enemy.Key);
                    idx++;
                }
            }
        }

        Match3Manager.Inst.GameStart(num);

        StartCoroutine(UIManager.Inst.Game.ShowGameStart(() =>
        {
            Match3Manager.Inst._state = eState.Idle;

            CurrMode.GameStart();

            CurrMode.CheckingGameEnd();

            StartCoroutine(Match3Manager.Inst.FindMatchSystem());
        }));
    }

    public void GameExit()
    {
        CurrMode.Exit();

        _myUnitCtrl.Exit();
        _enemyCtrl.Exit();

        Match3Manager.Inst.GameExit();
    }

    public void Pause()
    {
        CurrMode.Pause();
    }

    public IEnumerator Restart()
    {
        CurrMode.Restart();

        // 몬스터 스택 채우기
        _enemyCtrl.AddStack(1);

        bool isAllDieEnemy = false;

        // 유저 캐릭터 공격
        foreach (MyUnit fullStackMyUnit in _myUnitCtrl.FindFullStackUnit())
        {
            // 다 죽였다면
            List<Unit> _liveEnemyList = _enemyCtrl.FindLiveUnit();
            if (_liveEnemyList.Count == 0)
            {
                isAllDieEnemy = true;
                break;
            }

            // 랜덤 인덱스 상대 공격
            bool isEndSkill = false;
            fullStackMyUnit.UseSkill(_liveEnemyList, () =>
            {
                isEndSkill = true;
            });

            yield return new WaitUntil(() => isEndSkill == true);
        }

        if (isAllDieEnemy == false)
        {
            foreach (Enemy fullStackEnemyUnit in _enemyCtrl.FindFullStackUnit())
            {
                List<Unit> _liveMyUnitList = _myUnitCtrl.FindLiveUnit();
                if (_liveMyUnitList.Count == 0)
                    break;

                bool isEndSkill = false;
                fullStackEnemyUnit.UseSkill(_liveMyUnitList, () =>
                {
                    isEndSkill = true;
                });

                yield return new WaitUntil(() => isEndSkill == true);
            }
        }
    }

    public void RemoveBlocks(HashSet<ABlock> blocks)
    {
        CurrMode.RemoveBlocks(blocks);
    }

    public void RemoveBlocks(Match match)
    {
        CurrMode.RemoveBlocks(match);
    }

    public void RemoveBlock(ABlock block)
    {
        CurrMode.RemoveBlock(block);
    }

    public void RemoveSubTiles(HashSet<ASubTile> subTiles)
    {
        CurrMode.RemoveSubTiles(subTiles);
    }

    public void RemoveSubTile(ASubTile subTile)
    {
        CurrMode.RemoveSubTile(subTile);
    }
}
