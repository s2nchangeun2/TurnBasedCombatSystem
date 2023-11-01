using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleCharacter : MonoBehaviour
{
    private enum EmState
    {
        emIdle,
        emMove,
        emAttack,
    }

    [SerializeField]
    private BaseCharacter baseCharacter = null;

    [SerializeField]
    private GameObject goSelection = null; //�������� ĳ���� ǥ��.

    //
    private EmState _emState = default;
    private Vector3 _vec3TargetPos = Vector3.zero;
    private Action _onMoveComplete = null;
    private bool _bPlayerTeam = false;
    private HPSystem _hpSystem = null;

    private void Awake()
    {
        SetActiveSelection(false);
        _emState = EmState.emIdle;
    }

    private void Update()
    {
        switch (_emState)
        {
            case EmState.emIdle:
                break;
            case EmState.emMove:
                float fMoveSpeed = 10.0f;
                transform.position += (_vec3TargetPos - GetPosition()) * fMoveSpeed * Time.deltaTime;

                float fDestination = 1.0f;
                if (Vector3.Distance(GetPosition(), _vec3TargetPos) < fDestination)
                {
                    transform.position = _vec3TargetPos;
                    _onMoveComplete?.Invoke();
                }
                break;
            case EmState.emAttack:
                break;
        }
    }

    public bool IsDead() { return _hpSystem.IsDead(); }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetActiveSelection(bool bActive)
    {
        goSelection.SetActive(bActive);
    }

    public void Setup(UEnum.EmBattleTeam emTeam)
    {
        switch (emTeam)
        {
            case UEnum.EmBattleTeam.emPlayer:
                _bPlayerTeam = true;

                baseCharacter.SetAnim1();
                //baseCharacter.SetSprite(BattleSystem.Instance.sprite2DPlayer);
                break;
            case UEnum.EmBattleTeam.emEnemy:
                _bPlayerTeam = false;

                baseCharacter.SetAnim2();
                //baseCharacter.SetSprite(BattleSystem.Instance.sprite2DEnemy);
                break;
        }

        _hpSystem = new HPSystem(100);
        //HpBar ����.
        //
        _hpSystem.onHpChanged += HpSystemEvent;

        Idle();
    }

    private void HpSystemEvent()
    {
        //hpbar ����.
        //
    }

    private void Idle()
    {
        if (_bPlayerTeam)
            baseCharacter.PlayAnim_Idle(new Vector3(1, 0));
        else
            baseCharacter.PlayAnim_Idle(new Vector3(-1, 0));
    }

    private void Move(Vector3 vec3TargetPos, Action onMoveComplete)
    {
        _emState = EmState.emMove;

        _vec3TargetPos = vec3TargetPos;
        _onMoveComplete = onMoveComplete;

        if (vec3TargetPos.x > 0)
            baseCharacter.PlayAnim_MoveRight();
        else
            baseCharacter.PlayAnim_MoveLeft();
    }

    public void Attack(BattleCharacter target, Action onAttackComplete)
    {
        Vector3 vec3TargetPos = target.GetPosition() + (GetPosition() - target.GetPosition()).normalized * 10.0f;
        Vector3 vec3StartPos = GetPosition();

        //target��ġ���� �̵�.
        Move(vec3TargetPos, () =>
        {
            _emState = EmState.emAttack;

            //target��ġ ����.target ����.
            Vector3 vec3Dir = (target.GetPosition() - GetPosition()).normalized;
            baseCharacter.PlayAnim_Attack(vec3Dir, () =>
            {
                //target ������.
                int nDamage = UnityEngine.Random.Range(20, 50);
                target.Damage(this, nDamage);
            }, () =>
            {
                //���� �Ϸ�.origin ��ġ�� ���ƿ�.
                Move(vec3StartPos, () =>
                {
                    //origin ��ġ ����.Idle ���·� ���ƿ�.
                    _emState = EmState.emIdle;
                    baseCharacter.PlayAnim_Idle(vec3Dir);
                    onAttackComplete?.Invoke();
                });
            });
        });
    }

    public void Damage(BattleCharacter attacker, int nDamage)
    {
        _hpSystem.Damage(nDamage);
        Debug.Log($"Hp: {_hpSystem.GetHp()} / Position: {GetPosition()}");

        Vector3 vec3Dir = (GetPosition() - attacker.GetPosition()).normalized;

        //damage value�� UI�� ����.
        //

        //damage value text color setting.
        baseCharacter.SetColor(Color.red);

        //blood setting.
        //BloodHandler.SpawnBlood(GetPosition(), vec3Dir);

        if (_hpSystem.IsDead())
        {
            baseCharacter.PlayAnim_Die();
        }
    }
}
