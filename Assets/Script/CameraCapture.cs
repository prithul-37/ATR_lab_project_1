using System;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera TargetCamera;
    public int CaptureWidth = 640;
    public int CaptureHeight = 480;
    public int CaptureFrameRate = 30;

    [Header("Capture Control")]
    public bool AutoCapture = true;
    public KeyCode ManualCaptureKey = KeyCode.C;

    private RenderTexture _renderTexture;
    private Texture2D _captureTexture;
    private byte[] _lastCapturedFrame;
    private float _captureInterval;
    private float _lastCaptureTime;

    public static event Action<byte[]> OnFrameCaptured;

    void Start()
    {
        if (TargetCamera == null)
        {
            TargetCamera = Camera.main;
        }

        if (TargetCamera == null)
        {
            Debug.LogError("CameraCapture: No camera found! Please assign a target camera.");
            enabled = false;
            return;
        }

        InitializeCapture();
        _captureInterval = 1f / CaptureFrameRate;

        Debug.Log($"CameraCapture initialized: {CaptureWidth}x{CaptureHeight} @ {CaptureFrameRate}fps");
    }

    void InitializeCapture()
    {
        _renderTexture = new RenderTexture(CaptureWidth, CaptureHeight, 24);
        _captureTexture = new Texture2D(CaptureWidth, CaptureHeight, TextureFormat.RGB24, false);

        TargetCamera.targetTexture = _renderTexture;
    }

    void Update()
    {
        if (AutoCapture && Time.time - _lastCaptureTime >= _captureInterval)
        {
            CaptureFrame();
            _lastCaptureTime = Time.time;
        }

        if (Input.GetKeyDown(ManualCaptureKey))
        {
            CaptureFrame();
        }
    }

    public void CaptureFrame()
    {
        if (_renderTexture == null || _captureTexture == null)
        {
            Debug.LogWarning("CameraCapture: Render texture not initialized");
            return;
        }

        // Render the camera to the render texture
        TargetCamera.Render();

        // Read the render texture
        RenderTexture.active = _renderTexture;
        _captureTexture.ReadPixels(new Rect(0, 0, CaptureWidth, CaptureHeight), 0, 0);
        _captureTexture.Apply();
        RenderTexture.active = null;

        // Convert to byte array (JPEG format)
        _lastCapturedFrame = _captureTexture.EncodeToJPG(75);

        // Notify listeners
        OnFrameCaptured?.Invoke(_lastCapturedFrame);
    }

    public byte[] GetLastFrame()
    {
        return _lastCapturedFrame;
    }

    public bool HasCapturedFrame()
    {
        return _lastCapturedFrame != null && _lastCapturedFrame.Length > 0;
    }

    void OnDestroy()
    {
        if (_renderTexture != null)
        {
            TargetCamera.targetTexture = null;
            _renderTexture.Release();
        }

        if (_captureTexture != null)
        {
            DestroyImmediate(_captureTexture);
        }

        OnFrameCaptured = null;
    }

    void OnDrawGizmosSelected()
    {
        if (TargetCamera != null)
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = TargetCamera.transform.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero, TargetCamera.fieldOfView, TargetCamera.farClipPlane, TargetCamera.nearClipPlane, TargetCamera.aspect);
        }
    }
}