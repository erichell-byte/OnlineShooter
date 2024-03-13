using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private const string Grounded = "Grounded";
    private const string Speed = "Speed";
    private const string Crouch = "Crouch";

    [SerializeField] private Animator _footAnimator;
    [SerializeField] private Animator _commonAnimator;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private Character character;

    private void Update()
    {
        Vector3 localVelocity = character.transform.InverseTransformVector(character.velocity);
        float speed = localVelocity.magnitude / character.speed;
        float sign = Mathf.Sign(localVelocity.z);
        
        _footAnimator.SetFloat(Speed, speed * sign);
        _footAnimator.SetBool(Grounded, _checkFly.IsFly == false);
        _commonAnimator.SetBool(Crouch, character.isCrouch);
    }
}
