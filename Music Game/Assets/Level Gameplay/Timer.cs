using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI countdownTextGUI = null;
    [SerializeField]  TextMeshProUGUI timerTextGUI = null;

    public float RemainingTime { get; private set; }

    Coroutine timerRoutine = null;
    const int timerUpdateFrequency = 5;
    float maxTime;


    public static event System.Action TimerExpiredEvent;

    #region SETUP
    private void Awake()
    {
        maxTime = 10f;
        ShowTextGUI(timerTextGUI, false);
        ShowTextGUI(countdownTextGUI, false);
    }
    public void Initialize(float initialTime)
    {
        StopAllCoroutines();
        maxTime = initialTime;
        ResetGuessTimer();
        ShowTextGUI(timerTextGUI, false);
    }
    #endregion

    #region HELPER METHODS
    IEnumerator CountdownRoutine(int startTime, float delay, System.Action endOfCountdownDelegate)
    {
        yield return new WaitForSeconds(1f); // this delay is needed for smoother start
        ShowTextGUI(countdownTextGUI);
        while (startTime > 0)
        {
            countdownTextGUI.text = startTime.ToString();
            AudioManager.PlaySound(AudioManager.countdown, SoundType.UI);
            yield return new WaitForSeconds(delay);
            startTime--;
        }
        ShowTextGUI(countdownTextGUI, false);
        endOfCountdownDelegate?.Invoke();
    }
    IEnumerator TimerRoutine()
    {
        int counter = 0;

        while (RemainingTime > 2.5f)
        {
            RemainingTime -= Time.deltaTime;
            if (counter % timerUpdateFrequency == 0) UpdateTimerText();
            counter++;
            yield return null;
        }
        while (RemainingTime > 0.0f)
        {
            float deltaTime = Time.deltaTime;
            RemainingTime -= deltaTime;
            if (counter % timerUpdateFrequency == 0) UpdateTimerText();
            counter++;
            timerTextGUI.fontSize = Mathf.Lerp(timerTextGUI.fontSize, 95f, deltaTime);
            timerTextGUI.color = Color.Lerp(timerTextGUI.color, Color.red, deltaTime);
            yield return null;
        }
        RemainingTime = 0f;
        UpdateTimerText();

        TimerExpiredEvent?.Invoke();
    }
    void UpdateTimerText()
    {
        int val = (int)(RemainingTime * 100.0f);
        int ones = val / 100;
        int hundreths = val % 100;
        string formattedTimerText = string.Format("{0}.{1:00}s", ones, hundreths);
        if (timerTextGUI) timerTextGUI.text = formattedTimerText;
    }
    void ShowTextGUI(TextMeshProUGUI textGUI, bool canShow = true)
    {
        textGUI.gameObject.SetActive(canShow);
    }
    #endregion

    #region UTILITY METHODS
    public void DisplayTimer(bool isEnabled = true)
    {
        ShowTextGUI(timerTextGUI, isEnabled);
    }
    public void StartCountdown(int startTime, float delay, System.Action endOfCountdownDelegate)
    {
        StartCoroutine(CountdownRoutine(startTime, delay, endOfCountdownDelegate));
    }
    public void StartGuessTimer()
    {
        StopGuessTimer();
        timerRoutine = StartCoroutine(TimerRoutine());
        //AudioManager.Instance.PlaySound(timerSound.name);
    }
    public void StopGuessTimer()
    {
        if (timerRoutine != null) StopCoroutine(timerRoutine);
        //AudioManager.Instance.StopSound(timerSound.name);
    }
    public void ResetGuessTimer()
    {
        StopGuessTimer();
        RemainingTime = maxTime;

        // reset timerGUI properties
        timerTextGUI.fontSize = 65f;
        timerTextGUI.color = Color.white;
        UpdateTimerText();
    }
    #endregion

    
}
