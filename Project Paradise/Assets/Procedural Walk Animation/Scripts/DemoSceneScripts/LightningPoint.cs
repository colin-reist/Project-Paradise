using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;


namespace LolopupkaAnimations2D
{
public class LightningPoint : MonoBehaviour
{
    public float litUpIntensity;
    public float fadeSpeed;
    public bool isPlaying;
    private Light2D light2D;

    private void Awake() 
    {
        light2D = GetComponent<Light2D>();
    }

    public IEnumerator StartLightningSrike()
    {
        isPlaying = true;

        light2D.intensity = litUpIntensity;

        while (light2D.intensity > 0)
        {
            yield return null;
            float newIntensity = Mathf.Lerp(light2D.intensity, 0, fadeSpeed * Time.deltaTime);

            if(newIntensity < .1f)
            {
                newIntensity = 0;
            }

            light2D.intensity = newIntensity;
        }

        isPlaying = false;
    }
}
}
