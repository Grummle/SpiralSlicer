using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using g3;

namespace HelixCoordinates
{
    internal class Program
    {
            public static double FilamentArea = 2.40528;
        private static void Main(string[] args)
        {


            var options = Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions);
        }

        private static void RunOptions(Options opts)
        {
            var builder = new DMesh3Builder();
            var reader = new StandardMeshReader {MeshBuilder = builder};
            var result = reader.Read(@"C:\Users\Dillon\Downloads\troll.small.centered.stl", ReadOptions.Defaults);
            List<DMesh3> meshes = null;
            if (result.code == IOCode.Ok) meshes = builder.Meshes;
            var spatial = new DMeshAABBTree3(meshes.First());
            spatial.Build();

            var points = new List<V3D>();

            var r = 500;
            var c = 0.031;

            IntrRay3Triangle3 strucktriangle = null;
            for (double t = 0; t < 200000; t = t + .1)
            {
                var castRay = new Ray3d(new Vector3d(0, 0, c * t),
                    new Vector3d(r * Math.Cos(t), r * Math.Sin(t), c * t));

                var hitTriangles = new List<int>();

                spatial.FindAllHitTriangles(castRay, hitTriangles);
                if (hitTriangles.Count == 0) break;

                for (var i = 0; i < hitTriangles.Count; i++)
                {
                    strucktriangle = MeshQueries.TriangleIntersection(meshes.First(), hitTriangles[i], castRay);
                    if (strucktriangle.Result == IntersectionResult.Intersects) break;
                }


                var rv = new V3D(r * Math.Cos(t), r * Math.Sin(t), c * t);
                var rp = new V3D(0, 0, c * t);
                var pn = new V3D(strucktriangle.Triangle.Normal.x, strucktriangle.Triangle.Normal.y, strucktriangle.Triangle.Normal.z);
                var pp = new V3D(strucktriangle.Triangle.V0.x, strucktriangle.Triangle.V0.y, strucktriangle.Triangle.V0.z);

                var intersectionPoint = V3D.IntersectPoint(rv, rp, pn, pp);
                intersectionPoint.z = c * t;

                points.Add(intersectionPoint);
            }



            var sb2 = new StringBuilder();
            double extrusionTotal = 0;
            sb2.Append($"G1 F{opts.Speed}\n");
            for (var i = 1; i < points.Count; i++)
            {
                var extrusionArea = (opts.Width - opts.LayerHeight)*opts.LayerHeight + Math.PI * Math.Pow(opts.LayerHeight/2,2);
                if (opts.FlatFirstLayer)
                { 
                    var t = (points[i].z / c);
                    if (t < 2 * Math.PI )
                    {
                        var additional = (t /( 2 * Math.PI)) * opts.LayerHeight;
                        var moarbigger = opts.LayerHeight + additional;
                        extrusionArea = (opts.Width - moarbigger) * moarbigger + Math.PI * Math.Pow(moarbigger / 2, 2);
                    }
                }
                var p = points[i];
                var pp = points[i - 1];
                var distance = Math.Sqrt(Math.Pow(pp.x - p.x, 2) + Math.Pow(pp.y - p.y, 2) + Math.Pow(pp.z - p.z, 2));
                var extrusion = (distance * extrusionArea) / FilamentArea;
                extrusionTotal = extrusionTotal + extrusion;
                var o = $"G1 X{p.x + 80} Y{p.y + 70} Z{p.z+opts.LayerHeight} E{extrusion} ;{distance}\n ";
                sb2.Append(o);
            }


            var gcode = sb2.ToString();
            var filecontents = "";

            if (!string.IsNullOrEmpty(opts.TemplatePath))
            {
                filecontents = File.ReadAllText(opts.TemplatePath);
                filecontents = filecontents.Replace("{{replaceme}}", gcode);
            }
            else
            {
                filecontents = gcode;
            }
            File.WriteAllText(opts.OutputPath, filecontents);

        }
    }
}