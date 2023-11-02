using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    private enum EmBattleTurn
    {
        emUnknown,
        emPlayerTeam,
        emEnemyTeam,
    }

    public enum EmBattleTeam
    {
        emPlayer,
        emEnemy,
    }

    #region [�ν����� ����]
    [SerializeField]
    private Transform traPlayerPos = null;

    [SerializeField]
    private Transform traEnemyPos = null;

    [SerializeField]
    public TextMeshProUGUI textTimer = null;

    //���� ����(�����͸� �޾ƿ��� ������).    
    public Sprite sprite2DPlayer = null;
    public Sprite sprite2DEnemy = null;
    #endregion

    private EmBattleTurn _emBattleState = EmBattleTurn.emUnknown;

    private BattleCharacter _player = null;
    private BattleCharacter _enemy = null;
    private BattleCharacter _battleCharacter = null;

    private Coroutine _coroutineTimer = null;
    private float _fBattleTimer = 180f; //3��.

    private bool _bSetting = false;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        SpawnCharacter(EmBattleTeam.emPlayer, (character) =>
        {
            _player = character;
            SetActiveBattleCharacter(_player);
        });

        SpawnCharacter(EmBattleTeam.emEnemy, (character) =>
        {
            _enemy = character;
        });

        _emBattleState = EmBattleTurn.emPlayerTeam;

        StartBattle();
    }

    private void SpawnCharacter(EmBattleTeam emTeam, Action<BattleCharacter> onComplete)
    {
        string strTeam = string.Empty;
        Transform traPos = null;

        switch (emTeam)
        {
            case EmBattleTeam.emPlayer:
                strTeam = "Player";
                traPos = traPlayerPos;
                break;
            case EmBattleTeam.emEnemy:
                strTeam = "Enemy";
                traPos = traEnemyPos;
                break;
        }

        AssetManager.Instance.Instantiate(strTeam, traPos, (go) =>
        {
            BattleCharacter battleCharacter = go.transform.GetComponent<BattleCharacter>();
            battleCharacter.Setup(emTeam);
            onComplete?.Invoke(battleCharacter);
            _bSetting = true;
        });
    }

    private void StartBattle()
    {
        StartCoroutine(BattleLoopCoroutine());
    }

    private IEnumerator BattleLoopCoroutine()
    {
        //Spawn �� Setting �Ϸ�� ������ ���.
        while (!_bSetting)
        {
            yield return null;
        }

        //Ÿ�̸� ������Ʈ.
        UpdateTimer();

        //�ڵ� ����.
        AutoBattle();
    }

    private void AutoBattle()
    {
        //������ Player Team����.
        _player.TakeTurn(_enemy, () => TakeNextTurn());
    }

    private void TakeNextTurn()
    {
        if (IsBattleOver())
        {
            StopTimer();
            return;
        }

        if (_battleCharacter == _player)
        {
            _emBattleState = EmBattleTurn.emEnemyTeam;
            SetActiveBattleCharacter(_enemy);
            _enemy.TakeTurn(_player, () => TakeNextTurn());
        }
        else
        {
            _emBattleState = EmBattleTurn.emPlayerTeam;
            SetActiveBattleCharacter(_player);
            _player.TakeTurn(_enemy, () => TakeNextTurn());
        }
    }

    /// <summary>
    /// ���� �������� ĳ���� ����.
    /// </summary>
    /// <param name="battleCharacter">������</param>
    private void SetActiveBattleCharacter(BattleCharacter battleCharacter)
    {
        if (_battleCharacter != null)
            _battleCharacter.SetActiveSelection(false);

        _battleCharacter = battleCharacter;
        _battleCharacter.SetActiveSelection(true);
    }

    private void UpdateTimer()
    {
        _coroutineTimer = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            int nMin = Mathf.FloorToInt(_fBattleTimer / 60);
            int nSec = Mathf.FloorToInt(_fBattleTimer % 60);
            textTimer.text = string.Format("{0:00}:{1:00}", nMin, nSec);

            _fBattleTimer -= Time.deltaTime;

            yield return null;
        }
    }

    private void StopTimer()
    {
        if (_coroutineTimer != null)
        {
            StopCoroutine(_coroutineTimer);
            _coroutineTimer = null;
        }

        _fBattleTimer = 60.0f;
    }

    private bool IsBattleOver()
    {
        if (_player.IsDead())
        {
            Debug.Log("Enemy Win!!");
            return true;
        }

        if (_enemy.IsDead())
        {
            Debug.Log("Player Win!!");
            return true;
        }

        if (IsTimeOver())
        {
            Debug.Log("Timer is Over!! Player Lose!!");
            return true;
        }

        return false;
    }

    private bool IsTimeOver()
    {
        return _fBattleTimer <= 0;
    }
}
