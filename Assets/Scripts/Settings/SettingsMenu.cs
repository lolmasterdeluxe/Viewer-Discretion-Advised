using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References – assign in Inspector")]
    [Tooltip("Master volume slider (0–1)")]
    public Slider masterVolumeSlider;

    [Tooltip("Music volume slider (0–1)")]
    public Slider musicVolumeSlider;

    [Tooltip("SFX volume slider (0–1)")]
    public Slider sfxVolumeSlider;

    [Tooltip("Optional: button to reset to defaults")]
    public Button resetButton;

    [Tooltip("Optional: feedback text (e.g. 'Settings saved')")]
    public Text statusText;

    [Header("Behavior")]
    public bool autoApplyOnChange = true;       // apply immediately when slider moves?
    public bool saveOnDisable = true;           // save when menu is closed/hidden?

    private const string PREF_MASTER = "Volume_Master";
    private const string PREF_MUSIC = "Volume_Music";
    private const string PREF_SFX = "Volume_SFX";

    private bool initialized = false;

    private void Awake()
    {
        // Optional: hide on start if this is a pause menu
        // gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        LoadCurrentVolumes();
        SetupListeners();

        if (statusText) statusText.text = "";
        initialized = true;
    }

    private void OnDisable()
    {
        if (saveOnDisable && initialized)
        {
            SaveVolumes();
        }
    }

    private void SetupListeners()
    {
        if (masterVolumeSlider) masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        if (musicVolumeSlider) musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxVolumeSlider) sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (resetButton) resetButton.onClick.AddListener(ResetToDefaults);
    }

    private void LoadCurrentVolumes()
    {
        // Default values if no prefs exist
        float master = PlayerPrefs.GetFloat(PREF_MASTER, 1f);
        float music = PlayerPrefs.GetFloat(PREF_MUSIC, 1f);
        float sfx = PlayerPrefs.GetFloat(PREF_SFX, 1f);

        // Apply to sliders
        if (masterVolumeSlider) masterVolumeSlider.value = master;
        if (musicVolumeSlider) musicVolumeSlider.value = music;
        if (sfxVolumeSlider) sfxVolumeSlider.value = sfx;

        // Make sure SoundManager has the same values
        ApplyVolumesToSoundManager();
    }

    private void ApplyVolumesToSoundManager()
    {
        if (SoundManager.Instance == null) return;

        if (masterVolumeSlider) SoundManager.Instance.SetVolume("Master", masterVolumeSlider.value);
        if (musicVolumeSlider) SoundManager.Instance.SetVolume("Music", musicVolumeSlider.value);
        if (sfxVolumeSlider) SoundManager.Instance.SetVolume("SFX", sfxVolumeSlider.value);
    }

    // ────────────────────────────────────────────────
    //   Slider callbacks
    // ────────────────────────────────────────────────

    private void OnMasterVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetVolume("Master", value);

        if (autoApplyOnChange && statusText)
            statusText.text = "Master volume updated";
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetVolume("Music", value);

        if (autoApplyOnChange && statusText)
            statusText.text = "Music volume updated";
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetVolume("SFX", value);

        if (autoApplyOnChange && statusText)
            statusText.text = "SFX volume updated";

        // Optional: play test sound when SFX slider is moved
        if (autoApplyOnChange)
            SoundManager.Instance.PlaySFX("ui_click");   // ← change to any short sound you have
    }

    // ────────────────────────────────────────────────
    //   Buttons / actions
    // ────────────────────────────────────────────────

    public void ResetToDefaults()
    {
        if (masterVolumeSlider) masterVolumeSlider.value = 1f;
        if (musicVolumeSlider) musicVolumeSlider.value = 1f;
        if (sfxVolumeSlider) sfxVolumeSlider.value = 1f;

        ApplyVolumesToSoundManager();

        if (statusText)
            statusText.text = "Defaults restored";
    }

    public void SaveVolumes()
    {
        if (masterVolumeSlider) PlayerPrefs.SetFloat(PREF_MASTER, masterVolumeSlider.value);
        if (musicVolumeSlider) PlayerPrefs.SetFloat(PREF_MUSIC, musicVolumeSlider.value);
        if (sfxVolumeSlider) PlayerPrefs.SetFloat(PREF_SFX, sfxVolumeSlider.value);

        PlayerPrefs.Save();

        if (statusText)
            statusText.text = "Settings saved";
    }

    // Optional: call this from an "Apply" button if autoApplyOnChange = false
    public void ApplyAll()
    {
        ApplyVolumesToSoundManager();
        SaveVolumes();

        if (statusText)
            statusText.text = "Settings applied and saved";
    }

    // ────────────────────────────────────────────────
    //   Public helpers – can be called from other menus / scripts
    // ────────────────────────────────────────────────

    public void ShowSettings()
    {
        gameObject.SetActive(true);
    }

    public void HideSettings()
    {
        SaveVolumes();
        gameObject.SetActive(false);
    }
}
