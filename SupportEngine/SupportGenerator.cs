using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using gs;
using g3;
using LSlicer.Data.Model;
using System.Diagnostics;
using EngineHelpers;

namespace SupportEngine
{
    /* 
     тут необходмо подключить логику из 
     https://github.com/gradientspace/gsSlicerPro/tree/20dac4fc8cee0e3fde1c7791eac71b3f5d103eb6/supports

    Нужно 
    1. загружать stl файл из спецификации.
    2. загружать необходимые параметры из спецификации - угол к которому еще нужно делать поддержки, тип поддержек и т.д. 
    (сначала можно захардкодить)
    3. преобразовать файл в те структуры данных, которые есть в либах gradientspace
    4. заюзать один из алгоритмов от gradientspace
    5. результат работы алгоритма преобразовать и записать в stl файл.
    6. записать пути к получившимся файлам в ISupportInfo[]
    */
    public static class SupportGenerator
    {
        public static ISupportInfo[] Do(JobSpecification spec) 
        {
            //Debugger.Launch();
            double gridResolution = 1;  
            List<DMesh3> meshes = new List<DMesh3>();
            List<DMesh3> supports = new List<DMesh3>();
            List<ISupportInfo> supportResults = new List<ISupportInfo>();
            bool _isEnd = false;
            Task.Run(() => 
            {
                TimeSpan duration = TimeSpan.Zero;
                while (!_isEnd)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    duration += TimeSpan.FromSeconds(1);
                    Console.WriteLine("Generating support. Duration:  " + duration.ToString(@"mm\:ss"));
                }
            });

            foreach (var modelInfo in spec.ModelFileInfo)
            {
                // 1.//3.
                meshes.Add(StandardMeshReader.ReadMesh(modelInfo.FullName));

                //4.
                var generator = new BlockSupportGenerator(meshes.Last(), gridResolution);
                //2. - будет тут
                generator.OverhangAngleDeg = 45;
                generator.Generate();
                supports.Add(generator.SupportMesh);

                supportResults.Add(
                    new SupportInfo() { 
                        MeshFilePath = modelInfo.FullName, 
                        SupportFilePath = spec.OutputFileInfo.FirstOrDefault().FullName }
                    );
            }

            // код тут

            //прогресс можно просто писать в консоль в таком формате
            //int progressTick = 100;
            //for (int i = 1; i <= 100; i++)
            //{
            //    // код тут

            //    Thread.Sleep(progressTick);
            //    Console.WriteLine($"Progress: {i} %");
            //}

            // 5.
            StandardMeshWriter.WriteMeshes(spec.OutputFileInfo.FirstOrDefault().FullName, supports, WriteOptions.Defaults);

            _isEnd = true;
            return supportResults.ToArray();
        }
    }
}
