using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Assistant : GameView
{
    private const int CARD_COUNT = 5;

    [SerializeField]
    private PlayingCardBehavior[] m_PlayingCards = new PlayingCardBehavior[CARD_COUNT];

    [SerializeField]
    private PlayingCardBehavior[] m_UserOrderedCards = new PlayingCardBehavior[CARD_COUNT];

    [SerializeField]
    private Text m_MessageText = default;

    private PlayingCardBehavior FirstEncodedCard { get { return m_UserOrderedCards[0]; } }
    private PlayingCardBehavior SecondEncodedCard { get { return m_UserOrderedCards[1]; } }
    private PlayingCardBehavior ThirdEncodedCard { get { return m_UserOrderedCards[2]; } }
    private PlayingCardBehavior ShownSuitCard { get { return m_UserOrderedCards[CARD_COUNT - 2]; } }
    private PlayingCardBehavior HiddenCard { get { return m_UserOrderedCards[CARD_COUNT - 1]; } }

    private PlayingCard[] m_CorrectOrderPlayingCards = new PlayingCard[CARD_COUNT];

    private Vector3 m_DraggedCardOriginalPos;

    public override void Display()
    {
        var deck = new PlayingCardDeck();
        for (int i = 0; i < CARD_COUNT; ++i)
        {
            m_PlayingCards[i].SetCardTo(deck.DealNextCard());
            m_PlayingCards[i].SetCardAsUsed(false);
            m_PlayingCards[i].GetComponent<DragAndDrop>().IsValidDropTarget = false;
        }

        for (int i = 0; i < CARD_COUNT; ++i)
        {
            SetCardHidden(m_UserOrderedCards[i], true);
        }

        if (m_MessageText)
        {
            m_MessageText.text = "Order the cards, then press Verify";
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
        card.GetComponentInChildren<Image>().enabled = false;
    }

    public void OnCardDragEnd(DragAndDrop card)
    {
        card.transform.position = m_DraggedCardOriginalPos;
        card.GetComponentInChildren<Image>().enabled = true;
    }

    public void OnCardDroppedOnto(DragAndDrop draggedObj, DragAndDrop dropTarget)
    {
        var draggedCard = draggedObj.GetComponent<PlayingCardBehavior>();
        var dropTargetCard = dropTarget.GetComponent<PlayingCardBehavior>();
        if (draggedCard == null || dropTargetCard == null)
        {
            Debug.Log("dropped onto something, but it's not a card....");
            return;
        }

        bool draggedCardIsDeckCard = m_PlayingCards.Contains(draggedCard);

        if (dropTargetCard.IsHidden || draggedCardIsDeckCard)
        {
            Debug.Log("consuming value of dragged card only, " + draggedCard.MyPlayingCard);
            dropTargetCard.SetCardTo(draggedCard.MyPlayingCard);
            SetCardHidden(dropTargetCard, false);

            // Avoid duplicating cards in the user area
            if (!draggedCardIsDeckCard)
            {
                // Dragged a face up user card onto a face down user card.
                // Besides setting the drop target to the new value,
                // we need to hide where it came from so we don't duplicate user cards.
                SetCardHidden(draggedCard, true);
            }
            else // dragged card is a deck card
            {
                // If that deck card is in the user area (besides where we just set it), hide it
                for (int i = 0; i < CARD_COUNT; ++i)
                {
                    if (m_UserOrderedCards[i] == dropTargetCard)
                    {
                        continue;
                    }
                    
                    if( m_UserOrderedCards[i].MyPlayingCard == draggedCard.MyPlayingCard)
                    {
                        SetCardHidden(m_UserOrderedCards[i], true);
                    }
                }
            }
        }
        else // dragged card is user card and the target already has a value
        {
            Debug.Log("Swapping values of the cards, ");
            var dropTargetCardValue = dropTargetCard.MyPlayingCard;
            dropTargetCard.SetCardTo(draggedCard.MyPlayingCard);
            draggedCard.SetCardTo(dropTargetCardValue);
        }

        UpdatePlacedCardsUsedState();
    }

    private void UpdatePlacedCardsUsedState()
    {
        for (int i = 0; i < CARD_COUNT; ++i)
        {
            var deckCard = m_PlayingCards[i];
            bool isUsed = false;
            for (int k = 0; k < CARD_COUNT; ++k)
            {
                var userCard = m_UserOrderedCards[k];
                if (!userCard.IsHidden && deckCard.MyPlayingCard == userCard.MyPlayingCard)
                {
                    isUsed = true;
                    break;
                }
            }

            deckCard.SetCardAsUsed(isUsed);
        }
    }

    public void OnVerifyCardOrderingClicked()
    {
        if (!m_MessageText)
        {
            return;
        }

        if (m_UserOrderedCards.Any(card => card.IsHidden))
        {
            m_MessageText.text = "Place all the cards first!";
            return;
        }

        if (ShownSuitCard.MySuit != HiddenCard.MySuit)
        {
            m_MessageText.text = "wrong suit!";
            return;
        }

        var hiddenCardRank = ShownSuitCard.MyRank;
        var firstOrderedCard = FirstEncodedCard.MyPlayingCard;
        var secondOrderedCard = SecondEncodedCard.MyPlayingCard;
        var thirdOrderedCard = ThirdEncodedCard.MyPlayingCard;
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

        if (HiddenCard.MyRank == hiddenCardRank)
        {
            m_MessageText.text = "CORRECT!";
        }
        else
        {
            m_MessageText.text = "Wrong rank! Currently encoded for the hidden card to be rank " + hiddenCardRank;
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
