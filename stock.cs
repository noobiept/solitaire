using System;
using System.Collections.Generic;
using System.Windows;


namespace Solitaire
    {
    public class Stock : Container
        {
        protected override Size ArrangeOverride( Size finalSize )
            {
            foreach( UIElement child in this.InternalChildren )
                {
                child.Arrange( new Rect( new Point( 0, 0 ), child.DesiredSize ) );
                }

            return finalSize;
            }


        public override bool canDrop( List<Card> cards )
            {
            return false;
            }
        }
    }
