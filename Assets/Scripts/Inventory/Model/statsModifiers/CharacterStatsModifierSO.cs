using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStatsModifierSO : ScriptableObject
{
    public abstract void AffecrtCharacter(GameObject character, float val);
}
