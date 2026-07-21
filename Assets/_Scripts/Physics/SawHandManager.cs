using UnityEngine;

/// <summary>
/// Keeps a Rigidbody attached to a surface while the saw button is held and
/// moves it in the direction captured when the attachment begins.
///
/// Requires: a Rigidbody on this GameObject and a Collider on the saw target.
/// 
/// Find parts that need fixing with "XXX" comments. The main missing piece is reading input for the saw button and movement direction.
/// </summary>

[RequireComponent(typeof(Rigidbody))]
public class SawHandManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Point at the edge of the circular saw. Defaults to this transform.")]
    [SerializeField] private Transform sawOrigin;
    [Tooltip("Reference used for Horizontal/Vertical movement. Defaults to this transform.")]
    [SerializeField] private Transform movementReference;

    [Header("Detection")]
    [SerializeField] private LayerMask sawableSurfaces = ~0;
    [SerializeField, Min(0.01f)] private float acquireDistance = 0.8f;
    [SerializeField, Min(0.01f)] private float maintainDistance = 0.25f;
    [SerializeField, Range(0f, 180f)] private float maxNormalChange = 35f;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float sawSpeed = 8f;
    [SerializeField, Min(0f)] private float adhesionStrength = 30f;
    [SerializeField, Min(0f)] private float releasePush = 1.5f;

    public bool IsAttached { get; private set; }

    private Rigidbody body;
    private bool buttonHeld;
    private bool gravityBeforeAttach;
    private Vector3 surfaceNormal;
    private Vector3 travelDirection;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        if (sawOrigin == null) sawOrigin = transform;
        if (movementReference == null) movementReference = transform;

    }

    private void Update()
    {
        //XXX  buttonHeld = NEEDS A WAY READ IF INPUT BUTTON IS HELD DOWN
    }

    private void FixedUpdate()
    {
        if (!buttonHeld)
        {
            Detach();
            return;
        }

        if (!IsAttached)
        {
            TryAttach();
            return;
        }

        MaintainAttachment();
    }

    private void TryAttach()
    {
        // The saw faces the surface it wants to bite into.
        if (!Physics.Raycast(sawOrigin.position, sawOrigin.forward, out RaycastHit hit,
                acquireDistance, sawableSurfaces, QueryTriggerInteraction.Ignore))
            return;

        surfaceNormal = hit.normal;
        // XXX travelDirection = GetInitialTravelDirection(surfaceNormal); GetInitialTravelDirection needs to read player input and project it onto the surface plane.
        if (travelDirection.sqrMagnitude < 0.001f)
            return;

        gravityBeforeAttach = body.useGravity;
        body.useGravity = false;
        IsAttached = true;
    }

    private void MaintainAttachment()
    {
        // Probe back toward the surface using the saved normal. This avoids
        // changing direction on small bumps and preserves straight movement.
        Vector3 probeStart = sawOrigin.position + surfaceNormal * maintainDistance;
        if (!Physics.Raycast(probeStart, -surfaceNormal, out RaycastHit hit,
                maintainDistance * 2f, sawableSurfaces, QueryTriggerInteraction.Ignore) ||
            Vector3.Angle(surfaceNormal, hit.normal) > maxNormalChange)
        {
            Detach();
            return;
        }

        Vector3 desiredSawPosition = hit.point + surfaceNormal * maintainDistance;
        float normalError = Vector3.Dot(desiredSawPosition - sawOrigin.position, surfaceNormal);

        // The normal component holds the saw against the wall; the tangent
        // component is deliberately fixed from the first frame of attachment.
        Vector3 targetVelocity = travelDirection * sawSpeed +
                                 surfaceNormal * (normalError * adhesionStrength);
        body.linearVelocity = targetVelocity;
    }

    //XXX This method needs to read the player's movement input and project it onto the surface plane.
    //private Vector3 GetInitialTravelDirection(Vector3 normal)
    //{
    //    XXX read inputs here
    //    Vector3 alongSurface = Vector3.ProjectOnPlane(/*input*/, normal);

    //    If no direction is held, continue in the current direction; if the
    //     player is stationary, use their forward direction as a fallback.
    //    if (alongSurface.sqrMagnitude < 0.001f)
    //        alongSurface = Vector3.ProjectOnPlane(body.linearVelocity, normal);
    //    if (alongSurface.sqrMagnitude < 0.001f)
    //        alongSurface = Vector3.ProjectOnPlane(transform.forward, normal);

    //    return alongSurface.normalized;
    //}

    public void Detach()
    {
        if (!IsAttached)
            return;

        IsAttached = false;
        body.useGravity = gravityBeforeAttach;
        body.linearVelocity += surfaceNormal * releasePush;
    }

    private void OnDrawGizmosSelected()
    {
        //visual stuff
        Transform origin = sawOrigin != null ? sawOrigin : transform;
        Gizmos.color = IsAttached ? Color.cyan : Color.yellow;
        Gizmos.DrawLine(origin.position, origin.position + origin.forward * acquireDistance);
    }
    public void Configure(Transform newSawOrigin)
    {
        if (newSawOrigin != null)
            sawOrigin = newSawOrigin;
    }
}
