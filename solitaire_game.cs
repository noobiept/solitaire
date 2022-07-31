using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;

namespace Solitaire
{
    public abstract class SolitaireGame
    {
        // data use for the drag and drop operation of cards
        public struct Drag
        {
            public bool isDragging;
            public const int diff = 30; // space between each card during the drag
            public List<Card> cardsDragging;
            public Point clickPosition;
            public Container originalContainer; // original container before the drag occurred. if the drag isn't valid, we need to return the cards to the original place
            public Container highlightedContainer; // when dragging a card on top of a container, highlight that container, and keep a reference to it (to know when to remove the highlight)
        }

        protected Timer timer;
        protected uint secondsPassed;
        protected Canvas canvas;
        protected readonly List<Container> droppableElements = new List<Container>();
        private Drag drag;

        protected readonly List<Foundation> foundations = new List<Foundation>();
        protected readonly List<Cell> cells = new List<Cell>();
        protected readonly List<Tableau> tableaus = new List<Tableau>();
        protected readonly List<Card> cards = new List<Card>();
        protected Stock stock;
        protected Waste waste;

        public SolitaireGame(Canvas canvas)
        {
            this.timer = new Timer(1000);
            this.timer.Elapsed += this.onTimeElapsed;

            this.canvas = canvas;
            this.drag.cardsDragging = new List<Card>();
        }

        public virtual void startGame(bool shuffle = true)
        {
            this.timer.Stop();
            this.secondsPassed = 0;
            this.updateTimePassed();
            this.timer.Start();
        }

        /**
         * Determine the dimensions of the cards stack.
         * Use the first card to determine the x/y/width.
         * The height is calculated from the number of cards.
         * The 'diff' is the space between each card.
         */
        private Utilities.Box cardsDimension(List<Card> cards)
        {
            var firstCard = cards[0];
            return new Utilities.Box
            {
                x = Canvas.GetLeft(firstCard),
                y = Canvas.GetTop(firstCard),
                width = firstCard.ActualWidth,
                height = firstCard.ActualHeight + Drag.diff * (cards.Count - 1)
            };
        }

        /**
         * Calculates the intersection area between the reference element and the droppable elements, and returns the one where the area was higher.
         */
        private Container collisionDetection(List<Card> cards)
        {
            var cardsBox = this.cardsDimension(cards);
            Container colliding = null;
            double collidingArea = 0;

            for (var a = 0; a < this.droppableElements.Count; a++)
            {
                var container = this.droppableElements[a];

                if (this.canDrop(cards, container))
                {
                    var containerBox = container.getDimensionBox();

                    var area = Utilities.calculateIntersectionArea(cardsBox, containerBox);

                    if (area > collidingArea)
                    {
                        collidingArea = area;
                        colliding = container;
                    }
                }
            }

            return colliding;
        }

        /**
         * Checks if the cards can be dropped into the given container. Can be extended by derived classes for games that have extra rules.
         */
        protected virtual bool canDrop(List<Card> cards, Container container)
        {
            if (container != this.drag.originalContainer && container.canDrop(cards))
            {
                return true;
            }

            return false;
        }

        /**
         * Finishes the drag operation, moving a list of cards to a container.
         */
        protected void moveCards(List<Card> cards, Container container)
        {
            if (this.drag.highlightedContainer != null)
            {
                this.drag.highlightedContainer.removeDropEffect();
                this.drag.highlightedContainer = null;
            }

            foreach (Card card in cards)
            {
                var parent = card.Parent as Panel;
                parent.Children.Remove(card);
                container.Children.Add(card);
            }

            this.drag.originalContainer = null;
            this.drag.cardsDragging.Clear();
            this.drag.isDragging = false;
        }

        private void positionCards(List<Card> cards, MouseEventArgs e)
        {
            var position = e.GetPosition(this.canvas);

            for (int a = 0; a < cards.Count; a++)
            {
                Canvas.SetLeft(cards[a], position.X - this.drag.clickPosition.X);
                Canvas.SetTop(cards[a], position.Y - this.drag.clickPosition.Y + Drag.diff * a);
            }

            var container = this.collisionDetection(cards);

            if (this.drag.highlightedContainer != null)
            {
                this.drag.highlightedContainer.removeDropEffect();
                this.drag.highlightedContainer = null;
            }

            if (container != null)
            {
                container.applyDropEffect();

                this.drag.highlightedContainer = container;
            }
        }

        protected void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            var card = (Card)sender;
            var parent = card.Parent as Container;

            if (!this.isCardDraggable(card))
            {
                return;
            }

            if (e.ClickCount == 2)
            {
                this.doubleClick(card, parent);
                return;
            }

            if (this.drag.isDragging == true)
            {
                this.moveCards(this.drag.cardsDragging, this.drag.originalContainer);
                return;
            }

            this.drag.isDragging = true;
            this.drag.clickPosition = Mouse.GetPosition(card);
            this.drag.originalContainer = parent;
            parent.dragCards(card, this.drag.cardsDragging);

            foreach (Card dragCard in this.drag.cardsDragging)
            {
                parent.Children.Remove(dragCard);
                this.canvas.Children.Add(dragCard);
            }

            this.positionCards(this.drag.cardsDragging, e);
        }

        protected void onMouseMove(object sender, MouseEventArgs e)
        {
            if (this.drag.isDragging)
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    this.moveCards(this.drag.cardsDragging, this.drag.originalContainer);
                }
                else
                {
                    this.positionCards(this.drag.cardsDragging, e);
                }
            }
        }

        protected void onMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!this.drag.isDragging)
            {
                return;
            }

            var container = this.collisionDetection(this.drag.cardsDragging);

            if (container != null)
            {
                this.cardsPlayed(this.drag.cardsDragging, this.drag.originalContainer, container);
            }
            // wasn't dropped on any container, so its not a valid drag operation. return to the original container
            else
            {
                this.moveCards(this.drag.cardsDragging, this.drag.originalContainer);
                Audio.playError();
            }
        }

        public void restart()
        {
            this.startGame();
        }

        public void restartSameGame()
        {
            this.startGame(false);
        }

        private void updateTimePassed()
        {
            Application.Current.Dispatcher.Invoke(
                (
                    () =>
                    {
                        var mainWindow = (MainWindow)Application.Current.MainWindow;

                        if (mainWindow != null)
                        {
                            mainWindow.TimePassed.Text =
                                "Time: " + Utilities.timeToString((int)this.secondsPassed);
                        }
                    }
                )
            );
        }

        private void onTimeElapsed(Object source, ElapsedEventArgs e)
        {
            this.secondsPassed++;
            this.updateTimePassed();
        }

        virtual public void clear()
        {
            this.timer.Stop();
        }

        /**
         * When the game ending condition is having all cards in the foundation.
         */
        virtual protected void checkGameEndFoundation()
        {
            int cardCount = this.cards.Count;
            int foundationCount = 0;

            foreach (var foundation in this.foundations)
            {
                foundationCount += foundation.Children.Count;
            }

            // game has ended
            if (cardCount == foundationCount)
            {
                this.timer.Stop();

                var best = Data.oneMoreWin(this.getGameKey(), this.secondsPassed);
                var message = String.Format(
                    "You Win!\nTime: {0}",
                    Utilities.timeToString((int)this.secondsPassed)
                );

                if (this.secondsPassed == best)
                {
                    message += "\nYou beat your best time!";
                }

                Audio.playVictory();
                MessageBox.Show(message, "Game Over!", MessageBoxButton.OK);
                this.startGame();
            }
        }

        virtual public void cardsPlayed(
            List<Card> cards,
            Container sourceContainer,
            Container destContainer
        )
        {
            this.moveCards(cards, destContainer);
            this.checkGameEnd();
            Audio.playDealingCard();
        }

        /**
         * Try to send the last card of a container to a foundation.
         */
        protected bool sendToFoundation(Container container)
        {
            var last = container.getLast();

            // need to have a list to work with the 'canDrop' function
            var cards = new List<Card>();
            cards.Add(last);

            foreach (var foundation in this.foundations)
            {
                if (foundation.canDrop(cards))
                {
                    this.cardsPlayed(cards, null, foundation);
                    return true;
                }
            }

            return false;
        }

        /**
         * Tries to move all the possible cards from the waste/tableau to the foundation.
         * Useful for the ending of a game, so that you don't have to manually move all the last cards.
         */
        protected void sendAllToFoundation()
        {
            // if a card was moved to the foundation
            // we keep checking until there's no more possible moves
            bool moved = false;

            do
            {
                moved = false;

                if (this.sendToFoundation(this.waste))
                {
                    moved = true;
                }

                foreach (var tableau in this.tableaus)
                {
                    if (this.sendToFoundation(tableau))
                    {
                        moved = true;
                    }
                }
            } while (moved == true);
        }

        virtual public void addMenuElements(Menu container) { }

        virtual public void addInfoElements(Panel container) { }

        virtual public void removeMenuElements(Menu container) { }

        virtual public void removeInfoElements(Panel container) { }

        abstract public void positionResizeElements();
        abstract protected bool isCardDraggable(Card card);
        abstract protected void checkGameEnd();
        abstract protected void doubleClick(Card card, Container parent);
        abstract public string getTitle();
        abstract public GameKey getGameKey();
        abstract public string getHelpUrl();
    }
}
