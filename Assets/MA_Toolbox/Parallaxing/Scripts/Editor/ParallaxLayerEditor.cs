#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using MA_Toolbox.Parallaxing;

namespace MA_Toolbox.ParallaxingEditor
{
    public class ParallaxLayerEditor : EditorWindow
    {
        public ParallaxLayerSetting parallaxLayerSetting = null;
        public string layerName = "";
        public bool zeroLayerStatic = true;
        public bool setLayerDepth = true;
        public bool invertLayerDepth = true;
        public bool resetChildDepth = false;

        [MenuItem("Tools/ParallaxLayerEditor")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            ParallaxLayerEditor window = (ParallaxLayerEditor)EditorWindow.GetWindow(typeof(ParallaxLayerEditor));
            window.Show();
        }

        void OnGUI()
        {
            parallaxLayerSetting = EditorGUILayout.ObjectField("Layer settings", parallaxLayerSetting, typeof(ParallaxLayerSetting), false) as ParallaxLayerSetting;
            layerName = EditorGUILayout.TextField("Layer name", layerName);
            zeroLayerStatic = EditorGUILayout.Toggle("Mark zero layers static", zeroLayerStatic);
            setLayerDepth = EditorGUILayout.Toggle("Set layer depth", setLayerDepth);
            invertLayerDepth = EditorGUILayout.Toggle("Invert layer depth", invertLayerDepth);
            resetChildDepth = EditorGUILayout.Toggle("Reset child depth", resetChildDepth);

            if (GUILayout.Button("Selection to layer groups"))
            {
                GameObject[] selection = Selection.gameObjects;
                Dictionary<LayerKey, ParallaxLayer> layers = new Dictionary<LayerKey, ParallaxLayer>();

                //Sort sprites into layers.
                foreach (GameObject go in selection)
                {
                    if (go.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
                    {
                        //Layer key.
                        LayerKey lk = new LayerKey
                        {
                            sortingOrder = spriteRenderer.sortingOrder,
                            sortingLayer = spriteRenderer.sortingLayerID
                        };

                        //Check if layer already exist.
                        if (layers.TryGetValue(lk, out ParallaxLayer layer))
                        {
                            go.transform.SetParent(layer.transform);
                            go.isStatic = layer.gameObject.isStatic;
                        }
                        //Create layer.
                        else
                        {
                            string lName = string.IsNullOrEmpty(layerName) ? "ParallaxLayer" : layerName;

                            GameObject newLayer = new GameObject
                            {
                                name = lName + " [" + spriteRenderer.sortingLayerName + " " + lk.sortingOrder.ToString() + "]"
                            };

                            //Mark as static when it doesn't move.
                            if (zeroLayerStatic)
                            {
                                float speedX = Mathf.Round(parallaxLayerSetting.layerSpeedCurveX.Evaluate(lk.sortingOrder) * 100f) / 100f;
                                float speedY = Mathf.Round(parallaxLayerSetting.layerSpeedCurveY.Evaluate(lk.sortingOrder) * 100f) / 100f;

                                if (speedX == 0.0f && speedY == 0.0f)
                                {
                                    newLayer.isStatic = true;
                                }
                            }

                            go.isStatic = newLayer.isStatic;

                            if (setLayerDepth)
                            {
                                newLayer.transform.position = new Vector3(newLayer.transform.position.x, newLayer.transform.position.y, invertLayerDepth ? -lk.sortingOrder : lk.sortingOrder);
                            }

                            //Add parallaxing component.
                            ParallaxLayer pl = newLayer.AddComponent<ParallaxLayer>();
                            pl.layerSetting = parallaxLayerSetting;
                            pl.sortingOrder = lk.sortingOrder;

                            //Add to dict and set parent.
                            layers.Add(new LayerKey { sortingOrder = spriteRenderer.sortingOrder, sortingLayer = spriteRenderer.sortingLayerID }, pl);
                            go.transform.SetParent(newLayer.transform);
                        }

                        if (resetChildDepth)
                        {
                            go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                        }
                    }
                }

                //Update layers.
                foreach (ParallaxLayer pl in layers.Values)
                {
                    pl.Init();
                }

                //Remove old layer.
                foreach (GameObject go in selection)
                {
                    if (go.TryGetComponent<ParallaxLayer>(out _))
                    {
                        if (go.transform.childCount <= 0)
                        {
                            DestroyImmediate(go);
                        }
                    }
                }

                //Mark scene dirty.
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                //Clear settings.
                layerName = "";
            }
        }

        public struct LayerKey
        {
            public int sortingOrder;
            public int sortingLayer;
        }
    }
}
#endif