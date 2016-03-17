using System.Windows;


namespace Solitaire
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
