using UnityEngine;
using System.Collections.Generic;

namespace WaterRipples
{
    internal class RippleDetector : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particles;

        private List<ParticleCollisionEvent> collisionEvents;

        private void Start()
        {
            collisionEvents = new List<ParticleCollisionEvent>();
            if (!RippleManager.Exists)
            {
                Destroy(this);
            }
        }

        private void OnParticleCollision(GameObject _target)
        {
            int numCollisionEvents = particles.GetCollisionEvents(_target, collisionEvents);
            for (int i = 0; i < numCollisionEvents; i++)
            {
                var e = collisionEvents[i];
                RippleManager.Instance.SetImpact(e.intersection);
            }
        }
    }
}