using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseCharacter : MonoBehaviour
{
    [SerializeField]
    private Image imageCharacter = null;

    [SerializeField]
    private Animator animator = null;

    private const string _C_STR_CHARACTER = "Character";
    protected const float _C_F_MOVESPEED = 10.0f;

    public void SetSprite(int nID)
    {
        string strName = string.Format("{0}{1}", _C_STR_CHARACTER, nID);
        AssetManager.Instance.LoadAssetAsync<Sprite>(strName, (sprite) => imageCharacter.sprite = sprite);
    }

    public void SetIdle(Vector3 vec3Dir)
    {
        //animator.Play("Idle");
    }

    public void SetMoveToRight() { }
    public void SetMoveToLeft() { }
    public void SetDie() { }

    /// <summary>   
    /// </summary>
    /// <param name="vec3Dir"></param>
    /// <param name="onHit">공격중</param>
    /// <param name="onComplete">공격완료</param>
    public void SetAttack(Vector3 vec3Dir, Action onHit, Action onComplete)
    {
        //GetAngleFromVector3(vec3Dir, out int nAngle);
        //SetSprite(nAngle);

        //animation.
        //

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
