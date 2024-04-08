using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skins : MonoBehaviour
{
    [SerializeField] private Material[] _materials;

    public int Length => _materials.Length;
    
    public Material GetMaterial(int index)
    {
        if (index < 0 || index >= _materials.Length) return _materials[0];

        return _materials[index];
    }
    
}
