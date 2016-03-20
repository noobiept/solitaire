using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Solitaire
    {
    public partial class MainWindow : Window
        {
        SolitaireGame currentGame;


        public MainWindow()
            {
            InitializeComponent();

            this.selectGame( typeof( FreeCell ) );
            }


        public void selectGame( Type gameType )
            {
            if ( this.currentGame != null )
                {
                if ( this.currentGame.GetType() == gameType )
                    {
                    this.currentGame.restart();
                    return;
                    }

                this.clearGame();
                }

            this.currentGame = (SolitaireGame) Activator.CreateInstance( gameType, new object[] { this.MainCanvas } );
            this.setupGameShortcuts();
            this.currentGame.addMenuElements( this.MainMenu );
            this.currentGame.addInfoElements( this.Info );
            this.currentGame.positionResizeElements();
            this.Title = this.currentGame.getTitle();
            }


        public void clearGame()
            {
            this.currentGame.removeMenuElements( this.MainMenu );
            this.currentGame.removeInfoElements( this.Info );
            this.currentGame.clear();
            this.MainCanvas.Children.Clear();
            this.removeGameShortcuts();
            this.currentGame = null;
            }


        /**
         * Game specific keyboard shortcuts.
         */
        private void setupGameShortcuts()
            {
            var shortcuts = this.currentGame.shortcuts;

            if ( shortcuts != null )
                {
                for (var a = 0 ; a < shortcuts.Length ; a++)
                    {
                    this.CommandBindings.Add( shortcuts[ a ] );
                    }
                }
            }


        private void removeGameShortcuts()
            {
            var shortcuts = this.currentGame.shortcuts;

            if ( shortcuts != null )
                {
                for (var a = 0 ; a < shortcuts.Length ; a++)
                    {
                    this.CommandBindings.Remove( shortcuts[ a ] );
                    }
                }
            }


        /**
         * Global keyboard shortcuts (works for any game).
         */
        private void setupGlobalShortcuts()
            {
                // ctrl + n -- start a new game
            var newGame = new RoutedCommand();
            newGame.InputGestures.Add( new KeyGesture( Key.N, ModifierKeys.Control ) );
            this.CommandBindings.Add( new CommandBinding( newGame, this.newGameClick ) );

                // ctrl + r -- restart the game
            var restart = new RoutedCommand();
            restart.InputGestures.Add( new KeyGesture( Key.R, ModifierKeys.Control ) );
            this.CommandBindings.Add( new CommandBinding( restart, this.restartSameGameClick ) );

                // ctrl + s -- open the statistics window
            var openStatistics = new RoutedCommand();
            openStatistics.InputGestures.Add( new KeyGesture( Key.S, ModifierKeys.Control ) );
            this.CommandBindings.Add( new CommandBinding( openStatistics, this.openStatisticsWindow ) );

                // ctrl + a -- open the about webpage
            var openAbout = new RoutedCommand();
            openAbout.InputGestures.Add( new KeyGesture( Key.A, ModifierKeys.Control ) );
            this.CommandBindings.Add( new CommandBinding( openAbout, this.openAboutPage ) );
            }


        /**
         * When the window is resized, we need to reposition the game elements.
         */
        private void onSizeChange( object sender, SizeChangedEventArgs e )
            {
            this.currentGame.positionResizeElements();
            }


        private void onStateChange( object sender, EventArgs e )
            {
            this.currentGame.positionResizeElements();
            }


        private void onWindowClosing( object sender, System.ComponentModel.CancelEventArgs e )
            {
            this.currentGame.clear();
            }


        private void newGameClick( object sender, RoutedEventArgs e )
            {
            this.currentGame.restart();
            }


        private void restartSameGameClick( object sender, RoutedEventArgs e )
            {
            this.currentGame.restartSameGame();
            }


        private void openStatisticsWindow( object sender, RoutedEventArgs e )
            {
            var statistics = new Statistics();
            statistics.ShowDialog();
            }


        private void openAboutPage( object sender, RoutedEventArgs e )
            {
            System.Diagnostics.Process.Start( "https://bitbucket.org/drk4/gold_mine" );
            }


        private void selectFreeCell( object sender, RoutedEventArgs e )
            {
            selectGame( typeof( FreeCell ) );
            }


        private void selectGoldMine( object sender, RoutedEventArgs e )
            {
            selectGame( typeof( GoldMine ) );
            }
        }
    }
