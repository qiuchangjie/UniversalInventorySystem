﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PatternRecipeEditorWindow : ExtendEditorWindow
{
    bool patternRecipe = true;
    bool patternConfg = false;
    bool patternCreation = false;

    Vector2 scrollPos;
    Vector2 creationPos;
    Vector2 slectablesItem;

    Vector2 gridSize;

    int selectedIndex;

    public static void Open(PatternRecipe dataObject)
    {
        PatternRecipeEditorWindow window = GetWindow<PatternRecipeEditorWindow>("Item Data Editor");
        window.serializedObject = new SerializedObject(dataObject);
    }

    private void OnGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        currentProperty = serializedObject.FindProperty("");
        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.MinWidth(100), GUILayout.ExpandHeight(true));

        if(GUILayout.Button("Pattern Recipe"))
        {
            patternRecipe = true;
            patternConfg = false;
            patternCreation = false;
        }
        if (GUILayout.Button("Pattern Config"))
        {
            patternRecipe = false;
            patternConfg = true;
            patternCreation = false;
        }
        if (GUILayout.Button("Pattern Creation"))
        {
            patternRecipe = false;
            patternConfg = false;
            patternCreation = true;
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        if (patternRecipe)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("key"));
        }

        if (patternConfg)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfFactors"));
            serializedObject.FindProperty("factors").arraySize = serializedObject.FindProperty("numberOfFactors").intValue;

            serializedObject.FindProperty("factors").isExpanded = EditorGUILayout.Foldout(serializedObject.FindProperty("factors").isExpanded, new GUIContent("Factors"));

            if (serializedObject.FindProperty("factors").isExpanded)
            {
                EditorGUI.indentLevel++;
                for(int i = 0;i < serializedObject.FindProperty("factors").arraySize; i++)
                {
                    EditorGUILayout.ObjectField(serializedObject.FindProperty("factors").GetArrayElementAtIndex(i));
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("numberOfProducts"));

            serializedObject.FindProperty("products").arraySize = serializedObject.FindProperty("numberOfProducts").intValue;

            serializedObject.FindProperty("products").isExpanded = EditorGUILayout.Foldout(serializedObject.FindProperty("products").isExpanded, new GUIContent("Products"));

            if (serializedObject.FindProperty("products").isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < serializedObject.FindProperty("products").arraySize; i++)
                {
                    EditorGUILayout.ObjectField(serializedObject.FindProperty("products").GetArrayElementAtIndex(i));
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndScrollView();
        }

        if (patternCreation)
        {
            EditorGUILayout.BeginHorizontal();

            #region SelectItem

            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(225));
            slectablesItem = EditorGUILayout.BeginScrollView(slectablesItem);

            List<Item> items = new List<Item>();
            for(int i = 0; i < serializedObject.FindProperty("factors").arraySize; i++)
            {
                items.Add(serializedObject.FindProperty("factors").GetArrayElementAtIndex(i).objectReferenceValue as Item);
            }
            
            List<GUIContent> possibleItems = new List<GUIContent>();
            foreach (Item item in items)
            {
                if (item == null)
                {
                    Debug.LogError("Null item provided");
                    continue;
                }
                Texture2D texture = item.sprite.texture;
                possibleItems.Add(new GUIContent(item.name, texture));
            }

            selectedIndex = GUILayout.SelectionGrid(selectedIndex, possibleItems.ToArray(), 1, GUILayout.MaxHeight(75));

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            #endregion
            //-----GRID-----//
            #region Grid

            EditorGUILayout.BeginVertical();

            creationPos = EditorGUILayout.BeginScrollView(creationPos);
            serializedObject.FindProperty("gridSize").vector2IntValue = EditorGUILayout.Vector2IntField("Grid size", serializedObject.FindProperty("gridSize").vector2IntValue);

            var gridRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            
            gridSize = serializedObject.FindProperty("gridSize").vector2IntValue;
            for(int i = 0;i < gridSize.y; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for(int j = 0;j < gridSize.x; j++)
                {
                    var xOffsetValue = j * (gridRect.width / gridSize.x);
                    var yOffsetValue = i * (gridRect.width / gridSize.x);
                    var sideValue = (gridRect.width / gridSize.x) - 10;

                    if((sideValue + 10) * gridSize.y > gridRect.height)
                    {
                        Debug.Log(sideValue * gridSize.y + " " + gridRect.height);
                        sideValue = (gridRect.height / gridSize.y) - 10;
                        xOffsetValue = j * (gridRect.height / gridSize.y);
                        yOffsetValue = i * (gridRect.height / gridSize.y);
                    }

                    Rect btnRect = new Rect(gridRect.x + xOffsetValue, gridRect.y + yOffsetValue, sideValue, sideValue);
                    GUI.Button(btnRect, new GUIContent("Place Item"));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            #endregion

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
