# Sui Crypto System - HTTP Integration Setup

## Overview
This system allows Unity to communicate with an HTTP server for handling blockchain transactions and status updates. Unity continuously listens for server updates via polling, enabling real-time notifications.

## Architecture

### Unity Side
- **Sui_HttpClient.cs**: Manages HTTP communication with the server
- **Sui_LevelManager.cs**: Integrates transaction flow with UI popups
- **Sui_ConfirmationPopUp.cs**: Gets user input for transactions
- **Sui_StatusPopUp.cs**: Displays transaction status

### Server Side
- **sui_server_example.py**: Flask server that handles transaction requests and status queries

## Setup Instructions

### 1. Install Python Dependencies

```bash
cd Python
pip install -r requirements.txt
```

### 2. Unity Setup

1. **Add HttpClient to Scene**:
   - Create an empty GameObject in your scene
   - Name it "SuiHttpClient"
   - Add the `Sui_HttpClient` component

2. **Configure Server URL**:
   - In Inspector, set `Server Url` to `http://localhost:8080`
   - Set `Transaction Endpoint` to `/transaction`
   - Set `Status Endpoint` to `/status`
   - Set `Updates Endpoint` to `/updates`
   - Adjust `Request Timeout` if needed (default: 30 seconds)

3. **Configure Auto-Listen**:
   - Check `Enable Auto Listen` to start listening on scene start
   - Set `Poll Interval` (default: 2 seconds) - how often Unity checks for updates
   - Lower intervals = faster updates but more network traffic

4. **Verify LevelManager Setup**:
   - Ensure `Sui_LevelManager` is in the scene
   - All robot functions now automatically send HTTP requests

### 3. Start the Server

```bash
cd Python
python sui_server_example.py
```

Server will start on `http://localhost:8080`

## Usage

### Transaction Flow

1. User clicks on a robot in Unity
2. Confirmation popup appears
3. User enters a command (e.g., "move forward", "activate", etc.)
4. User confirms the transaction
5. Unity sends POST request to server
6. Server processes transaction
7. Server responds with success/failure
8. Unity displays status in popup

### Continuous Listening Flow

1. Unity starts and begins polling `/updates` endpoint every 2 seconds (configurable)
2. Server maintains a queue of pending updates
3. When blockchain events occur, server pushes updates to the queue
4. Unity receives updates and displays them in status popup
5. Updates are removed from queue once delivered

### API Endpoints

#### POST /transaction
Initiates a new transaction.

**Request:**
```json
{
  "command": "move forward",
  "userId": "Robot1",
  "parameters": "speed:10"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Transaction completed successfully for Robot1",
  "transactionId": "550e8400-e29b-41d4-a716-446655440000",
  "data": "Processed command: move forward"
}
```

#### GET /status?transactionId={id}
Queries transaction status.

**Response:**
```json
{
  "status": "completed",
  "message": "Transaction completed",
  "transactionId": "550e8400-e29b-41d4-a716-446655440000"
}
```

#### GET /updates
Polled by Unity to receive pending status updates.

**Response (when update available):**
```json
{
  "status": "success",
  "message": "Blockchain transaction confirmed!",
  "transactionId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response (no updates):**
```json
{}
```

#### POST /push_update
Push a status update to Unity (server-side use or testing).

**Request:**
```json
{
  "status": "success",
  "message": "Your NFT was minted successfully!",
  "transactionId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Update queued successfully",
  "queueLength": 1
}
```

#### GET /transactions
Lists all transactions (debugging).

#### GET /pending_updates
Lists all pending updates in queue (debugging).

## Code Usage Examples

### Send Transaction from Unity

```csharp
Sui_HttpClient.Instance.InitiateTransaction(
    command: "activate robot",
    userId: "Robot1",
    parameters: "mode:auto",
    callback: (success, response) =>
    {
        if (success)
        {
            Debug.Log($"Transaction successful: {response.transactionId}");
        }
        else
        {
            Debug.LogError($"Transaction failed: {response.message}");
        }
    }
);
```

### Request Status from Unity

```csharp
Sui_HttpClient.Instance.RequestStatus(
    transactionId: "550e8400-e29b-41d4-a716-446655440000",
    callback: (success, statusUpdate) =>
    {
        if (success)
        {
            Debug.Log($"Status: {statusUpdate.status}");
        }
    }
);
```

### Change Server URL at Runtime

```csharp
Sui_HttpClient.Instance.SetServerUrl("http://192.168.1.100:8080");
```

### Control Listening at Runtime

```csharp
// Start listening
Sui_HttpClient.Instance.StartListening();

// Stop listening
Sui_HttpClient.Instance.StopListening();

// Change poll interval to 5 seconds
Sui_HttpClient.Instance.SetPollInterval(5.0f);
```

### Send Updates from Server to Unity

Using the helper script:
```bash
# Interactive mode
python send_update_to_unity.py

# Command line mode
python send_update_to_unity.py success "Transaction confirmed!"
python send_update_to_unity.py failed "Error occurred" tx-123

# Check pending updates
python send_update_to_unity.py --check
```

Using curl:
```bash
curl -X POST http://localhost:8080/push_update \
  -H "Content-Type: application/json" \
  -d '{"status":"success","message":"Blockchain confirmed!","transactionId":"123"}'
```

## Testing

### Test 1: Basic Transaction
1. Start the Python server
2. Run Unity scene
3. Click on any robot object
4. Enter "test command" in the popup
5. Confirm transaction
6. Check Unity console for success message
7. Check Python server console for received transaction

### Test 2: Status Query
1. After completing a transaction, note the transactionId from logs
2. Call `RequestStatus()` with that ID
3. Verify status is returned

### Test 3: Error Handling
1. Stop the Python server
2. Try to initiate a transaction in Unity
3. Verify error message is displayed in Unity

### Test 4: Server Push Notifications
1. Start the Python server and Unity
2. Run `python send_update_to_unity.py`
3. Choose option 5 to send multiple test messages
4. Verify messages appear in Unity status popup every 2 seconds
5. Try sending custom messages with different statuses

### Test 5: Real-time Updates
1. Keep Unity running with auto-listen enabled
2. In another terminal, run:
   ```bash
   python send_update_to_unity.py success "Blockchain event detected!"
   ```
3. Wait up to 2 seconds (poll interval)
4. Verify the message appears in Unity

## Customization

### Adding Custom Data to Transactions

Modify `TransactionRequest` in `Sui_HttpClient.cs`:

```csharp
[Serializable]
public class TransactionRequest
{
    public string command;
    public string userId;
    public string parameters;
    public string customField;  // Add your fields
}
```

### Changing Server Port

Unity:
```csharp
Sui_HttpClient.Instance.SetServerUrl("http://localhost:9000");
```

Python:
```python
app.run(host='0.0.0.0', port=9000, debug=True)
```

## Troubleshooting

### Connection Refused
- Verify Python server is running
- Check firewall settings
- Ensure port 8080 is not blocked

### Timeout Errors
- Increase `Request Timeout` in Unity Inspector
- Check server performance
- Verify network connectivity

### JSON Parsing Errors
- Ensure server response matches expected format
- Check Unity console for detailed error messages
- Verify all required fields are present in response

## Production Considerations

1. **Security**: Add authentication/authorization
2. **Database**: Replace in-memory storage with database
3. **Blockchain Integration**: Implement actual Sui blockchain calls
4. **Error Handling**: Add retry logic and better error messages
5. **Logging**: Implement comprehensive logging system
6. **HTTPS**: Use HTTPS in production environments
7. **WebSockets**: Consider WebSockets for real-time updates instead of polling
8. **Rate Limiting**: Add rate limiting for polling endpoint
9. **Update Queue Management**: Implement TTL and max queue size for updates
10. **Persistent Updates**: Store updates in database to survive server restarts

## Polling Configuration Guidelines

- **Fast updates (0.5-1s)**: High network traffic, use for critical real-time apps
- **Normal updates (2-5s)**: Balanced, suitable for most applications
- **Slow updates (10-30s)**: Low traffic, use for non-critical notifications

## Architecture Notes

### Why Polling Instead of WebSockets?

This implementation uses HTTP polling for simplicity and compatibility:
- Easier to debug and test
- Works with any HTTP client
- No persistent connection management
- Suitable for moderate update frequency

For production with high-frequency updates, consider upgrading to WebSockets for better performance.
