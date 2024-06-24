using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultipleEnergyBall : ASkill
{
    List<Projectile> projectiles = new List<Projectile>();
    float moveSpeed = 8f;
    float rotSpeed = 30f;

    Vector3 dir = Vector3.right;
    int cnt = 5;

    public override void Init(SkillData data, Unit unit)
    {
        base.Init(data, unit);
    }

    public override void Enter(List<Unit> targets)
    {
        this.targets = targets;

        var pro = Resources.Load("SkillObject/EnergyBall");
        for (int i = 0; i < cnt; i++)
        {
            Projectile projectile = ((GameObject)GameObject.Instantiate(pro)).GetComponent<Projectile>();
            projectile.SetActive(false);
            projectiles.Add(projectile);
        }
    }

    public override IEnumerator During()
    {
        Dictionary<int, bool> endCntCheckList = new Dictionary<int, bool>()
        {
            {0, false},
            {1, false},
            {2, false},
            {3, false},
            {4, false},
        };

        for (int i = 0; i < cnt; i++)
        {
            int rndIdx = Random.Range(0, targets.Count);
            Unit target = targets[rndIdx];

            float angle = Mathf.Atan2(target.transform.position.y, target.transform.position.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.AngleAxis(-20 + (i * 10), Vector3.forward);
            projectiles[i].gameObject.transform.position = unit.transform.position;
            projectiles[i].gameObject.transform.rotation = rot;
            projectiles[i].Enter(target);
        }

        while (endCntCheckList.Values.Where(value => value == true).Count() != cnt)
        {
            for (int idx = 0; idx < cnt; idx++)
            {
                if (endCntCheckList[idx] == true)
                    continue;

                Homing(idx);

                if (projectiles[idx].Target.HitPos.position.x - projectiles[idx].gameObject.transform.position.x < 0f)
                {
                    endCntCheckList[idx] = true;

                    bool isCri = Random.Range(0, 100) <= unit.critical_Per;
                    projectiles[idx].Target.Hit(data.Value, isCri);
                    projectiles[idx].Exit();
                }
            }
            
            yield return null;
        }

        Exit();
    }

    public override void Exit()
    {
        projectiles.Clear();
    }

    void Homing(int idx)
    {
        dir = projectiles[idx].Target.gameObject.transform.position - projectiles[idx].gameObject.transform.position;
        dir.z = 0f;
        dir.Normalize();

        float value = Vector3.Cross(dir, projectiles[idx].gameObject.transform.right).z;
        projectiles[idx].gameObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        projectiles[idx].gameObject.transform.Rotate(0, 0, rotSpeed * -value);
    }
}
