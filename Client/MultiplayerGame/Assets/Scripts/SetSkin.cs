using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
