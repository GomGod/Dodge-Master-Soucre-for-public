using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameOverImageTweening : MonoBehaviour
{
    private readonly Vector3 _rot0 = new(0, 0, 1);
    private readonly Vector3 _rot1 = new(0, 0, -1);
    private void OnEnable()
    {
        gameObject.transform.rotation = Quaternion.Euler(0,0,_rot1.z);
        gameObject.transform.DOKill();
        var seq = DOTween.Sequence();

        seq.Append(gameObject.transform.DORotate(_rot0, 2f)).Append(gameObject.transform.DORotate(_rot1, 2f)).SetLoops(-1);
        seq.Play();
    }
}
