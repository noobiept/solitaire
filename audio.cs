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


        static Audio()
            {
            Audio.musicPlayer.MediaEnded += Audio.changeMusic;
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
        }
    }
