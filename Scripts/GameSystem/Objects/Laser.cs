using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private SpriteRenderer laserSprite;
    [SerializeField] private AudioSource sfx;
    
    private Vector2 _direction;
    private bool _disableCollision = false;

    private void OnEnable()
    {
        sfx.Play();
        _disableCollision = false;
    }

    private void OnDisable()
    {
        GameManager.Instance.Score += 1;
    }

    public void SetDirection(Vector2 dir)
    {
        _direction = dir;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_disableCollision || other.gameObject.layer != LayerMask.NameToLayer($"Player")) return;
        
        GameManager.Instance.GameOver();
    }

    public void InitProjectile()
    {
        var targetColor = laserSprite.color;
        targetColor.a = 1;
        laserSprite.color = targetColor;
        targetColor.a = 0;
        transform.rotation = Quaternion.Euler(0, 0, _direction.y > _direction.x ? 90 : 0);
        laserSprite.DOColor(targetColor, 0.25f).OnComplete(()=>gameObject.SetActive(false));
        StartCoroutine(DisableCollider());
    }
    private IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(0.07f);
        _disableCollision = true;
    }
}

public struct LaserInfo
{
    public Vector2 Dir;
    public Vector2 GenPosVector;
    public float WarningDelay;
    public float AfterDelay;
}

