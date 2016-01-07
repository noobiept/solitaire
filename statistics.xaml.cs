using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace GoldMine
    {
    public partial class Statistics : Window
        {
        public Statistics()
            {
            InitializeComponent();

            this.updateUi();
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


        private void resetStatistics( object sender, RoutedEventArgs e )
            {
            Data.resetStatistics();
            this.updateUi();
            }


        private void closeWindow( object sender, RoutedEventArgs e )
            {
            this.Close();
            }
        }
    }
