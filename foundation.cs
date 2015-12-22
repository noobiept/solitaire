using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;


namespace GoldMine
    {
    public class Foundation : StackPanel
        {
        public Foundation( double left, double top )
            {
            this.Width = 200;
            this.Height = 200;
            this.Background = Brushes.AntiqueWhite;

            Canvas.SetLeft( this, left );
            Canvas.SetTop( this, top );
            }
        }
    }
