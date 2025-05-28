using Cinemachine;
using System.Collections;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public Bird[] birdPrefabs;
    public Transform launchPosition;
    private int birdIndex = 0;
    private Bird currentBird;
    private bool isDragging = false;
    public float launchPower = 8f;
    public float maxDragDistance = 2f;

    [Header("Slingshot Line Settings")]
    public LineRenderer slingshotLine;
    public float slingshotLineWidth = 1f;
    public float slingshotLineMaxLength = 3f;

    [Header("Trajectory Settings")]
    public int trajectoryPoints = 30;
    public float timeStep = 0.1f;
    public LineRenderer trajectoryRenderer;

    [Header("Launcher Activation Area")]
    public Transform activationCenter;
    public float activationRadius = 1.5f;

    [Header("Camera References")]
    public CinemachineVirtualCamera slingshotCam;
    public CinemachineVirtualCamera followCam;
    public CameraDrag cameraDrag;

    // 👉 Step 1: Tracking properties
    public int TotalBirds => birdPrefabs.Length;
    public int BirdsUsed => birdIndex;
    public int RemainingBirds => TotalBirds - BirdsUsed;

    void Start()
    {
        SpawnNextBird();
        slingshotLine.positionCount = 0;
        slingshotCam.Priority = 20;
        followCam.Priority = 10;

        if (cameraDrag != null)
            cameraDrag.isDraggable = true;
    }

    void Update()
    {
        if (currentBird == null)
        {
            if (cameraDrag != null)
                cameraDrag.isDraggable = true;
        }
        else if (!currentBird.HasLaunched && !isDragging)
        {
            if (cameraDrag != null)
                cameraDrag.isDraggable = true;
        }
        else
        {
            if (cameraDrag != null)
                cameraDrag.isDraggable = false;
        }

        if (Input.GetMouseButtonDown(0) && IsPointerInLauncherZone())
        {
            isDragging = true;
            slingshotLine.positionCount = 2;

            if (cameraDrag != null)
                cameraDrag.isDraggable = false;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld = new Vector3(mouseWorld.x, mouseWorld.y, 0f);
            Vector3 dragVector = mouseWorld - launchPosition.position;

            if (dragVector.magnitude > maxDragDistance)
                dragVector = dragVector.normalized * maxDragDistance;

            currentBird.transform.position = launchPosition.position + dragVector;

            Vector2 dir = (Vector2)(launchPosition.position - currentBird.transform.position);
            Vector2 launchVelocity = dir * launchPower;
            ShowTrajectory(currentBird.transform.position, launchVelocity);
            UpdateSlingshotLine(currentBird.transform.position);
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            if (cameraDrag != null)
                cameraDrag.isDraggable = false;

            if (currentBird != null)
            {
                Vector2 dir = (Vector2)(launchPosition.position - currentBird.transform.position);
                currentBird.Launch(dir * launchPower);

                followCam.Follow = currentBird.transform;
                slingshotCam.Priority = 10;
                followCam.Priority = 20;
            }

            slingshotLine.positionCount = 0;
            HideTrajectory();
        }
    }

    void UpdateSlingshotLine(Vector2 birdPosition)
    {
        Vector2 direction = birdPosition - (Vector2)launchPosition.position;
        float distance = Mathf.Min(direction.magnitude, slingshotLineMaxLength);
        Vector2 stretchedPosition = (Vector2)launchPosition.position + direction.normalized * distance;

        slingshotLine.SetPosition(0, launchPosition.position);
        slingshotLine.SetPosition(1, stretchedPosition);
    }

    void ShowTrajectory(Vector2 startPos, Vector2 startVelocity)
    {
        Vector3[] points = new Vector3[trajectoryPoints];
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            Vector2 pos = startPos + startVelocity * t + 0.5f * Physics2D.gravity * t * t;
            points[i] = pos;
        }

        trajectoryRenderer.positionCount = trajectoryPoints;
        trajectoryRenderer.SetPositions(points);
    }

    void HideTrajectory()
    {
        trajectoryRenderer.positionCount = 0;
    }

    void SpawnNextBird()
    {
        if (birdIndex >= birdPrefabs.Length) return;

        currentBird = Instantiate(birdPrefabs[birdIndex], launchPosition.position, Quaternion.identity);
        currentBird.GetComponent<Rigidbody2D>().isKinematic = true;
        currentBird.OnBirdStopped += HandleBirdFinished;

        if (cameraDrag != null)
            cameraDrag.isDraggable = true;
    }

    private void HandleBirdFinished(Bird bird)
    {
        bird.OnBirdStopped -= HandleBirdFinished;

        if (bird != null)
            Destroy(bird.gameObject);

        followCam.Priority = 10;
        slingshotCam.Priority = 20;

        birdIndex++;
        Debug.Log("Bird used, index is now: " + birdIndex);

        StartCoroutine(DelayBeforeNextBird());
    }

    public void FollowTarget(Transform target)
    {
        followCam.Follow = target;
        slingshotCam.Priority = 10;
        followCam.Priority = 20;
    }

    public void FollowTargetThenReturn(Transform target, float delay)
    {
        StartCoroutine(FollowThenReturnCoroutine(target, delay));
    }

    private bool IsPointerInLauncherZone()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        return Vector3.Distance(worldPos, activationCenter.position) <= activationRadius;
    }

    private IEnumerator DelayBeforeNextBird()
    {
        yield return new WaitForSeconds(1f);

        if (birdIndex >= birdPrefabs.Length)
        {
            Debug.Log("Out of birds, checking for game over");
            if (LevelManager.Instance != null)
                LevelManager.Instance.CheckForGameOver(); // Check if enemies remain
        }
        else
        {
            SpawnNextBird();
        }
    }

    private IEnumerator FollowThenReturnCoroutine(Transform target, float delay)
    {
        FollowTarget(target);
        yield return new WaitForSeconds(delay);

        followCam.Follow = null;
        slingshotCam.Priority = 20;
        followCam.Priority = 10;

        SpawnNextBird();
    }
}
