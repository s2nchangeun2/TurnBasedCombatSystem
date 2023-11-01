using System;

public class HPSystem
{
    public event Action onHpChanged = null;
    public event Action onDead = null;

    private int _nMaxHp = 0;
    private int _nHp = 0;

    public HPSystem(int nMaxHp)
    {
        _nMaxHp = nMaxHp;
        _nHp = nMaxHp;
    }

    public bool IsDead() { return _nHp <= 0; }

    public int GetHp() { return _nHp; }

    public void Damage(int nDamage)
    {
        _nHp -= nDamage;
        if (_nHp < 0)
            _nHp = 0;

        onHpChanged?.Invoke();

        if (_nHp <= 0)
            Die();
    }

    public void Die()
    {
        onDead?.Invoke();
    }
}
