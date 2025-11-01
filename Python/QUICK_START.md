# Quick Start Guide - Sui HTTP System with Auto-Listening

## What's New
Unity now **continuously listens** for updates from the server, enabling real-time notifications!

## Setup (3 Steps)

### 1. Start the Server
```bash
cd Python
python sui_server_example.py
```

### 2. Configure Unity
- Add `Sui_HttpClient` component to a GameObject
- Enable `Auto Listen` in Inspector (default: ON)
- Set `Poll Interval` to 2 seconds (default)

### 3. Run Unity Scene
The client will automatically start listening for updates!

## Testing the Listening Feature

### Option 1: Interactive Mode
```bash
python send_update_to_unity.py
```
Choose option 5 to send multiple test messages.

### Option 2: Command Line
```bash
python send_update_to_unity.py success "Blockchain confirmed!"
python send_update_to_unity.py failed "Transaction failed"
python send_update_to_unity.py info "Processing..."
```

### Option 3: Using Curl
```bash
curl -X POST http://localhost:8080/push_update \
  -H "Content-Type: application/json" \
  -d '{"status":"success","message":"Hello from server!","transactionId":"123"}'
```

## How It Works

```
┌─────────────┐                    ┌─────────────┐
│    Unity    │                    │   Server    │
│             │                    │             │
│  (Polling)  │────── GET /updates ────▶│  (Queue)   │
│             │◀───── Status Update ────│            │
│             │                    │             │
│ Every 2 sec │                    │  Pending    │
└─────────────┘                    │  Updates    │
                                   └─────────────┘
```

1. Unity polls `/updates` every 2 seconds
2. Server maintains a queue of pending updates
3. When Unity requests, server sends oldest update
4. Unity displays update in status popup
5. Update is removed from queue

## Inspector Settings

| Setting | Default | Description |
|---------|---------|-------------|
| Server Url | `http://localhost:8080` | Server address |
| Enable Auto Listen | ✓ | Start listening on scene start |
| Poll Interval | 2.0s | How often to check for updates |
| Request Timeout | 30s | HTTP request timeout |

## Control Listening via Code

```csharp
// Start listening
Sui_HttpClient.Instance.StartListening();

// Stop listening
Sui_HttpClient.Instance.StopListening();

// Change interval (e.g., check every 5 seconds)
Sui_HttpClient.Instance.SetPollInterval(5.0f);
```

## Common Use Cases

### 1. Blockchain Event Notifications
When a blockchain transaction is confirmed, server pushes notification to Unity:
```python
send_update("success", "NFT minted successfully!", "tx-abc123")
```

### 2. Progress Updates
Keep users informed about long-running processes:
```python
send_update("info", "Processing transaction... 50% complete")
```

### 3. Error Alerts
Notify users immediately when issues occur:
```python
send_update("failed", "Connection to blockchain lost")
```

## Troubleshooting

**Updates not appearing in Unity?**
- Check Unity console for "[Sui_HttpClient] Started listening" message
- Verify `Enable Auto Listen` is checked
- Make sure server is running
- Check server console for "[UPDATE SENT]" messages

**Too many network requests?**
- Increase `Poll Interval` (e.g., 5 or 10 seconds)
- Consider using WebSockets for production

**Server errors?**
- Check if Flask is installed: `pip install flask`
- Verify port 8080 is not in use
- Look for error messages in server console

## Performance Tips

**Poll Interval Recommendations:**
- **0.5-1 second**: Critical real-time updates (high traffic)
- **2-5 seconds**: Standard applications (balanced) ⭐ **Recommended**
- **10-30 seconds**: Low-priority notifications (low traffic)

## Next Steps

1. ✅ Test the basic setup with `send_update_to_unity.py`
2. ✅ Try sending different status types (success, failed, info)
3. ✅ Integrate with your blockchain backend
4. ✅ Customize update messages for your use case
5. ✅ Read full documentation in `SUI_HTTP_SETUP.md`

## Files Reference

| File | Purpose |
|------|---------|
| `Sui_HttpClient.cs` | Unity HTTP client with auto-listening |
| `sui_server_example.py` | Flask server with update queue |
| `send_update_to_unity.py` | Helper script to send updates |
| `SUI_HTTP_SETUP.md` | Full documentation |
| `QUICK_START.md` | This file |
