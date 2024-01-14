using System.Collections;
using UnityEngine;

namespace tezcat.Pseudorandom_Noise
{
    public class SamplePoint : MonoBehaviour
    {
        Material m_Mat;
        MeshRenderer m_MR;

        private void Awake()
        {
            m_MR = this.GetComponent<MeshRenderer>();
            m_Mat = m_MR.material;
        }

        public void setColor(Color color)
        {
            m_Mat.color = color;
        }
    }
}