using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingCardDeck
{
    public const int NUM_CARDS = (PlayingCard.MAX_RANK - PlayingCard.MIN_RANK + 1) * Suit.COUNT;

    private List<PlayingCard> m_RemainingCards = new List<PlayingCard>();
    private List<PlayingCard> m_DealtCards = new List<PlayingCard>();

    public PlayingCardDeck()
    {
        m_RemainingCards.Capacity = NUM_CARDS;
        m_DealtCards.Capacity = NUM_CARDS;

        for (int s = 0; s < Suit.COUNT; ++s)
        {
            for (int r = PlayingCard.MIN_RANK; r <= PlayingCard.MAX_RANK; ++r)
            {
                var card = new PlayingCard
                {
                    MyRank = r,
                    MySuit = s
                };
                m_RemainingCards.Add(card);
            }
        }
    }

    public void Reset()
    {
        m_RemainingCards.AddRange(m_DealtCards);
        m_DealtCards.Clear();
    }

    public PlayingCard DealNextCard()
    {
        int chosenIndex = Random.Range(0, m_RemainingCards.Count);
        PlayingCard chosenCard = m_RemainingCards[chosenIndex];
        m_RemainingCards[chosenIndex] = m_RemainingCards[m_RemainingCards.Count - 1];
        m_RemainingCards.RemoveAt(m_RemainingCards.Count - 1);
        m_DealtCards.Add(chosenCard);
        return chosenCard;
    }
}
