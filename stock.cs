using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;


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
