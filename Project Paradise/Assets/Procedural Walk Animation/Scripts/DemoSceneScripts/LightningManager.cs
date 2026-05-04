using System.Collections;
using UnityEngine;

namespace LolopupkaAnimations2D
{
public class LightningManager : MonoBehaviour
{
    public float minInterval;
    public float maxInterval;
    private LightningPoint[] lightningPoints;

    private void Awake() 
    {
        lightningPoints = this.GetComponentsInChildren<LightningPoint>();
    }

    void Start()
    {
        StartCoroutine(PlayLightning());
    }

    private IEnumerator PlayLightning()
    {
        float waitTime = Random.Range(minInterval, maxInterval);
        yield return new WaitForSeconds(waitTime);
        LightningPoint lightningPoint = GetRandomLightningPoint();
        lightningPoint.StartCoroutine(lightningPoint.StartLightningSrike());
        StartCoroutine(PlayLightning());
    }

    private LightningPoint GetRandomLightningPoint()
    {
        LightningPoint result = null;

        while (result == null)
        {
            int index = Random.Range(0, lightningPoints.Length);
            if(!lightningPoints[index].isPlaying)
            {
                result = lightningPoints[index];
            }
        } 

        return result;
    }
}
}
