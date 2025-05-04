using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

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

        public LODManager(int minChunkSize)
        {
            _minChunkSize = minChunkSize;
            _lods = ComputeLODsFromMinChunkSize(minChunkSize);
            _maxLODLevel = _lods.Length - 1;
        }

        public void SetMinChunkSize(int minChunkSize)
        {
            // Has side effects: recalculates _lods nad resets _maxLODLevel
            
            // Input validation
            if(minChunkSize < 1) throw new ArgumentOutOfRangeException($"minChunkSize cannot be less than 1. Provided: {minChunkSize}");
            if(minChunkSize % 2 != 0) throw new  ArgumentOutOfRangeException($"minChunkSize must be even. Provided: {minChunkSize}");
            
            _minChunkSize = minChunkSize;
            _lods = ComputeLODsFromMinChunkSize(minChunkSize);
            _maxLODLevel = _lods.Length - 1;

        }
        public int GetMinChunkSize() {return _minChunkSize;}
        public static int[] ComputeLODsFromMinChunkSize(int minChunkSize)
        {
            var factorList = new List<int>();
            if (minChunkSize <= 0)
                throw new ArgumentException("Number must be a positive integer.");

            for (var i = 1; i <= Math.Sqrt(minChunkSize); i++)
            {
                if (minChunkSize % i != 0) continue;
                factorList.Add(i);

                if (i != minChunkSize / i) // Avoid adding square roots twice
                    factorList.Add(minChunkSize / i);
            }

            factorList.Sort(); // Optional: return factors in ascending order

            var factorArray = factorList.ToArray();
            return factorArray;
        }

        public void SetMaxLODLevel(int maxLODLevel)
        {
            if(maxLODLevel < 0) throw new ArgumentException("maxLODLevel must be a non-negative integer.");
            if(maxLODLevel > _lods.Length-1) throw new ArgumentException($"not enough LODs to set max LOD level of {maxLODLevel}");
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

        public int[] GetLODs() {return _lods;}
    }
}
