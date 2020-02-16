using UnityEngine;
using MA_Toolbox.Utils;

namespace MA_Toolbox.Parallaxing
{
    [RequireComponent(typeof(Camera))]
    public class ParallaxCamera : MonoBehaviour
    {
        public bool moveParallax;

        [SerializeField]
        private bool useCulling = true;
        [SerializeField]
        private bool cullInEditor = false;

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

        [SerializeField, HideInInspector]
        private Vector3 storedPosition;

        private void Update()
        {
            cameraBounds = Camera.main.OrthographicBounds();
        }

        public void SavePosition()
        {
            storedPosition = transform.position;
        }

        public void RestorePosition()
        {
            transform.position = storedPosition;
        }

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