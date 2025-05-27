using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTrackedAudioTrigger : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;
    private AudioSource audioSource;

    [System.Serializable]
    public class ImageMusicPair
    {
        public string imageName;
        public AudioClip musicClip;
    }

    [SerializeField]
    private List<ImageMusicPair> imageMusicMappings = new List<ImageMusicPair>();

    // Le plateau à faire tourner, sera assigné via l'événement
    private GameObject platinePlateau;

    [SerializeField]
    private float rotationSpeed = 90f;

    private Dictionary<string, AudioClip> imageToMusic;

    private bool isSpinning = false;
    private AudioClip currentClip = null;

    // Champ public à assigner dans l’inspecteur (GameObject qui a PlaceOnPlane)
    [SerializeField]
    private PlaceOnPlane placeOnPlane;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        imageToMusic = new Dictionary<string, AudioClip>();
        foreach (var pair in imageMusicMappings)
        {
            imageToMusic[pair.imageName] = pair.musicClip;
        }
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

        if (placeOnPlane != null)
        {
            placeOnPlane.OnPlateauInstantiated += OnPlateauReady;
        }
        else
        {
            Debug.LogWarning("PlaceOnPlane non assigné dans ImageTrackedAudioTrigger.");
        }
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;

        if (placeOnPlane != null)
        {
            placeOnPlane.OnPlateauInstantiated -= OnPlateauReady;
        }
    }

    private void OnPlateauReady(GameObject plateau)
    {
        platinePlateau = plateau;
        Debug.Log("Plateau assigné dans ImageTrackedAudioTrigger.");
    }

    private void Update()
    {
        if (isSpinning && platinePlateau != null)
        {
            platinePlateau.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        AudioClip clipToPlay = null;

        foreach (var trackedImage in trackedImageManager.trackables)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                string imageName = trackedImage.referenceImage.name;
                if (imageToMusic.ContainsKey(imageName))
                {
                    clipToPlay = imageToMusic[imageName];
                    break;
                }
            }
        }

        if (clipToPlay != null)
        {
            if (clipToPlay != currentClip)
            {
                audioSource.clip = clipToPlay;
                audioSource.Play();
                currentClip = clipToPlay;
                Debug.Log("Lecture de : " + clipToPlay.name);
            }
            else if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            isSpinning = true;
        }
        else
        {
            // Aucune image détectée, ne stoppe rien pour l’instant
        }
    }
}
