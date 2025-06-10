using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCombiner
{
    private readonly Dictionary<string, Transform> _rootBoneDictionary = new Dictionary<string, Transform>();
    private readonly Transform[] _boneTransforms = new Transform[67];

    private readonly Transform _transform;

    public BoneCombiner(GameObject rootObj)
    {
        _transform = rootObj.transform;
        TraverseHierarchy(_transform);
    }

    public Transform AddLimb(GameObject bonedObj, List<string> boneNames)
    {
        var limb = ProcessBonedObject(bonedObj.GetComponentInChildren<SkinnedMeshRenderer>(), boneNames);
        limb.SetParent(_transform);
        return limb;
    }

    private Transform ProcessBonedObject(SkinnedMeshRenderer renderer, IReadOnlyList<string> boneNames)
    {
        var bonedObject = new GameObject().transform;

        var meshRenderer = bonedObject.gameObject.AddComponent<SkinnedMeshRenderer>();

        //var bones = renderer.bones;
        for (var i = 0; i < boneNames.Count; i++)
        {
            if (_rootBoneDictionary.TryGetValue(boneNames[i], out var bone))
            {
                _boneTransforms[i] = bone;
            }
            else
            {
                Debug.LogWarning($"Bone '{boneNames[i]}' not found in root bone dictionary.");
            }
        }

        meshRenderer.bones = _boneTransforms;
        meshRenderer.sharedMesh = renderer.sharedMesh;
        meshRenderer.materials = renderer.sharedMaterials;

        return bonedObject;

    }
    
    
    private void TraverseHierarchy(IEnumerable transform)
    {
        foreach (Transform child in transform)
        {
            if (!_rootBoneDictionary.ContainsKey(child.name))
            {
                _rootBoneDictionary.Add(child.name, child);
            }

            TraverseHierarchy(child);
        }
    }

    
}