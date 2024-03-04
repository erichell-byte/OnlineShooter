using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
   [SerializeField] private float speed;
   
   private float hInput;
   private float vInput;

   private void Update()
   {
      Move();
   }

   public void SetInput(float h, float v)
   {
      hInput = h;
      vInput = v;
   }
   
   private void Move()
   {
      Vector3 direction = new Vector3(hInput, 0, vInput).normalized;

      transform.position += direction * (speed * Time.deltaTime);
   }

   public void GetMoveInfo(out Vector3 position)
   {
      position = transform.position;
   }
}
