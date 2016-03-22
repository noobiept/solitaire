using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace Solitaire
    {
    public class Tableau : Container
        {
        /**
         * Drag all cards starting from the reference card.
         */
        public override void dragCards( Card refCard, List<Card> cardsDragging )
            {
            var reached = false;

            foreach( Card card in this.Children )
                {
                if( card == refCard )
                    {
                    reached = true;
                    }

                if( reached )
                    {
                    cardsDragging.Add( card );
                    }
                }
            }


        /**
         * Get the dimension box of the container (also considers its children).
         */
        public override Utilities.Box getDimensionBox()
            {
            var box = new Utilities.Box {
                x = Canvas.GetLeft( this ),
                y = Canvas.GetTop( this ),
                width = this.ActualWidth,
                height = this.ActualHeight
                };

            var lastCard = this.getLast();

            // the last card may be outside the container dimensions, so need to consider that
            if( lastCard != null )
                {
                var point = lastCard.TranslatePoint( new Point( 0, 0 ), this );
                var combinedHeight = point.Y + lastCard.ActualHeight;

                if( combinedHeight > box.height )
                    {
                    box.height = combinedHeight;
                    }
                }

            return box;
            }


        /**
         * A card is droppable if either the tableau is empty, or if the last card in the tableau is one value above the first card being dropped, and they have alternating colors.
         */
        public override bool canDrop( List<Card> cards )
            {
            var lastTableau = this.getLast();

            if( lastTableau != null )
                {
                Card firstDrag = cards[ 0 ];

                if( lastTableau.value - 1 == firstDrag.value &&
                     lastTableau.color != firstDrag.color )
                    {
                    return true;
                    }

                else
                    {
                    return false;
                    }
                }

            // its empty, so any card is valid
            else
                {
                return true;
                }
            }


        protected override Size ArrangeOverride( Size finalSize )
            {
            double availableHeight = 1.4 * finalSize.Height;
            double y = 0;
            double step = Math.Round( availableHeight * 0.15 );
            int count = this.InternalChildren.Count;
            double neededHeight = count * step;

            // if there's no space available for the default step, then calculate the possible step
            if( neededHeight > availableHeight )
                {
                step = Math.Floor( availableHeight / count );
                }

            foreach( UIElement child in this.InternalChildren )
                {
                child.Arrange( new Rect( new Point( 0, y ), child.DesiredSize ) );

                y += step;
                }

            return finalSize;
            }
        }
    }
