using UnityEngine;
using UnityEngine.UI;


namespace KD.MusicGame.Utility
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] float solidAlpha = 1f;
        [SerializeField] float clearAlpha = 0f;
        [SerializeField] float fadeOnDuration = 2f;
        [SerializeField] float fadeOffDuration = 2f;
        [SerializeField] MaskableGraphic[] graphicsToFade = null;

        public float FadeOnDuration { get { return fadeOnDuration; } }
        public float FadeOffDuration { get { return fadeOffDuration; } }

        void SetAlpha(float alpha)
        {
            foreach (MaskableGraphic graphic in graphicsToFade)
            {
                if (graphic) { graphic.canvasRenderer.SetAlpha(alpha); }
            }
        }

        void Fade(float targetAlpha, float duration)
        {
            foreach (MaskableGraphic graphic in graphicsToFade)
            {
                if (graphic) { graphic.CrossFadeAlpha(targetAlpha, duration, true); }
            }
        }

        public void FadeOff()
        {
            SetAlpha(solidAlpha);
            Fade(clearAlpha, fadeOffDuration);
        }
        public void FadeOn()
        {
            SetAlpha(clearAlpha);
            Fade(solidAlpha, fadeOnDuration);
        }
    }
}

