using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace GoldMine
    {
    public class Tableau : Panel
        {
        public Tableau() : base()
            {
            this.Width = 150;
            this.Height = 200;
            this.Background = Brushes.BlueViolet;
            }


        protected override Size MeasureOverride( Size availableSize )
            {
            Size panelDesiredSize = new Size();

            foreach (UIElement child in this.InternalChildren)
                {
                child.Measure( availableSize );
                panelDesiredSize = child.DesiredSize;
                }

            return panelDesiredSize;
            }


        protected override Size ArrangeOverride( Size finalSize )
            {
            int y = 0;
            int step = 20;

            foreach (UIElement child in this.InternalChildren)
                {
                child.Arrange( new Rect( new Point( 0, y ), child.DesiredSize ) );

                y += step;
                }

            return finalSize;
            }
        }
    }
