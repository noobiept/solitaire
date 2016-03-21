using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace Solitaire
    {
    public class Card : Image
        {
        public const double OriginalWidth = 672;
        public const double OriginalHeight = 976;
        public const double Ratio = OriginalWidth / OriginalHeight;

        public enum Suit { clubs, diamonds, hearts, spades };
        public enum Value { ace, two, three, four, five, six, seven, eight, nine, ten, jack, queen, king };
        public enum Color { black, red };


        public readonly Suit suit;
        public readonly Value value;
        public readonly Color color;
        public bool frontSide;


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
            this.frontSide = false;
            }


        public void showFront()
            {
            var mainResources = Application.Current.Resources;

            var imageName = this.value.ToString() + "_of_" + this.suit.ToString();
            this.Source = (ImageSource) mainResources[ imageName ];
            this.frontSide = true;
            }


        public bool hasFrontSide()
            {
            return this.frontSide;
            }
        }
    }
