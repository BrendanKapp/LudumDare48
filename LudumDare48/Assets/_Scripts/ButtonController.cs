using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private Vector3 startPosition;
    private bool hoverStatus = false;

    public int multiplier = 1;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.localPosition + Vector3.left * 50 * multiplier;
    }
    private void OnEnable()
    {
        Reset();
        Move(-5);
    }
    private void OnDisable()
    {
        Reset();
    }
    public void Move(int direction)
    {
        if (direction == 1)
        {
            if (!hoverStatus)
            {
                hoverStatus = true;
                StartCoroutine(MoveRoutine(direction));
                return;
            }
        } else if (direction == -1)
        {
            if (hoverStatus)
            {
                hoverStatus = false;
                StartCoroutine(MoveRoutine(direction));
                return;
            }
        }
        if (gameObject.activeInHierarchy) StartCoroutine(MoveRoutine(direction));
    }
    public void Reset()
    {
        rectTransform.localPosition = startPosition;
    }
    private IEnumerator MoveRoutine (int direction)
    {
        for (int i = 0; i < 10; i++)
        {
            rectTransform.localPosition += Vector3.left * direction * multiplier;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
