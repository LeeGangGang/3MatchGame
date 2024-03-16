using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameTextPool : MonoBehaviour
{
    readonly ObjectPoolQueue<GameText> _pool = new ObjectPoolQueue<GameText>();
    [SerializeField] GameObject _textPrefab;

    public void Init()
    {
        //Pool Setting
        for (int i = 0; i < 10; i++)
        {
            _pool.Enqueue(CreateScoreText());
        }
    }

    public void EnterScore(Vector3 pos, string text)
    {
        GameText txt = _pool.GetObject() ?? CreateScoreText();
        if (txt == null)
            txt = CreateScoreText();
        
        txt.ScoreEnter(pos, text);
    }

    public void EnterDamge(Vector3 pos, string text, bool isCri)
    {
        GameText txt = _pool.GetObject() ?? CreateScoreText();
        if (txt == null)
            txt = CreateScoreText();

        txt.DamgeEnter(pos, text, isCri);
    }

    public void AllExitScore()
    {
        _pool.DestroyAll();
    }

    GameText CreateScoreText()
    {
        GameObject go = Instantiate(_textPrefab, this.transform);
        return go.GetComponent<GameText>();
    }
}
