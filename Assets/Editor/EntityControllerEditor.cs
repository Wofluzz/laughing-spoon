using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntitySO))]
public class EntityControllerEditor : Editor
{
    #region SerializedProperties
    SerializedProperty Name;

    SerializedProperty EntitySprite;
    SerializedProperty EntityAnimationTree;
    SerializedProperty EntityType;

    SerializedProperty IsInvincible;
    SerializedProperty MaxHealth;
    SerializedProperty DamageToPlayer;

    SerializedProperty IsLoots;
    SerializedProperty Loots;
    SerializedProperty LootPool;

    SerializedProperty IsMove;
    SerializedProperty MoveSpeed;
    SerializedProperty rotateSpeed;
    SerializedProperty CanFollowPlayer;

    SerializedProperty DetectionRadius;

    SerializedProperty Sounds;

    bool entityMonsterGroup, entityBossGroup, entityMovementsGroup, entityAIGroup = false;
    #endregion

    private void OnEnable()
    {
        Name = serializedObject.FindProperty("Name");

        EntitySprite = serializedObject.FindProperty("EntitySprite");
        EntityAnimationTree = serializedObject.FindProperty("EntityAnimationTree");
        EntityType = serializedObject.FindProperty("EntityType");

        IsInvincible = serializedObject.FindProperty("IsInvincible");
        MaxHealth = serializedObject.FindProperty("MaxHealth");
        DamageToPlayer = serializedObject.FindProperty("DamageToPlayer");

        IsLoots = serializedObject.FindProperty("IsLoots");
        Loots = serializedObject.FindProperty("Loots");
        LootPool = serializedObject.FindProperty("LootPool");

        IsMove = serializedObject.FindProperty("IsMove");
        MoveSpeed = serializedObject.FindProperty("MoveSpeed");
        rotateSpeed = serializedObject.FindProperty("rotateSpeed");
        CanFollowPlayer = serializedObject.FindProperty("CanFollowPlayer");

        DetectionRadius = serializedObject.FindProperty("DetectionRadius");

        Sounds = serializedObject.FindProperty("Sounds");
    }

    public override void OnInspectorGUI()
    {
        EntitySO _entitySO = (EntitySO)target;

        serializedObject.Update();

        EditorGUILayout.PropertyField(Name);
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(EntitySprite);
        EditorGUILayout.PropertyField(EntityAnimationTree);
        EditorGUILayout.LabelField("Entity Type");
        EntityType.enumValueIndex = GUILayout.Toolbar(EntityType.enumValueIndex, new string[] { "Monster", "Animals", "Boss", "Human" });

        switch (_entitySO.EntityType)
        {
            case global::EntityType.Monster:
                ShowMonsterGroup(_entitySO);
                ShowLootGroup(_entitySO);
                ShowMovementGroup(_entitySO);
                break;
            case global::EntityType.Animals:
                ShowHealthGroup(_entitySO);
                ShowLootGroup(_entitySO);
                ShowMovementGroup(_entitySO);
                break;
            case global::EntityType.Boss:
                ShowBossGroup(_entitySO);
                ShowLootGroup(_entitySO);
                ShowMovementGroup(_entitySO);
                break;
            case global::EntityType.Humans:
                ShowMovementGroup(_entitySO);
                break;
        }
        ShowSoundsGroup();
        serializedObject.ApplyModifiedProperties();
    }

    private void ShowLootGroup(EntitySO _entitySO)
    {
        EditorGUILayout.PropertyField(IsLoots);
        if (_entitySO.IsLoots)
        {
            if (Loots.isArray)
            {
                for (int i = 0; i < Loots.arraySize; i++)
                {
                    SerializedProperty lootItem = Loots.GetArrayElementAtIndex(i);
                    SerializedProperty item = lootItem.FindPropertyRelative("item");
                    SerializedProperty maxQuantity = lootItem.FindPropertyRelative("maxQuantity");
                    SerializedProperty minQuantity = lootItem.FindPropertyRelative("minQuantity");

                    if (item != null && maxQuantity != null && minQuantity != null)
                    {
                        float min = minQuantity.intValue;
                        float max = maxQuantity.intValue;

                        EditorGUILayout.BeginVertical("box");
                        EditorGUILayout.PropertyField(item);

                        // Affichage du slider MinMax
                        EditorGUILayout.MinMaxSlider(ref min, ref max, 0, 99);

                        // Mise à jour des valeurs min et max dans les SerializedProperty
                        minQuantity.intValue = Mathf.RoundToInt(min);
                        maxQuantity.intValue = Mathf.RoundToInt(max);

                        // Affichage des valeurs actuelles
                        EditorGUILayout.LabelField($"Min: {minQuantity.intValue}, Max: {maxQuantity.intValue}");

                        // Bouton pour supprimer un élément
                        if (GUILayout.Button("Remove Item"))
                        {
                            Loots.DeleteArrayElementAtIndex(i);
                            serializedObject.ApplyModifiedProperties();
                            return; // Sortez pour éviter les problèmes d'index après la suppression
                        }
                        EditorGUILayout.EndVertical();
                    }
                }

                // Bouton pour ajouter un nouvel élément de loot
                if (GUILayout.Button("Add New Loot Item"))
                {
                    Loots.arraySize++;
                }
            }
        }
    }




    private void ShowBossGroup(EntitySO _entitySO)
    {
        entityBossGroup = EditorGUILayout.BeginFoldoutHeaderGroup(entityBossGroup, "Entity Boss");
        if (entityBossGroup)
        {
            EditorGUILayout.PropertyField(IsInvincible);
            if (!_entitySO.IsInvincible)
            {
                EditorGUILayout.PropertyField(MaxHealth);
            }
            EditorGUILayout.PropertyField(DamageToPlayer);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void ShowSoundsGroup()
    {
        EditorGUILayout.PropertyField(Sounds);
    }

    private void ShowMovementGroup(EntitySO _entitySO)
    {
        entityMovementsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(entityMovementsGroup, "Movements");
        if (entityMovementsGroup)
        {
            EditorGUILayout.PropertyField(IsMove);
            if (_entitySO.IsMove)
            {
                EditorGUILayout.PropertyField(MoveSpeed);
                EditorGUILayout.PropertyField(rotateSpeed);
                EditorGUILayout.PropertyField(CanFollowPlayer);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (_entitySO.CanFollowPlayer)
            ShowIAGroup(_entitySO);

    }

    private void ShowIAGroup(EntitySO _entitySO)
    {
        entityAIGroup = EditorGUILayout.BeginFoldoutHeaderGroup(entityAIGroup, "AI");
        if (entityAIGroup)
        {
            EditorGUILayout.PropertyField(DetectionRadius);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void ShowHealthGroup(EntitySO _entitySO)
    {
        entityMonsterGroup = EditorGUILayout.BeginFoldoutHeaderGroup(entityMonsterGroup, "Health");
        if (entityMonsterGroup)
        {
            EditorGUILayout.PropertyField(IsInvincible);
            if (!_entitySO.IsInvincible)
            {
                EditorGUILayout.PropertyField(MaxHealth);
            }
            EditorGUILayout.PropertyField(DamageToPlayer);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void ShowMonsterGroup(EntitySO _entitySO)
    {
        entityMonsterGroup = EditorGUILayout.BeginFoldoutHeaderGroup(entityMonsterGroup, "Entity Monster");
        if (entityMonsterGroup)
        {
            EditorGUILayout.PropertyField(IsInvincible);
            if (!_entitySO.IsInvincible)
            {
                EditorGUILayout.PropertyField(MaxHealth);
            }
            EditorGUILayout.PropertyField(DamageToPlayer);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}