using System.Windows;

namespace Solitaire
{
    public partial class App : Application
    {
        private void applicationExit(object sender, ExitEventArgs e)
        {
            Data.save();
        }
    }
}
