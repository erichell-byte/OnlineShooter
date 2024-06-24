using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] private GameObject _healthBarGO;
    [SerializeField] private Health _health;
    [SerializeField] private Image _healhtBar;

    private float _maxHealth;
    
    private void Start()
    {
        _healthBarGO.SetActive(false);
        _health.UpdateHealth += UpdateView;
        _maxHealth = _health.max;
    }
    
    private void UpdateView(float newValue)
    {
        _healthBarGO.SetActive(true);
        _healhtBar.fillAmount = newValue / _maxHealth;
    }

    private void OnDestroy()
    {
        _health.UpdateHealth -= UpdateView;
    }

    
}
