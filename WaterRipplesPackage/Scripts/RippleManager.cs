using System.Collections.Generic;
using UnityEngine;

namespace WaterRipples
{
    public class RippleManager : MonoSingleton<RippleManager>
    {
        private const int MAX_HIT_COUNT = 100;

        private static readonly int RingDistortionHitArrayCount = Shader.PropertyToID("_RingDistortionHitArrayCount");
        private static readonly int RingDistortionHitArray = Shader.PropertyToID("_RingDistortionHitArray");
        private static readonly int RingDistortionHitTimeArray = Shader.PropertyToID("_RingDistortionHitTimeArray");

        [SerializeField]
        private float hitLifetime = 1f;

        public struct HitData
        {
            public Vector3 Pos;
            public float Time;
        }

        private Queue<HitData> hits = new Queue<HitData>();

        protected override void Start()
        {
            base.Start();
            InitHits();
        }

        protected override void Update()
        {
            base.Update();
            UpdateHits();
        }

        public void SetImpact(Vector3 _position)
        {
            if (hits.Count >= MAX_HIT_COUNT - 1)
            {
                Debug.LogError($"Could not set impact: max hit count already reached ({MAX_HIT_COUNT})");
                return;
            }

            hits.Enqueue(new HitData
            {
                Pos = _position,
                Time = Time.time
            });
        }

        private void InitHits()
        {
            Shader.SetGlobalInt(RingDistortionHitArrayCount, 0);
            Shader.SetGlobalFloatArray(RingDistortionHitTimeArray, new float[MAX_HIT_COUNT]);
            Shader.SetGlobalVectorArray(RingDistortionHitArray, new Vector4[MAX_HIT_COUNT]);
        }

        private void UpdateHits()
        {
            FreeHits();
            int hitCount = hits.Count;
            Shader.SetGlobalInt(RingDistortionHitArrayCount, hitCount);
            if (hits.Count == 0)
            {
                return;
            }
            if (hitCount < 0 || hitCount >= MAX_HIT_COUNT)
            {
                Debug.LogError($"Unhandled impact count: {hitCount}");
                return;
            }

            List<float> hitTime = new List<float>();
            List<Vector4> hitPos = new List<Vector4>();
            foreach (var hitData in hits.ToArray())
            {
                hitTime.Add(hitData.Time);
                hitPos.Add(hitData.Pos);
            }
            Shader.SetGlobalFloatArray(RingDistortionHitTimeArray, hitTime);
            Shader.SetGlobalVectorArray(RingDistortionHitArray, hitPos);
        }

        private void FreeHits()
        {
            while (hits.Count > 0 &&
                Time.time - hits.Peek().Time > hitLifetime)
            {
                hits.Dequeue();
            }
        }
    }
}