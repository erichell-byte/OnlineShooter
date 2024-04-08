using UnityEngine;

namespace Shooter
{
    public class SetSkin : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] _renderers;

        public void Set(Material material)
        {
            foreach (var renderer in _renderers)
            {
                renderer.material = material;
            }
        }

    }
}