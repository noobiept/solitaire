using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Input;


namespace GoldMine
    {
    public class Card : Image
        {
        public Card( double left, double top )
            {
            var uri = new Uri( @"images/card_back_orange.png", UriKind.Relative );
            var backSource = new BitmapImage( uri );
            this.Source = backSource;

            Canvas.SetLeft( this, left );
            Canvas.SetTop( this, top );
            }
        }
    }
