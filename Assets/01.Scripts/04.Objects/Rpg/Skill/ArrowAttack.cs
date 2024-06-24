using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAttack : ASkill
{
    Projectile projectile;
    Transform hitPos;
    Unit target;

    Vector2 org;
    float disX;
    float disY;

    float h = 0.5f;

    public override void Init(SkillData data, Unit unit)
    {
        base.Init(data, unit);
    }

    public override void Enter(List<Unit> targets)
    {
        target = targets[0];
        hitPos = target.HitPos;

        var pro = Resources.Load("SkillObject/NormalArrow");
        projectile = ((GameObject)GameObject.Instantiate(pro)).GetComponent<Projectile>();
        projectile.gameObject.transform.position = unit.transform.position;
        projectile.SetActive(false);
        org = projectile.gameObject.transform.position;

        disX = hitPos.position.x - org.x;
        disY = hitPos.position.y - org.y;
    }

    public override IEnumerator During()
    {
        projectile.Enter(target);

        float t = 0f;
        float timer = 0f;
        while (t < disX * 0.5f)
        {
            yield return null;
            t += Time.deltaTime * disX;
            timer += Time.deltaTime;
            ParabolaMove(t / 0.5f);
        }

        bool isCri = UnityEngine.Random.Range(0, 100) <= unit.critical_Per;
        target.Hit(data.Value, isCri);

        Exit();
    }

    public override void Exit()
    {
        projectile.Exit();
        projectile = null;
    }

    void ParabolaMove(float t)
    {
        float x = t;
        float a = 4 * h / -Mathf.Pow(disX, 2);
        float y = Mathf.Pow(x - (disX / 2f), 2) * a + h + (disY * (t / disX * 0.5f));
        Vector3 movePos = new Vector3(org.x + x, org.y + y, 0f);
        Vector3 dir = movePos - projectile.gameObject.transform.position;
        dir.z = 0f;
        dir.Normalize();

        float value = Vector3.Cross(projectile.gameObject.transform.right, dir).z;
        projectile.gameObject.transform.position = movePos;
        projectile.gameObject.transform.Rotate(0f, 0f, 10f * value);
    }
}
