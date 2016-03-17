using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace GoldMine
    {
    interface Solitaire
        {
        void restart();
        void positionResizeElements();
        void end();
        }


    public partial class MainWindow : Window
        {
        Solitaire currentGame;


        public MainWindow()
            {
            InitializeComponent();

            this.currentGame = new GoldMine( this.MainCanvas );
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
            this.currentGame.end();
            }
        }
    }
