using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField] private SpriteRenderer characterSprite;
    
    [SerializeField] public AudioSource deadSound;
    [SerializeField] public AudioSource dashSound;
    
    private readonly ObjectPool _afterImgPool = new();
    private static readonly int Fade = Shader.PropertyToID("_Fade");

    public bool disabled = false;
    
    public bool Moving
    {
        get => _moving;
        set
        {
            if(value && _moving != true)
                dashSound.Play();
            
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

    private Sequence characterSeq;
    
    public void PlayDeadFX()
    {
        var fadeValue = 1f;
        characterSprite.material.SetFloat(Fade, 1f);

        characterSeq = DOTween.Sequence();
        characterSeq.Append(DOTween.To(() => fadeValue, x =>
            {
                fadeValue = x;
                characterSprite.material.SetFloat(Fade, x);
            }, 0f, 0.66f));
        deadSound.Play();
        disabled = true;
    }

    public void ResetCharacter()
    {
        characterSeq?.Kill();
        characterSprite.material.SetFloat(Fade, 1f);
        disabled = false;
    }
    
    private void Start()
    {
        _afterImgPool.InitObjectPool(afterImagePrefab);
        StartCoroutine(AfterImageGenerator());
    }

    public void Left()
    {
        inputBuffer = Input.L; 
        _currentTimer = 0.0f;
    }

    public void Right()
    {
        inputBuffer = Input.R; 
        _currentTimer = 0.0f;
    }

    public void Up()
    {
        inputBuffer = Input.U; 
        _currentTimer = 0.0f;
    }

    public void Down()
    {
        inputBuffer = Input.D; 
        _currentTimer = 0.0f;
    }
    
    private enum Input {N,L,R,U,D}

    [SerializeField]private float inputRemain = 0.1f;
    [SerializeField]private Input inputBuffer = Input.N;
    private float _currentTimer = 0.0f;
    
    private void ProcessInput()
    {
        if (_moving || disabled)
            return;

        switch (inputBuffer)
        {
            case Input.N:
                break;
            case Input.L:
                transform.localScale = _leftScale;
                inputBuffer = Input.N;
                GameManager.Instance.TryCharacterMove(GameManager.MoveDirection.L);
                break;
            case Input.R:
                transform.localScale = _rightScale;
                inputBuffer = Input.N;
                GameManager.Instance.TryCharacterMove(GameManager.MoveDirection.R);
                break;
            case Input.U:
                inputBuffer = Input.N;
                GameManager.Instance.TryCharacterMove(GameManager.MoveDirection.U);
                break;
            case Input.D:
                inputBuffer = Input.N;
                GameManager.Instance.TryCharacterMove(GameManager.MoveDirection.D);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Update()
    {
        _currentTimer += Time.deltaTime;
        if (inputBuffer != Input.N && _currentTimer >= inputRemain)
        {
            inputBuffer = Input.N;
        }
        ProcessInput();
    }
}
