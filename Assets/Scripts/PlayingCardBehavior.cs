using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct Suit
{
    public enum Type
    {
        Club,
        Diamond,
        Heart,
        Spade
    }

    public const int COUNT = 4;

    public Type MySuit;

    public static implicit operator int(Suit s) => (int)s.MySuit;
    public static implicit operator Type(Suit s) => s.MySuit;

    static public Suit GetRandomSuit()
    {
        return new Suit((Type)Random.Range(0, COUNT));
    }

    public Suit(Type suit)
    {
        MySuit = suit;
    }
}

[System.Serializable]
public struct PlayingCard : System.IComparable<PlayingCard>, System.IEquatable<PlayingCard>
{
    // Ace = 1, King = 13
    public const int MIN_RANK = 1;
    public const int MAX_RANK = 13;

    [SerializeField, Range(MIN_RANK, MAX_RANK)]
    private int m_Rank;
    public int MyRank
    {
        get { return Mathf.Clamp(m_Rank, MIN_RANK, MAX_RANK); }
        set { m_Rank = Mathf.Clamp(value, MIN_RANK, MAX_RANK); }
    }
    public Suit MySuit;

    public int GetDeckValue()
    {
        return MyRank + MySuit * MAX_RANK;
    }

    public int CompareTo(PlayingCard other)
    {
        // The temperature comparison depends on the comparison of
        // the underlying Double values.
        if (MyRank == other.MyRank)
        {
            return ((int)MySuit).CompareTo(other.MySuit);
        }

        return MyRank.CompareTo(other.MyRank);
    }

    public static bool operator >(PlayingCard operand1, PlayingCard operand2)
    {
        return operand1.CompareTo(operand2) > 0;
    }

    // Define the is less than operator.
    public static bool operator <(PlayingCard operand1, PlayingCard operand2)
    {
        return operand1.CompareTo(operand2) < 0;
    }

    // Define the is greater than or equal to operator.
    public static bool operator >=(PlayingCard operand1, PlayingCard operand2)
    {
        return operand1.CompareTo(operand2) >= 0;
    }

    // Define the is less than or equal to operator.
    public static bool operator <=(PlayingCard operand1, PlayingCard operand2)
    {
        return operand1.CompareTo(operand2) <= 0;
    }

    static public PlayingCard GetRandomCard()
    {
        return new PlayingCard
        {
            MyRank = Random.Range(1, MAX_RANK + 1),
            MySuit = Suit.GetRandomSuit()
        };
    }

    public bool Equals(PlayingCard other)
    {
        return CompareTo(other) == 0;
    }

    public override bool Equals(object obj)
    {
        if (obj is PlayingCard otherCard)
        {
            return Equals(otherCard);
        }

        return false;
    }

    public static bool operator ==(PlayingCard lhs, PlayingCard rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(PlayingCard lhs, PlayingCard rhs)
    {
        return !lhs.Equals(rhs);
    }

    public override int GetHashCode() => (MyRank, MySuit).GetHashCode();
}

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

    public int GetCardDeckValue() { return m_PlayingCard.GetDeckValue(); }

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
