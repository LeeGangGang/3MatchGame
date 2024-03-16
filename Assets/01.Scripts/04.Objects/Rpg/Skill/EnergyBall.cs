using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : ASkill
{
    Vector3 _dir = Vector3.right;
    [SerializeField] float _moveSpeed = 8f;
    [SerializeField] float _rotSpeed = 15f;
    Transform _hitPos;
    Unit _target;

    [SerializeField] Animator _anim;
    [SerializeField] RuntimeAnimatorController _fx;

    public override void Enter(List<Unit> target)
    {
        _target = target[0];
        _hitPos = _target.HitPos;
    }

    public override IEnumerator During()
    {
        while (Vector2.Distance(transform.position, _hitPos.position) > 0.15f)
        {
            Homing();
            yield return null;
        }

        Exit();
    }

    public override void Exit()
    {
        _anim.runtimeAnimatorController = _fx;

        _target.Hit(30f, true);

        Destroy(gameObject, 0.7f);
    }

    void Homing()
    {
        _dir = _hitPos.position - transform.position;
        _dir.z = 0f;
        _dir.Normalize();

        float value = Vector3.Cross(_dir, transform.right).z;
        transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime);
        transform.Rotate(0, 0, _rotSpeed * -value);
    }
}
