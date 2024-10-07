using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LightController : MonoBehaviour
{
    private Light lightSource;

    [Header("Flicker Settings")]
    [SerializeField] public float minFlickerDuration = 1f; // Minimum flicker duration
    [SerializeField] public float maxFlickerDuration = 3f; // Maximum flicker duration
    [SerializeField] public float flickerFrequency = 0.1f; // Frequency of flickering
    [SerializeField] bool constantFlickering;

    private bool isFlickering = false; // Track flickering state
    private float flickerChance = 0.3f; // 30% chance to flicker when turning on

    private float originalIntensity;

    [SerializeField] bool guidingLight = false;

    private EventInstance constantFlickeringSound;

    private void Awake()
    {
        lightSource = GetComponent<Light>();

        if (lightSource == null)
        {
            Debug.LogError("Error: No Light component found on " + gameObject.name);
            enabled = false;
            return;
        }

        originalIntensity = lightSource.intensity;

        if (constantFlickering)
        {
            constantFlickeringSound = AudioManagerFMOD.Instance.CreateEventInstance(AudioManagerFMOD.Instance.SFXEvents.LightConstantFlickering);
            constantFlickeringSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            StartCoroutine(ConstantFlickerCoroutine());
        }
    }

    // Turn light on or off
    public void TurnOnOffLight(bool check)
    {
        if (!guidingLight)
        {
            if (!check && constantFlickering)
                StopConstantFlickering();
            else
                AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.LightTurnOn, transform.position);
            // Random chance to flicker when turning on/off
            if (check && Random.value < flickerChance) // When turning on
                FlickerLight();

            lightSource.enabled = check;
        }

    }

    // Flicker the light for a specified duration
    public void FlickerLight()
    {
        if (!isFlickering)
        {
            AudioManagerFMOD.Instance.PlayOneShot(AudioManagerFMOD.Instance.SFXEvents.LightFlickerOnce, transform.position);
            float randomFlickerDuration = Random.Range(minFlickerDuration, maxFlickerDuration);
            StartCoroutine(FlickerCoroutine(randomFlickerDuration));
        }
    }

    private IEnumerator FlickerCoroutine(float maxTime)
    {
        float timer = 0f;

        StopConstantFlickering();
        while (timer < maxTime)
        {
            // Randomize the intensity
            lightSource.intensity = Random.Range(0.2f, originalIntensity); // Adjust the max intensity as needed

            // Randomize the time interval for the next flicker
            float flickerTimer = Random.Range(flickerFrequency, flickerFrequency * 2);

            // Randomly turn the light on or off for a more dramatic effect
            lightSource.enabled = (Random.value > 0.3f); // 70% chance to stay on

            timer += flickerTimer;

            // Wait for the next flicker
            yield return new WaitForSeconds(flickerTimer);
        }

        lightSource.enabled = true; // Ensure the light is left on after flickering
        lightSource.intensity = originalIntensity;
        isFlickering = false; // Reset flickering state
    }

    // Constant flicker coroutine
    private IEnumerator ConstantFlickerCoroutine()
    {
        constantFlickeringSound.start();
        while (constantFlickering)
        {
            // Randomize the intensity
            lightSource.intensity = Random.Range(0.5f, originalIntensity);

            // Randomize the time interval for the next flicker
            float flickerTimer = Random.Range(flickerFrequency, flickerFrequency * 2);

            // Randomly turn the light on or off
            lightSource.enabled = (Random.value > 0.1f); // 90% chance to stay on

            // Wait for the next flicker
            yield return new WaitForSeconds(flickerTimer);
        }
    }

    private void StopConstantFlickering()
    {
        PLAYBACK_STATE playbackState;
        constantFlickeringSound.getPlaybackState(out playbackState);

        if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
        {
            constantFlickeringSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        constantFlickering = false;


    }

    // Change light color
    public void ChangeLightColor(Color newColor)
    {
        lightSource.color = newColor;
    }
}