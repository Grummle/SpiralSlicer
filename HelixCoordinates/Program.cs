using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using g3;
using QuantumConcepts.Formats.StereoLithography;

namespace HelixCoordinates
{
    class Program
    {
        static void Main(string[] args)
        {
            double zoff = .2;
            DMesh3Builder builder = new DMesh3Builder();
            StandardMeshReader reader = new StandardMeshReader() { MeshBuilder = builder };
            IOReadResult result = reader.Read(@"C:\Users\Dillon\Downloads\Troll v1.stl", ReadOptions.Defaults);
            List <DMesh3> meshes = null;
            if (result.code == IOCode.Ok)
            {
                meshes = builder.Meshes;
            }
            DMeshAABBTree3 spatial = new DMeshAABBTree3(meshes.First());
            spatial.Build();

            var points = new List<V3D>();
            var helixPoints = new List<V3D>();
            var buds = new List<Tuple<V3D, V3D>>();
            var r = 500;
            var c = 0.031;
            var step = 0;
            int hittirangle = -1;
            IntrRay3Triangle3 strucktriangle = null;
            for (double t = 0; t < 200000; t = t + .001)
            {
                if (step % 10 == 0)
                {
                    Console.WriteLine(t*c);
                }

                step++;

                var castRay = new Ray3d(new Vector3d(0,0,c*t),new Vector3d(r * Math.Cos(t),r * Math.Sin(t),c*t) );

                List<int> innerhittriangles = new List<int>();

                var returned2 = spatial.FindAllHitTriangles(castRay, innerhittriangles);
                if (innerhittriangles.Count == 0)
                {
                    break;}
                for (var i = 0; i < innerhittriangles.Count; i++)
                {

                    strucktriangle = MeshQueries.TriangleIntersection(meshes.First(), innerhittriangles[i], castRay);
                    if (strucktriangle.Result == IntersectionResult.Intersects)
                    {
                        hittirangle = innerhittriangles[i];
                        break;
                    }
                        else
                        {
                            hittirangle = -1;
                        }
                    }



                if (hittirangle == -1)
                {
                    break;}


                var triangle = strucktriangle; //MeshQueries.TriangleIntersection(meshes.First(),hittirangle, castRay);

                var rv = new V3D(r * Math.Cos(t),r * Math.Sin(t),c*t);
                var rp = new V3D(0,0,c*t);
                var pn = new V3D(triangle.Triangle.Normal.x, triangle.Triangle.Normal.y, triangle.Triangle.Normal.z);
                var pp = new V3D(triangle.Triangle.V0.x,triangle.Triangle.V0.y,triangle.Triangle.V0.z);
                helixPoints.Add(new V3D(r * Math.Cos(t),r * Math.Sin(t),c*t));

                var intersectionPoint = IntersectPoint(rv, rp, pn, pp);
                buds.Add(new Tuple<V3D, V3D>(new V3D(r * Math.Cos(t),r * Math.Sin(t),c*t),intersectionPoint));
                intersectionPoint.z = c * t;

                points.Add(intersectionPoint);



            }


            var sb = new StringBuilder();
            var output = "";
            for(var i = 1; i<points.Count;i++)
            {
                var o =
                    $"Line,Cartesian,{points[i - 1].x},{points[i - 1].y},{points[i - 1].z},{points[i].x},{points[i].y},{points[i].z},Print,0.4,0.2"+Environment.NewLine;
                sb.Append(o);

            }

            var sb2 = new StringBuilder();
            double laste = 0;
            for (var i = 1; i < points.Count; i++)
            {
                var p = points[i];
                var pp = points[i - 1];
                var distance = Math.Sqrt(Math.Pow(pp.x - p.x, 2) + Math.Pow(pp.y - p.y, 2) + Math.Pow(pp.z - p.z, 2));
                var extrusion = distance * .2;
                laste = laste + extrusion;
                var o = $"G1 X{p.x+80} Y{p.y+70} Z{p.z+zoff} E{laste} ;{distance}\n ";
                sb2.Append(o);

            }


            var hexsb= new StringBuilder();
//            hexsb.Append("M83" + Environment.NewLine);
            laste = 0;
            for (var i = 1; i < helixPoints.Count; i++)
            {
                var p = helixPoints[i];
                var pp = helixPoints[i - 1];
                var distance = Math.Sqrt(Math.Pow(pp.x - p.x, 2) + Math.Pow(pp.y - p.y, 2) + Math.Pow(pp.z - p.z, 2));
                    Console.WriteLine( distance);
                var extrusion =  distance * .02;
                laste = laste + extrusion;
                var o = $"G1 X{p.x+80} Y{p.y+70} Z{p.z} E0.1\n";
                hexsb.Append(o);

            }


            var hxstring = hexsb.ToString();

            File.WriteAllText($@"c:\temp\hex{DateTime.Now.Ticks}.gcode",hxstring);



            var gcode = sb2.ToString();

            var filecontents = File.ReadAllText(@"c:\temp\template.troll.2mm.gcode");
            filecontents = filecontents.Replace("{{tempaltesub}}", gcode);
            File.WriteAllText($@"c:\temp\templated{DateTime.Now.Ticks}.gcode",filecontents);

            //            File.WriteAllText(@"c:\temp\gen.csv",sb.ToString());
            File.WriteAllText($@"c:\temp\derpitydo{DateTime.Now.Ticks}.gcode",gcode);



        }

        class V3D
        {
            public double x, y, z;

            public V3D(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
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
        static V3D IntersectPoint(V3D rayVector, V3D rayPoint, V3D planeNormal, V3D planePoint)
        {
            var diff = rayPoint - planePoint;
            var prod1 = diff.Dot(planeNormal);
            var prod2 = rayVector.Dot(planeNormal);
            var prod3 = prod1 / prod2;
            return rayPoint - rayVector * prod3;
        }

        static V3D IntersectDisk(V3D rayVector, V3D rayPoint, V3D planeNormal, V3D planePoint)
        {
            var diff = rayPoint - planePoint;
            var prod1 = diff.Dot(planeNormal);
            var prod2 = rayVector.Dot(planeNormal);
            var prod3 = prod1 / prod2;
            return rayPoint - rayVector * prod3;
        }
    }


}
