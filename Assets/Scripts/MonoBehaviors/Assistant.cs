using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Assistant : GameView
{
    private const int CARD_COUNT = 5;

    [SerializeField]
    private PlayingCardBehavior[] m_PlayingCards = new PlayingCardBehavior[CARD_COUNT];

    [SerializeField]
    private PlayingCardBehavior[] m_UserOrderedCards = new PlayingCardBehavior[CARD_COUNT];

    private PlayingCardBehavior HiddenCard { get { return m_PlayingCards[CARD_COUNT - 1]; } }

    private PlayingCard[] m_CorrectOrderPlayingCards = new PlayingCard[CARD_COUNT];

    private Vector3 m_DraggedCardOriginalPos;

    public override void Display()
    {
        var deck = new PlayingCardDeck();
        for (int i = 0; i < CARD_COUNT; ++i)
        {
            m_PlayingCards[i].SetCardTo(deck.DealNextCard());
            m_PlayingCards[i].GetComponent<DragAndDrop>().IsValidDropTarget = false;
        }

        for (int i = 0; i < CARD_COUNT; ++i)
        {
            SetCardHidden(m_UserOrderedCards[i], true);
        }
    }

    private void SetCardHidden(PlayingCardBehavior card, bool hide)
    {
        card.SetCardHidden(hide);
        card.GetComponent<DragAndDrop>().IsValidDragTarget = !hide;
    }

    public void OnCardDragStart(DragAndDrop card)
    {
        m_DraggedCardOriginalPos = card.transform.position;
    }

    public void OnCardDragEnd(DragAndDrop card)
    {
        card.transform.position = m_DraggedCardOriginalPos;
    }

    public void OnCardDroppedOnto(DragAndDrop draggedObj, DragAndDrop dropTarget)
    {
        var draggedCard = draggedObj.GetComponent<PlayingCardBehavior>();
        var dropTargetCard = dropTarget.GetComponent<PlayingCardBehavior>();
        if (draggedCard == null || dropTargetCard == null)
        {
            return;
        }

        bool draggedCardIsDeckCard = m_PlayingCards.Contains(draggedCard);

        if (dropTargetCard.IsHidden || draggedCardIsDeckCard)
        {
            dropTargetCard.SetCardTo(draggedCard.MyPlayingCard);
            SetCardHidden(dropTargetCard, false);
        }
        else // dragged card is user card and the target already has a value
        {
            var dropTargetCardValue = dropTargetCard.MyPlayingCard;
            dropTargetCard.SetCardTo(draggedCard.MyPlayingCard);
            draggedCard.SetCardTo(dropTargetCardValue);
        }
    }

    public void OnVerifyCardOrderingClicked()
    {
        if (m_UserOrderedCards[0].MySuit != m_UserOrderedCards[4].MySuit)
        {
            Debug.Log("wrong suit!");
            return;
        }

        var hiddenCardRank = m_UserOrderedCards[0].MyRank;
        var firstOrderedCard = m_UserOrderedCards[1].MyPlayingCard;
        var secondOrderedCard = m_UserOrderedCards[2].MyPlayingCard;
        var thirdOrderedCard = m_UserOrderedCards[3].MyPlayingCard;
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

        if (m_UserOrderedCards[4].MyRank == hiddenCardRank)
        {
            Debug.Log("CORRECT!");
        }
        else
        {
            Debug.Log("Wrong rank! Currently encoded for the last card to be rank " + hiddenCardRank);
        }

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
