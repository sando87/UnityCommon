using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class PrefabSelectorAttribute : PropertyAttribute
{
    public readonly string path;
    public PrefabSelectorAttribute(string _path)
    {
        this.path = _path;
    }
}