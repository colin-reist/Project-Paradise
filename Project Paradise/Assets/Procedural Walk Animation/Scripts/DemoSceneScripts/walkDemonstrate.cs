using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LolopupkaAnimations2D
{

public class walkDemonstrate : MonoBehaviour
{
    public float waitingTime;
    public float speed;
    public Transform[] targets;
    public UnityEvent reachTargetPointEvent;

    private int index = 0;
    private bool reachedTarget = false;

    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, targets[index].position, Time.deltaTime * speed);

        if(Vector3.Distance(transform.position, targets[index].position) <= .1f && !reachedTarget)
        {
            reachedTarget = true;
            StartCoroutine(Delay(waitingTime));
        }

    }

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        reachedTarget = false;
        index++;       
        reachTargetPointEvent.Invoke(); 
        if(index == targets.Length) index = 0;
    }
}

}
