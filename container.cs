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
        public Container()
            {
            this.Width = 150;
            this.Height = 200;
            }


        public void applyDropEffect()
            {
            this.Opacity = 0.5;
            }


        public void removeDropEffect()
            {
            this.Opacity = 1;
            }


        protected override Size MeasureOverride( Size availableSize )
            {
            Size panelDesiredSize = new Size();

            foreach( UIElement child in this.InternalChildren )
                {
                child.Measure( availableSize );
                panelDesiredSize = child.DesiredSize;
                }

            return panelDesiredSize;
            }
        }
    }
