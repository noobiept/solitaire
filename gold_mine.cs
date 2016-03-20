using System;
using System.Timers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Solitaire
    {
    class GoldMine : SolitaireGame
        {
        private TextBlock stockLeft;

        private Timer timer;
        private uint secondsPassed;

        private readonly List<Card> cards = new List<Card>();
        private readonly Stock stock;
        private readonly Waste waste;
        private readonly List<Foundation> foundations = new List<Foundation>();
        private readonly List<Tableau> tableaus = new List<Tableau>();
        

        public GoldMine( Canvas mainCanvas, StackPanel customButtons, StackPanel customInfo ) : base( mainCanvas )
            {
            this.timer = new Timer( 1000 );
            this.timer.Elapsed += this.onTimeElapsed;

            Data.load();

                // initialize all the game elements
            this.stock = new Stock();
            this.stock.MouseUp += this.onStockMouseUp;
            this.canvas.Children.Add( this.stock );

            this.waste = new Waste();
            this.canvas.Children.Add( this.waste );

            for (int a = 0 ; a < 4 ; a++)
                {
                var foundation = new Foundation();

                this.canvas.Children.Add( foundation );
                this.droppableElements.Add( foundation );
                this.foundations.Add( foundation );
                }

            for (int a = 0 ; a < 7 ; a++)
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

                    this.cards.Add( card );
                    }
                }

            this.addMenuElements( customButtons );
            this.addInfoElements( customInfo );
            this.initKeyboardShortcuts();
            this.startGame();
            }


        public override void addMenuElements( StackPanel container )
            {
            var button = new Button();
            button.ToolTip = "ctrl + f";
            button.Content = "To Foundation";
            button.Click += this.toFoundationClick;

            container.Children.Add( button );
            }


        public override void addInfoElements( StackPanel container )
            {
            var textBlock = new TextBlock();
            container.Children.Add( textBlock );

            this.stockLeft = textBlock;
            }


        /**
         * Return an array of game specific keyboard shortcuts (to be added to the main window).
         */
        public void initKeyboardShortcuts()
            {
                // ctrl + f -- try to move all the possible cards to the foundation
            var moveToFoundation = new RoutedCommand();
            moveToFoundation.InputGestures.Add( new KeyGesture( Key.F, ModifierKeys.Control ) );

            this.shortcuts = new CommandBinding[] {
                new CommandBinding( moveToFoundation, this.toFoundationClick )
                };
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

                // add all the shuffled cards to the stock
            foreach (Card card in this.cards)
                {
                card.showBack();
                this.stock.Children.Add( card );
                }

            this.updateStockLeft();
            this.timer.Stop();
            this.secondsPassed = 0;
            this.updateTimePassed();
            this.timer.Start();
            }


        /**
         * Checks if the game has ended, and if so then show a message.
         * The game is over when all the cards are in the foundations.
         */
        protected override void checkGameEnd()
            {
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

                var best = Data.oneMoreWin( this.secondsPassed );//HERE
                var message = String.Format( "You Win!\nTime: {0}", Utilities.timeToString( (int) this.secondsPassed ) );

                if ( this.secondsPassed == best )
                    {
                    message += "\nYou beat your best time!";
                    }

                MessageBox.Show( message, "Game Over!", MessageBoxButton.OK );
                this.startGame();
                }
            }


        /**
         * When we click on the stock, we move 3 cards to the waste.
         */
        private void onStockMouseUp( object sender, MouseButtonEventArgs e )
            {
            var count = this.stock.Children.Count;

            for (int a = 0 ; a < 3 && count > 0 ; a++)
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
         * Try to send the last card of a container to a foundation.
         */
        private bool sendToFoundation( Container container )
            {
            var last = container.getLast();

            // need to have a list to work with the 'canDrop' function
            var cards = new List<Card>();
            cards.Add( last );

            foreach (var foundation in this.foundations)
                {
                if (foundation.canDrop( cards ))
                    {
                    this.moveCards( cards, foundation );
                    this.checkGameEnd();
                    return true;
                    }
                }

            return false;
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

            if (cardWidth > availableCardWidth)
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

            foreach (Tableau tableau in this.tableaus)
                {
                tableau.Width = cardWidth;
                tableau.Height = cardHeight;
                }

            foreach (Foundation foundation in this.foundations)
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

            foreach (var foundation in this.foundations)
                {
                Canvas.SetLeft( foundation, left );
                Canvas.SetTop( foundation, top );

                left -= cardWidth + 2 * horizontalMargin;
                }

                // add the tableau piles (where you can move any card to)
            left = horizontalMargin;
            top += cardHeight + verticalMargin;

            foreach (var tableau in this.tableaus)
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

            if (parent is Stock)
                {
                return false;
                }

                // the last card is draggable, the others aren't
            if (parent is Waste)
                {
                var last = this.waste.getLast();

                if (last != null && last != card)
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


        private void updateTimePassed()
            {
            Application.Current.Dispatcher.Invoke( (() => {
                var mainWindow = ((MainWindow) Application.Current.MainWindow);
                mainWindow.TimePassed.Text = "Time: " + Utilities.timeToString( (int) this.secondsPassed );
                }));
            }


        private void onTimeElapsed( Object source, ElapsedEventArgs e )
            {
            this.secondsPassed++;
            this.updateTimePassed();
            }
      

        public override void end()
            {
            this.timer.Stop();
            }


        /**
         * Tries to move all the possible cards from the waste/tableau to the foundation.
         * Useful for the ending of a game, so that you don't have to manually move all the last cards.
         */
        private void toFoundationClick( object sender, RoutedEventArgs e )
            {
            // if a card was moved to the foundation
            // we keep checking until there's no more possible moves
            bool moved = false;

            do
                {
                moved = false;

                if (this.sendToFoundation( this.waste ))
                    {
                    moved = true;
                    }

                foreach (var tableau in this.tableaus)
                    {
                    if (this.sendToFoundation( tableau ))
                        {
                        moved = true;
                        }
                    }
                }

            while (moved == true);
            }
        }
    }
