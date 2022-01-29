using UnityEngine;

namespace WaterRipples
{
    internal class PlaneBuilder : MonoBehaviour
    {
        [SerializeField]
        private Material material;

        [SerializeField]
        private Vector2Int vertices;

        private void Awake()
        {
            var meshFilter = gameObject.CreatePlane(vertices);

            var meshRenderer = meshFilter.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = meshFilter.gameObject.AddComponent<MeshRenderer>();
            }
            meshRenderer.material = material;
        }
    }
}