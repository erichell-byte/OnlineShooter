using System.Collections;
using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    [SerializeField] private EnemyCharacter enemy;
    [SerializeField] private GameObject damageSprite;
    [SerializeField] private int damageMultiplier;
    
    private Coroutine activeCoroutine;
    
    public void CauseDamageByBullet(int damage, Vector3 damagePoint, Vector3 normal)
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        enemy.ApplyDamage(damage * damageMultiplier);
        damageSprite.gameObject.SetActive(true);
        damageSprite.transform.position = damagePoint;
        damageSprite.transform.localRotation = Quaternion.LookRotation(normal,  damageSprite.transform.up);
        activeCoroutine = StartCoroutine(DelayTurnOffBlood());
    }
    
    private IEnumerator DelayTurnOffBlood()
    {
        yield return new WaitForSeconds(1f);
        damageSprite.gameObject.SetActive(false);
    }

}
