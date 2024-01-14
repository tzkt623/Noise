using UnityEngine;

namespace tezcat.Pseudorandom_Noise
{
    public static class Noise
    {
        #region interface
        public interface INosie
        {
            float getNoise(SmallXXHash3 hash, Vector3 position, int frequency);
        }

        public interface IGradient
        {
            float evaluate(SmallXXHash3 hash, float x);
            float evaluate(SmallXXHash3 hash, float x, float y);
            float evaluate(SmallXXHash3 hash, float x, float y, float z);

            float evaluateAfterInterpolation(float value);
        }

        public interface ILattice
        {
            LatticeSpan getLatticeSpan(float coordinate, int frequency);
        }
        #endregion

        #region Lattice
        public struct LatticeSpan
        {
            public int p0, p1;
            public float t;
            public float g0, g1;
        }

        public struct LatticeNormal : ILattice
        {
            public LatticeSpan getLatticeSpan(float coordinate, int frequency)
            {
                coordinate *= frequency;

                var point = Mathf.Floor(coordinate);
                LatticeSpan span;
                span.p0 = (int)point;
                span.p1 = span.p0 + 1;

                span.g0 = coordinate - span.p0;
                span.g1 = span.g0 - 1f;

                span.t = Tool.smooth(coordinate - point);
                return span;
            }
        }

        public struct LatticeTiling : ILattice
        {
            public LatticeSpan getLatticeSpan(float coordinate, int frequency)
            {
                coordinate *= frequency;

                float point = Mathf.Floor(coordinate);
                LatticeSpan span;
                span.p0 = (int)point;

                span.g0 = coordinate - span.p0;
                span.g1 = span.g0 - 1f;

                span.p0 %= frequency;
                span.p0 = Tool.select(span.p0 + frequency, span.p0, span.p0 < 0);
                span.p1 = span.p0 + 1;
                span.p1 = Tool.select(0, span.p1, span.p1 == frequency);

                span.t = Tool.smooth(coordinate - point);
                return span;
            }
        }

        public struct Empty : INosie
        {
            public readonly static Empty instance = new Empty();

            public float getNoise(SmallXXHash3 hash, Vector3 position, int frequency) => 0;
        }

        public struct Lattice1D<L, G>
            : INosie
            where L : struct, ILattice
            where G : struct, IGradient
        {
            public float getNoise(SmallXXHash3 hash, Vector3 position, int frequency)
            {
                var x = default(L).getLatticeSpan(position.x, frequency);

                var h0 = hash.eat(x.p0);
                var h1 = hash.eat(x.p1);

                var g = default(G);
                var lerp = Mathf.Lerp(g.evaluate(h0, x.g0), g.evaluate(h1, x.g1), x.t);

                return g.evaluateAfterInterpolation(lerp);
            }
        }

        public struct Lattice2D<L, G>
            : INosie
            where L : struct, ILattice
            where G : struct, IGradient
        {
            public float getNoise(SmallXXHash3 hash, Vector3 position, int frequency)
            {
                var l = default(L);
                var x = l.getLatticeSpan(position.x, frequency);
                var z = l.getLatticeSpan(position.z, frequency);

                var h0 = hash.eat(x.p0);
                var h1 = hash.eat(x.p1);

                var h00 = h0.eat(z.p0);
                var h01 = h0.eat(z.p1);

                var h10 = h1.eat(z.p0);
                var h11 = h1.eat(z.p1);

                var g = default(G);

                var lerp = Mathf.Lerp(
                    Mathf.Lerp(g.evaluate(h00, x.g0, z.g0), g.evaluate(h01, x.g0, z.g1), z.t),
                    Mathf.Lerp(g.evaluate(h10, x.g1, z.g0), g.evaluate(h11, x.g1, z.g1), z.t),
                    x.t);

                return g.evaluateAfterInterpolation(lerp);
            }
        }

        public struct Lattice3D<L, G>
            : INosie
            where L : struct, ILattice
            where G : struct, IGradient
        {
            public float getNoise(SmallXXHash3 hash, Vector3 position, int frequency)
            {
                var l = default(L);
                var x = l.getLatticeSpan(position.x, frequency);
                var y = l.getLatticeSpan(position.y, frequency);
                var z = l.getLatticeSpan(position.z, frequency);

                var h0 = hash.eat(x.p0);
                var h1 = hash.eat(x.p1);

                var h00 = h0.eat(y.p0);
                var h01 = h0.eat(y.p1);

                var h10 = h1.eat(y.p0);
                var h11 = h1.eat(y.p1);

                var h000 = h00.eat(z.p0);
                var h001 = h00.eat(z.p1);

                var h010 = h01.eat(z.p0);
                var h011 = h01.eat(z.p1);

                var h100 = h10.eat(z.p0);
                var h101 = h10.eat(z.p1);

                var h110 = h11.eat(z.p0);
                var h111 = h11.eat(z.p1);

                var g = default(G);

                var lerp = Mathf.Lerp(
                    Mathf.Lerp(
                        Mathf.Lerp(g.evaluate(h000, x.g0, y.g0, z.g0), g.evaluate(h001, x.g0, y.g0, z.g1), z.t),
                        Mathf.Lerp(g.evaluate(h010, x.g0, y.g1, z.g0), g.evaluate(h011, x.g0, y.g1, z.g1), z.t),
                        y.t),
                    Mathf.Lerp(
                        Mathf.Lerp(g.evaluate(h100, x.g1, y.g0, z.g0), g.evaluate(h101, x.g1, y.g0, z.g1), z.t),
                        Mathf.Lerp(g.evaluate(h110, x.g1, y.g1, z.g0), g.evaluate(h111, x.g1, y.g1, z.g1), z.t),
                        y.t),
                    x.t);

                return g.evaluateAfterInterpolation(lerp);
            }
        }

        #endregion

        #region Noise
        public readonly struct Value : IGradient
        {
            public float evaluate(SmallXXHash3 hash, float x) => hash.floats01A * 2f - 1f;

            public float evaluate(SmallXXHash3 hash, float x, float y) => hash.floats01A * 2f - 1f;

            public float evaluate(SmallXXHash3 hash, float x, float y, float z) => hash.floats01A * 2f - 1f;

            public float evaluateAfterInterpolation(float value) => value;
        }

        public readonly struct Perlin : IGradient
        {
            const float m_2DFactor = 2f / 0.53528f;
            const float m_3DFactor = 1f / 0.56290f;

            public float evaluate(SmallXXHash3 hash, float x)
            {
                return (1f + hash.floats01A) * Tool.select(-x, x, (hash & 1 << 8) == 0);
            }

            public float evaluate(SmallXXHash3 hash, float x, float y)
            {
                float gx = hash.floats01A * 2f - 1f;
                float gy = 0.5f - Mathf.Abs(gx);
                gx -= Mathf.Floor(gx + 0.5f);
                return (gx * x + gy * y) * m_2DFactor;
            }

            public float evaluate(SmallXXHash3 hash, float x, float y, float z)
            {
                float gx = hash.floats01A * 2f - 1f;
                float gy = hash.floats01D * 2f - 1f;
                float gz = 1f - Mathf.Abs(gx) - Mathf.Abs(gy);
                float offset = Mathf.Max(-gz, 0f);
                gx += Tool.select(-offset, offset, gx < 0f);
                gy += Tool.select(-offset, offset, gy < 0f);
                return (gx * x + gy * y + gz * z) * m_3DFactor;
            }

            public float evaluateAfterInterpolation(float value) => value;
        }

        public struct Turbulence<G> : IGradient where G : IGradient
        {
            public float evaluate(SmallXXHash3 hash, float x) => default(G).evaluate(hash, x);

            public float evaluate(SmallXXHash3 hash, float x, float y) => default(G).evaluate(hash, x, y);

            public float evaluate(SmallXXHash3 hash, float x, float y, float z) => default(G).evaluate(hash, x, y, z);

            public float evaluateAfterInterpolation(float value) => Mathf.Abs(default(G).evaluateAfterInterpolation(value));
        }
        #endregion
    }
}

