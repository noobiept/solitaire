using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace GoldMine
    {
    public class Container : Panel
        {
        public void applyDropEffect()
            {
            this.Opacity = 0.5;
            }


        public void removeDropEffect()
            {
            this.Opacity = 1;
            }
        }
    }
