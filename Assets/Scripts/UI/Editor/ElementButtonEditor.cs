using UnityEditor.UI;
using UnityEditor;

namespace Game.UI
{
    [CustomEditor(typeof(ElementButton), true)]
    public class ElementButtonEditor : ButtonEditor
    {
        SerializedProperty data;

        protected override void OnEnable()
        {
            base.OnEnable();

            data = serializedObject.FindProperty("data");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(data);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
