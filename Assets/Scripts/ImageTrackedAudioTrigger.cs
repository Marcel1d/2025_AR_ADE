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

    private Dictionary<string, AudioClip> imageToMusic;

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
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                string imageName = trackedImage.referenceImage.name;
                if (imageToMusic.ContainsKey(imageName))
                {
                    AudioClip clip = imageToMusic[imageName];
                    if (audioSource.clip != clip)
                    {
                        audioSource.clip = clip;
                        audioSource.Play();
                        Debug.Log("Lecture de : " + imageName);
                    }
                }
            }
        }

        foreach (var removedImage in eventArgs.removed)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
