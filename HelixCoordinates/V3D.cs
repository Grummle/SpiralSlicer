namespace HelixCoordinates
{
    public class V3D
    {
        public readonly double x;
        public readonly double y;
        public double z;

        public V3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static V3D IntersectPoint(V3D rayVector, V3D rayPoint, V3D planeNormal, V3D planePoint)
        {
            var diff = rayPoint - planePoint;
            var prod1 = diff.Dot(planeNormal);
            var prod2 = rayVector.Dot(planeNormal);
            var prod3 = prod1 / prod2;
            return rayPoint - rayVector * prod3;
        }

        public static V3D operator +(V3D lhs, V3D rhs)
        {
            return new V3D(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }

        public static V3D operator -(V3D lhs, V3D rhs)
        {
            return new V3D(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static V3D operator *(V3D lhs, double rhs)
        {
            return new V3D(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        }

        public double Dot(V3D rhs)
        {
            return x * rhs.x + y * rhs.y + z * rhs.z;
        }

        public override string ToString()
        {
            return string.Format("({0:F}, {1:F}, {2:F})", x, y, z);
        }
    }
}