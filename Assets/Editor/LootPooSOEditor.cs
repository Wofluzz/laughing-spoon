using Inventory.Model;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(LootPoolSO))]
public class LootPoolSOEditor : Editor
{
    #region SerializedProperties
    SerializedProperty lootPool;
    #endregion

    private void OnEnable()
    {
        // Find the lootPool property
        lootPool = serializedObject.FindProperty("lootPool");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Get the target LootPoolSO object
        LootPoolSO _lootPoolSO = (LootPoolSO)target;

        // Display the list of loot pool items
        EditorGUILayout.LabelField("Loot Pool Items", EditorStyles.boldLabel);

        // Ensure the list is not null and contains at least one item
        if (_lootPoolSO.lootPool == null)
        {
            _lootPoolSO.lootPool = new List<ItemsInLootPool>();
        }

        // Loop through the lootPool list and display each item's properties
        for (int i = 0; i < lootPool.arraySize; i++)
        {
            SerializedProperty lootItem = lootPool.GetArrayElementAtIndex(i);
            SerializedProperty item = lootItem.FindPropertyRelative("item");
            SerializedProperty maxQuantity = lootItem.FindPropertyRelative("MaxQuantity");
            SerializedProperty minQuantity = lootItem.FindPropertyRelative("MinQuantity");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(item);

            // Get the current values of min and max quantity
            float min = minQuantity.intValue;
            float max = maxQuantity.intValue;

            // Display the current min and max values
            EditorGUILayout.LabelField($"Min: {min}, Max: {max}");

            // Use MinMaxSlider to set the min and max values
            EditorGUILayout.MinMaxSlider(ref min, ref max, 0, 99);

            // Assign the modified values back to the SerializedProperty
            minQuantity.intValue = (int)min;
            maxQuantity.intValue = (int)max;

            if (GUILayout.Button("Remove Item"))
            {
                lootPool.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndVertical();
        }

        // Button to add a new item to the loot pool
        if (GUILayout.Button("Add New Item"))
        {
            lootPool.arraySize++;
        }

        // Apply changes to the serializedObject
        serializedObject.ApplyModifiedProperties();
    }
}
