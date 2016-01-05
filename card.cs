using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;


namespace GoldMine
    {
    public class Card : Image
        {
        public const double OriginalWidth = 672;
        public const double OriginalHeight = 976;
        public const double Ratio = OriginalWidth / OriginalHeight;

        public enum Suit { clubs, diamonds, hearts, spades };
        public enum Value { ace, two, three, four, five, six, seven, eight, nine, ten, jack, queen, king };
        public enum Color { black, red };


        public Suit suit;
        public Value value;
        public Color color;


        public Card( Suit suit, Value value )
            {
            this.suit = suit;
            this.value = value;

            if ( suit == Suit.clubs || suit == Suit.spades )
                {
                this.color = Color.black;
                }

            else
                {
                this.color = Color.red;
                }

            this.showBack();
            }


        public void showBack()
            {
            var mainResources = Application.Current.Resources;

            this.Source = (ImageSource) mainResources[ "card_back" ];
            }


        public void showFront()
            {
            var mainResources = Application.Current.Resources;

            var imageName = this.value.ToString() + "_of_" + this.suit.ToString();
            this.Source = (ImageSource) mainResources[ imageName ];
            }
        }
    }
