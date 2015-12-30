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
        bool isDragging = false;
        Point clickPosition;
        Container highlightedContainer;     // when dragging a card on top of a container, highlight that container, and keep a reference to it (to know when to remove the highlight)
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

                // add the mouse events to the last card
            var lastCard = this.waste.Children[ this.waste.Children.Count - 1 ];

            lastCard.MouseDown += this.onMouseDown;
            lastCard.MouseMove += this.onMouseMove;
            lastCard.MouseUp += this.onMouseUp;
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
            this.isDragging = true;
            this.clickPosition = Mouse.GetPosition( card );

            var parent = card.Parent as Panel;

            if ( parent != null )
                {
                parent.Children.Remove( card );
                }

            this.MainCanvas.Children.Add( card );

            this.positionCard( card, e );
            }


        private void onMouseMove( object sender, MouseEventArgs e )
            {
            if ( this.isDragging )
                {
                if ( e.LeftButton == MouseButtonState.Released )
                    {
                    this.isDragging = false;
                    return;
                    }

                var card = (Card) sender;
                this.positionCard( card, e );
                }
            }


        private void onMouseUp( object sender, MouseButtonEventArgs e )
            {
            var card = (Card) sender;
            this.isDragging = false;
            var container = this.collisionDetection( card );

            if ( container != null )
                {
                card.removeDropEffect();
                container.removeDropEffect();
                this.highlightedContainer = null;

                this.MainCanvas.Children.Remove( card );
                container.Children.Add( card );
                }
            }


        private void positionCard( Card card, MouseEventArgs e )
            {
            var position = e.GetPosition( this.MainCanvas );

            Canvas.SetLeft( card, position.X - this.clickPosition.X );
            Canvas.SetTop( card, position.Y - this.clickPosition.Y );

            var container = this.collisionDetection( card );

            if ( this.highlightedContainer != null )
                {
                this.highlightedContainer.removeDropEffect();
                this.highlightedContainer = null;
                }

            if ( container != null )
                {
                container.applyDropEffect();
                card.applyDropEffect();

                this.highlightedContainer = container;
                }

            else
                {
                card.removeDropEffect();
                }
            }
        }
    }
