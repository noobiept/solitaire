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
            int count = this.InternalChildren.Count;

            for (int a = 0 ; a < count ; a++)
                {
                var child = this.InternalChildren[ a ];

                child.Arrange( new Rect( new Point( x, 0 ), child.DesiredSize ) );

                    // the last 3 cards are place with a x-offset, the others are stacked in each other
                if ( a >= count - 3 )
                    {
                    x += step;
                    }
                }

            return finalSize;
            }
        }
    }
