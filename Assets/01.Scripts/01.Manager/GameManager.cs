using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    int stageNum = 0;
    public static GameManager Inst;

    [SerializeField] public GameTextPool TextPool;

    public eGameMode mode = eGameMode.None;
    private Dictionary<eGameMode, AMode> modeList = new Dictionary<eGameMode, AMode>();
    public AMode CurrMode => modeList[mode];

    public MyUnitController myUnitCtrl;
    public EnemyController enemyCtrl;

    List<AttackEvent> attackEventList;

    void Awake()
    {
        Inst = GetComponent<GameManager>();
    }

    private void Start()
    {
        modeList = FindObjectsOfType<AMode>(true).ToDictionary(x => x.Mode);

        myUnitCtrl = FindObjectOfType<MyUnitController>(true);
        enemyCtrl = FindObjectOfType<EnemyController>(true);

        TextPool.Init();
    }

    public void GameStart(int num)
    {
        stageNum = num;

        CurrMode.Init(stageNum);
        CurrMode.Enter();

        myUnitCtrl.Enter();
        enemyCtrl.Enter();

        attackEventList = new List<AttackEvent>();

        var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
        foreach (var myUnitType in udm.MyUnit.GetMySelectUnitDataList())
        {
            int level = udm.MyUnit.GetMyUnitData(myUnitType.Key)[0];
            myUnitCtrl.InsetUnit(myUnitType.Value - 1, myUnitType.Key.ToString(), level);
        }

        var sdm = (StageInfoDataModel)DataModelController.Inst.GetDataModel(eDataModel.StageInfoDataModel);
        int idx = 0;
        foreach (var enemyList in sdm.GetStageEnemyInfo(stageNum))
        {
            string enemyName = enemyList.Key.ToString();
            int lastIdx = idx;
            foreach (var enemy in enemyList.Value)
            {
                for (int i = idx; i < enemy.Value + lastIdx; i++)
                {
                    enemyCtrl.InsetUnit(i, enemyName, enemy.Key);
                    idx++;
                }
            }
        }

        Match3Manager.Inst.GameStart(stageNum);

        StartCoroutine(UIManager.Inst.Game.ShowGameStart(() =>
        {
            Match3Manager.Inst._state = eState.Idle;

            CurrMode.GameStart();

            StartCoroutine(Match3Manager.Inst.FindMatchSystem());

            StartCoroutine(AttackEventCo());
        }));
    }

    public void GameExit()
    {
        CurrMode.Exit();

        myUnitCtrl.Exit();
        enemyCtrl.Exit();

        attackEventList.Clear();
        StopAllCoroutines();

        Match3Manager.Inst.GameExit();
    }

    public void Pause()
    {
        CurrMode.Pause();
    }

    public IEnumerator Restart()
    {
        // 몬스터 스택 채우기
        enemyCtrl.AddStack(1);

        CheckUnitAttack();

        CurrMode.Restart();

        yield return null;
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

    void CheckUnitAttack()
    {
        bool isAllDieEnemy = false;

        // 유저 캐릭터 공격 확인
        foreach (MyUnit fullStackMyUnit in myUnitCtrl.FindFullStackUnit())
        {
            // 다 죽였다면
            List<Unit> _liveEnemyList = enemyCtrl.FindLiveUnit();
            if (_liveEnemyList.Count == 0)
            {
                isAllDieEnemy = true;
                break;
            }

            // 상대 공격 대기
            fullStackMyUnit.CheckUseSkill(_liveEnemyList, (atkEvents) =>
            {
                if (atkEvents.Count > 0)
                {
                    foreach (var atkEvent in atkEvents)
                    {
                        CurrMode.AddAttackEvent(atkEvent);
                        attackEventList.Add(atkEvent);
                    }
                }
            });
        }

        if (isAllDieEnemy == false)
        {
            foreach (Enemy fullStackEnemyUnit in enemyCtrl.FindFullStackUnit())
            {
                List<Unit> _liveMyUnitList = myUnitCtrl.FindLiveUnit();
                if (_liveMyUnitList.Count == 0)
                    break;

                fullStackEnemyUnit.CheckUseSkill(_liveMyUnitList, (atkEvents) =>
                {
                    foreach (var atkEvent in atkEvents)
                    {
                        CurrMode.AddAttackEvent(atkEvent);
                        attackEventList.Add(atkEvent);
                    }
                });
            }
        }

        //attackEventCo = AttackEventCo();
        //if (isUseingAtkEventCo == false)
        //    StartCoroutine(attackEventCo);
    }

    IEnumerator AttackEventCo()
    {
        bool isClear = false;
        while (true)
        {
            if (attackEventList.Count > 0)
            {
                AttackEvent eventData = attackEventList[0];
                string name = eventData.MyName;
                int idx = eventData.Index;
                string skill = eventData.SkillName;
                Unit unit;
                if (eventData.Type == 0)
                {
                    // 내유닛
                    unit = myUnitCtrl.GetUnit(name, idx);
                }
                else
                {
                    // 적유닛
                    unit = enemyCtrl.GetUnit(name, idx);
                }

                attackEventList.Remove(eventData);
                CurrMode.RemoveAttackEvent(eventData);

                Debug.Log(string.Format("{0} : {1}", unit.Name, eventData.SkillName));

                unit.AttackStart(skill, eventData.SkillIndex);

                // 공격이벤트 끝날 때까지 대기
                yield return new WaitUntil(() => unit.IsAttacking == false);

                // 게임 종료 체크
                // 적을 다 죽였을 경우 클리어
                List<Unit> _liveEnemyList = enemyCtrl.FindLiveUnit();
                if (_liveEnemyList.Count == 0)
                {
                    isClear = true;
                    break;
                }
                // 내 유닛이 다 죽었을 경우 실패
                List<Unit> _liveMyUnitList = myUnitCtrl.FindLiveUnit();
                if (_liveMyUnitList.Count == 0)
                {
                    isClear = false;
                    break;
                }
            }

            yield return null;
        }

        if (isClear == false)
            yield return StartCoroutine(UIManager.Inst.Game.ShowGameEnd(0));
        else
            yield return StartCoroutine(UIManager.Inst.Game.ShowPraise(1));

        yield return StartCoroutine(Match3Manager.Inst.EndingUseItem());

        yield return new WaitForSeconds(1f);

        UIManager.Inst.Popup.OpenResultPopup(isClear, GetResultReward(isClear));
    }

    public void OnUnitDieEvent(string name)
    {
        List<AttackEvent> removeList = new List<AttackEvent>();
        int atkQueueCnt = attackEventList.Count;
        for (int i = 0; i < atkQueueCnt; i++)
        {
            string myName = attackEventList[i].MyName;
            if (myName == name)
                removeList.Add(attackEventList[i]);
        }

        attackEventList.RemoveAll(x => x.MyName == name);
        CurrMode.RemoveDieAttackEvent(attackEventList, removeList);
    }

    Dictionary<eProductType, Dictionary<int, int>> GetResultReward(bool isClear)
    {
        Dictionary<eProductType, Dictionary<int, int>> addItemsList = new Dictionary<eProductType, Dictionary<int, int>>();
    
        var mwdm = (MyWealthDataModel)DataModelController.Inst.GetDataModel(eDataModel.MyWealthDataModel);
        // TODO : 임시 데이터이므로 나중에 레벨링 해서 다시 조정할 것
        if (isClear)
        {
            // Gold
            {
                Dictionary<int, int> addGold = new Dictionary<int, int>();
                int goldCnt = Random.Range(stageNum * 3, stageNum * 4);
                addGold.Add(0, goldCnt);
                addItemsList.Add(eProductType.Gold, addGold);
                mwdm.Gold += goldCnt;
            }

            // Dia
            {
                Dictionary<int, int> addDia = new Dictionary<int, int>();
                int diaCnt = Random.Range(stageNum, stageNum + 10);
                addDia.Add(0, diaCnt);
                addItemsList.Add(eProductType.Dia, addDia);
                mwdm.Dia += diaCnt;
            }

            // Card
            if (Random.Range(0f, 100f) > 80f - stageNum)
            {
                Dictionary<int, int> addCard = new Dictionary<int, int>();

                var scdm = (StoreChartDataModel)DataModelController.Inst.GetDataModel(eDataModel.StoreChartDataModel);
                var cdm = (CardDataModel)DataModelController.Inst.GetDataModel(eDataModel.CardDataModel);
                int code = scdm.GetCardCode(eProductID.NormalCardPack);
                addCard.Add(code, 1);
                addItemsList.Add(eProductType.Card, addCard);

                cdm.MyCard.SetAddCard(code, 1);
            }

            // Unit
            if (Random.Range(0f, 100f) > 80f - stageNum)
            {
                Dictionary<int, int> addUnit = new Dictionary<int, int>();

                var scdm = (StoreChartDataModel)DataModelController.Inst.GetDataModel(eDataModel.StoreChartDataModel);
                var udm = (UnitDataModel)DataModelController.Inst.GetDataModel(eDataModel.UnitDataModel);
                eUnit code = scdm.GetUnitCode(eProductID.NormalUnitPack);
                addUnit.Add((int)code, 1);
                addItemsList.Add(eProductType.Unit, addUnit);

                udm.MyUnit.SetAddUnit(code, 1);
            }
        }
        else
        {
            Dictionary<int, int> addGold = new Dictionary<int, int>();

            int goldCnt = Random.Range(stageNum * 1, stageNum * 2);
            addGold.Add(0, goldCnt);
            addItemsList.Add(eProductType.Gold, addGold);
            mwdm.Gold += goldCnt;
        }

        return addItemsList;
    }
}
