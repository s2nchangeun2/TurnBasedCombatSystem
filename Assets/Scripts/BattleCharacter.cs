using System;
using UnityEngine;

public class BattleCharacter : BaseCharacter
{
    private enum EmState
    {
        emIdle,
        emMove,
        emAttack,
    }

    [SerializeField]
    private GameObject goTurn = null;

    [SerializeField]
    private Transform traHpBar = null;

    private EmState _emState = default;

    private HPSystem _hpSystem = null;
    private UIHpBar _uiHpBar = null;

    private bool _bPlayerTeam = false;

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

                SetAnim1();
                //baseCharacter.SetSprite(BattleSystem.Instance.sprite2DPlayer);
                break;
            case BattleSystem.EmBattleTeam.emEnemy:
                _bPlayerTeam = false;

                SetAnim2();
                //baseCharacter.SetSprite(BattleSystem.Instance.sprite2DEnemy);
                break;
        }

        _hpSystem = new HPSystem(100);
        AssetManager.Instance.Instantiate("UIHpBar", traHpBar, (go) =>
        {
            _uiHpBar = go.transform.GetComponent<UIHpBar>();
            _hpSystem.onHpChanged += _uiHpBar.UpdateHpBar;
        });

        Idle();
    }

    private void Idle()
    {
        if (_bPlayerTeam)
            PlayAnim_Idle(new Vector3(1, 0));
        else
            PlayAnim_Idle(new Vector3(-1, 0));
    }


    private Vector3 _vec3TargetPos = Vector3.zero;
    private Action _onMoveComplete = null;
    private void Move(Vector3 vec3TargetPos, Action onMoveComplete)
    {
        _emState = EmState.emMove;

        _vec3TargetPos = vec3TargetPos;
        _onMoveComplete = onMoveComplete;

        if (vec3TargetPos.x > 0)
            PlayAnim_MoveRight();
        else
            PlayAnim_MoveLeft();
    }

    public void TakeTurn(BattleCharacter target, Action onAttackComplete)
    {
        Vector3 vec3TargetPos = target.GetPosition() + (GetPosition() - target.GetPosition()).normalized * 10.0f;
        Vector3 vec3StartPos = GetPosition();

        //target위치까지 이동.
        Move(vec3TargetPos, () =>
        {
            _emState = EmState.emAttack;

            //target위치 도착.target 공격.
            Vector3 vec3Dir = (target.GetPosition() - GetPosition()).normalized;
            PlayAnim_Attack(vec3Dir, () =>
            {
                //target 때리기.
                int nDamage = UnityEngine.Random.Range(20, 50);
                target.Damage(this, nDamage);
            }, () =>
            {
                //공격 완료.origin 위치로 돌아옴.
                Move(vec3StartPos, () =>
                {
                    //origin 위치 도착.Idle 상태로 돌아옴.
                    _emState = EmState.emIdle;
                    PlayAnim_Idle(vec3Dir);
                    onAttackComplete?.Invoke();
                });
            });
        });
    }

    public void Damage(BattleCharacter attacker, int nDamage)
    {
        _hpSystem.Damage(nDamage);
        Debug.Log($"{this}'s Hp: {_hpSystem.GetHp()} / Position: {GetPosition()}");

        Vector3 vec3Dir = (GetPosition() - attacker.GetPosition()).normalized;

        //damage value를 UI로 띄우기.
        //

        //damage value text color setting.
        SetColor(Color.red);

        //blood setting.
        //BloodHandler.SpawnBlood(GetPosition(), vec3Dir);

        if (_hpSystem.IsDead())
        {
            PlayAnim_Die();
        }
    }

    public bool IsDead()
    {
        return _hpSystem.IsDead();
    }
}
