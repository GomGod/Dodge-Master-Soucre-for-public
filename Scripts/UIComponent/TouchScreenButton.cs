using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class TouchScreenButton : MonoBehaviour
{
    private TextMeshProUGUI _textTmp;
    private static readonly int ColorProperty = Shader.PropertyToID("_FaceColor");
    private Color _color;

    // Start is called before the first frame update
    private void Awake()
    {
        _textTmp = GetComponent<TextMeshProUGUI>();
        _color = _textTmp.fontMaterial.GetColor(ColorProperty);
    }

    private void OnEnable()
    {
        var intensity = 1.0f;
        _textTmp.fontMaterial.SetColor(ColorProperty, _color * intensity);
        
        var seq = DOTween.Sequence();
        seq.SetAutoKill(false)
            .Append(DOTween.To(() => intensity, x =>
            {
                intensity = x;
                _textTmp.fontMaterial.SetColor(ColorProperty, _color * intensity);
            }, 1.2f, 2f))
            .Append(DOTween.To(() => intensity, x =>
            {
                intensity = x;
                _textTmp.fontMaterial.SetColor(ColorProperty, _color * intensity);
            }, 1.0f, 2f))
            .SetLoops(-1);
    }
}
