using UnityEngine;
using UnityEditor;
using MA_Toolbox.Parallaxing;

namespace MA_Toolbox.ParallaxingEditor
{
    [CustomEditor(typeof(ParallaxCamera))]
    class ParallaxCameraEditor : Editor
    {
        private ParallaxCamera options;

        void Awake()
        {
            options = (ParallaxCamera)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Save Position"))
            {
                options.SavePosition();
                EditorUtility.SetDirty(options);
            }

            if (GUILayout.Button("Restore Position"))
            {
                options.RestorePosition();
            }
        }
    }
}