using System.Collections.Generic;
using BillUtils.SpaceShipData;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Sound/SoundData", order = 0)]
public class SoundData : ScriptableObject
{
    public List<SoundClip> soundClips;

    public AudioClip GetClip(SoundID soundID)
    {
        foreach (var soundClip in soundClips)
        {
            if (soundClip.soundID == soundID && soundClip.audioClip != null)
                return soundClip.audioClip;
        }
        Debug.LogWarning("SoundID not found: " + soundID);
        return null;
    }
}

[System.Serializable]
public class SoundClip
{
    public SoundID soundID;
    public AudioClip audioClip;
}
