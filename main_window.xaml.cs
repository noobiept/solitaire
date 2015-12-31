using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            public Card cardDragging;
            public Point clickPosition;
            public Container originalContainer;    // original container before the drag occurred. if the drag isn't valid, we need to return the cards to the original place
            public Container highlightedContainer; // when dragging a card on top of a container, highlight that container, and keep a reference to it (to know when to remove the highlight)
            }

        Drag drag;

        List<Container> droppableElements = new List<Container>();
        List<Card> cards = new List<Card>();
        Stock stock;
        Waste waste;


        public MainWindow()
            {
            InitializeComponent();

                // add the stock element in the top left
            int margin = 10;
            double left = margin;
            double top = margin;

            this.stock = new Stock();
            this.stock.MouseUp += this.onStockMouseUp;

            Canvas.SetLeft( this.stock, left );
            Canvas.SetTop( this.stock, top );

            this.MainCanvas.Children.Add( this.stock );

                // add the waste element next to the stock
            this.waste = new Waste();

            left += this.stock.Width + margin;

            Canvas.SetLeft( this.waste, left );
            Canvas.SetTop( this.waste, top );

            this.MainCanvas.Children.Add( this.waste );

                // add 4 foundations at the top right corner (the foundation is to where the cards need to be stacked (starting on an ace until the king)
            left = this.Width - margin;
            top = margin;

            for (int a = 0 ; a < 4 ; a++)
                {
                var foundation = new Foundation();

                left -= foundation.Width + margin;

                Canvas.SetLeft( foundation, left );
                Canvas.SetTop( foundation, top );

                this.MainCanvas.Children.Add( foundation );
                this.droppableElements.Add( foundation );
                }

                // add 7 tableau piles (where you can move any card to)
            left = margin;
            top = 300;  //HERE

            for (int a = 0 ; a < 7 ; a++)
                {
                var tableau = new Tableau();

                Canvas.SetLeft( tableau, left );
                Canvas.SetTop( tableau, top );

                left += tableau.Width + margin;

                this.MainCanvas.Children.Add( tableau );
                this.droppableElements.Add( tableau );
                }

            startGame();
            }


        private void startGame()
            {
            for (int a = 0 ; a < 7 ; a++)
                {
                var card = new Card();

                card.MouseDown += this.onMouseDown;
                card.MouseMove += this.onMouseMove;
                card.MouseUp += this.onMouseUp;

                this.stock.Children.Add( card );
                this.cards.Add( card );
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

                var card = this.stock.Children[ lastPosition ];
                this.stock.Children.RemoveAt( lastPosition );
                this.waste.Children.Add( card );

                count = this.stock.Children.Count;
                }
            }


        private Container collisionDetection( Image image )
            {
            for (var a = 0 ; a < this.droppableElements.Count ; a++)
                {
                var element = this.droppableElements[ a ];

                if ( this.boxBoxCollision( image, element ) )
                    {
                    return element;
                    }
                }

            return null;
            }


        private bool boxBoxCollision( FrameworkElement one, FrameworkElement two )
            {
            var oneLeft = Canvas.GetLeft( one );
            var oneTop = Canvas.GetTop( one );
            var twoLeft = Canvas.GetLeft( two );
            var twoTop = Canvas.GetTop( two );
            
            return !(
                    oneLeft > twoLeft + two.ActualWidth ||
                    oneLeft + one.ActualWidth < twoLeft ||
                    oneTop > twoTop + two.ActualHeight ||
                    oneTop + one.ActualHeight < twoTop
                );
            }


        private void onMouseDown( object sender, MouseButtonEventArgs e )
            {
            var card = (Card) sender;
            var parent = card.Parent as Container;

            if ( !this.isCardDraggable( card ) )
                {
                return;
                }

            if ( this.drag.isDragging == true )
                {
                this.moveCard( this.drag.cardDragging, this.drag.originalContainer );
                return;
                }

            this.drag.isDragging = true;
            this.drag.cardDragging = card;
            this.drag.clickPosition = Mouse.GetPosition( card );
            this.drag.originalContainer = parent;

            parent.Children.Remove( card );
            this.MainCanvas.Children.Add( card );

            this.positionCard( card, e );
            }


        private void onMouseMove( object sender, MouseEventArgs e )
            {
            if ( this.drag.isDragging )
                {
                var card = (Card) sender;

                if ( e.LeftButton == MouseButtonState.Released )
                    {
                    this.moveCard( this.drag.cardDragging, this.drag.originalContainer );
                    }

                else
                    {
                    this.positionCard( card, e );
                    }
                }
            }


        private void onMouseUp( object sender, MouseButtonEventArgs e )
            {
            if ( !this.drag.isDragging )
                {
                return;
                }

            var card = (Card) sender;
            var container = this.collisionDetection( card );

            if ( container != null )
                {
                this.moveCard( this.drag.cardDragging, container );
                }

                // wasn't dropped on any container, so its not a valid drag operation. return to the original container
            else
                {
                this.moveCard( this.drag.cardDragging, this.drag.originalContainer );
                }
            }


        /**
         * Finishes the drag operation, moving the card to a container.
         */
        private void moveCard( Card card, Container container )
            {
            card.removeDropEffect();

            if ( this.drag.highlightedContainer != null )
                {
                this.drag.highlightedContainer.removeDropEffect();
                this.drag.highlightedContainer = null;
                }

            this.MainCanvas.Children.Remove( card );
            container.Children.Add( card );
            this.drag.originalContainer = null;
            this.drag.cardDragging = null;
            this.drag.isDragging = false;
            }


        private void positionCard( Card card, MouseEventArgs e )
            {
            var position = e.GetPosition( this.MainCanvas );

            Canvas.SetLeft( card, position.X - this.drag.clickPosition.X );
            Canvas.SetTop( card, position.Y - this.drag.clickPosition.Y );

            var container = this.collisionDetection( card );

            if ( this.drag.highlightedContainer != null )
                {
                this.drag.highlightedContainer.removeDropEffect();
                this.drag.highlightedContainer = null;
                }

            if ( container != null )
                {
                container.applyDropEffect();
                card.applyDropEffect();

                this.drag.highlightedContainer = container;
                }

            else
                {
                card.removeDropEffect();
                }
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
        }
    }
