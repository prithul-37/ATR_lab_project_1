using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class HttpServer : MonoBehaviour
{
    [Header("Server Settings")]
    public int Port = 8080;
    public bool StartOnAwake = true;

    [Header("Camera Settings")]
    public CameraCapture CameraCapture;

    private HttpListener _httpListener;
    private Thread _listenerThread;
    private bool _isRunning = false;

    public static event Action<string> OnCommandReceived;

    void Awake()
    {
        if (StartOnAwake)
        {
            StartServer();
        }
    }

    void OnDestroy()
    {
        StopServer();
    }

    public void StartServer()
    {
        if (_isRunning) return;

        _httpListener = new HttpListener();
        _httpListener.Prefixes.Add($"http://localhost:{Port}/");
        _httpListener.Start();
        _isRunning = true;

        _listenerThread = new Thread(Listen);
        _listenerThread.Start();

        Debug.Log($"HTTP Server started on port {Port}");
    }

    public void StopServer()
    {
        if (!_isRunning) return;

        _isRunning = false;
        _httpListener?.Stop();
        _listenerThread?.Join(1000);

        Debug.Log("HTTP Server stopped");
    }

    private void Listen()
    {
        while (_isRunning && _httpListener != null)
        {
            try
            {
                HttpListenerContext context = _httpListener.GetContext();
                ProcessRequest(context);
            }
            catch (Exception e)
            {
                if (_isRunning)
                {
                    Debug.LogError($"HTTP Server error: {e.Message}");
                }
            }
        }
    }

    private void ProcessRequest(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        string responseString = "";
        int statusCode = 200;

        try
        {
            if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/command")
            {
                using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string commandJson = reader.ReadToEnd();

                    // Invoke the command on the main thread
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        OnCommandReceived?.Invoke(commandJson);
                    });

                    responseString = "{\"status\":\"success\",\"message\":\"Command received\"}";
                }
            }
            else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/camera/frame")
            {
                HandleCameraFrame(response);
                return; // Response handled by HandleCameraFrame
            }
            else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/camera/info")
            {
                HandleCameraInfo(out responseString);
            }
            else
            {
                statusCode = 404;
                responseString = "{\"status\":\"error\",\"message\":\"Endpoint not found\"}";
            }
        }
        catch (Exception e)
        {
            statusCode = 500;
            responseString = $"{{\"status\":\"error\",\"message\":\"{e.Message}\"}}";
        }

        // Send response
        response.StatusCode = statusCode;
        response.ContentType = "application/json";
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    private void HandleCameraFrame(HttpListenerResponse response)
    {
        try
        {
            if (CameraCapture == null)
            {
                response.StatusCode = 503;
                response.ContentType = "application/json";
                string errorJson = "{\"status\":\"error\",\"message\":\"Camera capture not available\"}";
                byte[] errorBuffer = Encoding.UTF8.GetBytes(errorJson);
                response.ContentLength64 = errorBuffer.Length;
                response.OutputStream.Write(errorBuffer, 0, errorBuffer.Length);
                response.OutputStream.Close();
                return;
            }

            byte[] frameData = CameraCapture.GetLastFrame();

            if (frameData == null || frameData.Length == 0)
            {
                response.StatusCode = 404;
                response.ContentType = "application/json";
                string errorJson = "{\"status\":\"error\",\"message\":\"No frame available\"}";
                byte[] errorBuffer = Encoding.UTF8.GetBytes(errorJson);
                response.ContentLength64 = errorBuffer.Length;
                response.OutputStream.Write(errorBuffer, 0, errorBuffer.Length);
                response.OutputStream.Close();
                return;
            }

            // Send JPEG frame
            response.StatusCode = 200;
            response.ContentType = "image/jpeg";
            response.ContentLength64 = frameData.Length;
            response.OutputStream.Write(frameData, 0, frameData.Length);
            response.OutputStream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"Camera frame error: {e.Message}");
            response.StatusCode = 500;
            response.OutputStream.Close();
        }
    }

    private void HandleCameraInfo(out string responseString)
    {
        if (CameraCapture == null)
        {
            responseString = "{\"status\":\"error\",\"message\":\"Camera capture not available\"}";
            return;
        }

        responseString = $"{{" +
            $"\"status\":\"success\"," +
            $"\"width\":{CameraCapture.CaptureWidth}," +
            $"\"height\":{CameraCapture.CaptureHeight}," +
            $"\"framerate\":{CameraCapture.CaptureFrameRate}," +
            $"\"hasFrame\":{(CameraCapture.HasCapturedFrame() ? "true" : "false")}" +
        $"}}";
    }
}