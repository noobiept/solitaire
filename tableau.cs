using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace GoldMine
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
                if ( card == refCard )
                    {
                    reached = true;
                    }

                if ( reached )
                    {
                    cardsDragging.Add( card );
                    }
                }
            }


        /**
         * A card is droppable if either the tableau is empty, or if the last card in the tableau is one value above the first card being dropped, and they have alternating colors.
         */
        public override bool canDrop( List<Card> cards )
            {
            if ( this.Children.Count > 0 )
                {
                Card lastTableau = (Card) this.Children[ this.Children.Count - 1 ];
                Card firstDrag = cards[ 0 ];

                if ( lastTableau.value - 1 == firstDrag.value &&
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
            int y = 0;
            int step = 25;

            foreach( UIElement child in this.InternalChildren )
                {
                child.Arrange( new Rect( new Point( 0, y ), child.DesiredSize ) );

                y += step;
                }

            return finalSize;
            }
        }
    }
