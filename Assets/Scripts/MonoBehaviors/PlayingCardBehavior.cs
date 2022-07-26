using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayingCardBehavior : MonoBehaviour
{
    [SerializeField]
    private PlayingCard m_PlayingCard = new PlayingCard();
    [SerializeField]
    private TextMeshProUGUI m_TextField = default;

    private Image m_HitBox = null;

    private bool m_IsHidden = false;
    public bool IsHidden { get { return m_IsHidden; } }

    public int MyRank
    {
        get { return m_PlayingCard.MyRank; }
        private set { m_PlayingCard.MyRank = value; }
    }

    public Suit MySuit
    {
        get { return m_PlayingCard.MySuit; }
        private set { m_PlayingCard.MySuit = value; }
    }

    public PlayingCard MyPlayingCard { get { return m_PlayingCard; } }

    private bool m_IsUsed = false;
    public bool IsUsed { get { return m_IsUsed; } }

    public void SetCardTo(int rank, Suit suit)
    {
        MyRank = rank;
        MySuit = suit;
        UpdateTextField();
    }

    public void SetCardTo(PlayingCard card)
    {
        m_PlayingCard = card;
        UpdateTextField();
    }

    public void SetCardHidden(bool hide)
    {
        m_IsHidden = hide;
        UpdateTextField();
    }

    public void SetCardAsUsed(bool used)
    {
        m_IsUsed = used;
        UpdateTextField();
    }

    private void Awake()
    {
        if (m_TextField)
        {
            m_HitBox = m_TextField.GetComponentInChildren<Image>();
            if (!m_HitBox)
            {
                var imageGO = new GameObject();
                imageGO.transform.SetParent(m_TextField.transform);
                m_HitBox = imageGO.AddComponent<Image>();
                m_HitBox.color = new Color(0, 0, 0, 0);
            }
        }
    }

    private void Start()
    {
        if (!m_TextField)
        {
            Debug.LogWarning("m_TextField not set - card wont update display");
        }

        UpdateTextField();
    }

    private Rect GetTextExtents()
    {
        if (!m_TextField)
        {
            return new Rect();
        }

        // The TMP text and mesh boundary is slightly smaller than the actual text
        // so we use the line extents which matches better.
        int lineCount = m_TextField.textInfo.lineCount;
        var lineInfos = m_TextField.textInfo.lineInfo;

        Vector2 minExtents = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 maxExtents = new Vector2(float.MinValue, float.MinValue);
        for (int i = 0; i < lineCount; i++)
        {
            var lineExtents = lineInfos[i].lineExtents;
            minExtents.x = Mathf.Min(minExtents.x, lineExtents.min.x);
            minExtents.y = Mathf.Min(minExtents.y, lineExtents.min.y);
            maxExtents.x = Mathf.Max(maxExtents.x, lineExtents.max.x);
            maxExtents.y = Mathf.Max(maxExtents.y, lineExtents.max.y);
        }

        Rect rect = new Rect();
        rect.min = minExtents;
        rect.max = maxExtents;
        return rect;
    }

    private void UpdateTextField()
    {
        if (m_TextField)
        {
            if (m_IsHidden)
            {
                m_TextField.text = "\U0001F0A0";
                m_TextField.color = Color.black;
            }
            else
            {
                m_TextField.text = CalculateUnicode();
                m_TextField.color = CalculateTextColor();
            }

            // In order to be able to get the new text info, we need to force the mesh update now
            m_TextField.ForceMeshUpdate();

            Rect extents = GetTextExtents();
            // Set the size first - this can do strange things to the position and scale
            m_HitBox.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, extents.width);
            m_HitBox.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, extents.height);
            m_HitBox.rectTransform.ForceUpdateRectTransforms();
            // After forcing the transform to be updated with the new size, we can set the correct position and reset the scale
            Vector3 worldTextCenter = m_TextField.transform.TransformPoint(extents.center);
            m_HitBox.rectTransform.position = worldTextCenter;
            m_HitBox.rectTransform.localScale = Vector3.one;
        }
    }

    private string CalculateUnicode()
    {
        int value = 127137 + (MyRank - 1) + 16 * (Suit.COUNT - MySuit - 1);
        if (MyRank > 11)
        {
            value += 1;
        }
        return char.ConvertFromUtf32(value).ToString();
    }

    private Color CalculateTextColor()
    {
        Color c = Color.black;
        switch(MySuit.MySuit)
        {
            case Suit.Type.Club:
            case Suit.Type.Spade:
                c = Color.black;
                break;
            case Suit.Type.Diamond:
            case Suit.Type.Heart:
                c = Color.red;
                break;
            default:
                c =  Color.black;
                break;
        }

        if (m_IsUsed)
        {
            c.a = 0.5f;
        }

        return c;
    }
}
