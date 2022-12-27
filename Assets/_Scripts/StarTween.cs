using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StarTween : MonoBehaviour
{
    private void OnEnable()
    {
        Sequence animationTween = DOTween.Sequence();
        for (int i =2; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Debug.Log("Play tween");
            child.localScale = Vector3.zero;
            animationTween.Append(child.DOPunchRotation(new Vector3(0,0, 360), 1f, 4, 0.5f)).Join(child.DOScale(new Vector3(1,1,1), 1f));
        }

        animationTween.Play();

    }
}
