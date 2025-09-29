import requests
import json
import time

class UnityController:
    def __init__(self, unity_url="http://localhost:8080"):
        self.unity_url = unity_url
        self.command_endpoint = f"{unity_url}/command"

    def send_command(self, movement_x=0.0, movement_y=0.0, is_running=False,
                    is_jumping=False, is_rotating_clockwise=False, is_rotating_anti_clockwise=False):
        """
        Send a command to Unity player controller

        Args:
            movement_x (float): Horizontal movement (-1 to 1, left to right)
            movement_y (float): Vertical movement (-1 to 1, backward to forward)
            is_running (bool): Whether player should run
            is_jumping (bool): Whether player should jump
            is_rotating_clockwise (bool): Whether player should rotate clockwise
            is_rotating_anti_clockwise (bool): Whether player should rotate anti-clockwise
        """
        command = {
            "MovementX": movement_x,
            "MovementY": movement_y,
            "IsRunning": is_running,
            "IsJumping": is_jumping,
            "IsRotatingClockwise": is_rotating_clockwise,
            "IsRotatingAntiClockwise": is_rotating_anti_clockwise
        }

        try:
            response = requests.post(self.command_endpoint, json=command, timeout=1.0)
            if response.status_code == 200:
                print(f"✓ Command sent successfully: {command}")
                return True
            else:
                print(f"✗ Failed to send command. Status: {response.status_code}")
                return False
        except requests.exceptions.RequestException as e:
            print(f"✗ Connection error: {e}")
            return False

    def move_forward(self, duration=1.0, running=False):
        """Move forward for specified duration"""
        print(f"Moving forward for {duration} seconds (running: {running})")
        self.send_command(movement_y=1.0, is_running=running)
        time.sleep(duration)
        self.stop()

    def move_backward(self, duration=1.0, running=False):
        """Move backward for specified duration"""
        print(f"Moving backward for {duration} seconds (running: {running})")
        self.send_command(movement_y=-1.0, is_running=running)
        time.sleep(duration)
        self.stop()

    def move_right(self, duration=1.0, running=False):
        """Move right for specified duration"""
        print(f"Moving right for {duration} seconds (running: {running})")
        self.send_command(movement_x=1.0, is_running=running)
        time.sleep(duration)
        self.stop()

    def move_left(self, duration=1.0, running=False):
        """Move left for specified duration"""
        print(f"Moving left for {duration} seconds (running: {running})")
        self.send_command(movement_x=-1.0, is_running=running)
        time.sleep(duration)
        self.stop()

    def jump(self):
        """Make player jump"""
        print("Jumping")
        self.send_command(is_jumping=True)
        time.sleep(0.1)  # Brief moment for jump registration
        self.send_command(is_jumping=False)

    def rotate_clockwise(self, duration=1.0):
        """Rotate clockwise for specified duration"""
        print(f"Rotating clockwise for {duration} seconds")
        self.send_command(is_rotating_clockwise=True)
        time.sleep(duration)
        self.stop_rotation()

    def rotate_anti_clockwise(self, duration=1.0):
        """Rotate anti-clockwise for specified duration"""
        print(f"Rotating anti-clockwise for {duration} seconds")
        self.send_command(is_rotating_anti_clockwise=True)
        time.sleep(duration)
        self.stop_rotation()

    def stop(self):
        """Stop all movement"""
        print("Stopping movement")
        self.send_command()

    def stop_rotation(self):
        """Stop rotation only"""
        print("Stopping rotation")
        self.send_command(is_rotating_clockwise=False, is_rotating_anti_clockwise=False)

def main():
    # Create controller instance
    controller = UnityController()

    print("Unity Player Controller - Python Client")
    print("Make sure Unity is running with HttpServer component active")
    print("=" * 50)

    # Test sequence
    try:
        # Test basic movement
        controller.move_forward(2.0)
        time.sleep(0.5)

        controller.move_right(1.5)
        time.sleep(0.5)

        controller.move_backward(2.0, running=True)  # Run backward
        time.sleep(0.5)

        controller.move_left(1.5, running=True)  # Run left
        time.sleep(0.5)

        # Test jumping
        controller.jump()
        time.sleep(1.0)

        # Test rotation
        controller.rotate_clockwise(2.0)
        time.sleep(0.5)

        controller.rotate_anti_clockwise(2.0)
        time.sleep(0.5)

        # Test complex movement (diagonal + running)
        print("Complex movement: Running diagonally forward-right")
        controller.send_command(movement_x=0.7, movement_y=0.7, is_running=True)
        time.sleep(2.0)
        controller.stop()

        print("Test sequence completed!")

    except KeyboardInterrupt:
        print("\nStopping...")
        controller.stop()

if __name__ == "__main__":
    main()