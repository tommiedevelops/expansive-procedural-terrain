using System.Collections;
using _Scripts.ChunkingSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
// ReSharper disable all
namespace EditModeTests
{
    public class LODManagerTests
    {
        [Test]
        public void Can_Instantiate_A_LODManager()
        {
            var lodManagerUnderTest = new LODManager();
            Assert.IsNotNull(lodManagerUnderTest);
        }

        [Test]
        public void Can_Set_Min_Chunk_Size()
        { // should probably also test correct errors for input validation
            var lodManagerUnderTest = new LODManager();
            int minChunkSize = 120;
            lodManagerUnderTest.SetMinChunkSize(minChunkSize);
            int receivedMinChunkSize = lodManagerUnderTest.GetMinChunkSize();
            
            Assert.That(receivedMinChunkSize, Is.EqualTo(minChunkSize));
        }

        [Test]
        public void Has_Default_Chunk_Size_Of_120()
        {
            var lodManagerUnderTest = new LODManager();
            int expectedMinChunkSize = 120;
            int receivedMinChunkSize =  lodManagerUnderTest.GetMinChunkSize();
            Assert.That(receivedMinChunkSize, Is.EqualTo(expectedMinChunkSize));
        }
        
        [Test]
        public void Can_Generate_Correct_LODs_From_Min_Chunk_Size()
        {
            var lodManagerUnderTest = new LODManager();
            const int testMinChunkSize = 12;
            lodManagerUnderTest.SetMinChunkSize(testMinChunkSize);
            var expectedLods = new int[] { 1, 2, 3, 4, 6, 12 };
            var lods = lodManagerUnderTest.ComputeLODsFromMinChunkSize();
            
            Assert.That(lods, Is.EquivalentTo(expectedLods));
        }
        
        [Test]
        public void Can_Generate_Correct_LODs_From_Larger_Chunk_Size()
        {
            var lodManagerUnderTest = new LODManager();
            const int chunkSize = 240;
            var expected = new int[] { 1, 2, 3, 4, 5, 6, 8, 10, 12, 15, 16, 20, 24, 30, 40, 48, 60, 80, 120, 240 };
            lodManagerUnderTest.SetMinChunkSize(chunkSize);
            var  lods = lodManagerUnderTest.ComputeLODsFromMinChunkSize();
            Assert.That(lods, Is.EquivalentTo(expected));
        }

        [Test]
        public void Stores_A_Max_LOD_Level()
        {
            var lodManagerUnderTest = new LODManager();
            int maxLODLevel = 5;
            lodManagerUnderTest.SetMaxLODLevel(maxLODLevel);
            int level = lodManagerUnderTest.GetMaxLODLevel();
            Assert.That(level, Is.EqualTo(maxLODLevel));
        }

        [TestCase(0,5,12)]
        [TestCase(5,5,1)]
        [TestCase(5, 4, 2)]
        [TestCase(7, 10,1)]
        public void Calculates_Correct_LOD(int level, int maxLODLevel, int expected)
        {
            var lodManagerUnderTest = new LODManager();
            lodManagerUnderTest.SetMinChunkSize(12); // {1,2,3,4,6,12}
            lodManagerUnderTest.SetMaxLODLevel(maxLODLevel);
            
            var lods = lodManagerUnderTest.ComputeLODsFromMinChunkSize();
            
            Assert.That(lodManagerUnderTest.ComputeLOD(level), Is.EqualTo(expected));
            
        }
    }
    
}
