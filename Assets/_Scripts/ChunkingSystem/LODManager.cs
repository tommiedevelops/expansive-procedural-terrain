using System;
using System.Collections.Generic;



// What is the purpose of this class?
// * To manage the number of vertices in each chunk as a function of sidelength.
namespace _Scripts.ChunkingSystem
{
    public class LODManager
    {
        // This value represents the minimum size of a single chunk in integer multiples of
        // world units. (that is, the distance between (1,0,0) and (0,0,0) in unity. 
        
        // ** TO DO ** 
        // This variable needs sufficient input validation
        private int _minChunkSize = 120;
        private int[] _lods;
        public void SetMinChunkSize(int minChunkSize) { this._minChunkSize = minChunkSize; }
        public int GetMinChunkSize() {return this._minChunkSize;}
        
        public static int[] ComputeLODs(int number)
        {
            var factorList = new List<int>();

            if (number <= 0)
                throw new ArgumentException("Number must be a positive integer.");

            for (int i = 1; i <= Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                {
                    factorList.Add(i);

                    if (i != number / i) // Avoid adding square roots twice
                        factorList.Add(number / i);
                }
            }

            factorList.Sort(); // Optional: return factors in ascending order

            int[] factorArray = factorList.ToArray();
            return factorArray;
        }
    }
}
