using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SnakeAbstractPart : MonoBehaviour
{
    [SerializeField] protected List<MeshRenderer> _renderer;


    public void SetMaterial(Material material)
    {
        foreach (var renderer in _renderer)
        {
            renderer.material = material;
        }
    }
}
