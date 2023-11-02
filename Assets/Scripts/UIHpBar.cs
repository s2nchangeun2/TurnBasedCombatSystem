using UnityEngine;
using UnityEngine.UI;

public class UIHpBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider = null;

    public void UpdateHpBar(float nHp)
    {
        slider.value = nHp;
    }
}
