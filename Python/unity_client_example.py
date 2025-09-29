import requests
import json
import time
import cv2
import numpy as np
from io import BytesIO
from PIL import Image

class UnityController:
    def __init__(self, unity_url="http://localhost:8080"):
        self.unity_url = unity_url
        self.command_endpoint = f"{unity_url}/command"
        self.camera_frame_endpoint = f"{unity_url}/camera/frame"
        self.camera_info_endpoint = f"{unity_url}/camera/info"

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

    def get_camera_info(self):
        """Get camera information"""
        try:
            response = requests.get(self.camera_info_endpoint, timeout=2.0)
            if response.status_code == 200:
                return response.json()
            else:
                print(f"✗ Failed to get camera info. Status: {response.status_code}")
                return None
        except requests.exceptions.RequestException as e:
            print(f"✗ Camera info connection error: {e}")
            return None

    def get_camera_frame(self, return_format='opencv'):
        """
        Get latest camera frame from Unity

        Args:
            return_format (str): 'opencv', 'pil', or 'bytes'

        Returns:
            Frame in requested format or None if failed
        """
        try:
            response = requests.get(self.camera_frame_endpoint, timeout=2.0)
            if response.status_code == 200:
                if return_format == 'bytes':
                    return response.content
                elif return_format == 'pil':
                    return Image.open(BytesIO(response.content))
                elif return_format == 'opencv':
                    # Convert JPEG bytes to OpenCV format
                    nparr = np.frombuffer(response.content, np.uint8)
                    frame = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
                    return frame
                else:
                    raise ValueError("return_format must be 'opencv', 'pil', or 'bytes'")
            else:
                print(f"✗ Failed to get camera frame. Status: {response.status_code}")
                return None
        except requests.exceptions.RequestException as e:
            print(f"✗ Camera frame connection error: {e}")
            return None

    def save_camera_frame(self, filename="unity_frame.jpg"):
        """Save current camera frame to file"""
        frame_bytes = self.get_camera_frame(return_format='bytes')
        if frame_bytes:
            with open(filename, 'wb') as f:
                f.write(frame_bytes)
            print(f"✓ Frame saved as {filename}")
            return True
        return False

    def display_camera_feed(self, window_name="Unity Camera Feed"):
        """
        Display live camera feed in OpenCV window
        Press 'q' to quit, 's' to save frame
        """
        print(f"Displaying camera feed. Press 'q' to quit, 's' to save frame")

        frame_count = 0
        while True:
            frame = self.get_camera_frame(return_format='opencv')
            if frame is not None:
                # Add frame counter to display
                cv2.putText(frame, f"Frame: {frame_count}", (10, 30),
                           cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)
                cv2.imshow(window_name, frame)
                frame_count += 1
            else:
                print("No frame received, retrying...")

            key = cv2.waitKey(100) & 0xFF  # 10 FPS display
            if key == ord('q'):
                break
            elif key == ord('s'):
                self.save_camera_frame(f"saved_frame_{int(time.time())}.jpg")

        cv2.destroyAllWindows()

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

        # Test camera functionality
        print("\n" + "=" * 50)
        print("Testing camera functionality...")

        # Get camera info
        camera_info = controller.get_camera_info()
        if camera_info:
            print(f"✓ Camera info: {camera_info}")

        # Save a single frame
        if controller.save_camera_frame("test_frame.jpg"):
            print("✓ Test frame saved")

        # Uncomment to display live feed
        # controller.display_camera_feed()

    except KeyboardInterrupt:
        print("\nStopping...")
        controller.stop()

def camera_feed_example():
    """Example for camera feed only"""
    controller = UnityController()

    print("Camera Feed Example")
    print("Make sure Unity is running with CameraCapture component active")
    print("=" * 50)

    # Get camera info
    info = controller.get_camera_info()
    if info:
        print(f"Camera: {info['width']}x{info['height']} @ {info['framerate']}fps")

    # Display live feed
    controller.display_camera_feed()

if __name__ == "__main__":
    import sys
    if len(sys.argv) > 1 and sys.argv[1] == "camera":
        camera_feed_example()
    else:
        main()