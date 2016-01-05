using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Expression.Media.Effects;


namespace GoldMine
    {
    public class Container : Panel
        {
        private ColorToneEffect dropEffect;


        public Container()
            {
            this.dropEffect = new ColorToneEffect();
            this.dropEffect.DarkColor = Colors.Black;
            this.dropEffect.LightColor = Colors.CornflowerBlue;
            }


        public void applyDropEffect()
            {
            this.Effect = this.dropEffect;
            }


        public void removeDropEffect()
            {
            this.Effect = null;
            }


        /**
         * Says if the given cards can be dropped unto this container or not.
         */
        public virtual bool canDrop( List<Card> cards )
            {
            return false;
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
