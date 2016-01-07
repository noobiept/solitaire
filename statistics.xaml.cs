using System.Windows;
using System.Windows.Input;


namespace GoldMine
    {
    public partial class Statistics : Window
        {
        public Statistics()
            {
            InitializeComponent();
            
            this.updateUi();
            this.setupKeyboardShortcuts();
            }


        private void updateUi()
            {
            uint totalWins = Data.DATA.totalWins;
            uint bestTime = Data.DATA.bestTime;

            this.TotalWins.Text = Data.DATA.totalWins.ToString();

            if ( bestTime == 0 )
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

            if ( result == MessageBoxResult.OK )
                {
                Data.resetStatistics();
                this.updateUi();
                }
            }


        private void closeWindow( object sender, RoutedEventArgs e )
            {
            this.Close();
            }
        }
    }
