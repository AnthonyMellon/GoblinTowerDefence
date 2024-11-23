using System.Collections.Generic;
using UnityEngine;

public class StructureContainer : MonoBehaviour
{
    private List<StructureBase> _structures = new List<StructureBase>();

    public void AddStructure(StructureBase structure)
    {
        _structures.Add(structure);
        structure.transform.SetParent(transform, false);
    }

    public void DestroyAllStructures()
    {
        for(int i = 0; i < _structures.Count; i++)
        {
            Destroy(_structures[i].gameObject);
        }

        _structures.Clear();
    }
}
