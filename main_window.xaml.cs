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
        List<Foundation> foundations = new List<Foundation>();


        public MainWindow()
            {
            InitializeComponent();

            var foundation = new Foundation( 400, 50 );

            MainCanvas.Children.Add( foundation );
            this.foundations.Add( foundation );

            var card = new Card( 50, 50 );

            card.MouseDown += this.onMouseDown;
            card.MouseMove += this.onMouseMove;
            card.MouseUp += this.onMouseUp;

            MainCanvas.Children.Add( card );
            }


        private void collisionDetection( Image image )
            {
            for (var a = 0 ; a < this.foundations.Count ; a++)
                {
                var foundation = this.foundations[ a ];

                if ( this.boxBoxCollision( image, foundation ) )
                    {
                    Console.WriteLine( "Collision!" );
                    }
                }
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
            var image = (Image) sender;
            this.isDragging = true;
            this.clickPosition = Mouse.GetPosition( image );
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

                var image = (Image) sender;
                var position = e.GetPosition( MainCanvas );

                Canvas.SetLeft( image, position.X - this.clickPosition.X );
                Canvas.SetTop( image, position.Y - this.clickPosition.Y );

                this.collisionDetection( image );
                }
            }


        private void onMouseUp( object sender, MouseButtonEventArgs e )
            {
            this.isDragging = false;
            }
        }
    }
