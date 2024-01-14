using UnityEngine;

namespace tezcat.Pseudorandom_Noise
{
    public static class Tool
    {
//         public static float select(float v1, float v2, bool flag)
//         {
//             return flag ? v1 : v2;
//         }

        public static T select<T>(T v1, T v2, bool flag)
        {
            return flag ? v1 : v2;
        }

        public static float smooth(float value)
        {
            return value * value * value * (value * (value * 6f - 15f) + 10f);
        }

        public static Vector3Int floorToInt(this Vector3 vector)
        {
            return new Vector3Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
        }

        public static Vector3Int toInt(this Vector3 vector)
        {
            return new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
        }

        public static Vector3Int add(this Vector3Int vector, int value)
        {
            return new Vector3Int(vector.x + value, vector.y + value, vector.z + value);
        }
    }
}