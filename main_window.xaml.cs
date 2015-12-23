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
        List<Panel> droppableElements = new List<Panel>();


        public MainWindow()
            {
            InitializeComponent();

                // add 4 foundations at the top right corner (the foundation is to where the cards need to be stacked (starting on an ace until the king)
            int margin = 10;
            double left = this.Width - margin;
            double top = margin;

            for (int a = 0 ; a < 4 ; a++)
                {
                var foundation = new Foundation();

                left -= foundation.Width + margin;

                Canvas.SetLeft( foundation, left );
                Canvas.SetTop( foundation, top );

                MainCanvas.Children.Add( foundation );
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

                MainCanvas.Children.Add( tableau );
                this.droppableElements.Add( tableau );
                }


            var card = new Card( 50, 50 );

            card.MouseDown += this.onMouseDown;
            card.MouseMove += this.onMouseMove;
            card.MouseUp += this.onMouseUp;

            MainCanvas.Children.Add( card );

            var card2 = new Card( 50, 300 );

            card2.MouseDown += this.onMouseDown;
            card2.MouseMove += this.onMouseMove;
            card2.MouseUp += this.onMouseUp;

            MainCanvas.Children.Add( card2 );
            }


        private Panel collisionDetection( Image image )
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
            var foundation = this.collisionDetection( card );

            if ( foundation != null )
                {
                card.removeEffect();
                MainCanvas.Children.Remove( card );
                foundation.Children.Add( card );
                }
            }


        private void positionCard( Card card, MouseEventArgs e )
            {
            var position = e.GetPosition( MainCanvas );

            Canvas.SetLeft( card, position.X - this.clickPosition.X );
            Canvas.SetTop( card, position.Y - this.clickPosition.Y );

            var foundation = this.collisionDetection( card );

            if (foundation != null)
                {
                card.applyEffect();
                }

            else
                {
                card.removeEffect();
                }
            }
        }
    }
