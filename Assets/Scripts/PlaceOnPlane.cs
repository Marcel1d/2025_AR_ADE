using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    private GameObject m_PlacedPrefab;

    [SerializeField]
    private GameObject visualObject;

    public GameObject spawnedObject { get; private set; }

    public event Action<GameObject> OnPlateauInstantiated;

    private ARRaycastManager m_RaycastManager;
    private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    private bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    private void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = s_Hits[0].pose;

            if (spawnedObject == null)
            {
                Quaternion customRotation = hitPose.rotation * Quaternion.Euler(-90f, 0f, 0f);
                spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, customRotation);
                spawnedObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

                Transform plateauTransform = spawnedObject.transform.Find("plateau");
                if (plateauTransform != null)
                {
                    OnPlateauInstantiated?.Invoke(plateauTransform.gameObject);
                }
                else
                {
                    Debug.LogWarning("Impossible de trouver l'enfant 'plateau' dans le prefab.");
                }
            }
            else
            {
                spawnedObject.transform.position = hitPose.position;
            }

            if (visualObject != null)
            {
                visualObject.SetActive(false);
            }
        }
    }
}
