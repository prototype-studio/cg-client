using UnityEngine;

namespace UI
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationSpeed;
        
        private void Update()
        {
            transform.rotation *= Quaternion.Euler(rotationSpeed * Time.deltaTime);
        }
    }
}