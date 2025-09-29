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
}