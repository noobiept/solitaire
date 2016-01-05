using System;
using System.Collections.Generic;


namespace GoldMine
    {
    public static class Utilities
        {
        public struct Box
            {
            public double x;
            public double y;
            public double width;
            public double height;
            }


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


        public static double calculateIntersectionArea( Box one, Box two )
            {
            var left = Math.Max( one.x, two.x );
            var right = Math.Min( one.x + one.width, two.x + two.width );
            var bottom = Math.Min( one.y + one.height, two.y + two.height );
            var top = Math.Max( one.y, two.y );

                // if there's an intersection
            if (left < right && bottom > top)
                {
                return (right - left) * (bottom - top);
                }

            return 0;
            }


        public static bool boxBoxCollision( Box one, Box two )
            {
            return !(
                    one.x > two.x + two.width ||
                    one.x + one.width < two.x ||
                    one.y > two.y + two.height ||
                    one.y + one.height < two.y
                );
            }
        }
    }
