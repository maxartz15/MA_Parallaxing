using UnityEngine;
using MA_Toolbox.Utils;

namespace MA_Toolbox.Parallaxing
{
    [RequireComponent(typeof(Camera))]
    public class ParallaxCamera : MonoBehaviour
    {
        [SerializeField]
        private bool moveParallax = true;
        [SerializeField]
        private bool useCulling = true;
        [SerializeField]
        private bool cullInEditor = true;

        public bool Parallaxing
        { 
            get
            {
                return moveParallax;
            }
        }

        public bool Culling
        {
            get
            {
                if (useCulling)
                {
#if UNITY_EDITOR
                    if (cullInEditor && Application.isPlaying)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
#else
				return true;
#endif
                }
                else
                {
                    return false;
                }
            }
        }

        public Bounds cameraBounds
        {
            get;
            private set;
        }

        private void Update()
        {
            cameraBounds = Camera.main.OrthographicBounds();
        }

#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private Vector3 storedPosition = Vector3.zero;
        public void EditorSavePosition()
        {
            storedPosition = this.transform.position;
        }

        public void EditorRestorePosition()
        {
            this.transform.position = storedPosition;
        }
#endif

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            if (cameraBounds != null)
            {
                Gizmos.DrawWireCube(cameraBounds.center, cameraBounds.size);
            }

            Gizmos.color = Color.white;
        }
    }
}