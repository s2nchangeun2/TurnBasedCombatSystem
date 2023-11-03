using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIDamage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textDamage = null;

    private const int _C_N_FONTSIZE = 35;

    private Vector3 _vecDestPos = new Vector3(0, 200, 0);

    private void OnEnable()
    {
        Sequence seq = DOTween.Sequence();

        transform.localScale = Vector3.one;

        seq.Append(transform.DOScale(2.7f, 0.17f).SetEase(Ease.InOutBounce));
        seq.Append(transform.DOLocalMove(_vecDestPos, 1.0f).SetEase(Ease.OutBack));               
        seq.AppendInterval(0.2f);        
        seq.Append(transform.DOScale(0f, 0.1f).OnComplete(() => Destroy(gameObject)));
    }

    public void SetDamage(int nDamage)
    {
        textDamage.SetText(nDamage.ToString());
        textDamage.fontSize = _C_N_FONTSIZE;
        textDamage.color = Color.yellow;
    }
}
