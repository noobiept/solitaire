using System.Windows;
using System.Windows.Input;


namespace Solitaire
    {
    public partial class Statistics : Window
        {
        GameKey gameKey;


        public Statistics( GameKey gameKey )
            {
            InitializeComponent();

            this.gameKey = gameKey;
            this.updateUi();
            this.setupKeyboardShortcuts();
            }


        private void updateUi()
            {
            var gameData = Data.get( this.gameKey );
            uint totalWins = gameData.totalWins;
            uint bestTime = gameData.bestTime;

            this.TotalWins.Text = gameData.totalWins.ToString();

            if( bestTime == 0 )
                {
                this.BestTime.Text = "---";
                }

            else
                {
                this.BestTime.Text = Utilities.timeToString( (int) bestTime );
                }
            }


        private void setupKeyboardShortcuts()
            {
            // esc -- close the window
            var close = new RoutedCommand();
            close.InputGestures.Add( new KeyGesture( Key.Escape ) );
            CommandBindings.Add( new CommandBinding( close, this.closeWindow ) );
            }


        private void resetStatistics( object sender, RoutedEventArgs e )
            {
            var result = MessageBox.Show( "Reset the statistics?", "Reset the statistics?", MessageBoxButton.OKCancel );

            if( result == MessageBoxResult.OK )
                {
                Data.resetStatistics( this.gameKey );
                this.updateUi();
                }
            }


        private void closeWindow( object sender, RoutedEventArgs e )
            {
            this.Close();
            }
        }
    }
