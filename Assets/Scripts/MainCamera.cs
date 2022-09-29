using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    [SerializeField]
    private float _cameraShakeDuration = 1f;
    [SerializeField]
    AnimationCurve curve;

    IEnumerator ShakeCameraRoutine()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        float strength = 0;

        while (elapsedTime < _cameraShakeDuration)
        {
            elapsedTime += Time.deltaTime;
            strength = curve.Evaluate(elapsedTime / _cameraShakeDuration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }

    public void ShakeCamera()
    {
        StartCoroutine(ShakeCameraRoutine());
    }

}
