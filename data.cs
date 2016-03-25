using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Solitaire
    {
    public static class Data
        {
        public class GameData
            {
            public uint totalWins;
            public uint bestTime;
            }

        public struct OptionsData
            {
            public double soundVolume;
            public double musicVolume;
            public int klondikeDraw;
            }

        public struct AppData
            {
            public Dictionary<GameKey, GameData> data;

            public OptionsData options;
            public GameKey selectedGame;
            public int version;         // version of the loaded data structure (useful to compare with the application version, when updating from different versions that have incompatible changes)
            }

        static public AppData DATA;

#if DEBUG
        const string FILE_NAME = "data_debug.txt";
#else
        const string FILE_NAME = "data.txt";
#endif
        static string DATA_PATH = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "solitaire", Data.FILE_NAME );

        const int DATA_VERSION = 1;  // current version of the application data


        static public void load()
            {
            try
                {
                StreamReader file = new StreamReader( Data.DATA_PATH, Encoding.UTF8 );

                string jsonData = file.ReadToEnd();
                file.Close();

                Data.DATA = JsonConvert.DeserializeObject<AppData>( jsonData );

                if( Data.DATA.version != Data.DATA_VERSION )
                    {
                    Data.update();
                    }
                }

            catch( Exception )
                {
                Data.loadDefaults();
                }
            }


        static private void loadDefaults()
            {
            Data.DATA = new AppData {
                data = new Dictionary<GameKey, GameData>(),
                selectedGame = GameKey.FreeCell,
                options = { musicVolume = 0.5, soundVolume = 0.5 },
                version = Data.DATA_VERSION
                };
            }


        /**
         * Update the data from a previous version.
         */
        static private void update()
            {
            // no updates yet
            }


        static public void save()
            {
            string dataJson = JsonConvert.SerializeObject( Data.DATA );

            // make sure there's a directory created (otherwise the stream writer call will fail)
            System.IO.Directory.CreateDirectory( Path.GetDirectoryName( Data.DATA_PATH ) );
            StreamWriter file = new StreamWriter( Data.DATA_PATH, false, Encoding.UTF8 );

            file.Write( dataJson );
            file.Close();
            }


        static public uint oneMoreWin( GameKey gameKey, uint time )
            {
            var gameData = Data.getGame( gameKey );

            gameData.totalWins++;

            // first win
            if( gameData.bestTime == 0 )
                {
                gameData.bestTime = time;
                }

            else if( time < gameData.bestTime )
                {
                gameData.bestTime = time;
                }

            return gameData.bestTime;
            }


        static public void resetStatistics( GameKey gameKey )
            {
            Data.DATA.data[ gameKey ] = new GameData { bestTime = 0, totalWins = 0 };
            }


        static public GameData getGame( GameKey key )
            {
            if( !Data.DATA.data.ContainsKey( key ) )
                {
                Data.DATA.data[ key ] = new GameData { bestTime = 0, totalWins = 0 };
                }

            return Data.DATA.data[ key ];
            }


        static public GameKey SelectedGame
            {
            get { return Data.DATA.selectedGame; }
            set { Data.DATA.selectedGame = value; }
            }


        static public double SoundVolume
            {
            get { return Data.DATA.options.soundVolume; }
            set { Data.DATA.options.soundVolume = value; }
            }


        static public double MusicVolume
            {
            get { return Data.DATA.options.musicVolume; }
            set { Data.DATA.options.musicVolume = value; }
            }


        static public int KlondikeDraw
            {
            get { return Data.DATA.options.klondikeDraw; }
            set { Data.DATA.options.klondikeDraw = value; }
            }
        }
    }