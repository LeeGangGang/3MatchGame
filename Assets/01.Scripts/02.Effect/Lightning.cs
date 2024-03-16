using System.Collections;
using UnityEngine;

public class Lightning : FxBase
{
    Vector2[] posList = new Vector2[10];
    LineRenderer line;
    public Vector2 startPos;
    public Vector2 endPos;
    public bool perlin = true;

    protected override void OnEnter(Vector3 startPos, Vector3 endPos)
    {
        this.startPos = startPos;
        this.endPos = endPos; 
        StartLight();
    }

    void StartLight()
    {
        line = GetComponent<LineRenderer>();
        StartCoroutine(StartLightning());
    }

    IEnumerator StartLightning()
    {
        while (true)
        {
            posList = new Vector2[10];
            line.positionCount = posList.Length;

            for (var i = 0; i < posList.Length; i++)
            {
                posList[i] = Vector2.Lerp(startPos, endPos, (float)i / line.positionCount);
                line.SetPosition(i, posList[i]);
            }
            if (perlin)
            {
                for (var i = 0; i < posList.Length; i++)
                {
                    posList[i] = new Vector2(posList[i].x, posList[i].y + Mathf.PerlinNoise(Random.value, Random.value) * Random.Range(-1f, 1f));
                    line.SetPosition(i, posList[i]);
                }
            }
            posList[0] = startPos;
            posList[posList.Length - 1] = endPos;
            yield return new WaitForEndOfFrame();
        }
    }

    protected override void OnExit()
    {
        StopAllCoroutines();
    }
}