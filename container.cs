using System.Collections.Generic;
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
            this.Background = new SolidColorBrush( Color.FromRgb( 87, 129, 50 ) );

            this.dropEffect = new ColorToneEffect();
            this.dropEffect.DarkColor = Colors.Black;
            this.dropEffect.LightColor = Colors.CornflowerBlue;
            }


        /**
         * Get the dimension box of the container.
         */
        public virtual Utilities.Box getDimensionBox()
            {
            var box = new Utilities.Box();

            box.x = Canvas.GetLeft( this );
            box.y = Canvas.GetTop( this );
            box.width = this.ActualWidth;
            box.height = this.ActualHeight;

            return box;
            }


        public Card getLast()
            {
            if ( this.Children.Count > 0 )
                {
                return (Card) this.Children[ this.Children.Count - 1 ];
                }

            return null;
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
