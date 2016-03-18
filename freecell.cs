using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Solitaire
    {
    class FreeCell : SolitaireGame
        {
        private Canvas canvas;
        private readonly List<Foundation> foundations = new List<Foundation>();
        private readonly List<Container> droppableElements = new List<Container>();


        public FreeCell( Canvas mainCanvas, StackPanel customButtons, StackPanel customInfo )
            {
            this.canvas = mainCanvas;

            for (int a = 0 ; a < 4 ; a++)
                {
                var foundation = new Foundation();

                this.canvas.Children.Add( foundation );
                this.droppableElements.Add( foundation );
                this.foundations.Add( foundation );
                }
            }


        public override void addMenuElements( StackPanel container )
            {
            
            }


        public override void addInfoElements( StackPanel container )
            {

            }


        public override void restart()
            {

            }


        public override void restartSameGame()
            {

            }


        public override void positionResizeElements()
            {
                // the layout is a grid with 7 columns and 3 lines
                // each position has space for a card + margin
                // we calculate these values from the available window dimensions
            var canvasWidth = this.canvas.ActualWidth;
            var canvasHeight = this.canvas.ActualHeight;
            var positionWidth = canvasWidth / 7;
            var positionHeight = canvasHeight / 3;
            var availableCardWidth = positionWidth * 0.9;

            var cardHeight = positionHeight * 0.9;
            var cardWidth = cardHeight * Card.Ratio;

            if (cardWidth > availableCardWidth)
                {
                cardWidth = availableCardWidth;
                cardHeight = cardWidth / Card.Ratio;
                }

            var horizontalMargin = (positionWidth - cardWidth) / 2;   // divide by 2 since there's margin in both sides
            var verticalMargin = (positionHeight - cardHeight) / 2;

            foreach (Foundation foundation in this.foundations)
                {
                foundation.Width = cardWidth;
                foundation.Height = cardHeight;
                }

                // add the foundations in the top right corner (the foundation is to where the cards need to be stacked (starting on an ace until the king)
            double top = verticalMargin;
            double left = canvasWidth - cardWidth - horizontalMargin;

            foreach (var foundation in this.foundations)
                {
                Canvas.SetLeft( foundation, left );
                Canvas.SetTop( foundation, top );

                left -= cardWidth + 2 * horizontalMargin;
                }
            }


        public override void end()
            {

            }
        }
    }
