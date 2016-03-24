using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace Solitaire
    {
    // needs to be the same as the class name
    public enum GameKey
        {
        GoldMine, FreeCell, Klondike
        }

    public partial class MainWindow : Window
        {
        SolitaireGame currentGame;


        public MainWindow()
            {
            InitializeComponent();

            Data.load();
            var selectedGame = Data.getSelectedGame();
            Type gameType = Type.GetType( "Solitaire." + selectedGame.ToString() );

            this.selectGame( gameType );
            Audio.playBackgroundMusic();
            }


        public void selectGame( Type gameType )
            {
            if( this.currentGame != null )
                {
                if( this.currentGame.GetType() == gameType )
                    {
                    this.currentGame.restart();
                    return;
                    }

                this.clearGame();
                }

            this.currentGame = (SolitaireGame) Activator.CreateInstance( gameType, new object[] { this.MainCanvas } );
            this.currentGame.addMenuElements( this.MainMenu );
            this.currentGame.addInfoElements( this.Info );
            this.currentGame.positionResizeElements();
            this.Title = this.currentGame.getTitle();

            Data.setSelectedGame( this.currentGame.getGameKey() );
            }


        public void clearGame()
            {
            this.currentGame.removeMenuElements( this.MainMenu );
            this.currentGame.removeInfoElements( this.Info );
            this.currentGame.clear();
            this.MainCanvas.Children.Clear();
            this.currentGame = null;
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


        private void openOptionsWindow( object sender, RoutedEventArgs e )
            {
            var options = new Options( this.currentGame );
            options.ShowDialog();
            }


        private void openStatisticsWindow( object sender, RoutedEventArgs e )
            {
            var statistics = new Statistics( this.currentGame.getGameKey() );
            statistics.ShowDialog();
            }


        private void openHelpWindow( object sender, RoutedEventArgs e )
            {
            var help = new Help( this.currentGame );
            help.ShowDialog();
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


        private void selectKlondike( object sender, RoutedEventArgs e )
            {
            selectGame( typeof( Klondike ) );
            }
        }
    }
