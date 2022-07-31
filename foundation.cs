using System.Collections.Generic;
using System.Windows;

namespace Solitaire
{
    public class Foundation : Container
    {
        /**
         * A card is droppable if:
         *     - Its an ace and the foundation is empty.
         *     - Its 1 value higher than the last card, and of the same suit.
         * Can only drop 1 card at a time.
         */
        public override bool canDrop(List<Card> cards)
        {
            if (cards.Count == 1)
            {
                var card = cards[0];

                if (card == null)
                {
                    return false;
                }

                var last = this.getLast();

                if (last != null)
                {
                    if (card.value == last.value + 1 && card.suit == last.suit)
                    {
                        return true;
                    }
                }
                else if (card.value == Card.Value.ace)
                {
                    return true;
                }
            }

            return false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in this.InternalChildren)
            {
                child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
            }

            return finalSize;
        }
    }
}
