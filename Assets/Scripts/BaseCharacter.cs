using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseCharacter : MonoBehaviour
{
    public enum EmDirection
    {
        emUnknown,
        emLeft,
        emRight
    }

    [SerializeField]
    private Image imageCharacter = null;

    [SerializeField]
    private Animator animator = null;

    [SerializeField]
    private EmDirection _emDirection = EmDirection.emUnknown;

    protected const float _C_F_MOVESPEED = 6.0f;

    private float _fDirection = 0f;

    private void Awake()
    {
        SetDirection(_emDirection);
    }

    public void SetDirection(EmDirection emDirection)
    {
        _fDirection = _emDirection == EmDirection.emLeft ? -1.0f : 1.0f;
        imageCharacter.transform.localScale = new Vector3(_fDirection, 1.0f, 1.0f);
    }

    private void ChangeDirection()
    {
        _fDirection *= -1.0f;
    }

    public void SetIdle()
    {
        animator.Play("Idle");
    }

    public void SetMoveToRight(bool bAcitve)
    {
        animator.SetBool("Move", bAcitve);
    }

    public void SetMoveToLeft(bool bActive)
    {
        ChangeDirection();
        animator.SetBool("Move", bActive);
    }

    public void SetDie()
    {
        animator.SetBool("Die", true);
    }

    public void SetAttack(bool bActive)
    {
        animator.SetBool("Attack", bActive);
    }

    /// <summary>   
    /// </summary>
    /// <param name="vec3Dir"></param>
    /// <param name="onHit">공격중</param>
    /// <param name="onComplete">공격완료</param>
    public void SetAttack(Vector3 vec3Dir, Action onHit, Action onComplete)
    {
        onHit?.Invoke();
        onComplete?.Invoke();
    }

    /// <summary>
    /// 좌표평면에서 수평축으로부터 한 점까지의 각도.
    /// </summary>
    /// <param name="vec3Dir"></param>
    /// <returns></returns>
    public void GetAngleFromVector3(Vector3 vec3Dir, out int nAngle)
    {
        if (vec3Dir.x == 0f && vec3Dir.y == 0f)
            vec3Dir = new Vector2(0, -1);

        double n = Math.Atan2(vec3Dir.y, vec3Dir.x) * Mathf.Rad2Deg;
        if (n < 0)
            n += 360;

        nAngle = (int)Math.Round((float)n / 45);
    }
}
