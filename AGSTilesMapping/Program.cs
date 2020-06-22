using AGSTilesMapping.Core;
using CommandLine;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AGSTilesMapping
{
    class Program
    {
        #region 成员变量
        /// <summary>
        /// 参数解析错误
        /// </summary>
        private static bool IsParsingErrors { get; set; }
        /// <summary>
        /// 输入的文件信息
        /// </summary>
        private static FileInfo InputFileInfo { get; set; }
        /// <summary>
        /// 输出的目录信息
        /// </summary>
        private static DirectoryInfo OutputDirectoryInfo { get; set; }
        /// <summary>
        /// 切图最小级别
        /// </summary>
        private static int MinL { get; set; }
        /// <summary>
        /// 切图最大级别
        /// </summary>
        private static int MaxL { get; set; }
        /// <summary>
        /// 处理的线程数量
        /// </summary>
        private static int ThreadsCount { get; set; }
        #endregion

        static async Task Main(string[] args)
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
            IAoInitialize m_AoInitialize = new AoInitialize();
            // m_AoInitialize.InitializedProduct();
            esriLicenseStatus licenseStatus = esriLicenseStatus.esriLicenseUnavailable;
            licenseStatus = m_AoInitialize.IsProductCodeAvailable(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);
            m_AoInitialize.Initialize(esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB);

            try
            {
                Parser.Default.ParseArguments<Options>(args).WithParsed(ParseConsoleOptions)
                      .WithNotParsed(error => IsParsingErrors = true);
            }
            catch (Exception exception)
            {
                Helpers.ErrorHelper.PrintException(exception);
                return;
            }

            if (IsParsingErrors)
            {
                Helpers.ErrorHelper.PrintError(Strings.ParsingError);
                return;
            }

            ConsoleProgress<double> consoleProgress = new ConsoleProgress<double>(System.Console.WriteLine);

            try
            {
                //Check for errors.
                Helpers.CheckHelper.CheckDirectory(OutputDirectoryInfo, true);

                TileFileInfo tileFileInfo= await Helpers.CheckHelper.CheckInputFile(InputFileInfo).ConfigureAwait(false);

                //Switch on CoordinateSystem.
                switch (tileFileInfo.SpatialReferenceType)
                {
                    case Enums.SpatialReferenceType.GeoCoordinateSystem:
                        await AGSTile.GenerateGeoGoordinateSystemTiles(tileFileInfo, OutputDirectoryInfo.FullName, MinL, MaxL, consoleProgress, ThreadsCount);
                        break;
                    case Enums.SpatialReferenceType.ProjCoordinateSystem:
                        await AGSTile.GenerateProjGoordinateSystemTiles(tileFileInfo, OutputDirectoryInfo.FullName, MinL,MaxL,consoleProgress, ThreadsCount);
                        break;
                    case Enums.SpatialReferenceType.None:
                        Helpers.ErrorHelper.PrintError(Strings.CoorNotSupported);
                        break;

                    default:
                        Helpers.ErrorHelper.PrintError(Strings.CoorNotSupported);

                        return;
                }
            }
            catch (Exception exception)
            {
                Helpers.ErrorHelper.PrintException(exception);
                return;
            }
            Environment.Exit(0);
            Console.WriteLine("处理完成");
        }

        private static void ParseConsoleOptions(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.InputFilePath))
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.OptionIsEmpty, "-i/--input"));
                IsParsingErrors = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(options.OutputDirectoryPath))
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.OptionIsEmpty, "-o/--output"));
                IsParsingErrors = true;

                return;
            }
            if (string.IsNullOrWhiteSpace(options.MinL))
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.OptionIsEmpty, "--minL"));
                IsParsingErrors = true;

                return;
            }
            if (string.IsNullOrWhiteSpace(options.MaxL))
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.OptionIsEmpty, "--maxL"));
                IsParsingErrors = true;

                return;
            }
            InputFileInfo = new FileInfo(options.InputFilePath);
            OutputDirectoryInfo = new DirectoryInfo(options.OutputDirectoryPath);
            MinL = int.Parse( options.MinL);
            MaxL = int.Parse(options.MaxL);
            if (MinL < 0|| MinL>19)
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.MinLError, "--maxL"));
                IsParsingErrors = true;

                return;
            }
            if (MaxL < 1 || MaxL > 19)
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.ValueError, "--maxL"));
                IsParsingErrors = true;

                return;
            }
            if (MinL>=MaxL)
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.ValueError));
                IsParsingErrors = true;

                return;
            }
            ThreadsCount = options.ThreadsCount;
        }
    }
        
}
