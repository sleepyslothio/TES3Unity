using UnityEngine;

namespace TESUnity
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
    }
}