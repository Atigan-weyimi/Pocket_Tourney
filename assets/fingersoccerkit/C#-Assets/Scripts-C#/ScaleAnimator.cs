using System;
using System.Collections;
using UnityEngine;

public class ScaleAnimator : MonoBehaviour
{
	/// *************************************************************************///
	/// This function animates the halo object around units when they have the trun.
	/// *************************************************************************///
	private readonly float intensity = 1.2f; //scale ratio

    private readonly float animSpeed = 1.5f; //scale animation speed

    //animation
    private float startScaleX;
    private float startScaleY;
    private float endScaleX;
    private float endScaleY;

    private float _time;

    private void Awake()
    {
        startScaleX = transform.localScale.x;
        startScaleY = transform.localScale.y;
        endScaleX = startScaleX * intensity;
        endScaleY = startScaleY * intensity;
    }

    private void OnEnable()
    {
        _time = 0f;
        transform.localScale = new Vector3(startScaleX, startScaleY, transform.localScale.z);
    }

    private void Update()
    {
        var scale = (Mathf.Cos(_time * Mathf.PI * animSpeed + Mathf.PI) + 1f) * 0.5f;
        transform.localScale = new Vector3(
            Mathf.Lerp(startScaleX, endScaleX, scale),
            Mathf.Lerp(startScaleY, endScaleY, scale), 
            transform.localScale.z);

        _time += Time.deltaTime;
    }
}