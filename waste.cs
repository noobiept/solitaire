using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace GoldMine
    {
    class Waste : Container
        {
        public Waste()
            {
            this.Background = Brushes.GreenYellow;
            }


        protected override Size ArrangeOverride( Size finalSize )
            {
            int x = 0;
            int step = 20;

            foreach( UIElement child in this.InternalChildren )
                {
                child.Arrange( new Rect( new Point( x, 0 ), child.DesiredSize ) );

                x += step;
                }

            return finalSize;
            }
        }
    }
