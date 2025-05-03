using System.Collections;
using _Scripts.ChunkingSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
        public void Can_Generate_LODs_From_Min_Chunk_Size()
        {
            int testMinChunkSize = 12;
            var expectedLods = new int[] { 1, 2, 3, 4, 6, 12 };
            var lods = LODManager.ComputeLODs(testMinChunkSize);
            
            Assert.That(lods, Is.EquivalentTo(expectedLods));
        }
        
    }
    
}
