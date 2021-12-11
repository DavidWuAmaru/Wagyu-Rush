using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TransitionControl : MonoBehaviour
{
    [SerializeField] private Image transitionMask;
    [SerializeField] private Color targetColor;
    [SerializeField] private float duration;
    [SerializeField] private bool isClosing;

    // Start is called before the first frame update
    void Start()
    {
        Tween t = transitionMask.DOColor(targetColor, duration).SetEase(Ease.InOutSine);

        t.OnComplete(OnComplete);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnComplete()
    {
        if (!isClosing)
        {
            Destroy(this.gameObject);
        }
    }

    public float GetDuration()
    {
        return duration;
    }
}
