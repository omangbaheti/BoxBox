using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleParticleSystem : MonoBehaviour
{
    private ParticleSystem confettiParticleSystem;

    private void Start()
    {
        confettiParticleSystem = GetComponent<ParticleSystem>();
        confettiParticleSystem.Pause();
        LevelManager.OnPlayerWin += TriggerConfetti;
    }

    private void TriggerConfetti()
    {
        confettiParticleSystem.Play();
    }

    private void OnDestroy()
    {
        LevelManager.OnPlayerWin -= TriggerConfetti;
    }
}
