using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using System.Collections.Generic;

public class SphereSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject spherePrefab;
    public Transform rightController;
    public LayerMask sphereLayer;
    public float maxRayDistance = 10f;

    private bool triggerPressed = false;
    private SphereBehavior currentlyHovered;
    void Start()
    {
        SetFloorTrackingOrigin();
    }

    void SetFloorTrackingOrigin()
    {
        var xrInputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(xrInputSubsystems);

        foreach (var subsystem in xrInputSubsystems)
        {
            // Attempt to set Floor-Level tracking origin
            subsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);
        }
    }
    void Update()
    {
        // Check trigger input
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed) && isPressed && !triggerPressed)
        {
            triggerPressed = true;
            SpawnSphere();
        }
        else if (!isPressed)
        {
            triggerPressed = false;
        }

        DetectHover();
    }

    void SpawnSphere()
    {
        float randomSize = Random.Range(0.1f, 2f);
        Vector3 spawnPos = rightController.position;

        GameObject newSphere = Instantiate(spherePrefab, spawnPos, Quaternion.identity);
        newSphere.name = "Sphere_" + Time.time.ToString("F2"); // Unique name

        // Log it using the logger
        SphereLogger.Instance?.LogSphere(newSphere);
        newSphere.transform.localScale = Vector3.one * randomSize;
        newSphere.layer = Mathf.RoundToInt(Mathf.Log(sphereLayer.value, 2)); // Apply correct layer

        SphereBehavior behavior = newSphere.AddComponent<SphereBehavior>();
        behavior.Initialize();
    }

    void DetectHover()
    {
        Ray ray = new Ray(rightController.position, rightController.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, sphereLayer))
        {
            SphereBehavior hovered = hit.collider.GetComponent<SphereBehavior>();
            if (hovered != null && hovered != currentlyHovered)
            {
                if (currentlyHovered != null)
                    currentlyHovered.OnHoverExit();

                currentlyHovered = hovered;
                currentlyHovered.OnHoverEnter();
            }
        }
        else if (currentlyHovered != null)
        {
            currentlyHovered.OnHoverExit();
            currentlyHovered = null;
        }
    }
}

