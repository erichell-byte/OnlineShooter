using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Image _healhtBar;
    
    private void Awake()
    {
        _health.OnChangeInPercent += UpdateView;
    }

    private void UpdateView(float progress)
    {
        _healhtBar.fillAmount = progress;
    }
}
