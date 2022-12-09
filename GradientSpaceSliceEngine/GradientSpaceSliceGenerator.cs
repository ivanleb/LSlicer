using EngineHelpers;
using g3;
using gs;
using gs.info;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GradientSpaceSliceEngine
{
    internal class GradientSpaceSliceGenerator
    {
        internal static ISlicingInfo[] Do(JobSpecification spec)
        {
            double gridResolution = 0.1;

            List<ISlicingInfo> slicingResults = new List<ISlicingInfo>();
            bool _isEnd = false;
            Task.Run(() =>
            {
                TimeSpan duration = TimeSpan.Zero;
                while (!_isEnd)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    duration += TimeSpan.FromSeconds(1);
                    Console.WriteLine("Generating gcode. Duration:  " + duration.ToString(@"mm\:ss"));
                }
            });

            return Slice(spec, gridResolution).ToArray();
        }

        private static List<ISlicingInfo> Slice(JobSpecification spec, double gridResolution)
        {
            List<ISlicingInfo> slicingResults = new List<ISlicingInfo>();

            foreach (var modelInfo in spec.ModelFileInfo)
            {
                double thickness = 0.2;
                Logger.Log.Info($"Slicing:\n-{modelInfo.Name}\n-{thickness}");
                // 1.//3.
                List<DMesh3> meshes = new List<DMesh3>();
                meshes.Add(StandardMeshReader.ReadMesh(modelInfo.FullName));

                DMesh3 mesh = meshes.Last();// cylgen.Generate().MakeDMesh();
                MeshTransforms.ConvertYUpToZUp(mesh);       // g3 meshes are usually Y-up

                // center mesh above origin
                AxisAlignedBox3d bounds = mesh.CachedBounds;
                Vector3d baseCenterPt = bounds.Center - bounds.Extents.z * Vector3d.AxisZ;
                MeshTransforms.Translate(mesh, -baseCenterPt);

                // create print mesh set
                PrintMeshAssembly mesheAssembly = new PrintMeshAssembly();
                mesheAssembly.AddMesh(mesh, PrintMeshOptions.Default());

                // create settings
                //MakerbotSettings settings = new MakerbotSettings(Makerbot.Models.Replicator2);
                //PrintrbotSettings settings = new PrintrbotSettings(Printrbot.Models.Plus);
                //MonopriceSettings settings = new MonopriceSettings(Monoprice.Models.MP_Select_Mini_V2);
                RepRapSettings settings = new RepRapSettings(RepRap.Models.Unknown);

                // do slicing
                MeshPlanarSlicer slicer = new MeshPlanarSlicer()
                {
                    LayerHeightMM = thickness// settings.LayerHeightMM
                };

                slicer.Add(mesheAssembly);
                PlanarSliceStack slices = slicer.Compute();

                // run print generator
                SingleMaterialFFFPrintGenerator printGen =
                    new SingleMaterialFFFPrintGenerator(mesheAssembly, slices, settings);

                var path = Path.Combine(
                    Path.GetDirectoryName(spec.OutputFileInfo.FirstOrDefault().FullName),
                    Path.GetFileNameWithoutExtension(spec.OutputFileInfo.FirstOrDefault().FullName) + ".gcode");

                if (printGen.Generate())
                {

                    // export gcode
                    GCodeFile gcode = printGen.Result;
                    using (StreamWriter w = new StreamWriter(path))
                    {
                        StandardGCodeWriter writer = new StandardGCodeWriter();
                        writer.WriteFile(gcode, w);
                    }
                }


                slicingResults.Add(
                    new SlicingInfo()
                    {
                        FilePath = path,
                        Name = Path.GetFileName(path)
                    }
                );

            }

            return slicingResults;
        }
    }
}