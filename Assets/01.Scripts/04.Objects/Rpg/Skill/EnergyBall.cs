using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : ASkill
{
    Projectile projectile;
    float moveSpeed = 8f;
    float rotSpeed = 15f;

    Vector3 dir = Vector3.right;
    Transform hitPos;
    Unit target;

    public override void Init(SkillData data, Unit unit)
    {
        base.Init(data, unit);
    }

    public override void Enter(List<Unit> targets)
    {
        target = targets[0];
        hitPos = target.HitPos;

        var pro = Resources.Load("SkillObject/EnergyBall");
        projectile = ((GameObject)GameObject.Instantiate(pro)).GetComponent<Projectile>();
        float angle = Mathf.Atan2(target.transform.position.y, target.transform.position.x) * Mathf.Rad2Deg;
        float a_angle = angle + UnityEngine.Random.Range(-10, 10);
        Quaternion rot = Quaternion.AngleAxis(a_angle, Vector3.forward);
        projectile.gameObject.transform.position = unit.transform.position;
        projectile.gameObject.transform.rotation = rot;
        projectile.SetActive(false);
    }

    public override IEnumerator During()
    {
        projectile.Enter(target);

        while (hitPos.position.x - projectile.gameObject.transform.position.x >= 0f)
        {
            yield return null;
            Homing();
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

    void Homing()
    {
        dir = hitPos.position - projectile.gameObject.transform.position;
        dir.z = 0f;
        dir.Normalize();

        float value = Vector3.Cross(dir, projectile.gameObject.transform.right).z;
        projectile.gameObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        projectile.gameObject.transform.Rotate(0, 0, rotSpeed * -value);
    }
}
