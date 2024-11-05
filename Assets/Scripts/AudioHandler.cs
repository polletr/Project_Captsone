using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] EventReference audioClip;

    EventInstance audioInstance;


    public void PlayOneShotAudio(Transform soundPoint)
    {
        AudioManagerFMOD.Instance.PlayOneShot(audioClip, soundPoint.position);
    }

    public void PlayAudioInstance()
    {
        audioInstance = AudioManagerFMOD.Instance.CreateEventInstance(audioClip);
        audioInstance.start();
    }

    public void Play3DAudio()
    {
        audioInstance = AudioManagerFMOD.Instance.CreateEventInstance(audioClip);
        audioInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        audioInstance.start();
    }


    public void StopAudioInstance()
    {
        PLAYBACK_STATE playbackState;
        audioInstance.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.PLAYING)
        {
            Debug.Log("Stopping Audio");
            audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

}
