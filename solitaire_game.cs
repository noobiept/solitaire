using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Solitaire
    {
    public abstract class SolitaireGame
        {
        public CommandBinding[] shortcuts;

        abstract public void startGame( bool shuffle= true );
        abstract public void restart();
        abstract public void restartSameGame();
        abstract public void positionResizeElements();
        abstract public void end();

        abstract public void addMenuElements( StackPanel container );
        abstract public void addInfoElements( StackPanel container );
        }
    }
