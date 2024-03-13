using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private Transform _head;
    public Vector3 targetPosition { get; private set; } = Vector3.zero;
    
    private Quaternion targetRotationX;
    private Quaternion targetRotationY;
    private float _velocityMagnitude = 0;
    private float _rotationXMagnitude = 0;
    private float _rotationYMagnitude = 0;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (_velocityMagnitude > .1f)
        {
            float maxDistance = _velocityMagnitude * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, maxDistance);
        }
        else
        {
            transform.position = targetPosition;
        }

        if (_rotationXMagnitude > .01f)
        {
            float maxRotation = _rotationXMagnitude * Time.deltaTime;
            _head.localRotation = Quaternion.RotateTowards(_head.localRotation, targetRotationX, maxRotation);
        }

        if (_rotationYMagnitude > .01f)
        {
            float maxRotation = _rotationYMagnitude * Time.deltaTime;
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotationY, maxRotation);
        }
    }

    public void SetSpeed(float value) => speed = value;

    public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval)
    {
        targetPosition = position + (velocity * averageInterval);
        _velocityMagnitude = velocity.magnitude;
        
        this.velocity = velocity;
    }

    public void SetRotateX(float value)
    {
        // _head.localEulerAngles = new Vector3(value, 0, 0);
        var targetEuler = new Vector3(value, 0, 0);
        targetRotationX = Quaternion.Euler(targetEuler);
        _rotationXMagnitude = Quaternion.Angle(_head.rotation ,targetRotationX);
    }

    public void SetRotateY(float value)
    {
        // transform.localEulerAngles = new Vector3(0, value, 0);
        var targetEuler = new Vector3(0, value, 0);
        targetRotationY = Quaternion.Euler(targetEuler);
        _rotationYMagnitude = Quaternion.Angle(transform.localRotation, targetRotationY);
    }
}
