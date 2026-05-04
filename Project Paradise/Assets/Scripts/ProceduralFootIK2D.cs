using UnityEngine;

public class ProceduralFootIK2D : MonoBehaviour
{
    [Header("IK Targets (racine de la scène)")]
    public Transform leftFootTarget;
    public Transform rightFootTarget;

    [Header("Pivot hanche (LeftLegUpper / RightLegUpper)")]
    public Transform leftHip;
    public Transform rightHip;

    [Header("Spread - écart latéral des pieds depuis le centre")]
    public float footSpreadX = 0.12f;

    [Header("Stride - décalage avant/arrière")]
    public float strideForward = 0.08f;
    public float strideBack    = 0.05f;

    [Header("Détection sol")]
    public LayerMask groundLayer;
    public float raycastDistance  = 0.8f;
    public float footHeightOffset = 0.02f;

    [Header("Pas")]
    public float stepTriggerDistance = 0.2f;
    public float stepHeight          = 0.2f;
    [Tooltip("Durée du pas à l'arrêt")]
    public float stepDurationMax     = 0.18f;
    [Tooltip("Durée du pas à vitesse maximale")]
    public float stepDurationMin     = 0.06f;
    [Tooltip("Vitesse max du player (= moveSpeed dans PlayerMovement)")]
    public float playerMaxSpeed      = 5f;
    public AnimationCurve stepCurve  = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Sécurité")]
    [Tooltip("Si un pied attend son tour depuis plus de X secondes, on débloque")]
    public float maxWaitTime = 0.4f;

    [Header("Body crouch")]
    public Transform bodyBone;
    public float bodyRestLocalY  = 0f;
    public float maxCrouchAmount = 0.08f;
    public float crouchSpeed     = 8f;

    [Header("Debug")]
    public bool showDebugLogs = false;

    private Rigidbody2D _rb;

    private Vector3 _leftPos,  _rightPos;
    private Vector3 _leftFrom, _leftTo;
    private Vector3 _rightFrom, _rightTo;
    private float   _leftT  = 1f;
    private float   _rightT = 1f;

    private bool _leftTurn = true;
    private int  _leftRole  =  1;
    private int  _rightRole = -1;

    // Temps d'attente de chaque pied
    private float _leftWait  = 0f;
    private float _rightWait = 0f;

    // Durée dynamique calculée chaque frame
    private float _currentStepDuration = 0.15f;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        if (bodyBone != null && bodyRestLocalY == 0f)
            bodyRestLocalY = bodyBone.localPosition.y;

        _leftPos  = GetGround(GetRayOrigin(true))  ?? leftFootTarget.position;
        _rightPos = GetGround(GetRayOrigin(false)) ?? rightFootTarget.position;

        _leftFrom  = _leftPos;  _leftTo  = _leftPos;
        _rightFrom = _rightPos; _rightTo = _rightPos;
    }

    void Update()
    {
        // Durée du pas adaptée à la vitesse courante
        float speed  = _rb != null ? Mathf.Abs(_rb.linearVelocity.x) : 0f;
        float speedT = Mathf.Clamp01(speed / Mathf.Max(playerMaxSpeed, 0.01f));
        _currentStepDuration = Mathf.Lerp(stepDurationMax, stepDurationMin, speedT);

        // Avance les pas en cours
        AdvanceStep(ref _leftT,  ref _leftPos,  _leftFrom,  _leftTo,  leftHip,  leftFootTarget);
        AdvanceStep(ref _rightT, ref _rightPos, _rightFrom, _rightTo, rightHip, rightFootTarget);

        // Accumule le temps d'attente des pieds posés
        if (_leftT  >= 1f) _leftWait  += Time.deltaTime;
        if (_rightT >= 1f) _rightWait += Time.deltaTime;

        // Tente de déclencher un pas
        TryTrigger(true);
        TryTrigger(false);

        UpdateCrouch();
    }

    void AdvanceStep(ref float t, ref Vector3 pos,
                     Vector3 from, Vector3 to,
                     Transform hip, Transform ikTarget)
    {
        if (t >= 1f) { pos = to; ikTarget.position = pos; return; }

        t += Time.deltaTime / Mathf.Max(_currentStepDuration, 0.01f);
        t  = Mathf.Clamp01(t);

        float eased = stepCurve.Evaluate(t);

        if (hip != null)
        {
            Vector3 pivot   = hip.position;
            Vector3 fromDir = from - pivot;
            Vector3 toDir   = to   - pivot;
            float   fromMag = fromDir.magnitude;
            float   toMag   = toDir.magnitude;

            if (fromMag > 0.001f && toMag > 0.001f)
            {
                Vector3 dir = Vector3.Slerp(fromDir.normalized, toDir.normalized, eased);
                float   mag = Mathf.Lerp(fromMag, toMag, eased);
                pos = pivot + dir * mag;
            }
            else
                pos = Vector3.Lerp(from, to, eased);
        }
        else
            pos = Vector3.Lerp(from, to, eased);

        // Y indépendant : arc Sin entre les deux hauteurs de sol
        // Le pied passe AU DESSUS du sol, jamais dedans
        float groundY = Mathf.Lerp(from.y, to.y, eased);
        pos.y = groundY + Mathf.Sin(eased * Mathf.PI) * stepHeight;

        ikTarget.position = pos;
    }

    void TryTrigger(bool isLeft)
    {
        float myT    = isLeft ? _leftT    : _rightT;
        float otherT = isLeft ? _rightT   : _leftT;

        // Ce pied est déjà en mouvement
        if (myT < 1f) return;
        // L'autre pied est en mouvement → attendre
        if (otherT < 1f) return;

        bool myTurn   = (isLeft == _leftTurn);
        float myWait  = isLeft ? _leftWait  : _rightWait;

        // Déblocage par timeout : si on attend trop longtemps, on force
        bool timedOut = myWait > maxWaitTime;

        if (!myTurn && !timedOut) return;

        Vector3  rayOrigin = GetRayOrigin(isLeft);
        Vector3? ground    = GetGround(rayOrigin);
        if (ground == null) return;

        Vector3 currentPos = isLeft ? _leftPos : _rightPos;
        float   dist       = Vector3.Distance(currentPos, ground.Value);

        if (dist < stepTriggerDistance) return;

        // Déclenche
        if (isLeft)
        {
            _leftFrom = _leftPos; _leftTo = ground.Value; _leftT = 0f;
            _leftWait = 0f;
        }
        else
        {
            _rightFrom = _rightPos; _rightTo = ground.Value; _rightT = 0f;
            _rightWait = 0f;
        }

        _leftRole  = -_leftRole;
        _rightRole = -_rightRole;
        _leftTurn  = !isLeft;

        if (showDebugLogs)
            Debug.Log($"[IK] STEP {(isLeft?"L":"R")} " +
                      $"{currentPos.x:F2}→{ground.Value.x:F2} " +
                      $"dist={dist:F2} dur={_currentStepDuration:F3}{(timedOut?" TIMEOUT":"")}");
    }

    Vector3 GetRayOrigin(bool isLeft)
    {
        Transform hip     = isLeft ? leftHip : rightHip;
        Vector3   hipPos  = hip != null ? hip.position : transform.position;

        // Prédit la position future du corps pour que le pied arrive au bon endroit
        // sans retard visible — on raycaste là où le corps SERA quand le pas finit
        Vector3 velocity  = _rb != null ? (Vector3)_rb.linearVelocity : Vector3.zero;
        Vector3 base_     = hipPos + velocity * _currentStepDuration;

        float facing   = GetFacingDir();
        float velX     = _rb != null ? _rb.linearVelocity.x : 0f;
        bool  isMoving = Mathf.Abs(velX) > 0.05f;

        // Spread toujours en world space absolu
        float spread = isLeft ? -footSpreadX : footSpreadX;

        float stride = 0f;
        if (isMoving)
        {
            int   role = isLeft ? _leftRole : _rightRole;
            float amt  = role > 0 ? strideForward : strideBack;
            stride = facing * role * amt;
        }

        return new Vector3(base_.x + spread + stride, base_.y, base_.z);
    }

    float GetFacingDir() => Mathf.Sign(transform.lossyScale.x);

    Vector3? GetGround(Vector3 from)
    {
        RaycastHit2D hit = Physics2D.Raycast(from, Vector2.down, raycastDistance, groundLayer);
        if (hit.collider != null)
            return (Vector3)hit.point + Vector3.up * footHeightOffset;
        return null;
    }

    void UpdateCrouch()
    {
        if (bodyBone == null) return;
        float span   = Mathf.Abs(_leftPos.x - _rightPos.x);
        float t      = Mathf.Clamp01(span / 0.4f);
        float crouch = t * t * maxCrouchAmount;
        Vector3 lp   = bodyBone.localPosition;
        lp.y = Mathf.Lerp(lp.y, bodyRestLocalY - crouch, Time.deltaTime * crouchSpeed);
        bodyBone.localPosition = lp;
    }

    void OnDrawGizmos()
    {
        DrawRayGizmo(true,  Color.green);
        DrawRayGizmo(false, Color.red);
        if (!Application.isPlaying) return;
        DrawArc(_leftFrom,  _leftTo,  leftHip,  Color.green);
        DrawArc(_rightFrom, _rightTo, rightHip, Color.red);
        Gizmos.color = Color.green; Gizmos.DrawWireSphere(_leftPos,  0.05f);
        Gizmos.color = Color.red;   Gizmos.DrawWireSphere(_rightPos, 0.05f);
    }

    void DrawRayGizmo(bool isLeft, Color c)
    {
        Vector3 o = Application.isPlaying ? GetRayOrigin(isLeft)
            : ((isLeft ? leftHip : rightHip) != null
                ? (isLeft ? leftHip.position : rightHip.position)
                : transform.position);
        RaycastHit2D hit = Physics2D.Raycast(o, Vector2.down, raycastDistance, groundLayer);
        Gizmos.color = c;
        if (hit.collider != null) { Gizmos.DrawLine(o, hit.point); Gizmos.DrawWireSphere(hit.point, 0.04f); }
        else { Gizmos.color = new Color(c.r,c.g,c.b,0.3f); Gizmos.DrawLine(o, o + Vector3.down * raycastDistance); }
    }

    void DrawArc(Vector3 from, Vector3 to, Transform hip, Color c)
    {
        if (hip == null) return;
        Vector3 pivot = hip.position;
        Vector3 fd = from - pivot, td = to - pivot;
        if (fd.magnitude < 0.001f || td.magnitude < 0.001f) return;
        Gizmos.color = new Color(c.r, c.g, c.b, 0.4f);
        Vector3 prev = from;
        for (int i = 1; i <= 10; i++)
        {
            float   tt  = i / 10f;
            Vector3 dir = Vector3.Slerp(fd.normalized, td.normalized, tt);
            float   mag = Mathf.Lerp(fd.magnitude, td.magnitude, tt);
            Vector3 p   = pivot + dir * mag;
            p.y += Mathf.Sin(tt * Mathf.PI) * stepHeight;
            Gizmos.DrawLine(prev, p); prev = p;
        }
    }
}