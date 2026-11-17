using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("0 = шар стоїть, 1 = рухається разом із камерою")]
    [SerializeField, Range(0f, 1.5f)] private float parallaxFactor = 0.5f;

    private Transform cam;
    private Vector3 startPosition;
    private Vector3 camStartPosition;

    private void Start()
    {
        cam = Camera.main.transform;
        startPosition = transform.position;
        camStartPosition = cam.position;
    }

    private void LateUpdate()
    {
        Vector3 camDelta = cam.position - camStartPosition;
        transform.position = startPosition + new Vector3(camDelta.x, camDelta.y, 0f) * parallaxFactor;
    }
}
