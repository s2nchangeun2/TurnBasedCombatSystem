using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private enum EmBattleState
    {
        emWait, //wait for player
        emAct,
    }

    [SerializeField]
    private Transform traPlayerPos = null;

    [SerializeField]
    private Transform traEnemyPos = null;

    public Sprite sprite2DPlayer = null;
    public Sprite sprite2DEnemy = null;

    //
    private EmBattleState _emBattleState;
    private BattleCharacter _player = null;
    private BattleCharacter _enemy = null;
    private BattleCharacter _battleCharacter = null;

    private void Start()
    {
        SpawnCharacter(UEnum.EmBattleTeam.emPlayer, (character) =>
        {
            _player = character;
            SetActiveBattleCharacter(_player);
        });

        SpawnCharacter(UEnum.EmBattleTeam.emEnemy, (character) =>
        {
            _enemy = character;
        });

        _emBattleState = EmBattleState.emWait;
    }

    private void Update()
    {
        if (_emBattleState == EmBattleState.emWait)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _emBattleState = EmBattleState.emAct;
                _player.Attack(_enemy, () => SetNextBattleCharacter());
            }
        }
    }

    private void SpawnCharacter(UEnum.EmBattleTeam emTeam, Action<BattleCharacter> onActHandelerT)
    {
        string strTeam = string.Empty;
        Transform traPos = null;

        switch (emTeam)
        {
            case UEnum.EmBattleTeam.emPlayer:
                strTeam = "Player";
                traPos = traPlayerPos;
                break;
            case UEnum.EmBattleTeam.emEnemy:
                strTeam = "Enemy";
                traPos = traEnemyPos;
                break;
        }

        AssetManager.Instance.Instantiate(strTeam, traPos, (go) =>
        {
            BattleCharacter battleCharacter = go.transform.GetComponent<BattleCharacter>();
            battleCharacter.Setup(emTeam);
            onActHandelerT?.Invoke(battleCharacter);
        });
    }

    /// <summary>
    /// 현재 공격중인 캐릭터.
    /// </summary>
    /// <param name="battleCharacter"></param>
    private void SetActiveBattleCharacter(BattleCharacter battleCharacter)
    {
        if (_battleCharacter != null)
            _battleCharacter.SetActiveSelection(false);

        _battleCharacter = battleCharacter;
        _battleCharacter.SetActiveSelection(true);
    }

    private void SetNextBattleCharacter()
    {
        //Test.
        if (IsBattleOver_Test()) return;

        if (_battleCharacter == _player)
        {
            SetActiveBattleCharacter(_enemy);
            _emBattleState = EmBattleState.emAct;
            _enemy.Attack(_player, () => SetNextBattleCharacter());
        }
        else
        {
            SetActiveBattleCharacter(_player);
            _emBattleState = EmBattleState.emWait;
        }
    }

    //Test.
    private bool IsBattleOver_Test()
    {
        if (_player.IsDead())
        {
            //player dead, enemy win
            Debug.Log("Enemy Win!!");
            return true;
        }

        if (_enemy.IsDead())
        {
            //enemy dead, player win
            Debug.Log("Player Win!!");
            return true;
        }

        return false;
    }
}
