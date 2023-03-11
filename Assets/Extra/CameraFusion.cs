using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFusion : MonoBehaviour
{
    public Camera CameraBase;
    public Camera CameraOverlay;

    private RenderBuffer DepthBuffer = new();
    private RenderBuffer ColorBuffer = new();
    void Start()
    {
        CameraBase.SetTargetBuffers(ColorBuffer, DepthBuffer);
        CameraOverlay.SetTargetBuffers(ColorBuffer, DepthBuffer);
    }
}
