// 
//  Copyright (c) 2022 Sendbird, Inc.
// 

namespace Sendbird.Calls
{
    /// <summary>
    /// A Long Range with half-open interval from a lower bound up to, an upper bound.
    /// </summary>
    public class SbRange
    {
        /// <summary>
        /// Determines the value for lower bound.
        /// </summary>
        public long? LowerBound { get; set; }
        /// <summary>
        /// Determines the value for upper bound.
        /// </summary>
        public long? UpperBound { get; set; }

        /// <summary>
        /// Creates range that represents greater than or equal to given lower bound and less than or equal to given upper bound.
        /// </summary>
        public SbRange()
        {
            LowerBound = null;
            UpperBound = null;
        }

        /// <summary>
        /// Creates range that represents greater than or equal to given lower bound and less than or equal to given upper bound.
        /// </summary>
        /// <param name="lowerBound">Determines the value for lower bound.</param>
        /// <param name="upperBound">Determines the value for upper bound.</param>
        public SbRange(long lowerBound, long upperBound)
        {
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        /// <summary>
        /// Creates range that represents less than or equal to given upper bound.
        /// </summary>
        /// <param name="upperBound">Determines the value for upper bound.</param>
        /// <returns></returns>
        public static SbRange LessThanOrEqualTo(long upperBound)
        {
            return new SbRange { UpperBound = upperBound };
        }

        /// <summary>
        /// Creates range that represents greater than or equal to given lower bound.
        /// </summary>
        /// <param name="lowerBound">Determines the value for lower bound.</param>
        /// <returns></returns>
        public static SbRange GreaterThanOrEqualTo(long lowerBound)
        {
            return new SbRange { LowerBound = lowerBound };
        }
    }
}