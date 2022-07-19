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

    private bool m_IsHidden = false;

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

    private void Start()
    {
        if (!m_TextField)
        {
            Debug.LogWarning("m_TextField not set - card wont update display");
        }

        UpdateTextField();
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
        switch(MySuit.MySuit)
        {
            case Suit.Type.Club:
            case Suit.Type.Spade:
                return Color.black;
            case Suit.Type.Diamond:
            case Suit.Type.Heart:
                return Color.red;
            default:
                return Color.black;
        }
    }
}
