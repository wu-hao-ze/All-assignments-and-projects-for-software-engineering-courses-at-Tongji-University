using UnityEngine;

namespace Unity.VisualScripting
{
    public static class MathfEx
    {
        public static float Wrap(float x, float m)
        {
            return (x % m + m) % m;
        }

        public static float NearestMultiple(float x, float f)
        {
            return Mathf.RoundToInt(x / f) * f;
        }

        public static float HigherMultiple(float x, float f)
        {
            return Mathf.CeilToInt(x / f) * f;
        }

        public static Vector2 Bezier(Vector2 s, Vector2 e, Vector2 st, Vector2 et, float t)
        {
            return (((-s + 3 * (st - et) + e) * t + (3 * (s + et) - 6 * st)) * t + 3 * (st - s)) * t + s;
        }

        public static Vector3 Bezier(Vector3 s, Vector3 e, Vector3 st, Vector3 et, float t)
        {
            return (((-s + 3 * (st - et) + e) * t + (3 * (s + et) - 6 * st)) * t + 3 * (st - s)) * t + s;
        }

        public static Matrix4x4 ScaleAroundPivot(Vector3 pivot, Vector3 scale)
        {
            return Matrix4x4.TRS(pivot, Quaternion.identity, scale) * Matrix4x4.TRS(-pivot, Quaternion.identity, Vector3.one);
        }

        public static Vector3 GetT(this Matrix4x4 trs)
        {
            return trs.GetColumn(3);
        }

        public static Quaternion GetR(this Matrix4x4 trs)
        {
            return Quaternion.LookRotation(trs.GetColumn(2), trs.GetColumn(1));
        }

        public static Vector3 GetS(this Matrix4x4 trs)
        {
            return new Vector3(trs.GetColumn(0).magnitude, trs.GetColumn(1).magnitude, trs.GetColumn(2).magnitude);
        }
    }
}
