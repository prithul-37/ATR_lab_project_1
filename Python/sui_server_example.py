"""
Example HTTP Server for Sui Crypto System
Receives transaction requests from Unity and sends status updates
"""

from flask import Flask, request, jsonify
import json
import uuid
from datetime import datetime

app = Flask(__name__)

# Store transactions in memory (use database in production)
transactions = {}

# Queue for pending status updates to send to Unity
pending_updates = []

@app.route('/transaction', methods=['POST'])
def handle_transaction():
    """
    Receives transaction requests from Unity
    Expected JSON format:
    {
        "command": "move robot",
        "userId": "Robot1",
        "parameters": "additional data"
    }
    """
    try:
        data = request.get_json()

        # Extract transaction details
        command = data.get('command', '')
        user_id = data.get('userId', '')
        parameters = data.get('parameters', '')

        print(f"\n[TRANSACTION RECEIVED]")
        print(f"Command: {command}")
        print(f"User ID: {user_id}")
        print(f"Parameters: {parameters}")

        # Generate transaction ID
        transaction_id = str(uuid.uuid4())

        # Store transaction
        transactions[transaction_id] = {
            'command': command,
            'userId': user_id,
            'parameters': parameters,
            'status': 'processing',
            'timestamp': datetime.now().isoformat()
        }

        # Simulate transaction processing
        # In production, this would interact with blockchain
        success = True  # Simulate success

        if success:
            transactions[transaction_id]['status'] = 'completed'
            response = {
                'success': True,
                'message': f'Transaction completed successfully for {user_id}',
                'transactionId': transaction_id,
                'data': f'Processed command: {command}'
            }
            print(f"[SUCCESS] Transaction ID: {transaction_id}")
        else:
            transactions[transaction_id]['status'] = 'failed'
            response = {
                'success': False,
                'message': 'Transaction failed',
                'transactionId': transaction_id,
                'data': ''
            }
            print(f"[FAILED] Transaction ID: {transaction_id}")

        return jsonify(response), 200

    except Exception as e:
        print(f"[ERROR] {str(e)}")
        return jsonify({
            'success': False,
            'message': f'Server error: {str(e)}',
            'transactionId': '',
            'data': ''
        }), 500


@app.route('/status', methods=['GET'])
def get_status():
    """
    Returns status of a specific transaction
    Query parameter: transactionId
    """
    try:
        transaction_id = request.args.get('transactionId', '')

        if transaction_id in transactions:
            transaction = transactions[transaction_id]
            response = {
                'status': transaction['status'],
                'message': f"Transaction {transaction['status']}",
                'transactionId': transaction_id
            }
            print(f"\n[STATUS REQUEST] Transaction {transaction_id}: {transaction['status']}")
            return jsonify(response), 200
        else:
            response = {
                'status': 'not_found',
                'message': 'Transaction not found',
                'transactionId': transaction_id
            }
            return jsonify(response), 404

    except Exception as e:
        print(f"[ERROR] {str(e)}")
        return jsonify({
            'status': 'error',
            'message': f'Server error: {str(e)}',
            'transactionId': ''
        }), 500


@app.route('/updates', methods=['GET'])
def get_updates():
    """
    Returns pending status updates for Unity client
    Unity polls this endpoint periodically
    """
    global pending_updates

    try:
        if len(pending_updates) > 0:
            # Get the oldest update
            update = pending_updates.pop(0)
            print(f"\n[UPDATE SENT] Status: {update['status']}, Message: {update['message']}")
            return jsonify(update), 200
        else:
            # No updates available - return empty
            return jsonify({}), 200

    except Exception as e:
        print(f"[ERROR] {str(e)}")
        return jsonify({
            'status': 'error',
            'message': f'Server error: {str(e)}',
            'transactionId': ''
        }), 500


@app.route('/push_update', methods=['POST'])
def push_update():
    """
    Manually push a status update to Unity
    Used for testing or for blockchain events
    Expected JSON format:
    {
        "status": "success",
        "message": "Your transaction was confirmed",
        "transactionId": "optional-id"
    }
    """
    global pending_updates

    try:
        data = request.get_json()

        status = data.get('status', 'info')
        message = data.get('message', '')
        transaction_id = data.get('transactionId', '')

        update = {
            'status': status,
            'message': message,
            'transactionId': transaction_id
        }

        pending_updates.append(update)

        print(f"\n[UPDATE QUEUED]")
        print(f"Status: {status}")
        print(f"Message: {message}")
        print(f"Transaction ID: {transaction_id}")
        print(f"Queue length: {len(pending_updates)}")

        return jsonify({
            'success': True,
            'message': 'Update queued successfully',
            'queueLength': len(pending_updates)
        }), 200

    except Exception as e:
        print(f"[ERROR] {str(e)}")
        return jsonify({
            'success': False,
            'message': f'Server error: {str(e)}'
        }), 500


@app.route('/transactions', methods=['GET'])
def list_transactions():
    """
    Lists all transactions (for debugging)
    """
    return jsonify({
        'count': len(transactions),
        'transactions': transactions
    }), 200


@app.route('/pending_updates', methods=['GET'])
def list_pending_updates():
    """
    Lists all pending updates (for debugging)
    """
    return jsonify({
        'count': len(pending_updates),
        'updates': pending_updates
    }), 200


if __name__ == '__main__':
    print("="*50)
    print("Sui Crypto System - HTTP Server")
    print("="*50)
    print("\nEndpoints:")
    print("  POST /transaction     - Receive transaction requests")
    print("  GET  /status          - Get transaction status")
    print("  GET  /updates         - Get pending updates (polled by Unity)")
    print("  POST /push_update     - Push status update to Unity")
    print("  GET  /transactions    - List all transactions")
    print("  GET  /pending_updates - List all pending updates")
    print("\nServer starting on http://localhost:8080")
    print("="*50)
    print("\nExample: Push update to Unity using curl:")
    print('  curl -X POST http://localhost:8080/push_update -H "Content-Type: application/json" -d "{\\"status\\":\\"success\\",\\"message\\":\\"Blockchain confirmed!\\",\\"transactionId\\":\\"123\\"}"')
    print("="*50)

    app.run(host='0.0.0.0', port=8080, debug=True)
