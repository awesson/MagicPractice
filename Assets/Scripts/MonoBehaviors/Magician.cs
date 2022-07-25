using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Magician : GameView
{
    private const int CARD_COUNT = 5;

    [SerializeField]
    private PlayingCardBehavior[] m_PlayingCards = new PlayingCardBehavior[CARD_COUNT];

    private PlayingCardBehavior FirstEncodedCard { get { return m_PlayingCards[0]; } }
    private PlayingCardBehavior SecondEncodedCard { get { return m_PlayingCards[1]; } }
    private PlayingCardBehavior ThirdEncodedCard { get { return m_PlayingCards[2]; } }
    private PlayingCardBehavior PivotCard { get { return m_PlayingCards[CARD_COUNT - 2]; } }
    private PlayingCardBehavior HiddenCard { get { return m_PlayingCards[CARD_COUNT - 1]; } }

    private PlayingCardDeck m_PlayingCardDeck = new PlayingCardDeck();

    public override void Display()
    {
        // Draw 5 random cards
        m_PlayingCardDeck.Reset();
        PlayingCard[] randomPlayingCards = new PlayingCard[CARD_COUNT];
        for (int i = 0; i < CARD_COUNT; ++i)
        {
            randomPlayingCards[i] = m_PlayingCardDeck.DealNextCard();
        }

        // Find the first pair which share the same suit
        int first = 0;
        int hidden = 1;
        int delta = 0;
        for (int i = 0; i < CARD_COUNT && delta == 0; ++i)
        {
            for (int k = i + 1; k < CARD_COUNT; ++k)
            {
                if (randomPlayingCards[i].MySuit == randomPlayingCards[k].MySuit)
                {
                    delta = randomPlayingCards[i].MyRank - randomPlayingCards[k].MyRank;
                    delta += PlayingCard.MAX_RANK;
                    delta %= PlayingCard.MAX_RANK;
                    if (delta > 6)
                    {
                        first = i;
                        hidden = k;
                        delta = Mathf.Abs(delta - PlayingCard.MAX_RANK);
                    }
                    else
                    {
                        first = k;
                        hidden = i;
                    }
                    break;
                }
            }
        }
        PivotCard.SetCardTo(randomPlayingCards[first]);
        HiddenCard.SetCardTo(randomPlayingCards[hidden]);

        // Order the remaining cards based on if they are the highest, lowest, or middle card and which delta we are encoding.
        var orderedCards = new SortedSet<PlayingCard>(randomPlayingCards.Where((_, i) => i != first && i != hidden));
        var firstInOrder = orderedCards.GetEnumerator();

        // The first card encodes either 0, 2, or 4 depending on if it's low, med, or high.
        while (firstInOrder.MoveNext() && delta > 2)
        {
            delta -= 2;
        }
        FirstEncodedCard.SetCardTo(firstInOrder.Current);

        // The last two cards either encode 1 or 2, depending on if they are sorted or reverse sorted order, respectfully.
        var remainingCards = new List<PlayingCard>(orderedCards.Where(x => x != firstInOrder.Current));
        if (delta > 1)
        {
            remainingCards.Reverse();
        }
        SecondEncodedCard.SetCardTo(remainingCards[0]);
        ThirdEncodedCard.SetCardTo(remainingCards[1]);

        HiddenCard.SetCardHidden(true);
    }

    public void ShowHiddenCard()
    {
        HiddenCard.SetCardHidden(false);
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
