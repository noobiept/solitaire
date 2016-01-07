using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace GoldMine
    {
    public partial class MainWindow : Window
        {
            // data use for the drag and drop operation of cards
        public struct Drag
            {
            public bool isDragging;
            public const int diff = 20;            // space between each card during the drag
            public List<Card> cardsDragging;
            public Point clickPosition;
            public Container originalContainer;    // original container before the drag occurred. if the drag isn't valid, we need to return the cards to the original place
            public Container highlightedContainer; // when dragging a card on top of a container, highlight that container, and keep a reference to it (to know when to remove the highlight)
            }

        Drag drag;
        Timer timer;
        uint secondsPassed;

        List<Container> droppableElements = new List<Container>();
        List<Card> cards = new List<Card>();
        Stock stock;
        Waste waste;
        List<Foundation> foundations = new List<Foundation>();
        List<Tableau> tableaus = new List<Tableau>();


        public MainWindow()
            {
            InitializeComponent();

            this.drag.cardsDragging = new List<Card>();
            this.timer = new Timer( 1000 );
            this.timer.Elapsed += this.onTimeElapsed;

            Data.load();

                // initialize all the game elements
            this.stock = new Stock();
            this.stock.MouseUp += this.onStockMouseUp;
            this.MainCanvas.Children.Add( this.stock );

            this.waste = new Waste();
            this.MainCanvas.Children.Add( this.waste );

            for (int a = 0 ; a < 4 ; a++)
                {
                var foundation = new Foundation();

                this.MainCanvas.Children.Add( foundation );
                this.droppableElements.Add( foundation );
                this.foundations.Add( foundation );
                }

            for (int a = 0 ; a < 7 ; a++)
                {
                var tableau = new Tableau();

                this.MainCanvas.Children.Add( tableau );
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


        private void startGame()
            {
                // disconnect the cards from their previous container
            foreach( Card card in this.cards )
                {
                var parent = card.Parent as Panel;

                if ( parent != null )
                    {
                    parent.Children.Remove( card );
                    }
                }

            Utilities.shuffle( this.cards );

                // add all the shuffled cards to the stock
            foreach( Card card in this.cards )
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
        private void checkGameEnd()
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

                var best = Data.oneMoreWin( this.secondsPassed );
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


        /**
         * When double-clicking on a card, see if we can send that card to a foundation.
         */
        private void sendToFoundation( Card card )
            {
                // can only send it if its the last card of its parent container
            var parent = card.Parent as Container;
            var last = parent.Children[ parent.Children.Count - 1 ];

            if ( last != card )
                {
                return;
                }

                // need to have a list to work with the 'canDrop' function
            var cards = new List<Card>();
            cards.Add( card );

            foreach( var foundation in this.foundations )
                {
                if ( foundation.canDrop( cards ) )
                    {
                    this.moveCards( cards, foundation );
                    this.checkGameEnd();
                    return;
                    }
                }
            }


        /**
         * Calculates the intersection area between the reference element and the droppable elements, and returns the one where the area was higher.
         */
        private Container collisionDetection( List<Card> cards )
            {
            var cardsBox = this.cardsDimension( cards );
            Container colliding = null;
            double collidingArea = 0;

            for (var a = 0 ; a < this.droppableElements.Count ; a++)
                {
                var container = this.droppableElements[ a ];

                if ( container.canDrop( cards ) )
                    {
                    var containerBox = container.getDimensionBox();

                    var area = Utilities.calculateIntersectionArea( cardsBox, containerBox );

                    if ( area > collidingArea )
                        {
                        collidingArea = area;
                        colliding = container;
                        }
                    }
                }
                
            return colliding;
            }


        private void onMouseDown( object sender, MouseButtonEventArgs e )
            {
            var card = (Card) sender;
            var parent = card.Parent as Container;

            if ( !this.isCardDraggable( card ) )
                {
                return;
                }

            if ( e.ClickCount == 2 )
                {
                this.sendToFoundation( card );
                return;
                }

            if ( this.drag.isDragging == true )
                {
                this.moveCards( this.drag.cardsDragging, this.drag.originalContainer );
                return;
                }

            this.drag.isDragging = true;
            this.drag.clickPosition = Mouse.GetPosition( card );
            this.drag.originalContainer = parent;
            parent.dragCards( card, this.drag.cardsDragging );

            foreach( Card dragCard in this.drag.cardsDragging )
                {
                parent.Children.Remove( dragCard );
                this.MainCanvas.Children.Add( dragCard );
                }

            this.positionCards( this.drag.cardsDragging, e );
            }


        private void onMouseMove( object sender, MouseEventArgs e )
            {
            if ( this.drag.isDragging )
                {
                if ( e.LeftButton == MouseButtonState.Released )
                    {
                    this.moveCards( this.drag.cardsDragging, this.drag.originalContainer );
                    }

                else
                    {
                    this.positionCards( this.drag.cardsDragging, e );
                    }
                }
            }


        private void onMouseUp( object sender, MouseButtonEventArgs e )
            {
            if ( !this.drag.isDragging )
                {
                return;
                }

            var container = this.collisionDetection( this.drag.cardsDragging );

            if ( container != null )
                {
                this.moveCards( this.drag.cardsDragging, container );
                this.checkGameEnd();
                }

                // wasn't dropped on any container, so its not a valid drag operation. return to the original container
            else
                {
                this.moveCards( this.drag.cardsDragging, this.drag.originalContainer );
                }
            }

        
        /**
         * When the window is resized, we need to reposition the game elements.
         */
        private void onSizeChange( object sender, SizeChangedEventArgs e )
            {
            this.positionResizeElements();
            }


        private void onStateChange( object sender, EventArgs e )
            {
            this.positionResizeElements();
            }


        /**
         * Position/resize all the elements in the right place (given the current width/height of the canvas).
         */
        private void positionResizeElements()
            {
                // the layout is a grid with 7 columns and 3 lines
                // each position has space for a card + margin
                // we calculate these values from the available window dimensions
            var canvasWidth = this.MainCanvas.ActualWidth;
            var canvasHeight = this.MainCanvas.ActualHeight;
            var positionWidth = canvasWidth / 7;
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
         * Finishes the drag operation, moving a list of cards to a container.
         */
        private void moveCards( List<Card> cards, Container container )
            {
            if ( this.drag.highlightedContainer != null )
                {
                this.drag.highlightedContainer.removeDropEffect();
                this.drag.highlightedContainer = null;
                }

            foreach( Card card in cards )
                {
                var parent = card.Parent as Panel;
                parent.Children.Remove( card );
                container.Children.Add( card );
                }

            this.drag.originalContainer = null;
            this.drag.cardsDragging.Clear();
            this.drag.isDragging = false;
            }


        private void positionCards( List<Card> cards, MouseEventArgs e )
            {
            var position = e.GetPosition( this.MainCanvas );

            for (int a = 0 ; a < cards.Count ; a++)
                {
                Canvas.SetLeft( cards[ a ], position.X - this.drag.clickPosition.X );
                Canvas.SetTop( cards[ a ], position.Y - this.drag.clickPosition.Y + Drag.diff * a );
                }

            var container = this.collisionDetection( cards );

            if ( this.drag.highlightedContainer != null )
                {
                this.drag.highlightedContainer.removeDropEffect();
                this.drag.highlightedContainer = null;
                }

            if ( container != null )
                {
                container.applyDropEffect();

                this.drag.highlightedContainer = container;
                }
            }


        /**
         * Determine the dimensions of the cards stack.
         * Use the first card to determine the x/y/width.
         * The height is calculated from the number of cards.
         * The 'diff' is the space between each card.
         */
        private Utilities.Box cardsDimension( List<Card> cards )
            {
            var firstCard = cards[ 0 ];
            return new Utilities.Box {
                x = Canvas.GetLeft( firstCard ),
                y = Canvas.GetTop( firstCard ),
                width = firstCard.ActualWidth,
                height = firstCard.ActualHeight + Drag.diff * (cards.Count - 1)
                };
            }


        /**
         * Depending on where the card is located, it may be draggable or not.
         */
        private bool isCardDraggable( Card card )
            {
            var parent = card.Parent;

            if ( parent is Stock )
                {
                return false;
                }

                // the last card is draggable, the others aren't
            if ( parent is Waste )
                {
                if ( this.waste.Children.Count != 0 )
                    {
                    var last = this.waste.Children[ this.waste.Children.Count - 1 ];

                    if ( last != card )
                        {
                        return false;
                        }
                    }
                }

            return true;
            }


        private void newGameClick( object sender, RoutedEventArgs e )
            {
            this.startGame();
            }


        private void updateStockLeft()
            {
            this.StockLeft.Text = "In stock: " + this.stock.Children.Count;
            }


        private void updateTimePassed()
            {
            this.TimePassed.Text = "Time: " + Utilities.timeToString( (int) this.secondsPassed );
            }


        private void onTimeElapsed( Object source, ElapsedEventArgs e )
            {
            this.Dispatcher.Invoke( (Action) (() =>
                {
                this.secondsPassed++;
                this.updateTimePassed();
                }));
            }


        private void onWindowClosing( object sender, System.ComponentModel.CancelEventArgs e )
            {
            this.timer.Stop();
            }


        private void openStatisticsWindow( object sender, RoutedEventArgs e )
            {
            var statistics = new Statistics();
            statistics.ShowDialog();
            }
        }
    }
