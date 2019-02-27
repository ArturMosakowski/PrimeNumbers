using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers
{
    class Cycle
    {
        public List<int> primeNumber = new List<int>();
        public int cycleNumber { get; set; }
        public int lastPrimeNumber { get; set; }
        public int lastFoundMin { get; set; }
        public int lastFoundSec { get; set; }
        public int cycleTimeMin { get; set; }
        public int cycleTimeSec { get; set; }

        /// <summary>
        /// Checking if number is prime.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool IfPrime(int number)
        {
            if (number < 2)
            {
                return false;
            }

            for (int i = 2; i * i <= number; i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
