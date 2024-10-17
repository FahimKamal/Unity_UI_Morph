using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;

public class UIMorpher : MonoBehaviour
{
    [SerializeField] 
    private List<RectTransform> uiElements = new List<RectTransform>();
    
    [SerializeField]
    private List<UIElementLocationValue> uiElementLocationValues = new List<UIElementLocationValue>();

    private void OnEnable()
    {
        OrientationDetector.Instance.OnPortraitOrientationChanged.AddListener(OnPortraitOrientationChanged);
        OrientationDetector.Instance.OnLandscapeOrientationChanged.AddListener(OnLandscapeOrientationChanged);
    }
    
    private void OnDisable()
    {
        OrientationDetector.Instance.OnPortraitOrientationChanged.RemoveListener(OnPortraitOrientationChanged);
        OrientationDetector.Instance.OnLandscapeOrientationChanged.RemoveListener(OnLandscapeOrientationChanged);
    }

    [Button]
    public void SavePortraitUILocations()
    {
        SaveUILocations(isPortrait:true); // Save current positions as portrait
    }

    [Button]
    public void SaveLandscapeUILocations()
    {
        SaveUILocations(isPortrait:false); // Save current positions as landscape
    }
    
    [Button]
    public void OnPortraitOrientationChanged()
    {
        ApplyOrientation(isPortrait:true); // Apply portrait positions
    }

    [Button]
    public void OnLandscapeOrientationChanged()
    {
        ApplyOrientation(isPortrait:false); // Apply landscape positions
    }

    [Button]
    public void ClearUILocations()
    {
        uiElementLocationValues.Clear();
    }

    // Helper function to apply orientation-specific positions
    private void ApplyOrientation(bool isPortrait)
    {
        foreach (var locationValue in uiElementLocationValues)
        {
            RectTransformData targetTransform = isPortrait ? locationValue.portraitTransform : locationValue.landscapeTransform;
            
            // Check if the UI element exists and has a RectTransform
            if (locationValue.uiElement != null && locationValue.uiElement.TryGetComponent<RectTransform>(out var uiRectTransform))
            {
                // Apply the saved transform values
                
                uiRectTransform.ApplyRectTransformData(targetTransform);
            }
        }
    }

    // Helper function to save UI element locations
    private void SaveUILocations(bool isPortrait)
    {
        // Iterate through uiElements and find or create corresponding UIElementLocationValue
        foreach (var uiElement in uiElements)
        {
            if (uiElement != null)
            {
                // Find existing location value
                var index = uiElementLocationValues
                    .FindIndex((x) =>
                    {
                        if (x.uiElement == uiElement) return true;
                        return false;
                    });
                UIElementLocationValue locationValue;

                if (index != -1) // Found existing value
                {
                    locationValue = uiElementLocationValues[index];
                }
                else // Create new value if not found
                {
                    locationValue = new UIElementLocationValue
                    {
                        name = uiElement.name,
                        uiElement = uiElement,
                        portraitTransform = new RectTransformData(),
                        landscapeTransform = new RectTransformData()
                    };
                    uiElementLocationValues.Add(locationValue);
                }

                // Copy RectTransform values to the correct field
                if (isPortrait)
                {
                    locationValue.portraitTransform = uiElement.CopyRectTransformData();
                }
                else
                {
                    locationValue.landscapeTransform = uiElement.CopyRectTransformData();
                }

                uiElement.PrintValues();
            }
        }
    }
}


[Serializable]
public class UIElementLocationValue
{
    public string name;
    public RectTransform uiElement;
    public RectTransformData portraitTransform;
    public RectTransformData landscapeTransform;
}

[Serializable]
public class RectTransformData
{
    public Vector3 LocalPosition;
    public Vector3 LocalScale;
    public Vector2 SizeDelta;
    public Vector2 AnchorMin;
    public Vector2 AnchorMax;
    public Vector2 AnchoredPosition;
    public Vector2 Pivot;

    public RectTransformData(RectTransform rectTransform)
    {
        LocalPosition = rectTransform.localPosition;
        LocalScale = rectTransform.localScale;
        SizeDelta = rectTransform.sizeDelta;
        AnchorMin = rectTransform.anchorMin;
        AnchorMax = rectTransform.anchorMax;
        AnchoredPosition = rectTransform.anchoredPosition;
        Pivot = rectTransform.pivot;
    }

    public RectTransformData()
    {
        
    }
}

public static class RectTransformExtensions
{
    public static RectTransformData CopyRectTransformData(this RectTransform rectTransform)
    {
        return new RectTransformData(rectTransform);
    }

    public static void ApplyRectTransformData(this RectTransform rectTransform, RectTransformData rectTransformData)
    {
        if (rectTransformData == null)
        {
            return;
        }

        rectTransform.localPosition = rectTransformData.LocalPosition;
        rectTransform.localScale = rectTransformData.LocalScale;
        rectTransform.sizeDelta = rectTransformData.SizeDelta;
        rectTransform.anchorMin = rectTransformData.AnchorMin;
        rectTransform.anchorMax = rectTransformData.AnchorMax;
        rectTransform.anchoredPosition = rectTransformData.AnchoredPosition;
        rectTransform.pivot = rectTransformData.Pivot;
    }

    public static void PrintValues(this RectTransform rectTransform)
    {
        Debug.Log(rectTransform.localPosition);
        Debug.Log(rectTransform.localScale);
        Debug.Log(rectTransform.sizeDelta);
        Debug.Log(rectTransform.anchorMin);
        Debug.Log(rectTransform.anchorMax);
        Debug.Log(rectTransform.anchoredPosition);
        Debug.Log(rectTransform.pivot);
    }
}