using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGSTilesMapping
{
    public class Enums
    {
        public enum SpatialReferenceType
        { 
            [Description("未知坐标系")]
            None=1,
            [Description("地理坐标系")]
            GeoCoordinateSystem,
            [Description("投影坐标系")]
            ProjCoordinateSystem
        }
        public enum EnumSpatialType
        {
            [Description("Shp File")]
            ShpFile = 0,
            [Description("File Geodatabase")]
            GdbFile,
            [Description("Personal Geodatabase")]
            MdbFile,
            [Description("SDE空间数据库数据")]
            SdeFeatureClass,
            [Description("tif数据")]
            tif,
            [Description("img数据")]
            img,
        }
    }
}
