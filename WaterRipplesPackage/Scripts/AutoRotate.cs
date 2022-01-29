using UnityEngine;

namespace WaterRipples
{
    public class AutoRotate : MonoBehaviour
    {
        [SerializeField]
        private Vector3 speed = Vector3.zero;

        private void Update()
        {
            transform.Rotate(speed * Time.deltaTime, Space.Self);
        }
    }
}
