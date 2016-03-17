using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Solitaire
    {
    interface SolitaireGame
        {
        void restart();
        void restartSameGame();
        void positionResizeElements();
        void end();

        void addMenuElements( StackPanel container );
        void addInfoElements( StackPanel container );
        }


    public partial class MainWindow : Window
        {
        SolitaireGame currentGame;


        public MainWindow()
            {
            InitializeComponent();

            this.currentGame = new GoldMine( this.MainCanvas, this.CustomButtons, this.CustomInfo );
            this.setupKeyboardShortcuts();
            }


        private void setupKeyboardShortcuts()
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

            // ctrl + f -- try to move all the possible cards to the foundation
            /*var moveToFoundation = new RoutedCommand();
            moveToFoundation.InputGestures.Add( new KeyGesture( Key.F, ModifierKeys.Control ) );
            this.CommandBindings.Add( new CommandBinding( moveToFoundation, this.toFoundationClick ) );*/
            //HERE

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
            this.currentGame.end();
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
        }
    }
