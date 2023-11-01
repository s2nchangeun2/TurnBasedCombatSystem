using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseCharacter : MonoBehaviour
{
    [SerializeField]
    private Image imageCharacter = null;

    public void SetSprite(Sprite sprite)
    {
        imageCharacter.sprite = sprite;
    }

    public void SetColor(Color color) { }

    public void SetAnim1() { }
    public void SetAnim2() { }

    public void PlayAnim_Attack(Vector3 vec3Dir, Action onHit, Action onComplete)
    {
        //���� ���� ����Ͽ� ��ġ �̵�.
        float nAngle = GetAngleFromVector(vec3Dir);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, nAngle));

        //�ִϸ��̼� ���.
        //

        onHit?.Invoke();
        onComplete?.Invoke();
    }

    public void PlayAnim_Idle(Vector3 vec3Pos) { }
    public void PlayAnim_MoveRight() { }
    public void PlayAnim_MoveLeft() { }
    public void PlayAnim_Die() { }

    /// <summary>
    /// ��ǥ��鿡�� ���������κ��� �� �������� ����.
    /// </summary>
    /// <param name="vec3Dir"></param>
    /// <returns></returns>
    public float GetAngleFromVector(Vector3 vec3Dir)
    {
        if (vec3Dir.x == 0f && vec3Dir.y == 0f) vec3Dir = new Vector2(0, -1);

        double n = Math.Atan2(vec3Dir.y, vec3Dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        //int angle = (int)Math.Round(n / 45);
        float angle = (float)n / 45;

        return angle;
    }
}
