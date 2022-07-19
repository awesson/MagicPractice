using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Magician : MonoBehaviour
{
    private const int CARD_COUNT = 5;

    [SerializeField]
    private PlayingCardBehavior[] m_playingCards = new PlayingCardBehavior[CARD_COUNT];
    private PlayingCardBehavior HiddenCard { get { return m_playingCards[CARD_COUNT - 1]; } }

    public void Display()
    {
        PlayingCard[] randomPlayingCards = new PlayingCard[CARD_COUNT];
        for (int i = 0; i < CARD_COUNT; ++i)
        {
            var newCard = PlayingCard.GetRandomCard();
            var setPlayingCards = new System.ArraySegment<PlayingCard>(randomPlayingCards, 0, i);
            while (setPlayingCards.Any(pc => pc == newCard))
            {
                newCard = PlayingCard.GetRandomCard();
            }
            randomPlayingCards[i] = newCard;
        }

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
        m_playingCards[0].SetCardTo(randomPlayingCards[first]);
        HiddenCard.SetCardTo(randomPlayingCards[hidden]);

        // Order the other cards and check which one should go first
        var orderedCards = new SortedSet<PlayingCard>(randomPlayingCards.Where((_, i) => i != first && i != hidden));
        var firstInOrder = orderedCards.GetEnumerator();
        // The first card encodes either 0, 2, or 4 depending on if it's low, med, or high.
        while (firstInOrder.MoveNext() && delta > 2)
        {
            delta -= 2;
        }
        m_playingCards[1].SetCardTo(firstInOrder.Current);
        // The last two cards either encode 1 or 2, depending on if they are sorted or reverse sorted order, respectfully.
        var remainingCards = new List<PlayingCard>(orderedCards.Where(x => x != firstInOrder.Current));
        if (delta > 1)
        {
            remainingCards.Reverse();
        }
        m_playingCards[2].SetCardTo(remainingCards[0]);
        m_playingCards[3].SetCardTo(remainingCards[1]);

        HiddenCard.SetCardHidden(true);
    }

    public void ShowHiddenCard()
    {
        HiddenCard.SetCardHidden(false);
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
