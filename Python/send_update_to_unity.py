"""
Helper script to send status updates to Unity
This simulates blockchain events or external notifications
"""

import requests
import json
import sys
import time

SERVER_URL = "http://localhost:8080"


def send_update(status, message, transaction_id=""):
    """
    Send a status update to Unity via the server

    Args:
        status: "success", "failed", "pending", "info", etc.
        message: The message to display in Unity
        transaction_id: Optional transaction ID
    """
    url = f"{SERVER_URL}/push_update"

    payload = {
        "status": status,
        "message": message,
        "transactionId": transaction_id
    }

    try:
        response = requests.post(url, json=payload)

        if response.status_code == 200:
            result = response.json()
            print(f"✓ Update sent successfully!")
            print(f"  Status: {status}")
            print(f"  Message: {message}")
            print(f"  Queue length: {result.get('queueLength', 0)}")
            return True
        else:
            print(f"✗ Failed to send update: {response.status_code}")
            print(f"  Response: {response.text}")
            return False

    except requests.exceptions.ConnectionError:
        print(f"✗ Cannot connect to server at {SERVER_URL}")
        print(f"  Make sure the server is running!")
        return False
    except Exception as e:
        print(f"✗ Error: {str(e)}")
        return False


def check_pending_updates():
    """Check how many updates are pending"""
    url = f"{SERVER_URL}/pending_updates"

    try:
        response = requests.get(url)
        if response.status_code == 200:
            data = response.json()
            count = data.get('count', 0)
            print(f"\nPending updates: {count}")
            if count > 0:
                print("Updates in queue:")
                for i, update in enumerate(data.get('updates', []), 1):
                    print(f"  {i}. [{update['status']}] {update['message']}")
        else:
            print(f"✗ Failed to check pending updates: {response.status_code}")
    except Exception as e:
        print(f"✗ Error checking updates: {str(e)}")


def interactive_mode():
    """Interactive mode to send custom updates"""
    print("="*60)
    print("Send Status Updates to Unity - Interactive Mode")
    print("="*60)

    while True:
        print("\nOptions:")
        print("  1. Send success message")
        print("  2. Send failure message")
        print("  3. Send custom message")
        print("  4. Check pending updates")
        print("  5. Send multiple test messages")
        print("  0. Exit")

        choice = input("\nEnter choice: ").strip()

        if choice == "1":
            message = input("Enter success message: ").strip()
            if message:
                send_update("success", message)

        elif choice == "2":
            message = input("Enter failure message: ").strip()
            if message:
                send_update("failed", message)

        elif choice == "3":
            status = input("Enter status (success/failed/info/pending): ").strip()
            message = input("Enter message: ").strip()
            transaction_id = input("Enter transaction ID (optional): ").strip()
            if message:
                send_update(status, message, transaction_id)

        elif choice == "4":
            check_pending_updates()

        elif choice == "5":
            print("\nSending multiple test messages...")
            test_messages = [
                ("success", "Transaction confirmed on blockchain!"),
                ("info", "Processing your request..."),
                ("success", "Robot 1 activated successfully"),
                ("failed", "Connection timeout - please try again"),
                ("success", "All systems operational")
            ]

            for status, message in test_messages:
                send_update(status, message)
                time.sleep(0.5)

            print(f"\n✓ Sent {len(test_messages)} test messages")

        elif choice == "0":
            print("Exiting...")
            break

        else:
            print("Invalid choice!")


def main():
    """Main entry point"""

    if len(sys.argv) > 1:
        # Command line mode
        if sys.argv[1] == "--check":
            check_pending_updates()
        elif len(sys.argv) >= 3:
            status = sys.argv[1]
            message = sys.argv[2]
            transaction_id = sys.argv[3] if len(sys.argv) > 3 else ""
            send_update(status, message, transaction_id)
        else:
            print("Usage:")
            print("  python send_update_to_unity.py                    # Interactive mode")
            print("  python send_update_to_unity.py --check            # Check pending updates")
            print("  python send_update_to_unity.py STATUS MESSAGE [ID]  # Send update")
            print("\nExamples:")
            print('  python send_update_to_unity.py success "Transaction completed!"')
            print('  python send_update_to_unity.py failed "Error occurred" tx-123')
    else:
        # Interactive mode
        interactive_mode()


if __name__ == "__main__":
    main()
