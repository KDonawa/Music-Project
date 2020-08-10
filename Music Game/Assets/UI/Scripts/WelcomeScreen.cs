using UnityEngine;
using UnityEngine.UI;

public class WelcomeScreen : MonoBehaviour
{
    [SerializeField] Button tapButton = null;

    private void Awake()
    {
        tapButton.onClick.AddListener(ButtonPressed);
    }
    private void OnDestroy()
    {
        tapButton.onClick.RemoveListener(ButtonPressed);
    }

    void ButtonPressed()
    {
        AudioManager.PlaySound(AudioManager.buttonSelect1, SoundType.SFX);
        SceneTransitions.PlayTransition(InTransition.FADE_IN, OutTransition.FADE_OUT, () => GameManager.LoadStartScene());
    }
}
