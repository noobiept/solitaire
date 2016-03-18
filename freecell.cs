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
        private readonly List<Cell> cells = new List<Cell>();
        private readonly List<Tableau> tableaus = new List<Tableau>();
        private readonly List<Container> droppableElements = new List<Container>();
        private readonly List<Card> cards = new List<Card>();


        public FreeCell( Canvas mainCanvas, StackPanel customButtons, StackPanel customInfo )
            {
            this.canvas = mainCanvas;

            for (int a = 0 ; a < 4 ; a++)
                {
                var cell = new Cell();

                this.canvas.Children.Add( cell );
                this.droppableElements.Add( cell );
                this.cells.Add( cell );
                }

            for (int a = 0 ; a < 4 ; a++)
                {
                var foundation = new Foundation();

                this.canvas.Children.Add( foundation );
                this.droppableElements.Add( foundation );
                this.foundations.Add( foundation );
                }

            for (int a = 0 ; a < 8 ; a++)
                {
                var tableau = new Tableau();

                this.canvas.Children.Add( tableau );
                this.droppableElements.Add( tableau );
                this.tableaus.Add( tableau );
                }

            foreach (Card.Suit suit in Enum.GetValues( typeof( Card.Suit ) ))
                {
                foreach (Card.Value value in Enum.GetValues( typeof( Card.Value ) ))
                    {
                    var card = new Card( suit, value );

                    //card.MouseDown += this.onMouseDown;
                    //card.MouseMove += this.onMouseMove; //HERE
                    //card.MouseUp += this.onMouseUp;
                    card.showFront();

                    this.cards.Add( card );
                    }
                }

            this.startGame();
            }


        public override void startGame( bool shuffle= true )
            {
                // disconnect the cards from their previous container
            foreach (Card card in this.cards)
                {
                var parent = card.Parent as Panel;

                if (parent != null)
                    {
                    parent.Children.Remove( card );
                    }
                }

            if (shuffle == true)
                {
                Utilities.shuffle( this.cards );
                }

            int tableauPosition = 0;
            int maxTableau = this.tableaus.Count;

            foreach (Card card in this.cards)
                {
                this.tableaus[ tableauPosition ].Children.Add( card );

                tableauPosition++;
                if ( tableauPosition >= maxTableau )
                    {
                    tableauPosition = 0;
                    }
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
                // the layout is a grid with 8 columns and 3 lines
                // each position has space for a card + margin
                // we calculate these values from the available window dimensions
            var canvasWidth = this.canvas.ActualWidth;
            var canvasHeight = this.canvas.ActualHeight;
            var positionWidth = canvasWidth / 8;
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

            foreach (Cell cell in this.cells)
                {
                cell.Width = cardWidth;
                cell.Height = cardHeight;
                }

            foreach (Foundation foundation in this.foundations)
                {
                foundation.Width = cardWidth;
                foundation.Height = cardHeight;
                }

            foreach (Tableau tableau in this.tableaus)
                {
                tableau.Width = cardWidth;
                tableau.Height = cardHeight;
                }

            double top = verticalMargin;
            double left = horizontalMargin;

                // 4 cells in the top-left
            foreach (Cell cell in this.cells)
                {
                Canvas.SetLeft( cell, left );
                Canvas.SetTop( cell, top );

                left += cardWidth + 2 * horizontalMargin;
                }

                // 4 foundations in the top-right
            foreach (Foundation foundation in this.foundations)
                {
                Canvas.SetLeft( foundation, left );
                Canvas.SetTop( foundation, top );

                left += cardWidth + 2 * horizontalMargin;
                }

                // 8 tableaus in the 2nd line
            left = horizontalMargin;
            top += cardHeight + verticalMargin;

            foreach (var tableau in this.tableaus)
                {
                Canvas.SetLeft( tableau, left );
                Canvas.SetTop( tableau, top );

                left += cardWidth + 2 * horizontalMargin;
                }
            }


        public override void end()
            {

            }
        }
    }
