#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using MA_Toolbox.Parallaxing;

namespace MA_Toolbox.ParallaxingEditor
{
    [CustomEditor(typeof(ParallaxCamera))]
    class ParallaxCameraEditor : Editor
    {
        private ParallaxCamera camera;

        void Awake()
        {
            camera = (ParallaxCamera)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            if (GUILayout.Button("Save Position"))
            {
                camera.EditorSavePosition();
                EditorUtility.SetDirty(camera);
            }

            if (GUILayout.Button("Restore Position"))
            {
                camera.EditorRestorePosition();
                EditorUtility.SetDirty(camera);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif