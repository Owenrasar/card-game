using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    private Card card;
    private SerializedProperty actionsProp;
    private ReorderableList actionsList;

    private void OnEnable()
    {
        card = (Card)target;
        actionsProp = serializedObject.FindProperty("actions");

        actionsList = new ReorderableList(serializedObject, actionsProp, true, true, false, true);

        actionsList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Actions");
        };

        actionsList.drawElementCallback = (rect, index, active, focused) =>
        {
            SerializedProperty element = actionsProp.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        };

        actionsList.onRemoveCallback = list =>
        {
            // Get the object before removal
            var element = actionsProp.GetArrayElementAtIndex(list.index);
            var action = element.objectReferenceValue as ScriptableObject;

            // Remove reference from list
            actionsProp.DeleteArrayElementAtIndex(list.index);

            // Destroy the sub-asset itself
            if (action != null)
            {
                DestroyImmediate(action, true);
                AssetDatabase.SaveAssets();
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspectorExceptActions();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add New Action", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Attack")) AddAction<Attack>();
        if (GUILayout.Button("Add Dodge")) AddAction<Dodge>();
        if (GUILayout.Button("Add Block")) AddAction<Block>();
        if (GUILayout.Button("Add Blank")) AddAction<Blank>();

        EditorGUILayout.Space();
        actionsList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawDefaultInspectorExceptActions()
    {
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            if (iterator.name != actionsProp.name)
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
            enterChildren = false;
        }
    }

    private void AddAction<T>() where T : Action
    {
        T newAction = ScriptableObject.CreateInstance<T>();
        newAction.name = typeof(T).Name;

        AssetDatabase.AddObjectToAsset(newAction, card);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(card));

        card.actions.Add(newAction);

        EditorUtility.SetDirty(card);
        AssetDatabase.SaveAssets();
    }
}