using System;
using UnityEngine;

public class BattleCharacter : BaseCharacter
{
    private enum EmState
    {
        emIdle,
        emMove,
        emAttack,
        emDie,
    }

    [SerializeField]
    private GameObject goTurn = null;

    [SerializeField]
    private Transform traHpBar = null;

    private EmState _emState = default;

    private HPSystem _hpSystem = null;

    private bool _bPlayerTeam = false;

    private const string _C_STR_UIHPBAR = "UIHpBar";
    private const string _C_STR_UIDAMAGE = "UIDamage";

    private void Start()
    {
        SetActiveSelection(false);
        _emState = EmState.emIdle;
    }

    private void Update()
    {
        switch (_emState)
        {
            case EmState.emIdle:
                UpdateIdle();
                break;
            case EmState.emMove:
                UpdateMove();
                break;
            case EmState.emAttack:
                UpdateAttack();
                break;
            case EmState.emDie:
                UpdateDie();
                break;
        }
    }

    private Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetActiveSelection(bool bActive)
    {
        goTurn.SetActive(bActive);
    }

    public void Setup(BattleSystem.EmBattleTeam emTeam)
    {
        switch (emTeam)
        {
            case BattleSystem.EmBattleTeam.emPlayer:
                _bPlayerTeam = true;
                break;
            case BattleSystem.EmBattleTeam.emEnemy:
                _bPlayerTeam = false;
                break;
        }

        _hpSystem = new HPSystem(100);
        //hp bar 띄우기.
        AssetManager.Instance.Instantiate(_C_STR_UIHPBAR, traHpBar, (go) =>
        {
            UIHpBar _uiHpBar = go.transform.GetComponent<UIHpBar>();
            _hpSystem.onHpChanged += _uiHpBar.UpdateHpBar;
        });
    }

    private void UpdateIdle()
    {
        SetIdle();
    }

    private void UpdateMove()
    {
        bool bRightDir = _vec3TargetPos.x > 0;
        if (bRightDir) SetMoveToRight(true);
        else SetMoveToLeft(true);

        transform.position += (_vec3TargetPos - GetPosition()) * _C_F_MOVESPEED * Time.deltaTime;

        if (Vector3.Distance(GetPosition(), _vec3TargetPos) < 30.0f)
        {
            if (bRightDir) SetMoveToRight(false);
            else SetMoveToLeft(false);

            transform.position = _vec3TargetPos;
            _onMoveComplete?.Invoke();
        }
    }

    private void UpdateAttack()
    {
        SetAttack(() =>
        {
            _onComplete?.Invoke();
        });
    }

    private void UpdateDie()
    {
        SetDie();
    }

    public void TakeTurn(BattleCharacter target, Action onAttackComplete)
    {
        Vector3 vec3TargetPos = target.GetPosition() + (GetPosition() - target.GetPosition()).normalized * 10.0f;
        Vector3 vec3StartPos = GetPosition();

        //target위치까지 이동.
        Move(vec3TargetPos, () =>
        {
            //target 위치 도착.
            //target 공격.
            Attack(target, () =>
            {
                //공격 완료.
                Move(vec3StartPos, () =>
                {
                    //origin 위치 도착.
                    _emState = EmState.emIdle;
                    onAttackComplete?.Invoke();
                });
            });
        });
    }

    private Vector3 _vec3TargetPos = Vector3.zero;
    private Action _onMoveComplete = null;
    private void Move(Vector3 vec3TargetPos, Action onMoveComplete)
    {
        _emState = EmState.emMove;

        _vec3TargetPos = vec3TargetPos;
        _onMoveComplete = onMoveComplete;
    }

    private Action _onComplete = null;
    private void Attack(BattleCharacter target, Action onComplete)
    {
        _emState = EmState.emAttack;

        int nDamage = UnityEngine.Random.Range(20, 50);
        target.Damage(this, nDamage);

        _onComplete = onComplete;
    }

    public void Damage(BattleCharacter attacker, int nDamage)
    {
        _hpSystem.Damage(nDamage);

        Debug.Log($"{this}'s Hp: {_hpSystem.GetHp()} / Position: {GetPosition()}");

        //damage value를 UI로 띄우기.
        AssetManager.Instance.Instantiate(_C_STR_UIDAMAGE, transform, (go) =>
        {
            UIDamage uiDamage = go.transform.GetComponent<UIDamage>();
            uiDamage.SetDamage(nDamage);
        });

        if (IsDead())
        {
            _emState = EmState.emDie;
        }
    }

    public bool IsDead()
    {
        return _hpSystem.IsDead();
    }
}
