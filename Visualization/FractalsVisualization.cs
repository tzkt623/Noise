using UnityEngine;

namespace tezcat.Pseudorandom_Noise
{
    public class FractalsVisualization : Visualization
    {
        public enum NoiseType
        {
            Empty = 0,
            Value,
            Perlin,
            ValueTurbulence,
            PerlinTurbulence
        }
        public NoiseType m_NoiseType = NoiseType.Empty;

        public Settings m_Settings = Settings.Default;

        protected override int seed => m_Settings.seed;
        protected override int noiseType => (int)m_NoiseType;

        protected override float generateNoise(Vector3 position, SmallXXHash3 hash)
        {
            float sum = 0f;
            int frequency = m_Settings.frequency;
            float amplitude = 1f;
            float amplitudeSum = 0f;

            for (int o = 0; o < m_Settings.octaves; o++)
            {
                sum += amplitude * this.currentNoiseGenerator.getNoise(hash + o, position, frequency);

                frequency *= m_Settings.lacunarity;
                amplitude *= m_Settings.persistence;
                amplitudeSum += amplitude;
            }

            return sum / amplitudeSum;
        }
    }
}