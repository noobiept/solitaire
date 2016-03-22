using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;


namespace Solitaire
    {
    class GoldMine : SolitaireGame
        {
        private MenuItem toFoundation;
        private TextBlock stockLeft;


        public GoldMine( Canvas mainCanvas ) : base( mainCanvas )
            {
            // initialize all the game elements
            this.stock = new Stock();
            this.stock.MouseUp += this.onStockMouseUp;
            this.canvas.Children.Add( this.stock );

            this.waste = new Waste();
            this.canvas.Children.Add( this.waste );

            for( int a = 0 ; a < 4 ; a++ )
                {
                var foundation = new Foundation();

                this.canvas.Children.Add( foundation );
                this.droppableElements.Add( foundation );
                this.foundations.Add( foundation );
                }

            for( int a = 0 ; a < 7 ; a++ )
                {
                var tableau = new Tableau();

                this.canvas.Children.Add( tableau );
                this.droppableElements.Add( tableau );
                this.tableaus.Add( tableau );
                }

            foreach( Card.Suit suit in Enum.GetValues( typeof( Card.Suit ) ) )
                {
                foreach( Card.Value value in Enum.GetValues( typeof( Card.Value ) ) )
                    {
                    var card = new Card( suit, value );

                    card.MouseDown += this.onMouseDown;
                    card.MouseMove += this.onMouseMove;
                    card.MouseUp += this.onMouseUp;

                    this.cards.Add( card );
                    }
                }

            this.startGame();
            }


        public override void addMenuElements( Menu container )
            {
            var button = new MenuItem();
            button.ToolTip = "ctrl + f";
            button.Header = "To _Foundation";
            button.Click += this.toFoundationClick;

            container.Items.Add( button );
            this.toFoundation = button;
            }


        public override void removeMenuElements( Menu container )
            {
            container.Items.Remove( this.toFoundation );
            }


        public override void addInfoElements( Panel container )
            {
            var textBlock = new TextBlock();
            textBlock.Margin = new Thickness( 5 );
            container.Children.Add( textBlock );

            this.stockLeft = textBlock;
            this.updateStockLeft();
            }


        public override void removeInfoElements( Panel container )
            {
            container.Children.Remove( this.stockLeft );
            }


        public override void startGame( bool shuffle = true )
            {
            // disconnect the cards from their previous container
            foreach( Card card in this.cards )
                {
                var parent = card.Parent as Panel;

                if( parent != null )
                    {
                    parent.Children.Remove( card );
                    }
                }

            if( shuffle == true )
                {
                Utilities.shuffle( this.cards );
                }

            // add all the shuffled cards to the stock
            foreach( Card card in this.cards )
                {
                card.showBack();
                this.stock.Children.Add( card );
                }

            base.startGame( shuffle );
            }


        /**
         * Checks if the game has ended, and if so then show a message.
         * The game is over when all the cards are in the foundations.
         */
        protected override void checkGameEnd()
            {
            this.checkGameEndFoundation();
            }


        /**
         * When we click on the stock, we move 3 cards to the waste.
         */
        private void onStockMouseUp( object sender, MouseButtonEventArgs e )
            {
            var count = this.stock.Children.Count;

            for( int a = 0 ; a < 3 && count > 0 ; a++ )
                {
                int lastPosition = count - 1;

                var card = (Card) this.stock.Children[ lastPosition ];
                this.stock.Children.RemoveAt( lastPosition );
                this.waste.Children.Add( card );

                card.showFront();

                count = this.stock.Children.Count;
                }

            this.updateStockLeft();
            }


        protected override void doubleClick( Card card, Container parent )
            {
            this.sendToFoundation( parent );
            }


        /**
         * Position/resize all the elements in the right place (given the current width/height of the canvas).
         */
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

            if( cardWidth > availableCardWidth )
                {
                cardWidth = availableCardWidth;
                cardHeight = cardWidth / Card.Ratio;
                }

            var horizontalMargin = (positionWidth - cardWidth) / 2;   // divide by 2 since there's margin in both sides
            var verticalMargin = (positionHeight - cardHeight) / 2;

            // resize all the elements
            foreach( Card card in this.cards )
                {
                card.Height = cardHeight;   // the image will maintain the aspect ratio, so only need to set one
                }

            foreach( Tableau tableau in this.tableaus )
                {
                tableau.Width = cardWidth;
                tableau.Height = cardHeight;
                }

            foreach( Foundation foundation in this.foundations )
                {
                foundation.Width = cardWidth;
                foundation.Height = cardHeight;
                }

            this.waste.Width = cardWidth;
            this.waste.Height = cardHeight;

            this.stock.Width = cardWidth;
            this.stock.Height = cardHeight;

            // position all the elements
            // add the stock element in the top left
            double left = horizontalMargin;
            double top = verticalMargin;

            Canvas.SetLeft( this.stock, left );
            Canvas.SetTop( this.stock, top );

            // add the waste element next to the stock
            left += cardWidth + horizontalMargin * 2;

            Canvas.SetLeft( this.waste, left );
            Canvas.SetTop( this.waste, top );

            // add the foundations in the top right corner (the foundation is to where the cards need to be stacked (starting on an ace until the king)
            left = canvasWidth - cardWidth - horizontalMargin;

            foreach( var foundation in this.foundations )
                {
                Canvas.SetLeft( foundation, left );
                Canvas.SetTop( foundation, top );

                left -= cardWidth + 2 * horizontalMargin;
                }

            // add the tableau piles (where you can move any card to)
            left = horizontalMargin;
            top += cardHeight + verticalMargin;

            foreach( var tableau in this.tableaus )
                {
                Canvas.SetLeft( tableau, left );
                Canvas.SetTop( tableau, top );

                left += cardWidth + 2 * horizontalMargin;
                }
            }


        /**
         * Depending on where the card is located, it may be draggable or not.
         */
        protected override bool isCardDraggable( Card card )
            {
            var parent = card.Parent;

            if( parent is Stock )
                {
                return false;
                }

            // the last card is draggable, the others aren't
            if( parent is Waste )
                {
                var last = this.waste.getLast();

                if( last != null && last != card )
                    {
                    return false;
                    }
                }

            return true;
            }


        private void updateStockLeft()
            {
            this.stockLeft.Text = "In stock: " + this.stock.Children.Count;
            }


        /**
         * Tries to move all the possible cards from the waste/tableau to the foundation.
         * Useful for the ending of a game, so that you don't have to manually move all the last cards.
         */
        private void toFoundationClick( object sender, RoutedEventArgs e )
            {
            this.sendAllToFoundation();
            }


        public override string getTitle()
            {
            return "Gold Mine";
            }


        public override GameKey getGameKey()
            {
            return GameKey.goldMine;
            }
        }
    }
