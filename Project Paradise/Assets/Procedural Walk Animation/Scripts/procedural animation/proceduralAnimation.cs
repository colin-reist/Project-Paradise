using System.Collections;
using System;
using UnityEngine;
using static UnityEngine.Mathf;
using System.Collections.Generic;
using UnityEngine.U2D.IK;
using UnityEditor;
using UnityEngine.Events;


namespace LolopupkaAnimations2D
{
    
public class proceduralAnimation : MonoBehaviour
{
    #region  Legs settings
    
    [Tooltip("The platformer character type uses raycasts to determine the foot positions underneath it, while the top-down character type moves IK targets to default positions and places them on nearby walls when available. Use the platformer type if your game view is from the side, and select the top-down type if your game features a top-down view.")]
    [SerializeField] private CharacterType characterType = CharacterType.platformer;
    [Tooltip("defines how high in units the leg will move during the step")]
    [SerializeField] private float stepHeight = 1f;
    [Tooltip("defines speed of the leg movement during the step")]
    [SerializeField] private float stepSpeed = 2f;
    
    [Tooltip("defines how far forward will the leg move during the step")]
    [SerializeField] private float stepForwardOffset = 0;
    [Tooltip("This defines the overall speed of the animation. By default, each leg takes a step every one second. If you set this parameter to 2, each leg will take a step every half second.")]
    [Range(0, 10)] [SerializeField] private float animationSpeed = 1;
    [Tooltip("A time in seconds that every foot spends on the ground after a step (by default a leg will make a step wait one second and make another one)")]
    [Range(0, 10)] [SerializeField] private float stepsInterval = 1;
    [Tooltip("If you want specific timings for your animation, use this option. Each slider represents an offset in seconds for taking a step. For example, setting the value to 0.5 means that the leg will take a step only half a second after the start of the animation.")]
    [SerializeField] private bool customStepTimings;
    [DynamicRangeAttribute("stepsInterval")]
    [SerializeField] private float[] manualStepTimings;
    [Tooltip("layers that character will detect as ground")]
    [SerializeField] private LayerMask layerMask = 1;
    [SerializeField] private AnimationCurve verticalLegMovement = new AnimationCurve(new Keyframe(0, 0, 0, 2.5f), new Keyframe(0.5f, 1), new Keyframe(1, 0, -2.5f, 0));
    [Tooltip("represents the speed of each leg animation by default the legs moves slowly at first than fast and then slowly again towards the end of the animation")]
    [SerializeField] private AnimationCurve easingFunction = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private List<Transform> legsIkTargets;
    [Tooltip("the minimum distance that the character have to move before each leg will take a step")]
    [SerializeField][Range(0.01f, 1f)] private float minDistanceToMakeStep = 0.1f;
    
    [Tooltip("If the distance between the leg and its default position exceeds this value, the leg will take a step immediately, ignoring the step timings. This can be useful if your character teleports somewhere, as all the legs will instantly move to the new position rather than one by one.")]
    [SerializeField] private float maxDistanceToMakeStep = 10f;
    [SerializeField] private DebugVisualisation debugVisualisation;
    [SerializeField] float groundCheckOffset = 3;
    [SerializeField] float groundCheckRange = 6;
    [SerializeField] float groundCheckRadius = 1;

    [SerializeField] private bool playFootstepSound = false;
    [SerializeField] private bool spawnParticles = false;
    [SerializeField] private GameObject stepVFX;
    [SerializeField] private AudioClip[] stepSVX;
    [SerializeField] private UnityEvent OnStepFinishedUnityEvent;

    public enum DebugVisualisation {dontShow, showForOneLeg, showForAllLegs};

    public enum GroundCheckMethod {fast, accurate}

    public EventHandler<Vector3> OnStepFinished;

    public enum CharacterType {platformer, topDown}

    private Vector3[] lastLegPositions;
    private Vector3[] defaultLegPositions;
    private Vector3[] targetStepPositions;
    private Vector3 velocity;
    private Vector3 smoothedVelocity;
    private Vector3 lastVelocity;
    private Vector3 lastPosition;

    private float[] footTimings;
    private float[] arcHeitMultiply;
    private float[] bodyStepOffsetY;
    
    private int nbLegs;
    
    private bool[] isLegMoving;

    #endregion

    #region  body settings
    [SerializeField] private Transform body;
    [SerializeField] private Vector2 bodyOffset = new Vector2(0, 1);
    [Tooltip("With the value set to 0, the body will instantly move to the average leg positions. With the value set to 1, the body will move very smoothly.")]
    [Range(0, 1f)] [SerializeField] private float bodyPositionSmoothing = .5f;
    [Tooltip("How much legs vertical movement affect body position")]
    [Range(0, 5f)][SerializeField] private float legsImpact = 1f;
    [Tooltip("The first option calculates the body position based on the average leg positions, which is great for more dynamic animations. For example, insects or robots are a great fit for this option. The second option will smoothly interpolate towards the root transform of the model, as represented by the graph below that appears when you chose it. The blue line on the graph represents the movement of the body when you move the character.")] 
    [SerializeField] private BodyPositionTarget bodyPositionTarget = BodyPositionTarget.averageLegPosition;
    [Tooltip("The first option is great for more uneven terrains because it rotates the whole model based on the leg positions. This also allows the model to walk upside down since the raycast direction for checking the ground will also change. The second option rotates only the body bone, which is ideal if you want your character to move more evenly, as this won't change the raycast direction for ground detection. The feet will always choose target step points right under them.")]
    [SerializeField] private BodyRotationTarget bodyRotationTarget = BodyRotationTarget.rootTransform;
    
    [Range(0.1f, 5f)] [SerializeField] private float frequency = 2f;
    [Range(0.1f, 2f)] [SerializeField] private float damping = 0.5f;
    [Range(-5f, 5f)] [SerializeField] private float response = -1f;

    [Tooltip("rotates body based on the average leg positions")]
    [SerializeField] private bool bodyOrientation;
    
    [Tooltip("With the value set to 0, the body will instantly rotate to match the average leg positions. With the value set to 1, the body will rotate very smoothly.")]
    [Range(0, 1)] [SerializeField] private float bodyRotationSmoothing = .2f;

    [Tooltip("Regular means the body will rotate towards the direction of movement, while Inverse means it will rotate in the opposite direction.")]
    [SerializeField] private TiltDirection tiltDirection = TiltDirection.SameAsMovingDirection;
    [Tooltip("slightly rotates the body towards or away from moving direction")]
    [SerializeField] private bool bodyTilt = true;
    
    [Tooltip("how much in degrees the body will rotate while moving")]
    [Range(.01f, 30f)] [SerializeField] private float maxTilt = 5;
    [Range(.01f, 10)] [SerializeField] private float tiltSpeed = 5f; 

    private enum TiltDirection {SameAsMovingDirection, InvertedMovingDirection}
    private enum BodyRotationTarget {rootTransform, rootBone}
    public enum BodyPositionTarget {averageLegPosition, rootTransform, none}
    private int tiltRotationMultiplyer => tiltDirection == TiltDirection.SameAsMovingDirection ? -1:1;

    public int[] linkedLegsId;
    public int currentInspectorTab = 1;

    private Quaternion lastBodyUp;
    private Vector3 lastBodyPosition;
    private Quaternion lastBodyRotation;
    
    private Vector3 xp;
    private Vector3 y, yd;

    private float k1 => damping / (PI * frequency);
    private float k2 => 1 / ((2 * PI * frequency) * (2 * PI * frequency));
    private float k3 => response * damping / (2 * PI * frequency);
    #endregion

    private AudioSource audioSource;
    private IKManager2D IKManager;

    private void Awake()
    {
        if(!TryGetComponent<AudioSource>(out audioSource) && playFootstepSound)
        {
            Debug.LogError("You enabled footsteps sounds option in the step effects tab but didn't add an audio source to this object for it to play the sounds");
        }

        #region Start legs settings
        nbLegs = legsIkTargets.Count;

        defaultLegPositions = new Vector3[nbLegs];
        lastLegPositions = new Vector3[nbLegs];
        targetStepPositions = new Vector3[nbLegs];
        
        footTimings = new float[nbLegs];
        arcHeitMultiply = new float[nbLegs];
        bodyStepOffsetY = new float[nbLegs];
        isLegMoving = new bool[nbLegs];

        for (int i = 0; i < nbLegs; ++i)
        {
            if(customStepTimings)
            {
                footTimings[i] = manualStepTimings[i];
            }
        }
        #endregion
    }

    private void Start() 
    {
        footTimings = GetStepTimingsOffset();
        SecondOrderSystem(body.position);

        for (int i = 0; i < nbLegs; i++)
        {
            lastLegPositions[i] = legsIkTargets[i].position;
            defaultLegPositions[i] = transform.InverseTransformPoint(legsIkTargets[i].position);  

            lastBodyPosition = body.position;
            lastBodyUp = body.localRotation;
            lastBodyRotation = body.rotation; 
        }


        if(bodyRotationTarget == BodyRotationTarget.rootBone)
        {
            bodyTilt = false;
        }

    }

    private void Update() 
    {   
        calculateVelocity();
        GlueLegsToGround();
        HandleOrientation();
        TiltBody();
        HandleBodyPosition(characterType);
    }

    private void FixedUpdate()
    {
        CalculateStepTimings();
    }

    
    #region legs logic
    private void CalculateStepTimings()
    {
        for (int i = 0; i < nbLegs; ++i)
        {
            //move legs when everytime footTimings reache limit
            footTimings[i] += Time.deltaTime * animationSpeed;

            float dist = GetDistanceToTarget(i);

            if(footTimings[i] >= stepsInterval) 
            {
                footTimings[i] = 0;

                if(dist >= minDistanceToMakeStep && !isLegMoving[i])
                {   
                    StartCoroutine(MoveLeg(i));
                }
            }
            else if (dist >= maxDistanceToMakeStep && !isLegMoving[i])
            {
                StartCoroutine(MoveLeg(i));
            }
        }
    }

    private void calculateVelocity()
    {
        float smoothFactor = 0.1f;
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        smoothedVelocity = Vector3.Lerp(lastVelocity, velocity, smoothFactor);
        lastVelocity = velocity;
        lastPosition = transform.position;
    }

    private void GlueLegsToGround()
    {
        for (int i = 0; i < nbLegs; ++i)
        {
            // fit legs to the ground
            if(isLegMoving[i]) continue;
            legsIkTargets[i].position = TargetPoint.FitToTheGround(lastLegPositions[i], this.transform, layerMask, groundCheckOffset, groundCheckRange, groundCheckRadius, characterType);
            lastLegPositions[i] = legsIkTargets[i].position;
        }
    }
    
    private IEnumerator MoveLeg(int index)
    {
        Vector3 startPosition = transform.InverseTransformPoint(lastLegPositions[index]);
        isLegMoving[index] = true;

        float distance = Vector3.Distance(legsIkTargets[index].position, transform.TransformPoint(defaultLegPositions[index]));

        arcHeitMultiply[index] = Remap(distance, minDistanceToMakeStep, stepHeight, minDistanceToMakeStep / 2, 1);

        float current = 0;
        bodyStepOffsetY[index] = 0;

        while(current < 1)
        {
            current += Time.deltaTime * stepSpeed;
        Vector3 v1 = transform.TransformPoint(
            defaultLegPositions[index]) + 
            GetClampedVelocity(stepForwardOffset, smoothedVelocity);
        targetStepPositions[index] = 
            TargetPoint.FitToTheGround(v1, this.transform, layerMask, groundCheckOffset, groundCheckRange, groundCheckRadius, characterType);

            bodyStepOffsetY[index] = verticalLegMovement.Evaluate(current) * stepHeight * arcHeitMultiply[index];

            legsIkTargets[index].localPosition = Vector3.Lerp(startPosition, 
            transform.InverseTransformPoint(targetStepPositions[index] + transform.up * Mathf.Clamp(bodyStepOffsetY[index], 0, stepHeight)),
            easingFunction.Evaluate(current));

            yield return null;
        }

        // snap the Ik target position to the target position to make sure it matches the target step position
        legsIkTargets[index].position = targetStepPositions[index];
        lastLegPositions[index] = legsIkTargets[index].position;
        isLegMoving[index] = false;

        InvokeOnStepFinishedEvents(targetStepPositions[index]);
    }

    #endregion

    private void InvokeOnStepFinishedEvents(Vector3 particlesSpawnPosition)
    {
        //events for a custom logic when a step is finished for example you can add camera shake everytime a character makes a step
        // first is a simple unity event that you can set up in the inspector and the second one is if you need more advanced custom logic handeled in some other script
        OnStepFinishedUnityEvent.Invoke();
        OnStepFinished?.Invoke(this, particlesSpawnPosition);

        if(playFootstepSound)
        {
            audioSource.PlayOneShot(stepSVX[UnityEngine.Random.Range(0, stepSVX.Length - 1)]);
        }
        if(spawnParticles)
        {
            Instantiate(stepVFX, particlesSpawnPosition, Quaternion.identity);
        }
    }
    
    private void HandleBodyPosition(CharacterType characterType)
    {
        switch (characterType)
        {
            case CharacterType.platformer:
            HandleBodyPositionPlatformer();
            break;
            
            case CharacterType.topDown:
            HandleBodyPositionTopDown();
            break;
        }
    }
    private void HandleBodyPositionPlatformer()
    {
        float bodyPositionSmoothingMultiplier = 5f;

        Vector3 bodyPositionOffset = new Vector3(bodyOffset.x, bodyOffset.y, 0);

        switch(bodyPositionTarget)
        {
            case BodyPositionTarget.averageLegPosition:
            //calculate center of mass and lerp body position towards it
            Vector3 targetPosition = GetAverageBodyPosition() + transform.up * GetAverageLegHeight() * legsImpact;

            Vector3 targetPositionLocalOffset = transform.InverseTransformPoint(targetPosition) + bodyPositionOffset;
            Vector3 newPosition = transform.TransformPoint(targetPositionLocalOffset);
            body.position = Vector3.Lerp(lastBodyPosition, newPosition, Time.deltaTime * (bodyPositionSmoothingMultiplier / bodyPositionSmoothing));
            break;

            case BodyPositionTarget.rootTransform:
            //interpolate body position towards its parent position smoothly using differential equations (this allows to use interesting effects such as moving a bit further than the target than moving back to it smoothly)
            Vector3 targetPosition1 = new Vector3(transform.position.x, GetAverageBodyPosition().y, transform.position.z);

            Vector3 targetPositionLocalOffset1 = body.InverseTransformPoint(targetPosition1 + transform.up * GetAverageLegHeight() * legsImpact) + bodyPositionOffset;
            Vector3 newPosition1 = body.TransformPoint(targetPositionLocalOffset1);
            body.position = UpdatePos(Time.deltaTime, newPosition1, Vector3.zero);
            break;

            case BodyPositionTarget.none:
            //literally doesnt do anything cuz its like "none" what else does it have to do lol :/
            break;
        }

        lastBodyPosition = body.position;
    }

    private void HandleBodyPositionTopDown()
    {
        //same as platformer logic
        float bodyPositionSmoothingMultiplier = 5f;
        Vector3 bodyPositionOffset = new Vector3(bodyOffset.x, bodyOffset.y, 0);

        switch(bodyPositionTarget)
        {
            case BodyPositionTarget.averageLegPosition:
            Vector3 targetPosition = GetAverageBodyPosition();
            Vector3 targetPositionLocalOffset = body.InverseTransformPoint(targetPosition) + bodyPositionOffset;
            Vector3 newPosition = body.TransformPoint(targetPositionLocalOffset);
            body.position = Vector3.Lerp(lastBodyPosition, newPosition, Time.deltaTime * (bodyPositionSmoothingMultiplier / bodyPositionSmoothing));
            break;

            case BodyPositionTarget.rootTransform:
            Vector3 targetPosition1 = transform.position;

            Vector3 targetPositionLocalOffset1 = body.InverseTransformPoint(targetPosition1) + bodyPositionOffset;
            Vector3 newPosition1 = body.TransformPoint(targetPositionLocalOffset1);
            body.position = UpdatePos(Time.deltaTime, newPosition1, Vector3.zero);
            break;
        }

        lastBodyPosition = body.position;
    }

    Quaternion BodyRotation(Vector3 approximateForward, Vector3 exactUp) 
    {
        Quaternion zToUp = Quaternion.LookRotation(exactUp, -approximateForward);
        Quaternion yToz = Quaternion.Euler(90, 0, 0);
        return zToUp * yToz;
    }

    private void TiltBody()
    {
        if(!bodyTilt) return;
        body.localRotation = Quaternion.Slerp(lastBodyUp, Quaternion.Euler(0, 0, GetTilAmmount()), Time.deltaTime * tiltSpeed);
        lastBodyUp = body.localRotation;
    }

    private void HandleOrientation()
    {
        if(!bodyOrientation) return;

        Vector3 averegeNormal = CalculateNormal(legsIkTargets, this.transform);
        float bodyRotationSpeedMultiplier = 5f;
        Quaternion targetRotation;

        switch(bodyRotationTarget)
        {
            case BodyRotationTarget.rootBone:
            // only rotates the root bone which does not change raycast direction there fore the character wont be able to walk on the walls
                targetRotation = Quaternion.Slerp(lastBodyRotation, BodyRotation(body.forward, averegeNormal), Time.deltaTime * (bodyRotationSpeedMultiplier / bodyRotationSmoothing));

                body.rotation = targetRotation;
                lastBodyRotation = body.rotation;
            break;

            case BodyRotationTarget.rootTransform:
            // rotates the whole character and wiill detect ground based on its orientation 
                targetRotation = Quaternion.Slerp(lastBodyRotation, BodyRotation(transform.forward, averegeNormal), Time.deltaTime * (bodyRotationSpeedMultiplier / bodyRotationSmoothing));
        
                transform.rotation = targetRotation;
                lastBodyRotation = transform.rotation;
            break;

        }
    }

    private float GetTilAmmount()
    {
        Vector3 tiltVector = smoothedVelocity.normalized * maxTilt;
        if(tiltVector.x != 0)
        {
            return tiltVector.x * tiltRotationMultiplyer;
        }
        else
        {
            return tiltVector.y * tiltRotationMultiplyer;
        }
    }
    public Vector2 CalculateNormal(List<Transform> transforms, Transform parentTransform)
    {
        // Calculate the points based on the positions of the transforms
        List<Vector2> points = new List<Vector2>();
        foreach (Transform transform in transforms)
        {
            points.Add((Vector2)transform.position);
        }

        // Calculate the centroid of the points
        Vector2 centroid = Vector2.zero;
        foreach (Vector2 point in points)
        {
            centroid += point;
        }
        centroid /= points.Count;

        // Calculate the covariance matrix components
        float xx = 0.0f, xy = 0.0f, yy = 0.0f;
        foreach (Vector2 point in points)
        {
            Vector2 r = point - centroid;
            xx += r.x * r.x;
            xy += r.x * r.y;
            yy += r.y * r.y;
        }

        // Calculate the normal vector based on the covariance matrix
        float detX = yy;
        float detY = xx;

        Vector2 normal;
        if (detX > detY)
        {
            normal = new Vector2(detX, -xy);
        }
        else
        {
            normal = new Vector2(-xy, detY);
        }
        normal = normal.normalized;

        // Check if the normal is pointing in the same direction as the parentTransform's up vector If not, flip the normal vector by 180 degrees to maintain correct orientation
        if (Vector2.Dot(normal, parentTransform.up) < 0)
        {
            normal = -normal;
        }

        return normal;
    }

    public void SecondOrderSystem(Vector3 x0)
    {
        xp = x0;
        y = x0;
        yd = Vector3.zero;
    }

    public Vector3 UpdatePos(float T, Vector3 x, Vector3 xd) 
    {
        if(xd == Vector3.zero)
        {
            xd = (x - xp) / T;
            xp = x;
        }
        float k2_stable = Max(k2, 1.1f * (T*T/4 + T*k1/2));
        y = y + T * yd;
        yd = yd + T * (x + k3*xd - y - k1*yd) / k2_stable;

        return y; 
    }

    private Vector3 GetAverageBodyPosition()
    {
        Vector3 pos = new Vector3();

        for (int i = 0; i < nbLegs; ++i)
        {
            pos += legsIkTargets[i].position;
        }

        pos /= nbLegs;
        return pos;
    }
    private float GetDistanceToTarget(int index)
    {
        Vector3 origin = transform.TransformPoint(defaultLegPositions[index]);

        Vector3 target = TargetPoint.FitToTheGround(origin, this.transform, layerMask, groundCheckOffset, groundCheckRange, groundCheckRadius, characterType);

        return Vector3.Distance(legsIkTargets[index].position, target);
    }

    public float GetDistanceToGround(int index)
    {
        Vector3 leg = legsIkTargets[index].position;
        Ray ray = new Ray(leg + transform.up * .1f, -transform.up);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, layerMask))
        {
            return Vector3.Distance(leg, hit.point);
        }
        else 
        {
            return 0f;
        }
    }

    public float GetAverageLegHeight()
    {
        //to calculate body position and rase it when leg is moving based on legs distance to ground
        float averageHeight = 0;
        for (int i = 0; i < nbLegs; ++i)
        {
            averageHeight += bodyStepOffsetY[i];
        }

        averageHeight /= nbLegs;

        return averageHeight;
    }
    public float[] GetStepTimingsOffset()
    {
        float[] timings = new float[legsIkTargets.Count];
        if(customStepTimings)
        {
            for (int i = 0; i < nbLegs; i++)
            {
                timings[i] = manualStepTimings[i];
            }   
            return timings;
        } 


        float counter = 1;
        for (int i = 0; i < nbLegs; i++)
        {
           if(i != 0)
           {
                if(linkedLegsId[i] != linkedLegsId[i - 1] || linkedLegsId[i] == 0)
                {
                    counter++;
                }
           }
        }

        float step = stepsInterval / counter;
        float multiplyIndex = 1;

        for (int i = 0; i < nbLegs; i++)
        {
            if(i != 0)
            {
                if(linkedLegsId[i] != linkedLegsId[i - 1] || linkedLegsId[i] == 0)
                {
                    timings[i] = multiplyIndex * step;
                    multiplyIndex++;
                }
                else
                {
                    timings[i] = timings[i - 1];
                }
            }
        } 
        return timings;
    }    

    private Vector3 GetClampedVelocity(float maxLength, Vector3 velocity)
    {
        if(velocity.magnitude > maxLength)
        {
            return velocity.normalized * maxLength;
        }
        else if (velocity.magnitude < .1f) 
        {
            return Vector3.zero;
        }
        else return velocity;
    }

    public int GetLegCount()
    {
        IKManager = GetComponent<IKManager2D>();
        nbLegs = IKManager.solvers.Count;
        return nbLegs;
    }

    public IKManager2D GetIkManager()
    {
        IKManager = GetComponent<IKManager2D>();
        return IKManager;
    }

    public Vector2 GetBodyOffset()
    {
        return transform.InverseTransformPoint(body.position);
    }

    private float Remap(float input, float oldLow, float oldHigh, float newLow, float newHigh) 
    {
        float t = Mathf.InverseLerp(oldLow, oldHigh, input);
        return Mathf.Lerp(newLow, newHigh, t);
    }

    private void OnDrawGizmosSelected()
    {
        #if UNITY_EDITOR

        TryGetComponent<IKManager2D>(out IKManager);
        if(IKManager == null || IKManager.solvers.Count == 0) return;

        // show leg names for easier linking and experementing with walk types 
        for (int i = 0; i < IKManager.solvers.Count; i++)
        {
            // Set up the GUIStyle for the leg Ik targets names 
            int fontSize = 20;
            Color textColor = Color.cyan;
            GUIStyle style = new GUIStyle();
            style.fontSize = fontSize;
            style.normal.textColor = textColor;
            style.alignment = TextAnchor.MiddleCenter;
            
            int labelOffset = i%2 == 0 ? -1 : 1;
            Vector3 IkTargetPosition = IKManager.solvers[i].GetChain(i).target.position;
            Vector3 textPosition = IkTargetPosition - transform.up * .3f;
            //Handles.DrawLine(textPosition + transform.up * .05f, IkTargetPosition);
            //Handles.Label(textPosition, i.ToString(), style);
        }

        switch (debugVisualisation)
        {
        case DebugVisualisation.dontShow:
            // literally does nothing cuz it says dont show lol 
            break;
        case DebugVisualisation.showForOneLeg:

        Vector3 gizmozOriginOneLeg = new Vector3();

        if(!Application.isPlaying)
        {
            gizmozOriginOneLeg = legsIkTargets[0].transform.position;
        }
        else
        {
            gizmozOriginOneLeg = transform.TransformPoint(defaultLegPositions[0]);
        }

        Vector3 dir = gizmozOriginOneLeg - gizmozOriginOneLeg + transform.up * groundCheckOffset - transform.up * groundCheckRange;

        float dist = Vector3.Dot(dir, transform.up);
        Gizmos.DrawRay(gizmozOriginOneLeg + transform.up * groundCheckOffset - transform.right * groundCheckRadius / 2, transform.right * groundCheckRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(gizmozOriginOneLeg + transform.up * groundCheckOffset - transform.up * groundCheckRange - transform.right * groundCheckRadius / 2, transform.right * groundCheckRadius);

        if(dist >= 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(gizmozOriginOneLeg + transform.up * groundCheckOffset, -transform.up * groundCheckRange);
            Gizmos.DrawRay(gizmozOriginOneLeg + transform.up * groundCheckOffset -transform.up * groundCheckRange, Quaternion.Euler(0, 0, 45) * transform.up * groundCheckRadius / 2 * Mathf.Sqrt(2));
            Gizmos.DrawRay(gizmozOriginOneLeg + transform.up * groundCheckOffset -transform.up * groundCheckRange, Quaternion.Euler(0, 0, -45) * transform.up * groundCheckRadius / 2 * Mathf.Sqrt(2));

            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.TransformPoint(gizmozOriginOneLeg) + transform.up * groundCheckOffset + transform.right * groundCheckRadius / 2, -transform.up * groundCheckRange);
            Gizmos.DrawRay(transform.TransformPoint(gizmozOriginOneLeg) + transform.up * groundCheckOffset - transform.right * groundCheckRadius / 2, -transform.up * groundCheckRange);
        }
        else
        {
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gizmozOriginOneLeg + transform.up * groundCheckOffset, gizmozOriginOneLeg);
            Gizmos.DrawRay(gizmozOriginOneLeg, Quaternion.Euler(0, 0, 45) * transform.up * groundCheckRadius / 2 * Mathf.Sqrt(2));
            Gizmos.DrawRay(gizmozOriginOneLeg, Quaternion.Euler(0, 0, -45) * transform.up * groundCheckRadius / 2 * Mathf.Sqrt(2));

            Gizmos.color = Color.white;
            Gizmos.DrawRay(gizmozOriginOneLeg - transform.right * groundCheckRadius / 2, transform.right * groundCheckRadius);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(gizmozOriginOneLeg + transform.up * groundCheckOffset + transform.right * groundCheckRadius / 2, gizmozOriginOneLeg + transform.right * groundCheckRadius / 2);
            Gizmos.DrawLine(gizmozOriginOneLeg + transform.up * groundCheckOffset - transform.right * groundCheckRadius / 2, gizmozOriginOneLeg - transform.right * groundCheckRadius / 2);

            //dotted lines for representing how far down leg can move
            Gizmos.color = Color.green;
            Vector3 start = gizmozOriginOneLeg;
            Vector3 end = gizmozOriginOneLeg + transform.up * groundCheckOffset - transform.up * groundCheckRange;
            float spacing = .2f;
            float size = .1f;

            Vector3 direction = -transform.up;
            float distance = Vector2.Distance(start, end);

            // Draw the dots along the line
            for (float i = 0; i < distance; i += spacing)
            {
                if(i + spacing > distance)
                {
                    Gizmos.DrawLine(start + direction * i, gizmozOriginOneLeg + transform.up * groundCheckOffset - transform.up * groundCheckRange);
                    Gizmos.DrawLine(start + direction * i + transform.right * groundCheckRadius / 2, gizmozOriginOneLeg + transform.right * groundCheckRadius / 2 + transform.up * groundCheckOffset - transform.up * groundCheckRange);
                    Gizmos.DrawLine(start + direction * i - transform.right * groundCheckRadius / 2, gizmozOriginOneLeg - transform.right * groundCheckRadius / 2 + transform.up * groundCheckOffset - transform.up * groundCheckRange);
                    return;
                }
                Vector3 dotPosition = start + direction * i;
                Gizmos.DrawLine(dotPosition, dotPosition + direction * size);
                Gizmos.DrawLine(dotPosition + transform.right * groundCheckRadius / 2, dotPosition + transform.right * groundCheckRadius / 2 + direction * size);
                Gizmos.DrawLine(dotPosition - transform.right * groundCheckRadius / 2, dotPosition - transform.right * groundCheckRadius / 2 + direction * size);
            }
        }
            break;
        case DebugVisualisation.showForAllLegs:
        
        for (int i = 0; i < legsIkTargets.Count; ++i)
        { 

        Vector3 gizmozOriginAllLegs = new Vector3();

        if(!Application.isPlaying)
        {
            gizmozOriginAllLegs = legsIkTargets[i].transform.position;
        }
        else
        {
            gizmozOriginAllLegs = transform.TransformPoint(defaultLegPositions[i]);
        }
            
        Vector3 dir1 = gizmozOriginAllLegs - gizmozOriginAllLegs + transform.up * groundCheckOffset - transform.up * groundCheckRange;

        float dist1 = Vector3.Dot(dir1, transform.up);

        Gizmos.color = Color.white;
        Gizmos.DrawRay(gizmozOriginAllLegs + transform.up * groundCheckOffset - transform.right * groundCheckRadius / 2, transform.right * groundCheckRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(gizmozOriginAllLegs + transform.up * groundCheckOffset - transform.up * groundCheckRange - transform.right * groundCheckRadius / 2, transform.right * groundCheckRadius);

        if(dist1 >= 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(gizmozOriginAllLegs + transform.up * groundCheckOffset, -transform.up * groundCheckRange);
            Gizmos.DrawRay(gizmozOriginAllLegs + transform.up * groundCheckOffset -transform.up * groundCheckRange, Quaternion.Euler(0, 0, 45) * transform.up * groundCheckRadius / 2 * Mathf.Sqrt(2));
            Gizmos.DrawRay(gizmozOriginAllLegs + transform.up * groundCheckOffset -transform.up * groundCheckRange, Quaternion.Euler(0, 0, -45) * transform.up * groundCheckRadius / 2 * Mathf.Sqrt(2));

            Gizmos.color = Color.white;
            Gizmos.DrawRay(gizmozOriginAllLegs + transform.up * groundCheckOffset + transform.right * groundCheckRadius / 2, -transform.up * groundCheckRange);
            Gizmos.DrawRay(gizmozOriginAllLegs + transform.up * groundCheckOffset - transform.right * groundCheckRadius / 2, -transform.up * groundCheckRange); 
        }
        else
        {
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(gizmozOriginAllLegs + transform.up * groundCheckOffset, gizmozOriginAllLegs);
            Gizmos.DrawRay(gizmozOriginAllLegs, Quaternion.Euler(0, 0, 45) * transform.up * groundCheckRadius / 2 * Mathf.Sqrt(2));
            Gizmos.DrawRay(gizmozOriginAllLegs, Quaternion.Euler(0, 0, -45) * transform.up * groundCheckRadius / 2 * Mathf.Sqrt(2));

            Gizmos.color = Color.white;
            Gizmos.DrawRay(gizmozOriginAllLegs - transform.right * groundCheckRadius / 2, transform.right * groundCheckRadius);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(gizmozOriginAllLegs + transform.up * groundCheckOffset + transform.right * groundCheckRadius / 2, gizmozOriginAllLegs + transform.right * groundCheckRadius / 2);
            Gizmos.DrawLine(gizmozOriginAllLegs + transform.up * groundCheckOffset - transform.right * groundCheckRadius / 2, gizmozOriginAllLegs - transform.right * groundCheckRadius / 2);
            //dotted lines for representing how far down a leg can move
            Gizmos.color = Color.green;
            Vector3 start = gizmozOriginAllLegs;
            Vector3 end = gizmozOriginAllLegs + transform.up * groundCheckOffset - transform.up * groundCheckRange;
            float spacing = .2f;
            float size = .1f;
            Vector3 direction = -transform.up;
            float distance = Vector2.Distance(start, end);

            for (float j = 0; j < distance; j += spacing)
            {
                if(j + spacing > distance)
                {
                    Gizmos.DrawLine(start + direction * j, gizmozOriginAllLegs + transform.up * groundCheckOffset - transform.up * groundCheckRange);
                    Gizmos.DrawLine(start + direction * j + transform.right * groundCheckRadius / 2, gizmozOriginAllLegs + transform.right * groundCheckRadius / 2 + transform.up * groundCheckOffset - transform.up * groundCheckRange);
                    Gizmos.DrawLine(start + direction * j - transform.right * groundCheckRadius / 2, gizmozOriginAllLegs - transform.right * groundCheckRadius / 2 + transform.up * groundCheckOffset - transform.up * groundCheckRange);
                    continue;
                }
                Vector3 dotPosition = start + direction * j;
                Gizmos.DrawLine(dotPosition, dotPosition + direction * size);
                Gizmos.DrawLine(dotPosition + transform.right * groundCheckRadius / 2, dotPosition + transform.right * groundCheckRadius / 2 + direction * size);
                Gizmos.DrawLine(dotPosition - transform.right * groundCheckRadius / 2, dotPosition - transform.right * groundCheckRadius / 2 + direction * size);
            }
        }
        }
            break;
        }
        
        # endif
    }
}
}
