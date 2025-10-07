using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    public void UpdateHealthBar(float value)
    {
        if (slider == null) slider = GetComponent<Slider>();
        slider.value = value;
    }
}
