using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Character : MonoBehaviour, InputListener
{
    private bool _moving = false;
    [SerializeField] private Animator characterAnimator;
   
    private static readonly int IsDash = Animator.StringToHash("isDash");

    private readonly Vector3 _leftScale = new(-0.9f, 0.9f, 1);
    private readonly Vector3 _rightScale = new(0.9f, 0.9f, 1);
    
    
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float afterImageDelay = 0.15f;
    [SerializeField] private float afterImageRemain = 0.33f;

    private readonly ObjectPool _afterImgPool = new();

    public bool Moving
    {
        get => _moving;
        set
        {
            _moving = value;
            characterAnimator.SetBool(IsDash, _moving);
        }
    }

    private IEnumerator AfterImageGenerator()
    {
        while (true)
        {
            yield return new WaitUntil(()=>_moving);
            var img = _afterImgPool.ActivateObject().GetComponent<SpriteRenderer>();
            var tColor = img.color;
            tColor.a = 1;
            img.color = tColor;
            var transform1 = img.transform;
            var o = gameObject;
            transform1.position = o.transform.position;
            transform1.localScale = o.transform.localScale;
            img.DOFade(0f, afterImageRemain).OnComplete(()=>
            {
                img.gameObject.SetActive(false);
            });
            yield return new WaitForSeconds(afterImageDelay);
        }
    }
    
    private void Start()
    {
        _afterImgPool.InitObjectPool(afterImagePrefab);
        StartCoroutine(AfterImageGenerator());
    }

    public void Left()
    {
        transform.localScale = _leftScale;
        if (!Moving)
            GameManager.Instance.TryCharacterMove(GameManager.MoveDirection.L);
    }

    public void Right()
    {
        transform.localScale = _rightScale;
        if (!Moving)
            GameManager.Instance.TryCharacterMove(GameManager.MoveDirection.R);
    }

    public void Up()
    {
        if (!Moving)
            GameManager.Instance.TryCharacterMove(GameManager.MoveDirection.U);
    }

    public void Down()
    {
        if (!Moving)
            GameManager.Instance.TryCharacterMove(GameManager.MoveDirection.D);
    }
}
