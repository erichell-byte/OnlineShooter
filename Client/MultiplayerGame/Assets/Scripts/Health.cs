using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private HealthUI _ui;
    private int _max;
    private int _current;

    public void SetMax(int max)
    {
        _max = max;
        UpdateHealth();
    }

    public void SetCurrent(int current)
    {
        _current = current;
        UpdateHealth();
    }

    public void ApplyDamage(int damage)
    {
        _current -= damage;
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        _ui.UpdateUI(_max, _current);
    }
}
