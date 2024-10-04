using System.Collections;
using System.Collections.Generic;
using BillUtils.SpaceShipData;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private SoundData soundData;
    [SerializeField] private Toggle toggle;
    [SerializeField] private Slider slider;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource audioSourceSFX;
    [SerializeField] private bool soundOn;

    [SerializeField] private List<Button> normalButtonClick;
    [SerializeField] private List<Button> colorButtonClick;
    [SerializeField] private List<Button> partButtonClick;
    [SerializeField] private List<Slider> slidersTarget;
    [SerializeField] private List<Toggle> toggles;
    [SerializeField] private List<Button> downloadButtons;

    private void Awake()
    {
        Init();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        toggle.onValueChanged.AddListener(delegate { OnTriggerMusic(); });
        slider.onValueChanged.AddListener(delegate { OnChangeVolume(); });
        slider.value = 0.01f;
        audioSource.volume = slider.value;

        OnTriggerMusic();
        OnChangeVolume();
    }

    private void Init()
    {
        foreach (Button button in normalButtonClick)
        {
            SetSoundButton(button, SoundID.S_CLICK);
        }

        foreach (Button button in colorButtonClick)
        {
            SetSoundButton(button, SoundID.S_COLOR_CLICK);
        }

        foreach (Button button in partButtonClick)
        {
            SetSoundButton(button, SoundID.S_PART_CLICK);
        }

        foreach (Slider slider in slidersTarget)
        {
            SetSoundSlider(slider);
        }

        foreach (Toggle toggle in toggles)
        {
            SetSoundToggle(toggle);
        }
        foreach (Button button in downloadButtons)
        {
            SetSoundDownloadButton(button);
        }
    }

    public void PlaySoundButtonColor()
    {
        PlaySound(SoundID.S_COLOR_CLICK);
    }
    public void SetSoundButton(Button button, SoundID soundID)
    {
        if (button != null)
            button.onClick.AddListener(() => PlaySound(soundID));
    }

    private void SetSoundSlider(Slider slider)
    {
        if (slider != null)
            slider.onValueChanged.AddListener(value => PlaySound(SoundID.S_SLIDER));
    }

    private void SetSoundToggle(Toggle toggle)
    {
        if (toggle != null)
            toggle.onValueChanged.AddListener(isOn => PlaySound(SoundID.S_TOGGLE));
    }

    private void SetSoundDownloadButton(Button button)
    {
        if (button != null)
            button.onClick.AddListener(() => PlaySound(SoundID.S_DOWNLOAD));
    }

    AudioClip clip;
    public void PlaySound(SoundID soundID)
    {
        if (!soundOn)
            return;

        clip = soundData.GetClip(soundID);
        if (clip != null)
        {
            audioSourceSFX.pitch = Random.Range(1, 1.05f);
            audioSourceSFX.PlayOneShot(clip);
        }
    }

    private void OnTriggerMusic()
    {
        if (toggle.isOn)
        {
            soundOn = true;
            audioSource.volume = slider.value;
            return;
        }
        soundOn = false;
        audioSource.volume = 0;
    }

    private void OnChangeVolume()
    {
        if (!soundOn)
            return;

        audioSource.volume = slider.value;
    }

}
