using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

// What is the purpose of this class?
// * To manage the number of vertices in each chunk as a function of sidelength.
namespace _Scripts.ChunkingSystem
{
    
    //ReSharper disable all
    public class LODManager
    {
        // For the purposes of this program LOD represents the integer number of vertices used
        // to represent the sidelength of a chunk
        
        // This value represents the minimum size of a single chunk in integer multiples of
        // world units. (that is, the distance between (1,0,0) and (0,0,0) in unity. 
        
        // ** TO DO ** 
        // This variable needs sufficient input validation
        private int _minChunkSize = 120;
        private int[] _lods;
        private int _maxLODLevel;

        public void SetMinChunkSize(int minChunkSize) { this._minChunkSize = minChunkSize; }
        public int GetMinChunkSize() {return this._minChunkSize;}
        public int[] ComputeLODsFromMinChunkSize()
        {
            var factorList = new List<int>();
            var number = _minChunkSize;
            if (number <= 0)
                throw new ArgumentException("Number must be a positive integer.");

            for (var i = 1; i <= Math.Sqrt(number); i++)
            {
                if (number % i != 0) continue;
                factorList.Add(i);

                if (i != number / i) // Avoid adding square roots twice
                    factorList.Add(number / i);
            }

            factorList.Sort(); // Optional: return factors in ascending order

            var factorArray = factorList.ToArray();
            this._lods = factorArray;
            return factorArray;
        }

        public void SetMaxLODLevel(int maxLODLevel)
        {
            _maxLODLevel = maxLODLevel;
        }

        public int GetMaxLODLevel()
        {
            return _maxLODLevel;
        }

        public int ComputeLOD(int level)
        {
            int totalNumberLODS = _lods.Length;
            int lodIndex = Math.Min(_maxLODLevel, level);
            return _lods[totalNumberLODS-lodIndex-1];
        }
    }
}
