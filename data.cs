using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;


namespace GoldMine
    {
    public static class Data
        {
        public struct AppData
            {
            public uint totalWins;
            public uint bestTime;

            public int version;         // version of the loaded data structure (useful to compare with the application version, when updating from different versions that have incompatible changes)
            }

        static public AppData DATA;

        #if DEBUG
            const string FILE_NAME = "data_debug.txt";
        #else
            const string FILE_NAME = "data.txt";
        #endif
        static string DATA_PATH = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "gold_mine", Data.FILE_NAME );

        const int DATA_VERSION = 1;  // current version of the application data


        static public void load()
            {
            try
                {
                StreamReader file = new StreamReader( Data.DATA_PATH, Encoding.UTF8 );

                string jsonData = file.ReadToEnd();
                file.Close();

                Data.DATA = JsonConvert.DeserializeObject<AppData>( jsonData );

                if ( Data.DATA.version != Data.DATA_VERSION )
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
                    totalWins = 0,
                    bestTime = 0,
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
        

        static private void save()
            {
            string dataJson = JsonConvert.SerializeObject( Data.DATA );

                // make sure there's a directory created (otherwise the stream writer call will fail)
            System.IO.Directory.CreateDirectory( Path.GetDirectoryName( Data.DATA_PATH ) );
            StreamWriter file = new StreamWriter( Data.DATA_PATH, false, Encoding.UTF8 );

            file.Write( dataJson );
            file.Close();
            }


        static public uint oneMoreWin( uint time )
            {
            Data.DATA.totalWins++;
            
                // first win
            if ( Data.DATA.bestTime == 0 )
                {
                Data.DATA.bestTime = time;
                }

            else if ( time < Data.DATA.bestTime )
                {
                Data.DATA.bestTime = time;
                }

            Data.save();

            return Data.DATA.bestTime;
            }


        static public void resetStatistics()
            {
            Data.DATA.totalWins = 0;
            Data.DATA.bestTime = 0;
            Data.save();
            }
        }
    }