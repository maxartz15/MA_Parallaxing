using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MA_Toolbox.Utils
{
    public static class CameraUtils
    {
        public static Bounds OrthographicBounds(this Camera camera)
        {
            float screenAspect = (float)Screen.width / (float)Screen.height;
            float cameraHeight = camera.orthographicSize * 2;

            return new Bounds(camera.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, camera.farClipPlane));
        }
    }
}