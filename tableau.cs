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
        public Tableau()
            {
            this.Background = Brushes.BlueViolet;
            }


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


        protected override Size ArrangeOverride( Size finalSize )
            {
            int y = 0;
            int step = 20;

            foreach( UIElement child in this.InternalChildren )
                {
                child.Arrange( new Rect( new Point( 0, y ), child.DesiredSize ) );

                y += step;
                }

            return finalSize;
            }
        }
    }
