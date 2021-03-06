using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace HelixCoordinates
{
        public class Options
        {
            [Option('o', "OutputPath", Required = true, HelpText = "Path to file output should be saved in")]
            public string OutputPath { get; set; }

            [Option('l', "LayerHeight", Required = true, HelpText = "Layer height vase will be printed at")]
            public double LayerHeight { get; set; }

            [Option('s', "StartOffset", HelpText = "Amount of object that will be skipped starting at 0")]
            public double StartOffset { get; set; }

            [Option('g', "segments", Default = 500, HelpText = "How many line segments should be used")]
            public double Segemnts { get; set; }

            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('p', "speed", Default = 1900, HelpText = "Speed in mm per min")]
            public double Speed { get; set; }

            [Option('w', "linewidth", Default = .42, HelpText = "Width of the extrsion")]
            public double Width { get; set; }

            [Option('t', "template",
                HelpText = "Path to file that has {{replaceme}} where the generated gcode should be placed")]
            public string TemplatePath { get; set; }
            [Option('f',"FirstLayer",Default=true,HelpText = "Print the first layer flat, varing the layer height")]
            public bool FlatFirstLayer { get; set; }

            [Option('x',"xtransform",HelpText = "How many mm to transform the object on the x axis after slicing")]
            public double XTransform { get; set; }

            [Option('y',"ytransform",HelpText = "How many mm to transform the object on the y axis after slicing")]
            public double YTransform { get; set; }

            [Option('m',"modelpath",HelpText = "Path to model to slice.", Required = true)]
            public string ModelPath { get; set; }

            [Option('r',"relative-extrusion",HelpText = "Should extrusion lengths be relative")]
            public bool IsRelativeExtrusion { get; set; }
        }
}
