using UnityEngine;

namespace tezcat.Pseudorandom_Noise
{
    public class HashVisualization : Visualization
    {
        public enum NoiseType
        {
            Empty = 0,
            Value
        }
        public NoiseType m_NoiseType = NoiseType.Empty;

        [Range(-100, 100)]
        public int m_Seed = 0;


        protected override int seed => m_Seed;
        protected override int noiseType => (int)m_NoiseType;
    }
}