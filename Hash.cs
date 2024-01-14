using System.Collections;
using System.Collections.Generic;

namespace tezcat.Pseudorandom_Noise
{
    public readonly struct SmallXXHash
    {
        const uint primeA = 0b10011110001101110111100110110001;
        const uint primeB = 0b10000101111010111100101001110111;
        const uint primeC = 0b11000010101100101010111000111101;
        const uint primeD = 0b00100111110101001110101100101111;
        const uint primeE = 0b00010110010101100110011110110001;

        readonly uint m_Accumulator;

        public uint toUint() => m_Accumulator;

        public SmallXXHash(uint accumulator)
        {
            m_Accumulator = accumulator;
        }

        public SmallXXHash eat(int data)
        {
            return rotateLeft(m_Accumulator + (uint)data * primeC, 17) * primeD;
        }

        public SmallXXHash eat(byte data)
        {
            return rotateLeft(m_Accumulator + data * primeE, 11) * primeA;
        }

        private static uint rotateLeft(uint data, int steps) => (data << steps) | (data >> 32 - steps);

        public static SmallXXHash seed(int seed) => (uint)seed + primeE;

        public static implicit operator uint(SmallXXHash hash)
        {
            uint avalanche = hash.m_Accumulator;
            avalanche ^= avalanche >> 15;
            avalanche *= primeB;
            avalanche ^= avalanche >> 13;
            avalanche *= primeC;
            avalanche ^= avalanche >> 16;
            return avalanche;
        }

        public static implicit operator SmallXXHash(uint accumulator) => new SmallXXHash(accumulator);
    }

    public readonly struct SmallXXHash3
    {
        const uint primeA = 0b10011110001101110111100110110001;
        const uint primeB = 0b10000101111010111100101001110111;
        const uint primeC = 0b11000010101100101010111000111101;
        const uint primeD = 0b00100111110101001110101100101111;
        const uint primeE = 0b00010110010101100110011110110001;
        const float factor = 1f / 255f;

        readonly uint m_Accumulator;

        public uint toUint() => m_Accumulator;

        public uint bytesA => this & 255;
        public uint bytesB => (this >> 8) & 255;
        public uint bytesC => (this >> 16) & 255;
        public uint bytesD => (this >> 24) & 255;


        public float floats01A => bytesA * factor;
        public float floats01B => bytesB * factor;
        public float floats01C => bytesC * factor;
        public float floats01D => bytesD * factor;

        public SmallXXHash3(uint accumulator)
        {
            m_Accumulator = accumulator;
        }

        public SmallXXHash3 eat(int data)
        {
            return rotateLeft(m_Accumulator + (uint)data * primeC, 17) * primeD;
        }

        public SmallXXHash3 eat(byte data)
        {
            return rotateLeft(m_Accumulator + data * primeE, 11) * primeA;
        }

        private static uint rotateLeft(uint data, int steps) => (data << steps) | (data >> 32 - steps);

        public static SmallXXHash3 seed(int seed) => (uint)seed + primeE;

        public static implicit operator uint(SmallXXHash3 hash)
        {
            uint avalanche = hash.m_Accumulator;
            avalanche ^= avalanche >> 15;
            avalanche *= primeB;
            avalanche ^= avalanche >> 13;
            avalanche *= primeC;
            avalanche ^= avalanche >> 16;
            return avalanche;
        }

        public static implicit operator SmallXXHash3(uint accumulator) => new SmallXXHash3(accumulator);

        public static SmallXXHash3 operator +(SmallXXHash3 hash, int v) => hash.m_Accumulator + (uint)v;
    }
}

