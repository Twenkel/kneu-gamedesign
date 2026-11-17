using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    private Coroutine current;

    public void Stop(float duration = 0.05f)
    {
        if (current != null) return;
        current = StartCoroutine(Routine(duration));
    }

    private IEnumerator Routine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        current = null;
    }
}
