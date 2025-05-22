#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Inventory2D.Model;

[CustomEditor(typeof(ItemSO_2D))]
public class ItemSO_2DEditor : Editor
{
    private List<PowerUp_SO> allPowerUps;
    private string[] displayNames;

    private void OnEnable()
    {
        allPowerUps = AssetDatabase.FindAssets("t:PowerUp_SO")
            .Select(guid => AssetDatabase.LoadAssetAtPath<PowerUp_SO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(p => p != null)
            .ToList();

        displayNames = allPowerUps.Select(p => p.name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var item = (ItemSO_2D)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("<ItemImage>k__BackingField"));

        item.ItemAnimator = (RuntimeAnimatorController)EditorGUILayout.ObjectField("Item Animator", item.ItemAnimator, typeof(RuntimeAnimatorController), false);

        // Section PowerUps
        EditorGUILayout.LabelField("PowerUps", EditorStyles.boldLabel);

        if (allPowerUps.Count == 0)
        {
            EditorGUILayout.HelpBox("Aucun PowerUp_SO trouvé dans le projet.", MessageType.Warning);
        }
        else
        {
            for (int i = 0; i < item.PowerUp.Count; i++)
            {
                int selectedIndex = allPowerUps.IndexOf(item.PowerUp[i]);
                if (selectedIndex < 0) selectedIndex = 0;

                int newIndex = EditorGUILayout.Popup("PowerUp " + (i + 1), selectedIndex, displayNames);
                item.PowerUp[i] = allPowerUps[newIndex];
            }

            if (GUILayout.Button("Ajouter un PowerUp"))
            {
                if (allPowerUps.Count > 0)
                    item.PowerUp.Add(allPowerUps[0]);
            }

            if (item.PowerUp.Count > 0 && GUILayout.Button("Supprimer le dernier"))
            {
                item.PowerUp.RemoveAt(item.PowerUp.Count - 1);
            }
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(item);
    }
}
#endif