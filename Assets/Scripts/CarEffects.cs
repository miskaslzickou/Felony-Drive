using UnityEngine;
using static UnityEngine.InputSystem.DefaultInputActions;
using UnityEngine.Rendering.Universal;
public class CarEffects : MonoBehaviour
{
    public CarControllerV2 car; // Reference na skript pro ovládání auta
    public UI ui;
    [Header("Nastavení zvukových efektů")]
    public AudioClip engineAudioClip;
    [SerializeField] private AudioSource engineAudioSrc;
    public AudioClip honkAudioClip;
    [SerializeField] private AudioSource honkAudioSrc;
    private bool isHonking = false;
   
    [Header("Nastavení vizuálních efektů")]
    public float driftThreshold = 2f; // Rychlost, při které se spustí efekt driftu
    public TrailRenderer[] trailRenderers;
    public ParticleSystem[] driftParticles;
    public Animator animator;
    public Light2D [] headlight;

    private void Awake()
    {
        
        engineAudioSrc.clip = engineAudioClip;
        honkAudioSrc.clip = honkAudioClip;
    }
   
    public void Honk(bool state) {
        if (state) 
            honkAudioSrc.Play();
        else
            honkAudioSrc.Stop();
    }
    public void Lights(bool state)
    {
        ui.ChangeSpeedometerTint(state);
        foreach (var light in headlight)
        {
            
            light.enabled = state;
        }
    }
    public void StartEngineSound(bool state)
    {
        if (state)
        {
            //přidat zvuk startování motoru
            engineAudioSrc.Play();
        }
        else
        {
            engineAudioSrc.Stop();
            
           

        }
    }
    void UpdateVisuals()
    {
        if (car.isHandbrake || Mathf.Abs(car.rearLateralSpeed) > driftThreshold)
        {
            for (int i = 0; i < 2; i++)
            {
                trailRenderers[i].emitting = true;
                if (!driftParticles[i].isPlaying) driftParticles[i].Play();
            }
        }
        else
        {

            for (int i = 0; i < 2; i++)
            {
                trailRenderers[i].emitting = false;
                if (driftParticles[i].isPlaying) driftParticles[i].Stop();
            }
        }
        animator.SetBool("isBraking", (car.isBraking || car.isHandbrake));

    }
    void UpdateAudio()
    {
        engineAudioSrc.pitch = 1f + Mathf.Clamp01(car.normalizedSpeed); // Základní pitch 0.5, který se zvyšuje s rychlostí
    }
    // Update is called once per frame
    void Update()
    {
        UpdateVisuals();
        UpdateAudio();
    }
}
