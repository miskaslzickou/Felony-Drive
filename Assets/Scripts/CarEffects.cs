using UnityEngine;
using static UnityEngine.InputSystem.DefaultInputActions;
using System.Collections;
using UnityEngine.Rendering.Universal;
public class CarEffects : MonoBehaviour
{
    public CarControllerV2 car; // Reference na skript pro ovládání auta
    public UI ui;
    [Header("Nastavení zvukových efektů")]
    public AudioClip engineStartAudioClip;
    public AudioClip engineLoopAudioClip;
    public AudioClip honkAudioClip;
    [SerializeField] private AudioSource engineAudioSrc;
    [SerializeField] private AudioSource honkAudioSrc;
    private bool isHonking = false;
   
    [Header("Nastavení vizuálních efektů")]
    public float driftThreshold = 2f; // Rychlost, při které se spustí efekt driftu
    public TrailRenderer[] trailRenderers;
    public ParticleSystem[] driftParticles;
    public Animator animator;
    public Light2D [] headlight;
    private Vector3 originalScale;
    private void Awake()
    {
        
        engineAudioSrc.clip = engineStartAudioClip;
        honkAudioSrc.clip = honkAudioClip;
        originalScale = transform.localScale;
    }
   
    public void Honk() {
        if (car.isHonking) 
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
    IEnumerator EngineStartSequence()
    { 
        engineAudioSrc.clip=engineStartAudioClip;
        engineAudioSrc.loop = false;
        engineAudioSrc.Play();

        yield return new WaitForSeconds(engineStartAudioClip.length);
        transform.localScale = new Vector3(0.85f, 0.65f, 0.75f);
        engineAudioSrc.clip = engineLoopAudioClip;
        engineAudioSrc.loop = true;
        engineAudioSrc.Play();

    }
    
    public void StartEngineSound()
    {
        if (car.engineStarted)
        {
           StartCoroutine(EngineStartSequence());
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
        engineAudioSrc.pitch = 1f + Mathf.Clamp01(car.normalizedSpeed); // Základní pitch 1, který se zvyšuje s rychlostí
    }
    // Update is called once per frame
    void Update()
    {
        UpdateVisuals();
        UpdateAudio();

        if (!(transform.localScale != originalScale))
            return;
        
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 8f);
        if (Vector3.Distance(transform.localScale, originalScale) < 0.02f)
            transform.localScale = originalScale;
        
    }
}
