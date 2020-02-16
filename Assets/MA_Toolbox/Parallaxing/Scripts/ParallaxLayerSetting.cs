using UnityEngine;

namespace MA_Toolbox.Parallaxing
{
    [CreateAssetMenu(menuName = "Sprites/ParallaxLayerSettings")]
    public class ParallaxLayerSetting : ScriptableObject
    {
        public AnimationCurve layerSpeedCurveX = new AnimationCurve()
        {
            keys = new Keyframe[]
            {
                new Keyframe(-5.0f, 0.5f),
                new Keyframe(5.0f, -0.5f)
            }
        };
        public AnimationCurve layerSpeedCurveY = new AnimationCurve()
        {
            keys = new Keyframe[]
            {
                new Keyframe(-5.0f, 1.05f),
                new Keyframe(5.0f, 0.05f)
            }
        };
        public bool flip = false;
    }
}