using UnityEngine;
using System.Collections;

public class TremblingEffect : MonoBehaviour
{
    [SerializeField] private float force = 0.05f;
    [SerializeField] private float delayBetweenShakes = 3f;
    [SerializeField] private float shakeDuration = 0.2f;

    private Vector3 positionOriginale;

    void Start()
    {
        positionOriginale = transform.localPosition;
        StartCoroutine(TrembleRepeatedly());
    }

    private IEnumerator TrembleRepeatedly()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenShakes);
            yield return StartCoroutine(Tremble());
        }
    }

    private IEnumerator Tremble()
    {
        float elapsed = 0;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            float x = Random.Range(-force, force);
            float y = Random.Range(-force, force);

            transform.localPosition = positionOriginale + new Vector3(x, y, 0);

            yield return null;
        }

        transform.localPosition = positionOriginale;
    }
}
