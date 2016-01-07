using System.Windows;


namespace GoldMine
    {
    class Stock : Container
        {
        protected override Size ArrangeOverride( Size finalSize )
            {
            foreach( UIElement child in this.InternalChildren )
                {
                child.Arrange( new Rect( new Point( 0, 0 ), child.DesiredSize ) );
                }

            return finalSize;
            }
        }
    }
