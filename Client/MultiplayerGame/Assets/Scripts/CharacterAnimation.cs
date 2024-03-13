using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterAnimation : MonoBehaviour
{
    private const string Grounded = "Grounded";
    private const string Speed = "Speed";

    [SerializeField] private Animator _animator;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private Character character;

    private void Update()
    {
        Vector3 localVelocity = character.transform.InverseTransformVector(character.velocity);
        float speed = localVelocity.magnitude / character.speed;
        float sign = Mathf.Sign(localVelocity.z);
        
        _animator.SetFloat(Speed, speed * sign);
        _animator.SetBool(Grounded, _checkFly.IsFly == false);
    }
}
