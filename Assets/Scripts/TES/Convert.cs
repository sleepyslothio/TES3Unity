using System.Collections.Generic;
using UnityEngine;

namespace TES3Unity
{
    public static class Convert
    {
        public const int YardInMWUnits = 64;
        public const float MeterInYards = 1.09361f;
        public const float MeterInMWUnits = MeterInYards * YardInMWUnits;

        public const int ExteriorCellSideLengthInMWUnits = 8192;
        public const float ExteriorCellSideLengthInMeters = (float)ExteriorCellSideLengthInMWUnits / MeterInMWUnits;

        public static Quaternion RotationMatrixToQuaternion(Matrix4x4 matrix)
        {
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        }

        public static string CharToString(char[] array)
        {
            var list = new List<char>();

            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] != '\0')
                {
                    list.Add(array[i]);
                }
            }

            return new string(list.ToArray());
        }

        public static string RemoveNullChar(string str)
        {
            var list = new List<char>();

            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] != '\0')
                {
                    list.Add(str[i]);
                }
            }

            return new string(list.ToArray());
        }
    }
}