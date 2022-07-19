using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Assistant : MonoBehaviour
{
    private const int CARD_COUNT = 5;

    [SerializeField]
    private PlayingCardBehavior[] m_playingCards = new PlayingCardBehavior[CARD_COUNT];
    private PlayingCardBehavior HiddenCard { get { return m_playingCards[CARD_COUNT - 1]; } }

    private PlayingCard[] m_CorrectOrderPlayingCards = new PlayingCard[CARD_COUNT];

    public void Display()
    {
        for (int i = 0; i < CARD_COUNT; ++i)
        {
            var newCard = PlayingCard.GetRandomCard();
            var setPlayingCards = new System.ArraySegment<PlayingCardBehavior>(m_playingCards, 0, i);
            while (setPlayingCards.Any(pc => pc.MyPlayingCard == newCard))
            {
                newCard = PlayingCard.GetRandomCard();
            }
            m_playingCards[i].SetCardTo(newCard);
        }
    }

    public void OnVerifyCardOrderingClicked()
    {
        var hiddenCardRank = m_playingCards[0].MyRank;
        var firstOrderedCard = m_playingCards[1].MyPlayingCard;
        var secondOrderedCard = m_playingCards[2].MyPlayingCard;
        var thirdOrderedCard = m_playingCards[3].MyPlayingCard;
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
        if (m_playingCards.Length != CARD_COUNT)
        {
            PlayingCardBehavior[] correctSizeArray = new PlayingCardBehavior[CARD_COUNT];
            for (int i = 0; i < CARD_COUNT && i < m_playingCards.Length; ++i)
            {
                correctSizeArray[i] = m_playingCards[i];
            }
            m_playingCards = correctSizeArray;
        }
    }
}
