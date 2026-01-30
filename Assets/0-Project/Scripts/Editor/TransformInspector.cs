using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [CustomEditor(typeof(Transform))]
    public class TransformInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Transform t = (Transform) target;
            GUI.color = Color.white;
            EditorGUIUtility.LookLikeControls();
            
            EditorGUI.indentLevel = 0;
            Vector3 position = EditorGUILayout.Vector3Field("Position", t.localPosition);
            Vector3 eulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
            Vector3 scale = EditorGUILayout.Vector3Field("Scale", t.localScale);
            
            EditorGUIUtility.LookLikeInspector();
            
            if (GUI.changed)
            {
                Undo.RegisterUndo(t, "Transform Change");
                t.localPosition = FixIfNaN(position);
                t.localEulerAngles = FixIfNaN(eulerAngles);
                t.localScale = FixIfNaN(scale);
            }
        
            GUILayout.BeginHorizontal();
        
            GUI.color = new Color(0.5f, 0.8f, 1f);
            if (GUILayout.Button("Reset Position"))
            {
                Undo.RegisterUndo(t, "Reset Position " + t.name);
                t.transform.localPosition = Vector3.zero;
            }
        
            if (GUILayout.Button("Reset Rotation"))
            {
                Undo.RegisterUndo(t, "Reset Rotation " + t.name);
                t.transform.localRotation = Quaternion.identity;
            }
        
            if (GUILayout.Button("Reset Scale"))
            {
                Undo.RegisterUndo(t, "Reset Scale " + t.name);
                t.transform.localScale = Vector3.one;
            }
            
            GUILayout.EndHorizontal();
        
            GUILayout.BeginHorizontal();
        
            GUI.color = new Color(0.74f, 1f, 0.4f);
            if (GUILayout.Button("Enable Kinematics"))
            {
                Rigidbody[] rigidbodies = t.transform.GetComponentsInChildren<Rigidbody>();
                for (int i = 0; i < rigidbodies.Length; i++) rigidbodies[i].isKinematic = true;
            }
        
            GUI.color = new Color(1f, 0.67f, 0.4f);
            if (GUILayout.Button("Disable Kinematics"))
            {
                Rigidbody[] rigidbodies = t.transform.GetComponentsInChildren<Rigidbody>();
                for (int i = 0; i < rigidbodies.Length; i++) rigidbodies[i].isKinematic = false;
            }
        
            GUILayout.EndHorizontal();
        
            GUILayout.BeginHorizontal();
        
            GUI.color = new Color(0.74f, 1f, 0.4f);
            if (GUILayout.Button("Enable Colliders"))
            {
                Collider[] colliders = t.transform.GetComponentsInChildren<Collider>();
                for (int i = 0; i < colliders.Length; i++) colliders[i].enabled = true;
            }
        
            GUI.color = new Color(1f, 0.67f, 0.4f);
            if (GUILayout.Button("Disable Colliders"))
            {
                Collider[] colliders = t.transform.GetComponentsInChildren<Collider>();
                for (int i = 0; i < colliders.Length; i++) colliders[i].enabled = false;
            }
        
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
        }
        
        private Vector3 FixIfNaN(Vector3 v)
        {
            if (float.IsNaN(v.x)) v.x = 0;
            if (float.IsNaN(v.y)) v.y = 0;
            if (float.IsNaN(v.z)) v.z = 0;
            return v;
        }
    }
}