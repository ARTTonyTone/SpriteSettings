using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpriteSettings))]
[CanEditMultipleObjects]
public class SpriteSettingsEditor : Editor
{
    SerializedProperty _texture, _sprite, _assetType, _shaderMode, _color, _meshRenderer, _meshFilter, _material, _meshScale;

    private void OnEnable()
    {
        _texture = serializedObject.FindProperty("_texture");
        _sprite = serializedObject.FindProperty("_sprite");
        _assetType = serializedObject.FindProperty("_assetType");
        _shaderMode = serializedObject.FindProperty("_shaderMode");
        _color = serializedObject.FindProperty("_color");
        _meshRenderer = serializedObject.FindProperty("_meshRenderer");
        _meshFilter = serializedObject.FindProperty("_meshFilter");
        _material = serializedObject.FindProperty("_material");
        _meshScale = serializedObject.FindProperty("_meshScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SpriteSettings myTarget = (SpriteSettings)target;

        EditorGUILayout.LabelField("Source & Compatibility", EditorStyles.boldLabel);
        
        GUIContent shaderModeLabel = new("Shader Mode", "URP Lite uses the _BaseMap reference, and Legacy shaders use the _MainTex reference.");
        EditorGUILayout.PropertyField(_shaderMode, shaderModeLabel);
        
        EditorGUILayout.PropertyField(_assetType);
        
        if (_assetType.enumValueIndex == (int)SpriteSettings.AssetType.Sprite)
            EditorGUILayout.PropertyField(_sprite);
        else
            EditorGUILayout.PropertyField(_texture);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_color);
        EditorGUILayout.PropertyField(_material);
        EditorGUILayout.Slider(_meshScale, 0.1f, 10f, "Mesh Scale");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_meshRenderer);
        EditorGUILayout.PropertyField(_meshFilter);

        if (serializedObject.ApplyModifiedProperties())
        {
            myTarget.Refresh();
        }
    }
}