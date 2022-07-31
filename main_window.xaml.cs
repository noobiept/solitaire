using System;
using System.Diagnostics;
using System.Windows;


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
            Audio.init();
            Type gameType = Type.GetType( "Solitaire." + Data.SelectedGame.ToString() );

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

            Data.SelectedGame = this.currentGame.getGameKey();
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
            this.openExternalUrl( this.currentGame.getHelpUrl() );
            }


        private void openAboutPage( object sender, RoutedEventArgs e )
            {
            this.openExternalUrl( "https://github.com/noobiept/solitaire" );
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

        private void closeGame( object sender, RoutedEventArgs e )
            {
            Application.Current.Shutdown();
            }

        private void openExternalUrl(String url)
            {
            System.Diagnostics.Process.Start(
                new ProcessStartInfo
                    {
                    FileName = url,
                    UseShellExecute = true,
                    }
            );
        }
        }
    }
