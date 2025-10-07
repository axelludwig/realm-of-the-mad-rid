using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    private Entity entity;

    void Start()
    {
        entity = GetComponentInParent<Entity>();
        slider = GetComponent<Slider>();
    }

    public void UpdateHealthBar(float value)
    {
        slider.value = value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        
    }
}
