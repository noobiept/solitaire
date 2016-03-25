using System;
using System.Windows;
using System.Collections.Generic;


namespace Solitaire
    {
    public class Waste : Container
        {
        public int shownCount = 3;

        protected override Size ArrangeOverride( Size finalSize )
            {
            double x = 0;
            double step = Math.Round( finalSize.Width * 0.3 );
            int count = this.InternalChildren.Count;

            for( int a = 0 ; a < count ; a++ )
                {
                var child = this.InternalChildren[ a ];

                child.Arrange( new Rect( new Point( x, 0 ), child.DesiredSize ) );

                // the last cards (based on the 'shownCount' value) are place with a x-offset, the others are stacked in each other
                if( a >= count - this.shownCount )
                    {
                    x += step;
                    }
                }

            return finalSize;
            }


        public override bool canDrop( List<Card> cards )
            {
            return false;
            }
        }
    }
