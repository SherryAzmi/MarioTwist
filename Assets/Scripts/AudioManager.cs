using UnityEngine;

// Plain static store (not a scene object) so both scenes' UI can reach it via a
// forwarding method on their own always-present controller (GameManager / MainMenu)
// without needing a DontDestroyOnLoad singleton GameObject and its duplicate-on-
// reload pitfalls. Volumes persist across sessions via PlayerPrefs.
public static class AudioManager
{
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private static float musicVolume;
    private static float sfxVolume;
    private static bool loaded;

    public static float MusicVolume { get { EnsureLoaded(); return musicVolume; } }
    public static float SFXVolume { get { EnsureLoaded(); return sfxVolume; } }

    public static event System.Action<float> MusicVolumeChanged;
    public static event System.Action<float> SFXVolumeChanged;

    private static void EnsureLoaded()
    {
        if (loaded) return;
        loaded = true;
        musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
    }

    public static void SetMusicVolume(float volume)
    {
        EnsureLoaded();
        musicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        MusicVolumeChanged?.Invoke(musicVolume);
    }

    public static void SetSFXVolume(float volume)
    {
        EnsureLoaded();
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume);
        SFXVolumeChanged?.Invoke(sfxVolume);
    }
}
