using System;
using System.Windows;
using System.Windows.Input;

namespace Solitaire
{
    public partial class Options : Window
    {
        bool init = true;

        public Options(SolitaireGame game)
        {
            InitializeComponent();

            this.setupKeyboardShortcuts();
            this.updateUi();
            this.init = false;
        }

        private void updateUi()
        {
            var musicVolume = Audio.getMusicVolume() * 100;
            this.MusicVolume.Value = musicVolume;
            this.MusicVolumeLabel.Content = musicVolume + "%";

            var soundVolume = Audio.getSoundVolume() * 100;
            this.SoundVolume.Value = soundVolume;
            this.SoundVolumeLabel.Content = soundVolume + "%";

            var klondikeDraw = Data.KlondikeDraw;
            this.KlondikeDraw.Value = klondikeDraw;
            this.KlondikeDrawLabel.Content = klondikeDraw.ToString();
        }

        private void setupKeyboardShortcuts()
        {
            // esc -- close the window
            var close = new RoutedCommand();
            close.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBindings.Add(new CommandBinding(close, this.closeWindow));
        }

        private void closeWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void musicVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.init)
            {
                return;
            }

            var volume = this.MusicVolume.Value;

            Audio.setMusicVolume(volume / 100);
            this.MusicVolumeLabel.Content = volume + "%";
        }

        private void soundVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.init)
            {
                return;
            }

            var volume = this.SoundVolume.Value;

            Audio.setSoundVolume(volume / 100);
            this.SoundVolumeLabel.Content = volume + "%";
        }

        private void klondikeDrawChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.init)
            {
                return;
            }

            var draw = this.KlondikeDraw.Value;

            Data.KlondikeDraw = (int)draw;
            this.KlondikeDrawLabel.Content = draw.ToString();
        }
    }
}
