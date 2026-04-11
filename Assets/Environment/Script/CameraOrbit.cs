using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
  [Header("Target Settings")]
  public Transform Target;

  [Header("Orbit Settings")]
  public float OrbitSpeed = 50f;
  public float OrbitDistance = 5f;

  private float _currentAngle;

  private void Start()
  {
    if (Target != null)
    {
      _currentAngle = 0f;
      UpdateOrbitPosition();
    }
  }

  private void Update()
  {
    if (Target == null)
      return;

    if (Input.GetKey(KeyCode.A))
    {
      _currentAngle -= OrbitSpeed * Time.deltaTime;
      UpdateOrbitPosition();
    }
    else if (Input.GetKey(KeyCode.D))
    {
      _currentAngle += OrbitSpeed * Time.deltaTime;
      UpdateOrbitPosition();
    }

    if (Input.GetKeyDown(KeyCode.R))
    {
      ResetPosition();
    }
  }

  private void UpdateOrbitPosition()
  {
    float radians = _currentAngle * Mathf.Deg2Rad;
    float x = Target.position.x + Mathf.Sin(radians) * OrbitDistance;
    float z = Target.position.z + Mathf.Cos(radians) * OrbitDistance;

    transform.position = new Vector3(x, transform.position.y, z);
    transform.LookAt(Target);
  }

  private void ResetPosition()
  {
    _currentAngle = 0f;
    UpdateOrbitPosition();
  }
}
