using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Inst;

    bool _moving = false;
    [SerializeField] Transform _boardCamTr;

    [SerializeField] Camera _uiCam;

    [SerializeField] Camera _rpgCam;

    [SerializeField] Camera _boardCam;
    public Camera BoardCam => _boardCam;
    
    public Vector3 pos;

    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        Inst = this;
    }

    public void SetActiveInGameCamera(bool isActive)
    {
        _rpgCam.gameObject.SetActive(isActive);
        _boardCam.gameObject.SetActive(isActive);
    }

    public void SetBoardCameraPos(int _mapSizeX, int _mapSizeY)
    {
        _boardCam.transform.position = pos = new Vector3((_mapSizeX - 1) * 0.5f, (_mapSizeY -1f) * 0.5f, -10f);
        _boardCam.orthographicSize = (_mapSizeY + 1) / 2f + 0.5f;
    }

    public void ShakeBoard(float time)
    {
        if (_moving)
            return;

        _moving = true;
        StartCoroutine(ShakeBoardCo(time));
    }

    IEnumerator ShakeBoardCo(float time)
    {
        int moveCnt = (int)(time / 0.09f);
        while (moveCnt > 0)
        {
            bool isOneMoveEnd = false;
            Sequence seq = DOTween.Sequence();
            seq.Append(_boardCamTr.DOMoveY(pos.y - 0.05f, 0.03f).SetEase(Ease.Linear))
                .Append(_boardCamTr.DOMoveY(pos.y + 0.05f, 0.03f).SetEase(Ease.Linear))
                .Append(_boardCamTr.DOMoveY(pos.y, 0.03f).SetEase(Ease.Linear)).OnComplete(() =>
                {
                    isOneMoveEnd = true;
                });

            yield return new WaitUntil(() => isOneMoveEnd);
            moveCnt--;
        }

        _moving = false;
        yield return null;
    }
}
