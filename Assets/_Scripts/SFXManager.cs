using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    
    [SerializeField] private AudioClip swipeUpSound;
    [SerializeField] private AudioClip swipeRightSound;
    [SerializeField] private AudioClip winSound;

    private bool isSoundPlayed;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InputManager.SwipeAction += PlayMovementSound;
        LevelManager.OnPlayerWin += OnWinSound;
    }

    private void PlayMovementSound(Vector2 input)
    {
        if (input == Vector2.right || input == Vector2.left)
        {
            audioSource.clip = swipeRightSound;
        }
        else
        {
            audioSource.clip = swipeUpSound;
        }

        audioSource.Play();
    }

    private void OnWinSound()
    {
        if(isSoundPlayed) return;
        audioSource.clip = winSound;
        audioSource.PlayOneShot(winSound, 0.5f);
        isSoundPlayed = true;
    }

    private void OnDestroy()
    {
        InputManager.SwipeAction -= PlayMovementSound;
        LevelManager.OnPlayerWin -= OnWinSound;
    }
}

