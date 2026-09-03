using System.Collections;
using UnityEngine;

public class WorkerVisual : MonoBehaviour
{
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("Work")]
    [SerializeField] private float workDuration = 3f;

    private Vector3 idlePosition;
    private Vector3 workPosition = new Vector3(-3.7f, -2.25f, -2f);

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Worker'ın sahnedeki başlangıç konumunu kaydet.
        idlePosition = transform.position;

        StartCoroutine(WorkerRoutine());
    }

    public void PlayIdle()
    {
        animator.Play("Worker_Idle");
    }

    public void PlayWalkRight()
    {
        animator.Play("Worker_Walk_Right");
    }

    public void PlayWalkLeft()
    {
        animator.Play("Worker_Walk_Left");
    }

    public void PlayWorkLeft()
    {
        animator.Play("Worker_Work_Left");
    }

    private IEnumerator WorkerRoutine()
    {
        // Tezgâha git.
        yield return MoveRoutine(workPosition);

        // Çalış.
        PlayWorkLeft();
        yield return new WaitForSeconds(workDuration);

        // Başlangıç noktasına geri dön.
        yield return MoveRoutine(idlePosition);

        // Bekle.
        PlayIdle();
    }

    private IEnumerator MoveRoutine(Vector3 target)
    {
        if (target.x < transform.position.x)
            PlayWalkLeft();
        else
            PlayWalkRight();

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