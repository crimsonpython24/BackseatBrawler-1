using UnityEngine;
using UnityEngine.UI;

public class RoundTimeDisplay : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateDisplay(float a)
    {
        slider.SetValueWithoutNotify(a);
    }
}
