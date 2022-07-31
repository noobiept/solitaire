using System;
using System.Collections.Generic;
using System.Windows;

namespace Solitaire
{
    /**
     * A cell can only hold one card.
     */
    public class Cell : Container
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in this.InternalChildren)
            {
                child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
            }

            return finalSize;
        }

        public override bool canDrop(List<Card> cards)
        {
            if (this.Children.Count == 0 && cards.Count == 1)
            {
                return true;
            }

            return false;
        }
    }
}
