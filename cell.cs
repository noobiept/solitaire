using System;
using System.Windows;


namespace Solitaire
    {
    /**
     * A cell can only hold one card.
     */
    class Cell : Container
        {
        protected override Size ArrangeOverride( Size finalSize )
            {
            foreach (UIElement child in this.InternalChildren)
                {
                child.Arrange( new Rect( new Point( 0, 0 ), child.DesiredSize ) );
                }

            return finalSize;
            }
        }
    }
