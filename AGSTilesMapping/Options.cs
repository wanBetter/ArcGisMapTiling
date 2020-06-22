using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGSTilesMapping
{
        /// </summary>
        public class Options
        {
            #region Required

            /// <summary>
            /// Full path to input file.
            /// </summary>
            [Option('i', "input", Required = true, HelpText = "输入的shp数据格式.")]
            public string InputFilePath { get; set; }

            /// <summary>
            /// Full path to output directory.
            /// </summary>
            [Option('o', "output", Required = true, HelpText = "AGS地图瓦片输出路径.")]
            public string OutputDirectoryPath { get; set; }

        /// <summary>
        /// Minimum cropped zoom.
        /// </summary>
        [Option("minL", Required = true, HelpText = "Minimum cropped zoom.")]
        public string MinL { get; set; }

        /// <summary>
        /// Maximum cropped zoom.
        /// </summary>
        [Option("maxL", Required = true, HelpText = "Maximum cropped zoom.")]
        public string MaxL { get; set; }

        #endregion

            #region Optional

        /// <summary>
        /// Threads count.
        /// </summary>
        [Option("threads", Required = false, HelpText = "设置多线程数量.")]
            public int ThreadsCount { get; set; } = 5;

            #endregion
        }
}
