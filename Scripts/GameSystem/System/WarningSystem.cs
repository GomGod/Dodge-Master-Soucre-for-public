
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class WarningSystem : MonoBehaviour
{
    [SerializeField] private GameObject prefabWarningTile;
    
    private readonly ObjectPool _objPool = new();
    private static readonly int AlphaShaderParam = Shader.PropertyToID("_Alpha");

    public void Start()
    {
        _objPool.InitObjectPool(prefabWarningTile, 25);
    }

    public void SetWarningTile(Vector2 pos, float duration)
    {
        var tile = _objPool.ActivateObject().GetComponent<SpriteRenderer>();
        var alpha = 0f;
        tile.material.SetFloat(AlphaShaderParam, alpha);
        tile.transform.position = pos;
        
        DOTween.To(() => alpha, x =>
        {
            alpha = x;
            tile.material.SetFloat(AlphaShaderParam, alpha);
            
        }, 0.33f, duration).OnComplete(()=>tile.gameObject.SetActive(false));
    }
}