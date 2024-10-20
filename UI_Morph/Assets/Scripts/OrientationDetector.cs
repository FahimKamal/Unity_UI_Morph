using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class OrientationDetector : MonoBehaviour
{
    public static OrientationDetector Instance { get; private set; } // Singleton instance

    public UnityEvent OnPortraitOrientationChanged;
    public UnityEvent OnLandscapeOrientationChanged;

    public bool IsPortrait { get; private set; }

    private ScreenOrientation currentOrientation;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        currentOrientation = Screen.orientation; // Store initial orientation
        if (currentOrientation is ScreenOrientation.Portrait or ScreenOrientation.PortraitUpsideDown)
        {
            IsPortrait = true;
        }
    }

    private void Start()
    {
        StartCoroutine(OrientationCheckCoroutine());
    }

    private IEnumerator OrientationCheckCoroutine()
    {
        while (true) 
        {
            // Check for orientation change and ignore Unknown orientation
            if (Screen.orientation != currentOrientation && Screen.orientation != ScreenOrientation.Unknown)
            {
                currentOrientation = Screen.orientation; // Update current orientation

                switch (currentOrientation)
                {
                    case ScreenOrientation.Portrait:
                    case ScreenOrientation.PortraitUpsideDown:
                        OnPortraitOrientationChanged.Invoke();
                        IsPortrait = true;
                        break;
                    case ScreenOrientation.LandscapeLeft:
                    case ScreenOrientation.LandscapeRight:
                        OnLandscapeOrientationChanged.Invoke();
                        IsPortrait = false;
                        break;
                }
            }
            yield return new WaitForSeconds(0.5f); // Check every 1 second
        }
    }
}