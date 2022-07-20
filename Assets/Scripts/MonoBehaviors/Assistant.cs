using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Assistant : MonoBehaviour
{
    private const int CARD_COUNT = 5;

    [SerializeField]
    private PlayingCardBehavior[] m_PlayingCards = new PlayingCardBehavior[CARD_COUNT];

    [SerializeField]
    private PlayingCardBehavior[] m_UserOrderedCards = new PlayingCardBehavior[CARD_COUNT];

    private PlayingCardBehavior HiddenCard { get { return m_PlayingCards[CARD_COUNT - 1]; } }

    private PlayingCard[] m_CorrectOrderPlayingCards = new PlayingCard[CARD_COUNT];

    public void Display()
    {
        var deck = new PlayingCardDeck();
        for (int i = 0; i < CARD_COUNT; ++i)
        {
            m_PlayingCards[i].SetCardTo(deck.DealNextCard());
        }

        for (int i = 0; i < CARD_COUNT; ++i)
        {
            m_UserOrderedCards[i].SetCardHidden(true);
        }
    }

    public void OnVerifyCardOrderingClicked()
    {
        var hiddenCardRank = m_PlayingCards[0].MyRank;
        var firstOrderedCard = m_PlayingCards[1].MyPlayingCard;
        var secondOrderedCard = m_PlayingCards[2].MyPlayingCard;
        var thirdOrderedCard = m_PlayingCards[3].MyPlayingCard;
        if (firstOrderedCard > secondOrderedCard)
        {
            hiddenCardRank += 2;
        }
        if (firstOrderedCard > thirdOrderedCard)
        {
            hiddenCardRank += 2;
        }
        if (secondOrderedCard > thirdOrderedCard)
        {
            hiddenCardRank += 1;
        }
        hiddenCardRank = hiddenCardRank % PlayingCard.MAX_RANK;
        ++hiddenCardRank;
    }

    private void OnValidate()
    {
        if (m_PlayingCards.Length != CARD_COUNT)
        {
            PlayingCardBehavior[] correctSizeArray = new PlayingCardBehavior[CARD_COUNT];
            for (int i = 0; i < CARD_COUNT && i < m_PlayingCards.Length; ++i)
            {
                correctSizeArray[i] = m_PlayingCards[i];
            }
            m_PlayingCards = correctSizeArray;
        }
    }
}
