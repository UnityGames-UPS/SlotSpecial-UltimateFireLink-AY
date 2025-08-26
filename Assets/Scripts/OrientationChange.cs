using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class OrientationChange : MonoBehaviour
{
  [SerializeField] private CanvasScalerSwitcher canvasSwitch;
  [SerializeField] private RectTransform UIWrapper;
  [SerializeField] private CanvasScaler CanvasScaler;
  [SerializeField] private float MatchWidth = 0.25f;
  [SerializeField] private float MatchHeight = 1f;
  [SerializeField] private float PortraitMatchWandH = 0.5f;
  [SerializeField] private float transitionDuration = 0.5f;
  [SerializeField] private float waitForRotation = 1f;

  private Vector2 ReferenceAspect;
  private Tween matchTween;
  private Tween rotationTween;
  private Coroutine rotationRoutine;
  private bool isLandscape;
  public bool isMobile;
  private float rotationAngle;
  private void Awake()
  {
    ReferenceAspect = CanvasScaler.referenceResolution;
  }

  void SwitchDisplay(string dimensions)
  {
    if (rotationRoutine != null) StopCoroutine(rotationRoutine);
    rotationRoutine = StartCoroutine(RotationCoroutine(dimensions));
  }
  void DiviceCheck(string device)
  {
    Debug.Log("Unity: Received DeviceCheck:   " + device);
    if (device == "MB")
    {
      isMobile = true;
      changeTransforms();
      canvasSwitch.OnMobileDeviceDetected("A");
    }
    else if (device == "IP")
    {
      isMobile = true;
      changeTransforms();
      canvasSwitch.OnMobileDeviceDetected("I");
    }
    else
    {
      isMobile = false;
      canvasSwitch.OnMobileDeviceDetected("PC");
    }


  }
  void SwitchOrientation(string direction)
  {
    Debug.Log("Unity: Received SwitchOrientation:   " + direction);
    if (direction == "potrait-primary")
    {
      rotationAngle = 0f;

    }
    else if (direction == "landscape-secondary")
    {
      rotationAngle = 90f;

    }
    else if (direction == "landscape-primary")
    {
      rotationAngle = -90f;

    }
    else
    {
      rotationAngle = -180f;

    }
  }
  IEnumerator RotationCoroutine(string dimensions)
  {
    yield return new WaitForSecondsRealtime(waitForRotation);
    string[] parts = dimensions.Split(',');
    if (parts.Length == 2 && int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height) && width > 0 && height > 0)
    {
      Debug.Log($"Unity: Received Dimensions - Width: {width}, Height: {height}");

      isLandscape = width < height;

      // if (!isMobile) canvasSwitch.OnMobileDeviceDetected("pp");
      // else canvasSwitch.OnMobileDeviceDetected("A");
      if (isMobile)
      {
        changeTransforms();
        Quaternion targetRotation = Quaternion.Euler(0, 0, rotationAngle);
        if (rotationTween != null && rotationTween.IsActive()) rotationTween.Kill();
        rotationTween = UIWrapper.DOLocalRotateQuaternion(targetRotation, transitionDuration).SetEase(Ease.OutCubic);

        float currentAspectRatio = isLandscape ? (float)width / height : (float)height / width;
        float referenceAspectRatio = ReferenceAspect.x / ReferenceAspect.y;

        float targetMatch = isLandscape ? (currentAspectRatio > referenceAspectRatio ? MatchHeight : MatchWidth) : PortraitMatchWandH;
        if (matchTween != null && matchTween.IsActive()) matchTween.Kill();
        matchTween = DOTween.To(() => CanvasScaler.matchWidthOrHeight, x => CanvasScaler.matchWidthOrHeight = x, targetMatch, transitionDuration).SetEase(Ease.InOutQuad);

        Debug.Log($"matchWidthOrHeight set to: {targetMatch}");
      }
    }
    else
    {
      Debug.LogWarning("Unity: Invalid format received in SwitchDisplay");
    }
  }

  void changeTransforms()
  {
    UIWrapper.anchorMin = new Vector2(0.5f, 0.5f);
    UIWrapper.anchorMax = new Vector2(0.5f, 0.5f);

    UIWrapper.pivot = new Vector2(0.5f, 0.5f);

    UIWrapper.sizeDelta = new Vector2(1080, 2340);

    UIWrapper.anchoredPosition = Vector2.zero;
  }

#if UNITY_EDITOR
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.K))
    {
      SwitchDisplay(Screen.width + "," + Screen.height);
    }
  }
#endif
}
