using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWorkersManager : MonoBehaviour
{
    [Header("Worker")]
    [SerializeField] private WorkerVisual workerTemplate;
    [SerializeField] private int startingWorkerCount = 3;
    [SerializeField] private bool useTemplateAsFirstWorker = true;

    [Header("Points")]
    [SerializeField] private Transform[] homePoints;
    [SerializeField] private Transform workPoint;

    [Header("Fallback Layout")]
    [SerializeField] private Vector3 fallbackHomeStart = new Vector3(-2.8f, -3f, -2f);
    [SerializeField] private Vector3 fallbackHomeSpacing = new Vector3(1.3f, 0.05f, 0f);
    [SerializeField] private Vector3 fallbackWorkPosition = new Vector3(-3.7f, -2.35f, -2f);

    private readonly List<WorkerVisual> workers = new List<WorkerVisual>();
    private Coroutine roundRobinRoutine;

    private void Awake()
    {
        if (workerTemplate == null)
        {
            workerTemplate = GetComponentInChildren<WorkerVisual>(true);
        }

        if (workerTemplate == null || startingWorkerCount <= 0)
        {
            return;
        }

        for (int i = 0; i < startingWorkerCount; i++)
        {
            WorkerVisual worker = GetWorker(i);
            Transform homePoint = GetPoint(homePoints, i);

            if (homePoint != null)
            {
                worker.Configure(homePoint);
            }
            else
            {
                worker.Configure(GetFallbackHomePosition(i));
            }

            workers.Add(worker);
        }
    }

    private void Start()
    {
        if (workers.Count > 0)
        {
            roundRobinRoutine = StartCoroutine(RoundRobinRoutine());
        }
    }

    private void OnDisable()
    {
        if (roundRobinRoutine != null)
        {
            StopCoroutine(roundRobinRoutine);
            roundRobinRoutine = null;
        }
    }

    private IEnumerator RoundRobinRoutine()
    {
        int workerIndex = 0;

        while (true)
        {
            if (workers.Count == 0)
            {
                yield break;
            }

            WorkerVisual worker = workers[workerIndex % workers.Count];

            if (worker != null && worker.gameObject.activeInHierarchy)
            {
                yield return worker.RunWorkCycle(workPoint);
            }
            else
            {
                yield return null;
            }

            workerIndex++;
        }
    }

    private WorkerVisual GetWorker(int index)
    {
        if (index == 0 && useTemplateAsFirstWorker)
        {
            workerTemplate.gameObject.SetActive(true);
            return workerTemplate;
        }

        WorkerVisual worker = Instantiate(workerTemplate, transform);
        worker.name = $"Worker_{index + 1:00}";
        worker.gameObject.SetActive(true);
        return worker;
    }

    private Transform GetPoint(Transform[] points, int index)
    {
        if (points == null || points.Length == 0)
        {
            return null;
        }

        return points[index % points.Length];
    }

    private Vector3 GetFallbackHomePosition(int index)
    {
        return fallbackHomeStart + fallbackHomeSpacing * index;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        for (int i = 0; i < Mathf.Max(startingWorkerCount, 0); i++)
        {
            Gizmos.DrawWireSphere(GetFallbackHomePosition(i), 0.08f);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(workPoint != null ? workPoint.position : fallbackWorkPosition, 0.08f);
    }
}
