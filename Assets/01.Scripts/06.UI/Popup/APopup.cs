using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class APopup : MonoBehaviour
{
    [SerializeField] Image mainPanel;

    bool isEnter = false;

    public virtual void Init()
    {
        
    }

    public virtual void Enter()
    {
        isEnter = true;

        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            this.gameObject.transform.SetSiblingIndex(UIManager.Inst.Popup.OpenPopupStk.Count);
            UIManager.Inst.Popup.OpenPopupStk.Push(this);
        }
        this.gameObject.SetActive(true);
        mainPanel.transform.localScale = Vector3.zero;
        mainPanel.transform.DOScale(1f, 0.1f);
    }

    public virtual void Exit()
    {
        if (!isEnter)
            return;

        isEnter = false;
        
        if (SceneManager.GetActiveScene().name == "MainScene")
            UIManager.Inst.Popup.OpenPopupStk.Pop();

        mainPanel.transform.DOScale(0f, 0.1f).OnComplete(() =>
        {
            this.gameObject.SetActive(false);
        });
    }
}
