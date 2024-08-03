using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Stats Modifiers/Health")]
public class CharacterStatHealth : CharacterStatsModifierSO
{
    public override void AffecrtCharacter(GameObject character, float val)
    {
        Health health = character.GetComponent<Health>();
        if (health != null)
            health.AddHealth(val);
    }
}
