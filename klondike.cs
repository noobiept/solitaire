using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;

namespace Solitaire
{
    class Klondike : SolitaireGame
    {
        private MenuItem toFoundation;
        private int drawCount;

        public Klondike(Canvas mainCanvas) : base(mainCanvas)
        {
            this.stock = new Stock();
            this.stock.MouseUp += this.onStockMouseUp;
            this.canvas.Children.Add(this.stock);

            this.waste = new Waste();
            this.canvas.Children.Add(this.waste);

            for (int a = 0; a < 4; a++)
            {
                var foundation = new Foundation();

                this.canvas.Children.Add(foundation);
                this.droppableElements.Add(foundation);
                this.foundations.Add(foundation);
            }

            for (int a = 0; a < 7; a++)
            {
                var tableau = new Tableau();

                this.canvas.Children.Add(tableau);
                this.droppableElements.Add(tableau);
                this.tableaus.Add(tableau);
            }

            foreach (Card.Suit suit in Enum.GetValues(typeof(Card.Suit)))
            {
                foreach (Card.Value value in Enum.GetValues(typeof(Card.Value)))
                {
                    var card = new Card(suit, value);

                    card.MouseDown += this.onMouseDown;
                    card.MouseMove += this.onMouseMove;
                    card.MouseUp += this.onMouseUp;

                    this.cards.Add(card);
                }
            }

            this.startGame();
        }

        public override void startGame(bool shuffle = true)
        {
            // disconnect the cards from their previous container
            foreach (Card card in this.cards)
            {
                var parent = card.Parent as Panel;

                if (parent != null)
                {
                    parent.Children.Remove(card);
                }
            }

            if (shuffle == true)
            {
                Utilities.shuffle(this.cards);
            }

            // add 1 card to the first tableau, 2 cards to the second tableau, etc
            var cardsPerTableau = 1;
            var index = 0;

            foreach (var tableau in this.tableaus)
            {
                for (int a = index; a < index + cardsPerTableau; a++)
                {
                    var card = this.cards[a];

                    card.showBack();
                    tableau.Children.Add(card);
                }

                tableau.getLast().showFront();

                index += cardsPerTableau;
                cardsPerTableau++;
            }

            // the rest of the cards are added to the stock
            for (; index < this.cards.Count; index++)
            {
                var card = this.cards[index];

                card.showBack();
                this.stock.Children.Add(card);
            }

            this.drawCount = Data.KlondikeDraw;
            this.waste.shownCount = this.drawCount;
            base.startGame(shuffle);
        }

        /**
         * When we click on the stock, we move 3 cards to the waste.
         */
        private void onStockMouseUp(object sender, MouseButtonEventArgs e)
        {
            var count = this.stock.Children.Count;

            // move all the cards from the waste back to the stock
            if (count == 0)
            {
                while (this.waste.Children.Count > 0)
                {
                    Card card = (Card)this.waste.Children[0];
                    card.showBack();

                    this.waste.Children.Remove(card);
                    this.stock.Children.Add(card);
                }
            }
            // move some cards to the waste
            else
            {
                for (int a = 0; a < this.drawCount && count > 0; a++)
                {
                    int lastPosition = count - 1;

                    var card = (Card)this.stock.Children[lastPosition];
                    this.stock.Children.RemoveAt(lastPosition);
                    this.waste.Children.Add(card);

                    card.showFront();

                    count = this.stock.Children.Count;
                }
            }

            Audio.playDealingCard();
        }

        public override void cardsPlayed(
            List<Card> cards,
            Container sourceContainer,
            Container destContainer
        )
        {
            base.cardsPlayed(cards, sourceContainer, destContainer);

            foreach (var tableau in this.tableaus)
            {
                tableau.getLast()?.showFront();
            }
        }

        protected override void checkGameEnd()
        {
            this.checkGameEndFoundation();
        }

        public override string getTitle()
        {
            return "Klondike";
        }

        protected override bool isCardDraggable(Card card)
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

            if (!card.hasFrontSide())
            {
                return false;
            }

            return true;
        }

        public override GameKey getGameKey()
        {
            return GameKey.Klondike;
        }

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

            var horizontalMargin = (positionWidth - cardWidth) / 2; // divide by 2 since there's margin in both sides
            var verticalMargin = (positionHeight - cardHeight) / 2;

            // resize all the elements
            foreach (Card card in this.cards)
            {
                card.Height = cardHeight; // the image will maintain the aspect ratio, so only need to set one
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

            Canvas.SetLeft(this.stock, left);
            Canvas.SetTop(this.stock, top);

            // add the waste element next to the stock
            left += cardWidth + horizontalMargin * 2;

            Canvas.SetLeft(this.waste, left);
            Canvas.SetTop(this.waste, top);

            // add the foundations in the top right corner (the foundation is to where the cards need to be stacked (starting on an ace until the king)
            left = canvasWidth - cardWidth - horizontalMargin;

            foreach (var foundation in this.foundations)
            {
                Canvas.SetLeft(foundation, left);
                Canvas.SetTop(foundation, top);

                left -= cardWidth + 2 * horizontalMargin;
            }

            // add the tableau piles (where you can move any card to)
            var tableauHeight = positionHeight * 2 - verticalMargin;

            left = horizontalMargin;
            top += cardHeight + verticalMargin;

            foreach (var tableau in this.tableaus)
            {
                Canvas.SetLeft(tableau, left);
                Canvas.SetTop(tableau, top);

                left += cardWidth + 2 * horizontalMargin;
                tableau.AvailableHeight = tableauHeight;
            }
        }

        /**
         * Try to send the card to the foundation.
         */
        protected override void doubleClick(Card card, Container parent)
        {
            var cards = new List<Card>() { card };

            foreach (var foundation in this.foundations)
            {
                if (foundation.canDrop(cards))
                {
                    this.cardsPlayed(cards, null, foundation);
                    return;
                }
            }
        }

        public override void addMenuElements(Menu container)
        {
            var toFoundation = new MenuItem();
            toFoundation.ToolTip = "alt + f";
            toFoundation.Header = "To _Foundation";
            toFoundation.Click += this.toFoundationClick;

            container.Items.Add(toFoundation);
            this.toFoundation = toFoundation;
        }

        public override void removeMenuElements(Menu container)
        {
            container.Items.Remove(this.toFoundation);
        }

        /**
         * Tries to move all the possible cards from the waste/tableau to the foundation.
         * Useful for the ending of a game, so that you don't have to manually move all the last cards.
         */
        private void toFoundationClick(object sender, RoutedEventArgs e)
        {
            this.sendAllToFoundation();
        }

        /**
         * Add extra rule, only a king can be sent to an empty tableau.
         */
        protected override bool canDrop(List<Card> cards, Container container)
        {
            if (container is Tableau)
            {
                if (container.isEmpty() && cards[0].value != Card.Value.king)
                {
                    return false;
                }
            }

            return base.canDrop(cards, container);
        }

        public override string getHelpUrl()
        {
            return @"http://politaire.com/help/klondike";
        }
    }
}
