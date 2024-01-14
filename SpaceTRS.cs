using UnityEngine;

namespace tezcat.Pseudorandom_Noise
{
    [System.Serializable]
    public struct SpaceTRS
    {
        [SerializeField]
        public Vector3 translation;
        [SerializeField]
        public Vector3 rotation;
        [SerializeField]
        public Vector3 scale;

        public Matrix4x4 Matrix
        {
            get
            {
                return Matrix4x4.TRS(translation, Quaternion.Euler(rotation), scale);
            }
        }
    }
}

