using System;
using System.Collections.Generic;


namespace GoldMine
    {
    class Utilities
        {
        public static void shuffle<Type>( List<Type> list )
            {
            var currentIndex = list.Count;
            Type temporaryValue;
            int randomIndex;
            var random = new Random();

                // while there's still elements to shuffle
            while( currentIndex != 0 )
                {
                    // pick a remaining element
                randomIndex = random.Next( currentIndex );
                currentIndex--;

                    // swap it with the current element
                temporaryValue = list[ currentIndex ];
                list[ currentIndex ] = list[ randomIndex ];
                list[ randomIndex ] = temporaryValue;
                }
            }
        }
    }
