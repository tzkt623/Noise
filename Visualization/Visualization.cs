using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Pseudorandom_Noise
{
    public abstract class Visualization : MonoBehaviour
    {
        public enum ShapeType
        {
            Plane = 0,
            Sphere,
            OctahedronSphere,
            Torus,
            Octahedron
        }

        [Header("Prefab")]
        public SamplePoint pPoint;

        [Header("Control")]
        public bool m_IsTranslating = false;
        public Vector3 m_Translating;
        public bool m_IsRotating = false;
        public Vector3 m_Rotating;

        [Header("Config")]
        public ShapeType m_ShapeType = ShapeType.Plane;

        public bool m_IsTiling = false;

        [Range(1, 3)]
        public int m_Dimension = 1;

        [Range(1, 512)]
        public int m_Resolution = 64;

        [Range(0.1f, 10f)]
        public float m_InstanceScale = 1f;

        [Range(-0.5f, 0.5f)]
        public float m_Displacement = 1f;

        public SpaceTRS m_DomainConfig = new SpaceTRS()
        {
            scale = new Vector3(4, 4, 4)
        };


        private Matrix4x4 m_PositionTRS;
        private Matrix4x4 m_NormalTRS;
        private Matrix4x4 m_DomainTRS;

        protected bool m_IsDirty = false;
        protected int m_Capacity = 0;
        protected float m_InvResolution = 0;
        private List<SamplePoint> m_Pool = new List<SamplePoint>();

        Shapes.IShape[] m_ShapePainter =
        {
            new Shapes.Plane(),
            new Shapes.Sphere(),
            new Shapes.OctahedronSphere(),
            new Shapes.Torus(),
            new Shapes.Octahedron()
        };

        Noise.INosie[,] m_NormalNosieGenerator =
        {
            {
                Noise.Empty.instance,
                Noise.Empty.instance,
                Noise.Empty.instance,
            },
            {
                new Noise.Lattice1D<Noise.LatticeNormal, Noise.Value>(),
                new Noise.Lattice2D<Noise.LatticeNormal, Noise.Value>(),
                new Noise.Lattice3D<Noise.LatticeNormal, Noise.Value>()
            },
            {
                new Noise.Lattice1D<Noise.LatticeNormal, Noise.Perlin>(),
                new Noise.Lattice2D<Noise.LatticeNormal, Noise.Perlin>(),
                new Noise.Lattice3D<Noise.LatticeNormal, Noise.Perlin>()
            },
            {
                new Noise.Lattice1D<Noise.LatticeNormal, Noise.Turbulence<Noise.Value>>(),
                new Noise.Lattice2D<Noise.LatticeNormal, Noise.Turbulence<Noise.Value>>(),
                new Noise.Lattice3D<Noise.LatticeNormal, Noise.Turbulence<Noise.Value>>()
            },
            {
                new Noise.Lattice1D<Noise.LatticeNormal, Noise.Turbulence<Noise.Perlin>>(),
                new Noise.Lattice2D<Noise.LatticeNormal, Noise.Turbulence<Noise.Perlin>>(),
                new Noise.Lattice3D<Noise.LatticeNormal, Noise.Turbulence<Noise.Perlin>>()
            }
        };

        Noise.INosie[,] m_TilingNosieGenerator =
{
            {
                Noise.Empty.instance,
                Noise.Empty.instance,
                Noise.Empty.instance,
            },
            {
                new Noise.Lattice1D<Noise.LatticeTiling, Noise.Perlin>(),
                new Noise.Lattice2D<Noise.LatticeTiling, Noise.Perlin>(),
                new Noise.Lattice3D<Noise.LatticeTiling, Noise.Perlin>()
            },
            {
                new Noise.Lattice1D<Noise.LatticeTiling, Noise.Value>(),
                new Noise.Lattice2D<Noise.LatticeTiling, Noise.Value>(),
                new Noise.Lattice3D<Noise.LatticeTiling, Noise.Value>()
            },
            {
                new Noise.Lattice1D<Noise.LatticeTiling, Noise.Turbulence<Noise.Value>>(),
                new Noise.Lattice2D<Noise.LatticeTiling, Noise.Turbulence<Noise.Value>>(),
                new Noise.Lattice3D<Noise.LatticeTiling, Noise.Turbulence<Noise.Value>>()
            },
            {
                new Noise.Lattice1D<Noise.LatticeTiling, Noise.Turbulence<Noise.Perlin>>(),
                new Noise.Lattice2D<Noise.LatticeTiling, Noise.Turbulence<Noise.Perlin>>(),
                new Noise.Lattice3D<Noise.LatticeTiling, Noise.Turbulence<Noise.Perlin>>()
            }
        };

        protected abstract int seed { get; }
        protected abstract int noiseType { get; }
        protected Noise.INosie currentNoiseGenerator => m_IsTiling ? m_TilingNosieGenerator[this.noiseType, m_Dimension - 1]  : m_NormalNosieGenerator[this.noiseType, m_Dimension - 1];


        protected void OnEnable()
        {
            this.onMyEnable();
        }

        protected void OnDisable()
        {
            this.onMyDisable();
        }

        protected void OnValidate()
        {
            m_IsDirty = true;
        }

        protected void Update()
        {
            this.checkDirty();
            if (m_IsDirty || this.transform.hasChanged)
            {
                m_IsDirty = false;
                this.transform.hasChanged = false;

                this.onMyUpdate();
            }
        }

        protected virtual void checkDirty()
        {
            if (m_IsTranslating)
            {
                m_DomainConfig.translation += m_Translating * Time.deltaTime;
                m_IsDirty = true;
            }

            if (m_IsRotating)
            {
                m_DomainConfig.rotation += m_Rotating * Time.deltaTime;
                m_IsDirty = true;
            }
        }

        protected virtual void onMyEnable()
        {
            m_InvResolution = 1f / m_Resolution;
            m_Capacity = m_Resolution * m_Resolution;
            m_IsDirty = true;

            m_Pool.Capacity = m_Capacity;
        }

        protected virtual void onMyDisable()
        {

        }

        protected virtual void onMyUpdate()
        {
            var hash = SmallXXHash3.seed(this.seed);

            var scale = m_InstanceScale / m_Resolution;
            var localScale = new Vector3(scale, scale, scale);

            m_DomainTRS = m_DomainConfig.Matrix;

            m_PositionTRS = this.transform.localToWorldMatrix;
            //             m_PositionTRS.m00 = m_InstanceScale;
            //             m_PositionTRS.m11 = m_InstanceScale;
            //             m_PositionTRS.m22 = m_InstanceScale;

            m_NormalTRS = m_PositionTRS.inverse.transpose;

            for (int i = 0; i < m_Capacity; i++)
            {
                ///Shape生成部分
                ///对应教程里的Shape.Job
                var point = m_ShapePainter[(int)m_ShapeType].getPoint(i, m_Resolution, m_InvResolution);
                point.normal = m_NormalTRS.MultiplyVector(point.normal);
                point.normal.Normalize();

                ///噪声生成部分
                ///对应教程里面的Hash.Job
                var domainPosition = m_DomainTRS.MultiplyPoint(point.position);
                var noise = this.generateNoise(domainPosition, hash);

                ///
                var newPoint = m_PositionTRS.MultiplyPoint(point.position);
                var newNormal = point.normal * noise * m_Displacement;

                var ins = this.createPoint(i, null);
                ins.transform.position = newPoint + newNormal;
                ins.transform.localScale = localScale;
                ins.setColor(this.getColor(noise));
            }
        }

        protected SamplePoint createPoint(int index, Transform parent)
        {
            if (index < m_Pool.Count)
            {
                return m_Pool[index];
            }

            var obj = Instantiate(pPoint, parent);
            m_Pool.Add(obj);

            return obj;
        }


        protected virtual float generateNoise(Vector3 position, SmallXXHash3 hash)
        {
            return this.currentNoiseGenerator.getNoise(hash, position, 0);
        }

        protected virtual Color getColor(float noise)
        {
            return noise < 0 ? new Color(-noise, 0, 0) : new Color(noise, noise, noise);
        }
    }
}