using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    private Coroutine current;
    private float savedScale = 1f;

    public void Stop(float duration = 0.05f)
    {
        if (current != null) return;
        current = StartCoroutine(Routine(duration));
    }

    private IEnumerator Routine(float duration)
    {
        savedScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        if (!PauseController.IsPaused) Time.timeScale = savedScale;
        current = null;
    }
}
