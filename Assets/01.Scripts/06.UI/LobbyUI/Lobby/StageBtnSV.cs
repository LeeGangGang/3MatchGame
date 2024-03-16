using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBtnSV : MonoBehaviour
{
    [SerializeField] StageBtn stageBtn;
    [SerializeField] Transform contentTr;

    public void Init(int cnt)
    {
        for (int i = 1; i <= cnt; i++)
        {
            StageBtn btn = Instantiate(stageBtn, contentTr).GetComponent<StageBtn>();
            btn.Init(i);
        }
    }
}
