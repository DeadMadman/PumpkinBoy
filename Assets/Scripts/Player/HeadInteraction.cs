using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;

public class HeadInteraction : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;
    private HeadMovement headMovement;
    [SerializeField] private GameObject collarParticles;
    [SerializeField] private bool headAttachedInStart = false;
    [SerializeField] GameObject head;
    [SerializeField] private Transform headAttachTransform;

    [SerializeField] private MMFeedbacks pickupFeedbacks;
    [SerializeField] private MMFeedbacks throwFeedbacks;

    [SerializeField, Tooltip("The time in seconds needed for maximum throw angle")]
    private float maxThrowDuration = 3f;

    [SerializeField, Tooltip("The time in seconds that the max angle is held before reversing")]
    private float holdDuration = 2f;

    [SerializeField, Tooltip("The maximum throw angle in degrees"), Range(0, 90)]
    private float maxThrowAngle = 85f;
    private float currentAngle = 0f;

    [SerializeField] private int force = 20;

    private Rigidbody headRigidBody;

    private float startThrowTime = 0;

    private bool isAngleIncreasing = true;

    private float timeWhenHeadThrown;
    private float pickupDelay = 1f;

    //rendering the line
    private bool isCurrentlyThrowing = false;
    private Vector3 throwDirection = Vector3.zero;
    private LineRenderer lineRenderer;
    private List<Vector3> arcPositions;

    private int holdingHeadHash;
    private int throwHash;
    private int pickupHash;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerController = GetComponent<PlayerController>();
        
        lineRenderer = GetComponent<LineRenderer>();
        arcPositions = new List<Vector3>();
        headRigidBody = head.GetComponent<Rigidbody>();

        //get headmovement to disable trail renderer
        headMovement = GetComponentInChildren<HeadMovement>();

        holdingHeadHash = Animator.StringToHash("isHoldingHead");
        throwHash = Animator.StringToHash("Throw");
        pickupHash = Animator.StringToHash("Pickup");

        if (!headAttachedInStart)
        {
            SetHeadDetached();
        }
        else
        {
            SetHeadAttached();
        }

    }
    private void Start()
    {
        if (HeadMovement.headAttached)
            animator.SetBool(holdingHeadHash, true);
        else
            animator.SetBool(holdingHeadHash, false);

        headMovement.DisableTrailRenderer();
    }

    public void RegisterInputs(SimpleControls controls)
    {
        controls.Gameplay.Throw.started += OnThrowHead;
        controls.Gameplay.Throw.canceled += OnThrowHead;
    }
    public void DeregisterInputs(SimpleControls controls)
    {
        controls.Gameplay.Throw.started -= OnThrowHead;
        controls.Gameplay.Throw.canceled -= OnThrowHead;
    }

    private void SetHeadAttached()
    {
        head.tag = "Untagged";
        animator.SetBool(holdingHeadHash, true);

        HeadMovement.headAttached = true;

        headRigidBody.useGravity = false;
        headRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        headRigidBody.velocity = Vector3.zero;
        headRigidBody.angularVelocity = Vector3.zero;

        head.transform.parent = transform;
        head.transform.position = headAttachTransform.position;
        head.transform.localRotation = Quaternion.identity;
        collarParticles.SetActive(false);
    }

    private void SetHeadDetached()
    {
        head.tag = "Head";
        HeadMovement.headAttached = false;
        headRigidBody.useGravity = true;
        headRigidBody.constraints = RigidbodyConstraints.None;
        head.transform.parent = null;
        collarParticles.SetActive(true);
    }

    public void PickupHead()
    {
        head.tag = "Untagged";
        playerController.RegisterHeadToBody();

        animator.SetBool(holdingHeadHash, true);
        animator.SetTrigger(pickupHash);
        animator.ResetTrigger(throwHash);

        SetHeadAttached();
        headMovement.DisableTrailRenderer();
        pickupFeedbacks?.PlayFeedbacks(transform.position);
    }

    private void Update()
    {
        if (isCurrentlyThrowing)
        {
            float currentTime = Time.time;
            float step;
            if (isAngleIncreasing)
            {
                step = Mathf.Lerp(0, 1, (currentTime - startThrowTime) / maxThrowDuration);
                if (step == 1 && currentTime - startThrowTime >= holdDuration + maxThrowDuration)
                {
                    startThrowTime = Time.time;
                    isAngleIncreasing = false;
                } 
            }
            else
            {
                step = Mathf.Lerp(1, 0, (currentTime - startThrowTime) / maxThrowDuration);
                if (step == 0)
                {
                    startThrowTime = Time.time;
                    isAngleIncreasing = true;
                }
                    
            }
            currentAngle = step * maxThrowAngle;

            throwDirection = Quaternion.AngleAxis(-currentAngle, transform.right) * transform.forward;
            DrawThrowLine();
        }
    }

    private void FixedUpdate()
    {
        if (playerController.isHoldingBox)
            return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask("Player"));
        float currentTime = Time.time;
        if (currentTime - timeWhenHeadThrown >= pickupDelay)
        {
            foreach (var collider in colliders)
            {
                if (collider.tag == "Head" && !HeadMovement.headAttached)
                {
                    PickupHead();
                    return;
                }
            }
        }
    }

    private void DrawThrowLine()
    {
        if (isCurrentlyThrowing)
        {
            /*
             * x = x0 + v0t*cos(theta)
             * y = y0 + v0t*sin(theta)-g*t*t
             */
            float v0 = force;
            float g = 9.81f;
            float tMax = 2 * v0 * Mathf.Sin(Mathf.Deg2Rad * currentAngle) / g;

            int nrOfPoints = 12;
            arcPositions.Clear();

            lineRenderer.positionCount = 1;
            arcPositions.Add(headAttachTransform.position);

            for (int i = 1; i < nrOfPoints*2; i++)
            {
                float t = (float)i / nrOfPoints * tMax;
                float x = 0;
                float y = v0 * t * Mathf.Sin(Mathf.Deg2Rad * currentAngle) - (0.5f * g * t * t);
                float z = v0 * t * Mathf.Cos(Mathf.Deg2Rad * currentAngle);

                Vector3 nextPoint = new Vector3(x, y, z);
                nextPoint = headAttachTransform.TransformPoint(nextPoint);

                Physics.Linecast(arcPositions[i-1], nextPoint, out RaycastHit hit, 1, QueryTriggerInteraction.Ignore);

                if (hit.collider)
                {
                    arcPositions.Add(hit.point);
                    break;
                }
                arcPositions.Add(nextPoint);
            }
            lineRenderer.positionCount = arcPositions.Count;
            for (int i = 0; i < arcPositions.Count; i++)
            {
                lineRenderer.SetPosition(i, arcPositions[i]);
            }
        }
        else
            lineRenderer.positionCount = 0;
    }

    private void OnThrowHead(InputAction.CallbackContext context)
    {
        //check if head is attached, early return otherwise
        if (!HeadMovement.headAttached)
            return;

        if (context.started)
        {
            startThrowTime = Time.time;
            isCurrentlyThrowing = true;
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, headAttachTransform.position);
        }
        else
        {
            if (!isCurrentlyThrowing)
                return;

            throwFeedbacks?.PlayFeedbacks(transform.position);
            animator.SetBool(holdingHeadHash, false);
            animator.SetTrigger(throwHash);
            animator.ResetTrigger(pickupHash);
            SetHeadDetached();

            headRigidBody.AddForce(throwDirection * force, ForceMode.Impulse);
            headRigidBody.AddTorque(transform.right * 50f);
            isCurrentlyThrowing = false;
            timeWhenHeadThrown = Time.time;
            lineRenderer.positionCount = 0;
            isAngleIncreasing = true;
        }
    }

    public void DropHead()
    {
        if (!HeadMovement.headAttached)
            return;

        animator.SetBool(holdingHeadHash, false);
        animator.ResetTrigger(pickupHash);

        SetHeadDetached();

        Vector3 dropDirection = Quaternion.AngleAxis(-45f, transform.right) * -transform.forward;
        headRigidBody.AddForce(dropDirection*5, ForceMode.Impulse);
        headRigidBody.AddTorque(transform.right * 10f);
        timeWhenHeadThrown = Time.time;
        lineRenderer.positionCount = 0;
    }
}
