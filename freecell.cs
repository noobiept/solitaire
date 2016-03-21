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
        private readonly List<Foundation> foundations = new List<Foundation>();
        private readonly List<Cell> cells = new List<Cell>();
        private readonly List<Tableau> tableaus = new List<Tableau>();
        private readonly List<Card> cards = new List<Card>();


        public FreeCell( Canvas mainCanvas ) : base( mainCanvas )
            {
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

                    card.MouseDown += this.onMouseDown;
                    card.MouseMove += this.onMouseMove;
                    card.MouseUp += this.onMouseUp;
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

                if ( parent != null )
                    {
                    parent.Children.Remove( card );
                    }
                }

            if ( shuffle == true )
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

            base.startGame( shuffle );
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

            if ( cardWidth > availableCardWidth )
                {
                cardWidth = availableCardWidth;
                cardHeight = cardWidth / Card.Ratio;
                }

            var horizontalMargin = (positionWidth - cardWidth) / 2;   // divide by 2 since there's margin in both sides
            var verticalMargin = (positionHeight - cardHeight) / 2;

                // resize all the elements
            foreach (Card card in this.cards)
                {
                card.Height = cardHeight;   // the image will maintain the aspect ratio, so only need to set one
                }

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

                // first line
                // 4 cells, 4 foundations and some space in-between
            var inBetweenSpace = canvasWidth * 0.05;
            var firstPositionWidth = (canvasWidth - inBetweenSpace) / 8;
            var firstHorizontalMargin = (firstPositionWidth - cardWidth) / 2;

                // 4 cells in the top-left
            foreach (Cell cell in this.cells)
                {
                Canvas.SetLeft( cell, left );
                Canvas.SetTop( cell, top );

                left += cardWidth + 2 * firstHorizontalMargin;
                }

                // add space in-between
            left += inBetweenSpace - 2 * firstHorizontalMargin;

                // 4 foundations in the top-right
            foreach (Foundation foundation in this.foundations)
                {
                Canvas.SetLeft( foundation, left );
                Canvas.SetTop( foundation, top );

                left += cardWidth + 2 * firstHorizontalMargin;
                }

                // second line
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


        protected override void checkGameEnd()
            {
            this.sendCardsToFoundation();

            int cardCount = this.cards.Count;
            int foundationCount = 0;

            foreach( var foundation in this.foundations )
                {
                foundationCount += foundation.Children.Count;
                }

                // game has ended
            if ( cardCount == foundationCount )
                {
                this.timer.Stop();

                var best = Data.oneMoreWin( this.getGameKey(), this.secondsPassed );
                var message = String.Format( "You Win!\nTime: {0}", Utilities.timeToString( (int) this.secondsPassed ) );

                if ( this.secondsPassed == best )
                    {
                    message += "\nYou beat your best time!";
                    }

                MessageBox.Show( message, "Game Over!", MessageBoxButton.OK );
                this.startGame();
                }
            }


        protected override bool isCardDraggable( Card card )
            {
            var parent = card.Parent;

                // can't take a card once its added to the foundation
            if ( parent is Foundation )
                {
                return false;
                }

            if ( parent is Tableau )
                {
                var tableau = parent as Tableau;

                    // only if the stack of cards has the correct decreasing value, and alternating color
                var position = tableau.Children.IndexOf( card ) + 1;    // the next card
                var count = tableau.Children.Count;
                var currentCard = card;

                while( position < count )
                    {
                    var nextCard = tableau.Children[ position ] as Card;

                    if ( currentCard.value - 1 != nextCard.value || 
                         currentCard.color == nextCard.color )
                        {
                        return false;
                        }

                    currentCard = nextCard;
                    position++;
                    }
                }

            return true;
            }


        /**
         * Try to send the card to an empty 'Cell'.
         */
        protected override void doubleClick( Card card, Container parent )
            {
            foreach( var cell in this.cells )
                {
                if ( cell.isEmpty() )
                    {
                    this.moveCards( new List<Card> { card }, cell );
                    this.checkGameEnd();
                    return;
                    }
                }
            }


        /**
         * After each play, check if there are cards we can send directly to the foundation.
         */
        private void sendCardsToFoundation()
            {
            bool moved = false;

            do
                {
                moved = false;

                foreach( var cell in this.cells )
                    {
                    var cards = new List<Card>() { cell.getLast() };

                    foreach( var foundation in this.foundations )
                        {
                        if ( foundation.canDrop( cards ) )
                            {
                            this.moveCards( cards, foundation );
                            moved = true;
                            }
                        }
                    }

                foreach( var tableau in this.tableaus )
                    {
                    var cards = new List<Card>() { tableau.getLast() };

                    foreach( var foundation in this.foundations )
                        {
                        if ( foundation.canDrop( cards ) )
                            {
                            this.moveCards( cards, foundation );
                            moved = true;
                            }
                        }
                    }
                } while( moved == true );
            }


        /**
         * A group of cards can only be dragged if there's enough empty spaces (search freecell supermove).
         * Reference: http://www.solitairecentral.com/articles/FreecellPowerMovesExplained.html
         */
        protected override bool canDrop( List<Card> cards, Container container )
            {
            var emptyFreeCells = 0;
            var emptyColumns = 0;

            foreach( var cell in this.cells )
                {
                if ( cell.isEmpty() )
                    {
                    emptyFreeCells++;
                    }
                }

            foreach( var tableau in this.tableaus )
                {
                if ( tableau.isEmpty() )
                    {
                    emptyColumns++;
                    }
                }

                // only other empty containers count (not the one we're going to)
            if ( container.isEmpty() )
                {
                emptyColumns--;
                }

            if ( cards.Count > (1 + emptyFreeCells) * Math.Pow( 2, emptyColumns ) )
                {
                return false;
                }

            return base.canDrop( cards, container );
            }


        public override string getTitle()
            {
            return "FreeCell";
            }


        public override GameKey getGameKey()
            {
            return GameKey.freeCell;
            }
        }
    }
