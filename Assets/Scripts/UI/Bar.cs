using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    private Slider barSlider;
    [SerializeField]
    private Gradient gradient;

    private float desiredFill;
    private Image fillImage;
    private void Awake()
    {
        barSlider = GetComponent<Slider>();
        fillImage = barSlider.fillRect.GetComponent<Image>();
    }
    void Start()
    {
        barSlider = GetComponent<Slider>();
    }

    public void SetBar(int number)
    {
        desiredFill = number;
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(desiredFill - barSlider.value) > 1)
        {
            if(desiredFill > barSlider.value)
                barSlider.value += Time.deltaTime * 40;
            else barSlider.value -= Time.deltaTime * 40;
            fillImage.color = gradient.Evaluate(barSlider.normalizedValue);
        }
    }
}
