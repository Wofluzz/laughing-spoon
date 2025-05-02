using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Items/Item Parameter")]
public class ItemParameterSO : ScriptableObject
{
    [field: SerializeField]
    public string ParameterName { get; private set; }
}
