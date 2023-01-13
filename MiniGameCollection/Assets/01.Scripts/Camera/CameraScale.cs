using UnityEngine;

public class CameraScale : MonoBehaviour
{
    private void Awake()
    {
        float scale = (float)Screen.width / Screen.height;
        float multiply = 0.5625f / scale;

        if (multiply < 1)
            return;

        Camera camera = GetComponent<Camera>();
        camera.orthographicSize *= multiply;
        camera.fieldOfView *= multiply;
    }
}
