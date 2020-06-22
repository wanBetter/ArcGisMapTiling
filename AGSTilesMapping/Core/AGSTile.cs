using AGSTilesMapping.Helpers;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AGSTilesMapping.Core
{
    /// <summary>
    /// 万述明-ArcgisEngine切图工具
    /// 2020-06-22
    /// </summary>
    public class AGSTile
    {
        /// <summary>
        /// 写入投影坐标系切片
        /// </summary>
        /// <param name="tileFileInfo"></param>
        /// <param name="outPutDir"></param>
        /// <param name="minL"></param>
        /// <param name="maxL"></param>
        /// <param name="progress"></param>
        /// <param name="threadsCount"></param>
        /// <returns></returns>
        public static async Task GenerateProjGoordinateSystemTiles(TileFileInfo tileFileInfo,string outPutDir, int minL, int maxL, IProgress<double> progress, int threadsCount)
        {

            using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(threadsCount))
            {
                List<Task> tasks = new List<Task>();
                for (int i = minL; i < maxL; i++)
                {
                    await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                    try
                    {
                        WriteProjLevel(i, tileFileInfo, outPutDir);
                    }
                    finally { semaphoreSlim.Release(); }
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
                foreach (Task task in tasks) task.Dispose();
            }
           // progress.Report("处理完成");
        }
        /// <summary>
        /// 写入地理坐标系切片
        /// </summary>
        /// <param name="tileFileInfo"></param>
        /// <param name="outPutDir"></param>
        /// <param name="minL"></param>
        /// <param name="maxL"></param>
        /// <param name="progress"></param>
        /// <param name="threadsCount"></param>
        /// <returns></returns>
        public static async Task GenerateGeoGoordinateSystemTiles(TileFileInfo tileFileInfo, string outPutDir, int minL, int maxL, IProgress<double> progress, int threadsCount)
        {

            using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(threadsCount))
            {
                List<Task> tasks = new List<Task>();
                for (int i = minL; i < maxL; i++)
                {
                    await semaphoreSlim.WaitAsync().ConfigureAwait(false);
                    try
                    {
                        WriteGeoLevel(i, tileFileInfo, outPutDir);
                    }
                    finally { semaphoreSlim.Release(); }
                }
                await Task.WhenAll(tasks).ConfigureAwait(false);
                foreach (Task task in tasks) task.Dispose();
            }
        }
        /// <summary>
        /// 写入当前切片级别投影坐标系切片
        /// </summary>
        /// <param name="i"></param>
        /// <param name="tileFileInfo"></param>
        /// <param name="outPutDir"></param>
        public static void WriteProjLevel(int i, TileFileInfo tileFileInfo, string outPutDir)
        {
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
            IWorkspace pWs = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(tileFileInfo.filePath), 0);
            IFeatureClass pFeatureClass = (pWs as IFeatureWorkspace).OpenFeatureClass(System.IO.Path.GetFileNameWithoutExtension(tileFileInfo.filePath));
            IFeatureLayer featureLayer = new FeatureLayerClass
            {
                FeatureClass = pFeatureClass
            };
            IDataset dataSet = (IDataset)pFeatureClass;
            featureLayer.Name = dataSet.Name;
            IMap map = new MapClass();
            map.AddLayer(featureLayer);

            IMapControlDefault pMapControl = new MapControlClass();
            pMapControl.Map = map;
            IActiveView pActiveView = pMapControl.ActiveView;

            tagRECT rect = new tagRECT();
            rect.left = rect.top = 0;
            rect.right = 256;
            rect.bottom = 256;
            IExport pExport = null;

            IEnvelope pEnvelope = new EnvelopeClass();
            string temp = i.ToString();
            if (temp.Length < 2)
                temp = "0" + temp;
            System.IO.DirectoryInfo LevelDir = new System.IO.DirectoryInfo(outPutDir).CreateSubdirectory($"L{temp}");
            pMapControl.MapScale = TileParam.TileParamProjCoordinateSystem.TileScales[i];
            pMapControl.Refresh();
            Console.WriteLine($"Level：L{temp}：比例尺：{TileParam.TileParamProjCoordinateSystem.TileScales[i]} 分辨率{TileParam.TileParamProjCoordinateSystem.Resolution(TileParam.TileParamProjCoordinateSystem.TileScales[i])}");
            double imageHeighy = TileParam.TileParamProjCoordinateSystem.Resolution(TileParam.TileParamProjCoordinateSystem.TileScales[i]) * TileParam.TileParamProjCoordinateSystem.TileSize;
            for (int dy = 1, iRnum = 0; TileParam.TileOriginPrOj.OriginY - imageHeighy * dy > tileFileInfo.MinY - imageHeighy; dy++, iRnum++)
            {
                if (TileParam.TileOriginPrOj.OriginY - imageHeighy * dy > tileFileInfo.MaxY)
                    continue;
                string tmpy = iRnum.ToString("X");
                while (tmpy.Length < 8)
                    tmpy = "0" + tmpy;
                Console.WriteLine($"--行号：R{tmpy}");
                System.IO.DirectoryInfo RowDir = LevelDir.CreateSubdirectory($"R{tmpy}");

                for (int dx = 1, iCnum = 0; TileParam.TileOriginPrOj.OriginX + imageHeighy * dx < tileFileInfo.MaxX + imageHeighy; dx++, iCnum++)
                {
                    if (TileParam.TileOriginPrOj.OriginX + imageHeighy * dx < tileFileInfo.MinX)
                        continue;
                    string tmpx = iCnum.ToString("X");
                    while (tmpx.Length < 8)
                        tmpx = "0" + tmpx;
                    try
                    {
                        pEnvelope.PutCoords(TileParam.TileOriginPrOj.OriginX + imageHeighy * (dx - 1), TileParam.TileOriginPrOj.OriginY - imageHeighy * dy, TileParam.TileOriginPrOj.OriginX + imageHeighy * dx, TileParam.TileOriginPrOj.OriginY - imageHeighy * (dy - 1));
                        pExport = ToExporter("PNG");
                        pExport.ExportFileName = RowDir.FullName + "\\C" + tmpx + ".png";
                        Console.WriteLine($"----列号：C{tmpx}    {pExport.ExportFileName}");
                        pExport.Resolution = 96;
                        IEnvelope pPixelBoundsEnv = new EnvelopeClass();
                        pPixelBoundsEnv.PutCoords(rect.left, rect.top, rect.right, rect.bottom);
                        pExport.PixelBounds = pPixelBoundsEnv;
                        //开始导出，获取DC  
                        int hDC = pExport.StartExporting();
                        //导出
                        pActiveView.Output(hDC, 96, ref rect, pEnvelope, null);
                        //结束导出
                        pExport.FinishExporting();
                        //清理导出类
                        pExport.Cleanup();
                        RealeaseAO(pExport);
                        RealeaseAO(pPixelBoundsEnv);
                    }
                    catch (Exception ex){ Helpers.ErrorHelper.PrintException(ex); }
                }
            }
            RealeaseAO(workspaceFactory);
            RealeaseAO(pWs);
            RealeaseAO(pFeatureClass);
            RealeaseAO(featureLayer);
            RealeaseAO(map);
            RealeaseAO(pMapControl);
        }
        /// <summary>
        /// 写入当前切片级别地理坐标系切片
        /// </summary>
        /// <param name="i"></param>
        /// <param name="tileFileInfo"></param>
        /// <param name="outPutDir"></param>
        public static void WriteGeoLevel(int i, TileFileInfo tileFileInfo, string outPutDir)
        {
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
            IWorkspace pWs = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(tileFileInfo.filePath), 0);
            IFeatureClass pFeatureClass = (pWs as IFeatureWorkspace).OpenFeatureClass(System.IO.Path.GetFileNameWithoutExtension(tileFileInfo.filePath));
            IFeatureLayer featureLayer = new FeatureLayerClass
            {
                FeatureClass = pFeatureClass
            };
            IDataset dataSet = (IDataset)pFeatureClass;
            featureLayer.Name = dataSet.Name;
            IMap map = new MapClass();
            map.AddLayer(featureLayer);

            IMapControlDefault pMapControl = new MapControlClass();
            pMapControl.Map = map;
            IActiveView pActiveView = pMapControl.ActiveView;

            tagRECT rect = new tagRECT();
            rect.left = rect.top = 0;
            rect.right = 256;
            rect.bottom = 256;
            IExport pExport = null;

            IEnvelope pEnvelope = new EnvelopeClass();
            string temp = i.ToString();
            if (temp.Length < 2)
                temp = "0" + temp;
            System.IO.DirectoryInfo LevelDir = new System.IO.DirectoryInfo(outPutDir).CreateSubdirectory($"L{temp}");
            pMapControl.MapScale = TileParam.TileParamProjCoordinateSystem.TileScales[i];
            pMapControl.Refresh();
            Console.WriteLine($"Level：L{temp}：分辨率{TileParam.TileParamGeoCoordinateSystem.TileResolutions[i]}");
            double imageHeighy = TileParam.TileParamGeoCoordinateSystem.TileResolutions[i] * TileParam.TileParamGeoCoordinateSystem.TileSize;
            for (int dy = 1, iRnum = 0; TileParam.TileOriginGeo.OriginY - imageHeighy * dy > tileFileInfo.MinY - imageHeighy; dy++, iRnum++)
            {
                if (TileParam.TileOriginGeo.OriginY - imageHeighy * dy > tileFileInfo.MaxY)
                    continue;
                string tmpy = iRnum.ToString("X");
                while (tmpy.Length < 8)
                    tmpy = "0" + tmpy;
                Console.WriteLine($"--行号：R{tmpy}");
                System.IO.DirectoryInfo RowDir = LevelDir.CreateSubdirectory($"R{tmpy}");

                for (int dx = 1, iCnum = 0; TileParam.TileOriginGeo.OriginX + imageHeighy * dx < tileFileInfo.MaxX + imageHeighy; dx++, iCnum++)
                {
                    if (TileParam.TileOriginGeo.OriginX + imageHeighy * dx < tileFileInfo.MinX)
                        continue;
                    string tmpx = iCnum.ToString("X");
                    while (tmpx.Length < 8)
                        tmpx = "0" + tmpx;
                    try
                    {
                        pEnvelope.PutCoords(TileParam.TileOriginGeo.OriginX + imageHeighy * (dx - 1), TileParam.TileOriginGeo.OriginY - imageHeighy * dy, TileParam.TileOriginGeo.OriginX + imageHeighy * dx, TileParam.TileOriginGeo.OriginY - imageHeighy * (dy - 1));
                        pExport = ToExporter("PNG");
                        pExport.ExportFileName = RowDir.FullName + "\\C" + tmpx + ".png";
                        Console.WriteLine($"----列号：C{tmpx}    {pExport.ExportFileName}");
                        pExport.Resolution = 96;
                        IEnvelope pPixelBoundsEnv = new EnvelopeClass();
                        pPixelBoundsEnv.PutCoords(rect.left, rect.top, rect.right, rect.bottom);
                        pExport.PixelBounds = pPixelBoundsEnv;
                        //开始导出，获取DC  
                        int hDC = pExport.StartExporting();
                        //导出
                        pActiveView.Output(hDC, 96, ref rect, pEnvelope, null);
                        //结束导出
                        pExport.FinishExporting();
                        //清理导出类
                        pExport.Cleanup();
                        RealeaseAO(pExport);
                        RealeaseAO(pPixelBoundsEnv);
                    }
                    catch (Exception ex) { Helpers.ErrorHelper.PrintException(ex); }
                }
            }
            RealeaseAO(workspaceFactory);
            RealeaseAO(pWs);
            RealeaseAO(pFeatureClass);
            RealeaseAO(featureLayer);
            RealeaseAO(map);
            RealeaseAO(pMapControl);
        }
        public static IExport ToExporter(string f)
        {
            if (!string.IsNullOrEmpty(f))
                switch (f.ToLower())
                {
                    case "gif":
                        ExportGIFClass xx = new ExportGIFClass();
                        IRgbColor cc = new RgbColor();//透明色
                        cc.Blue = 255;
                        cc.Green = 255;
                        cc.Red = 255;
                        xx.TransparentColor = cc;
                        return xx;
                    case "jpeg":
                    case "jpg":
                        return new ExportJPEGClass();
                    case "bmp":
                        return new ExportBMPClass();
                }
            ExportPNGClass xx2 = new ExportPNGClass();
            IRgbColor cc2 = new RgbColor();//透明色
            cc2.Blue = 255;
            cc2.Green = 255;
            cc2.Red = 255;
            xx2.TransparentColor = cc2;
            return xx2;
        }

        public static void RealeaseAO(object obj)
        { 
            if(obj!=null)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        }

        public static async ValueTask<TileFileInfo> GetTileFileInfo(FileInfo fileInfo)
        {
            CheckHelper.CheckFile(fileInfo, true);
            TileFileInfo tileFileInfo = null;
            await Task.Run(() =>
            {
                IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactory();
                IWorkspace pWs = workspaceFactory.OpenFromFile(System.IO.Path.GetDirectoryName(fileInfo.FullName), 0);
                IFeatureClass pFeatureClass = (pWs as IFeatureWorkspace).OpenFeatureClass(System.IO.Path.GetFileNameWithoutExtension(fileInfo.FullName));
                IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
                double XMin = pGeoDataset.Extent.XMin;
                double YMin = pGeoDataset.Extent.YMin;
                double XMax = pGeoDataset.Extent.XMax;
                double YMax = pGeoDataset.Extent.YMax;
                Enums.EnumSpatialType enumSpatialType= Enums.EnumSpatialType.ShpFile;
                ISpatialReference spatialReference = pGeoDataset.SpatialReference;
                Enums.SpatialReferenceType SpatialReference = Enums.SpatialReferenceType.None;
                if (spatialReference is IGeographicCoordinateSystem)
                    SpatialReference = Enums.SpatialReferenceType.GeoCoordinateSystem;
                else if(spatialReference is IProjectedCoordinateSystem)
                    SpatialReference = Enums.SpatialReferenceType.ProjCoordinateSystem;
                else
                    SpatialReference = Enums.SpatialReferenceType.None;
                switch (fileInfo.Extension)
                {
                    case ".shp":
                        enumSpatialType = Enums.EnumSpatialType.ShpFile;
                        break;
                    case "img":
                        enumSpatialType = Enums.EnumSpatialType.img;
                        break;
                    case "tif":
                        enumSpatialType = Enums.EnumSpatialType.tif;
                        break;
                    default:
                        break;
                }
                tileFileInfo = new TileFileInfo()
                {
                    filePath = fileInfo.FullName,
                    fileSize = 0,
                    SpatialType= enumSpatialType,
                    MinX = XMin,
                    MinY = YMin,
                    MaxX = XMax,
                    MaxY = YMax,
                    SpatialReferenceType= SpatialReference
                };
                AGSTile.RealeaseAO(workspaceFactory);
                AGSTile.RealeaseAO(pWs);
                AGSTile.RealeaseAO(workspaceFactory);
                AGSTile.RealeaseAO(pFeatureClass);
                AGSTile.RealeaseAO(pGeoDataset);
            }).ConfigureAwait(false);
            return tileFileInfo;
        }

    }
}
