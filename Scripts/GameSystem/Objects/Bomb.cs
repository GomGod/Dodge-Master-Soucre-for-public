using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Bomb : MonoBehaviour
{
    [SerializeField] private List<Sprite> countDownSpritesInv;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer countDownRenderer;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private AudioSource countDownSound;

    [SerializeField] private List<GameObject> subParts;
    [SerializeField] private GameObject effectSet;
    

    private Vector2 position;
    
    public void InitMine(Vector2 pos)
    {
        position = pos; 
        var worldPos = GameManager.Instance.GetPositionFromGirdVector(pos);
        transform.position = worldPos;
    }
    private void OnEnable()
    {
        spriteRenderer.enabled = true;
        foreach(var obj in subParts)
            obj.SetActive(true);
        StartCoroutine(CountDown());
    }
    private void Boom()
    {
        explosionSound.Play();
        spriteRenderer.enabled = false;
        effectSet.SetActive(true);
        foreach(var obj in subParts)
            obj.SetActive(false);
        
        var playerPos = GameManager.Instance.GetCharacterPos();

        if (Vector2.Distance(playerPos ,position) <= 1f)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            GameManager.Instance.Score += 1;
        }
        
        StartCoroutine(WaitForDisable());
    }

    private IEnumerator CountDown()
    {
        for (var curCount = 0; curCount < 5; curCount += 1)
        {
            countDownSound.Play();
            countDownRenderer.sprite = countDownSpritesInv[curCount];
            yield return new WaitForSeconds(1f);
        }
        Boom();
    }

    private IEnumerator WaitForDisable()
    {
        yield return new WaitUntil(() => !explosionSound.isPlaying);
        gameObject.SetActive(false);
    }
}
