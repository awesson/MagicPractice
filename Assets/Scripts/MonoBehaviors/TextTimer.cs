using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTimer : MonoBehaviour
{
    [SerializeField]
    private Text m_DisplayText = default;

    private bool m_Running = false;
    private float m_AccurredTime = 0f;

    // Start is called before the first frame update
    public void StartStopwatch()
    {
        m_Running = true;
    }

    public void StopStopwatch()
    {
        m_Running = false;
    }

    public void ResetStopwatch()
    {
        m_AccurredTime = 0f;
    }

    public void ToggleVisibility()
    {
        var textRenderer = m_DisplayText?.canvasRenderer;
        if (textRenderer != null)
        {
            textRenderer.cull = !textRenderer.cull;
            m_DisplayText.OnCullingChanged();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Running)
        {
            m_AccurredTime += Time.deltaTime;
        }

        if (m_DisplayText != null)
        {
            m_DisplayText.text = m_AccurredTime.ToString("0.00");
        }
    }
}
