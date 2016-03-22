using System.Windows;
using System.Windows.Input;


namespace Solitaire
    {
    public partial class Help : Window
        {
        public Help( SolitaireGame game )
            {
            InitializeComponent();

            this.helpBody.Text = game.getHelpText();
            this.setupKeyboardShortcuts();
            }


        private void setupKeyboardShortcuts()
            {
            // esc -- close the window
            var close = new RoutedCommand();
            close.InputGestures.Add( new KeyGesture( Key.Escape ) );
            CommandBindings.Add( new CommandBinding( close, this.closeWindow ) );
            }


        private void closeWindow( object sender, RoutedEventArgs e )
            {
            this.Close();
            }
        }
    }
