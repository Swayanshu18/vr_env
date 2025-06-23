using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR;  // Add this for haptics

public class SphereBehavior : MonoBehaviour
{
    private float minScale = 0.1f;
    private float maxScale = 2f;
    private TextMeshPro sizeLabel;
    private bool isHovered = false;

    // Keep track of last scale to detect change
    private float lastScale;

    public void Initialize()
    {
        gameObject.tag = "Sphere";

        GameObject labelObj = new GameObject("SizeLabel");
        labelObj.transform.SetParent(transform);
        labelObj.transform.localPosition = Vector3.up * 0.6f;

        sizeLabel = labelObj.AddComponent<TextMeshPro>();
        sizeLabel.fontSize = 0.2f;
        sizeLabel.alignment = TextAlignmentOptions.Center;
        sizeLabel.text = "";
        sizeLabel.gameObject.SetActive(false);

        lastScale = transform.localScale.x;

        StartCoroutine(RandomSizeChange());
    }

    IEnumerator RandomSizeChange()
    {
        while (true)
        {
            float delta = Random.Range(-0.05f, 0.05f);
            float newScale = Mathf.Clamp(transform.localScale.x + delta, minScale, maxScale);

            if (Mathf.Abs(newScale - lastScale) > 0.001f)
            {
                transform.localScale = Vector3.one * newScale;

                if (isHovered)
                {
                    sizeLabel.text = $"Size: {newScale:F2}";
                    TriggerHaptics();
                }

                lastScale = newScale;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void OnHoverEnter()
    {
        isHovered = true;
        sizeLabel.text = $"Size: {transform.localScale.x:F2}";
        sizeLabel.gameObject.SetActive(true);
    }

    public void OnHoverExit()
    {
        isHovered = false;
        sizeLabel.gameObject.SetActive(false);
    }

    private void TriggerHaptics()
    {
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (rightHand.TryGetHapticCapabilities(out HapticCapabilities capabilities) && capabilities.supportsImpulse)
        {
            // Channel 0, amplitude 0.5, duration 0.1 seconds
            rightHand.SendHapticImpulse(0, 0.5f, 0.1f);
        }
    }
}
