using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persists across scenes

            // Load volumes from PlayerPrefs
            LoadVolumes();

            // Preload SFX pool
            InitializeSFXPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Audio Mixer")]
    public AudioMixer audioMixer; // Assign your AudioMixer asset (with Master, SFX, Music groups)

    [Header("Music")]
    public AudioSource musicSource;
    public AudioClip defaultMusic; // Optional startup music

    [Header("SFX Pool")]
    public int sfxPoolSize = 10; // Number of pooled AudioSources for SFX
    public AudioSource[] sfxSources; // Auto-populated

    [Header("Audio Clips Folder")]
    public string sfxClipsPath = "Audio/SFX"; // Resources/Audio/SFX/*.wav
    public string musicClipsPath = "Audio/Music"; // Resources/Audio/Music/*.ogg

    // Cached clips (loaded on demand)
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClips = new Dictionary<string, AudioClip>();

    private void Start()
    {
        if (defaultMusic != null) PlayMusic("MenuBGMLoop"); // Or auto-play defaultMusic
    }

    #region Public API - Easy to use from other scripts
    /// <summary> Play SFX by name (loads from Resources if needed). E.g.: SoundManager.Instance.PlaySFX("jump") </summary>
    public void PlaySFX(string clipName)
    {
        StartCoroutine(PlaySFXCoroutine(clipName));
    }

    /// <summary> Play Music by name, with optional loop & fade </summary>
    public void PlayMusic(string clipName, bool loop = true, float fadeTime = 1f)
    {
        StartCoroutine(PlayMusicCoroutine(clipName, loop, fadeTime));
    }

    /// <summary> Stop current music with fade </summary>
    public void StopMusic(float fadeTime = 1f)
    {
        StartCoroutine(FadeMusicOut(fadeTime));
    }

    /// <summary> Set volume for group (Master/SFX/Music) 0-1 </summary>
    public void SetVolume(string groupName, float volume)
    {
        volume = Mathf.Clamp01(volume);
        string paramName = groupName + "Volume";

        if (audioMixer != null)
            audioMixer.SetFloat(paramName, Mathf.Lerp(-80f, 0f, volume));

        // Save to prefs
        PlayerPrefs.SetFloat("Volume_" + groupName, volume);
        PlayerPrefs.Save();
    }

    /// <summary> Get current volume for group </summary>
    public float GetVolume(string groupName)
    {
        float vol;
        string paramName = groupName + "Volume";
        audioMixer.GetFloat(paramName, out vol);
        return Mathf.Lerp(0f, 1f, (vol + 80f) / 80f); // Convert dB back to 0-1
    }
    #endregion

    #region Private Implementation
    private void InitializeSFXPool()
    {
        sfxSources = new AudioSource[sfxPoolSize];
        GameObject poolParent = new GameObject("SFX Pool");
        poolParent.transform.SetParent(transform);

        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sourceObj = new GameObject($"SFX_{i}");
            sourceObj.transform.SetParent(poolParent.transform);
            sfxSources[i] = sourceObj.AddComponent<AudioSource>();
            sfxSources[i].playOnAwake = false;
            sfxSources[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0]; // Route to SFX group
        }
    }

    private IEnumerator PlaySFXCoroutine(string clipName)
    {
        AudioClip clip = GetClip(clipName, true); // true = SFX
        if (clip == null)
        {
            Debug.LogWarning($"SFX not found: {clipName}");
            yield break;
        }

        // Find free AudioSource
        AudioSource source = GetFreeSFXSource();
        if (source == null)
        {
            Debug.LogWarning("No free SFX sources!");
            yield break;
        }

        source.clip = clip;
        source.Play();

        // Auto-free when done
        yield return new WaitForSeconds(clip.length);
        source.Stop();
        source.clip = null;
    }

    private IEnumerator PlayMusicCoroutine(string clipName, bool loop, float fadeTime)
    {
        AudioClip clip = GetClip(clipName, false); // false = Music
        if (clip == null)
        {
            Debug.LogWarning($"Music not found: {clipName}");
            yield break;
        }

        // Fade out current
        yield return StartCoroutine(FadeMusicOut(fadeTime));

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();

        // Fade in
        yield return StartCoroutine(FadeMusicIn(fadeTime));
    }

    private IEnumerator FadeMusicOut(float fadeTime)
    {
        if (!musicSource.isPlaying) yield break;

        float startVol = musicSource.volume;
        float timer = 0;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime; // Ignore pause
            musicSource.volume = Mathf.Lerp(startVol, 0f, timer / fadeTime);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = 1f;
    }

    private IEnumerator FadeMusicIn(float fadeTime)
    {
        musicSource.volume = 0f;
        float timer = 0;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, 1f, timer / fadeTime);
            yield return null;
        }
    }

    private AudioClip GetClip(string name, bool isSFX)
    {
        string dict = isSFX ? "sfxClips" : "musicClips";
        var cache = isSFX ? sfxClips : musicClips;

        if (cache.ContainsKey(name))
            return cache[name];

        // Load from Resources
        string path = isSFX ? sfxClipsPath : musicClipsPath;
        AudioClip clip = Resources.Load<AudioClip>(System.IO.Path.Combine(path, name));

        if (clip != null)
        {
            cache[name] = clip;
            return clip;
        }

        return null;
    }

    private AudioSource GetFreeSFXSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying) return source;
        }
        return null; // All busy
    }

    private void LoadVolumes()
    {
        if (audioMixer == null) return;

        string[] groups = { "Master", "SFX", "Music" };
        foreach (string group in groups)
        {
            float savedVol = PlayerPrefs.GetFloat("Volume_" + group, 1f);
            SetVolume(group, savedVol);
        }
    }
    #endregion
}
/// <summary> Play</summary>
