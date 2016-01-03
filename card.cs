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
        public enum Suit { clubs, diamonds, hearts, spades };
        public enum Value { ace, two, three, four, five, six, seven, eight, nine, ten, jack, queen, king };


        private Suit suit;
        private Value value;


        public Card( Suit suit, Value value )
            {
            this.suit = suit;
            this.value = value;

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


        public void applyDropEffect()
            {
            this.Opacity = 0.5;
            }


        public void removeDropEffect()
            {
            this.Opacity = 1;
            }
        }
    }
