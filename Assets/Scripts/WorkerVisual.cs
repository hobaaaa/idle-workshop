using System.Collections;
using UnityEngine;

public class WorkerVisual : MonoBehaviour
{
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("Work")]
    [SerializeField] private float workDuration = 3f;

    [Header("Points")]
    [SerializeField] private Transform homePoint;

    private Vector3 homePosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        RefreshHomePosition();
        transform.position = homePosition;
        PlayIdle();
    }

    public void Configure(Transform newHomePoint)
    {
        homePoint = newHomePoint;
        RefreshHomePosition();
        transform.position = homePosition;
        PlayIdle();
    }

    public void Configure(Vector3 newHomePosition)
    {
        homePoint = null;
        homePosition = newHomePosition;
        transform.position = homePosition;
        PlayIdle();
    }

    public IEnumerator RunWorkCycle(Transform workPoint)
    {
        Vector3 targetWorkPosition = workPoint != null ? workPoint.position : new Vector3(-3.7f, -2.35f, -2f);

        yield return MoveRoutine(targetWorkPosition);

        PlayWorkLeft();
        yield return new WaitForSeconds(workDuration);

        RefreshHomePosition();
        yield return MoveRoutine(homePosition);

        PlayIdle();
    }

    public void PlayIdle()
    {
        if (animator != null)
        {
            animator.Play("Worker_Idle");
        }
    }

    public void PlayWalkRight()
    {
        if (animator != null)
        {
            animator.Play("Worker_Walk_Right");
        }
    }

    public void PlayWalkLeft()
    {
        if (animator != null)
        {
            animator.Play("Worker_Walk_Left");
        }
    }

    public void PlayWorkLeft()
    {
        if (animator != null)
        {
            animator.Play("Worker_Work_Left");
        }
    }

    private void RefreshHomePosition()
    {
        homePosition = homePoint != null ? homePoint.position : transform.position;
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        if (target.x < transform.position.x)
        {
            PlayWalkLeft();
        }
        else
        {
            PlayWalkRight();
        }

        while (Vector2.Distance(transform.position, target) > 0.02f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = target;
    }
}
