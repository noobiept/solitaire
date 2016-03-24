using System;
using System.Windows.Media;


namespace Solitaire
    {
    static public class Audio
        {
        static private MediaPlayer musicPlayer = new MediaPlayer();
        static private Uri[] musicUrls = new Uri[ 4 ]
            {
            new Uri( @"sounds/music1 - peaceful scene.mp3", UriKind.Relative ),
            new Uri( @"sounds/music2 - northern isles.mp3", UriKind.Relative ),
            new Uri( @"sounds/music3 - home.mp3", UriKind.Relative ),
            new Uri( @"sounds/music4 - cats and spies.mp3", UriKind.Relative )
            };
        static private int currentMusic = 0;

        static private MediaPlayer soundPlayer = new MediaPlayer();
        static private Uri dealingCard = new Uri( @"sounds/dealing card.mp3", UriKind.Relative );
        static private Uri error = new Uri( @"sounds/error.mp3", UriKind.Relative );
        static private Uri victory = new Uri( @"sounds/victory.mp3", UriKind.Relative );


        static public void init()
            {
            Audio.musicPlayer.MediaEnded += Audio.changeMusic;

            var options = Data.getOptions();
            Audio.musicPlayer.Volume = options.musicVolume;
            Audio.soundPlayer.Volume = options.soundVolume;
            }


        static public void playBackgroundMusic()
            {
            Audio.musicPlayer.Open( Audio.musicUrls[ Audio.currentMusic ] );
            Audio.musicPlayer.Play();
            }


        static private void changeMusic( object sender, EventArgs e )
            {
            Audio.currentMusic++;
            if( Audio.currentMusic >= Audio.musicUrls.Length )
                {
                Audio.currentMusic = 0;
                }
            Audio.playBackgroundMusic();
            }


        static public void playDealingCard()
            {
            Audio.soundPlayer.Open( Audio.dealingCard );
            Audio.soundPlayer.Play();
            }


        static public void playError()
            {
            Audio.soundPlayer.Open( Audio.error );
            Audio.soundPlayer.Play();
            }


        static public void playVictory()
            {
            Audio.soundPlayer.Open( Audio.victory );
            Audio.soundPlayer.Play();
            }


        /**
         * Set music volume (between 0 and 1).
         */
        static public void setMusicVolume( double volume )
            {
            Audio.musicPlayer.Volume = volume;
            Data.setMusicVolume( volume );
            }


        static public double getMusicVolume()
            {
            return Audio.musicPlayer.Volume;
            }


        /**
         * Set the game sound volume (between 0 and 1).
         */
        static public void setSoundVolume( double volume )
            {
            Audio.soundPlayer.Volume = volume;
            Data.setSoundVolume( volume );
            }


        static public double getSoundVolume()
            {
            return Audio.soundPlayer.Volume;
            }
        }
    }
