using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace GoldMine
    {
    public class Container : Panel
        {
        public void applyDropEffect()
            {
            this.Opacity = 0.5;
            }


        public void removeDropEffect()
            {
            this.Opacity = 1;
            }


        /**
         * Populate the list with the cards to be dragged. The reference card is one where the drag started.
         */
        public virtual void dragCards( Card refCard, List<Card> cardsDragging )
            {
            cardsDragging.Add( refCard );
            }


        protected override Size MeasureOverride( Size availableSize )
            {
            Size panelDesiredSize = new Size();

            foreach( UIElement child in this.InternalChildren )
                {
                child.Measure( availableSize );
                panelDesiredSize = child.DesiredSize;
                }

            return panelDesiredSize;
            }
        }
    }
