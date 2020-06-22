
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGSTilesMapping.TileParam
{
    public static class TileParamProjCoordinateSystem
    {
        /// <summary>
        /// AGS默认的切图比例尺
        /// </summary>

        public static readonly double[] TileScales = new double[20]{
        591657527.591555,
        295828763.79577702,
        147914381.89788899,
        73957190.948944002,
        36978595.474472001,
        18489297.737236001,
        9244648.8686180003,
        4622324.4343090001,
        2311162.2171550002,
        1155581.108577,
        577790.55428899999,
        288895.27714399999,
        144447.638572,
        72223.819285999998,
        36111.909642999999,
        18055.954822,
        9027.9774109999998,
        4513.9887049999998,
        2256.994353,
        1128.4971760000001
        };
        /// <summary>
        /// DPI
        /// </summary>
        public const int DPI = 96;
        /// <summary>
        /// AGS地图转换参数
        /// </summary>
        public const double Inch2Centimeter = 2.54000508001016;
        /// <summary>
        /// 切图分辨率
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static double Resolution(double scale) => scale * Inch2Centimeter / DPI/100;
        /// <summary>
        /// 瓦片大小
        /// </summary>
        public const ushort TileSize = 256;
        
    }
    public static class TileParamGeoCoordinateSystem
    {
        public static readonly double[] TileResolutions = new double[20]{
            0.15228550437313793,
            0.076142752186568963,
            0.038071376093284481,
            0.019035688046642241,
            0.0095178440233211203,
            0.0047589220116605602,
            0.0023794610058302801,
            0.00118973050291514,
            0.00059486525145757002,
            0.00029743262572878501,
            0.00015228550437313792,
            7.6142752186568962e-005,
            3.8071376093284481e-005,
            1.903568804664224e-005,
            9.5178440233211202e-006,
            4.7589220116605601e-006,
            2.3794610058302801e-006,
            1.18973050291514e-006,
            5.9486525145757001e-007,
            2.9743262572878501e-007
        };
        /// <summary>
        /// 瓦片大小
        /// </summary>
        public const ushort TileSize = 256;

        /// <summary>
        /// DPI
        /// </summary>
        public const int DPI = 96;
        /// <summary>
        /// AGS地图转换参数
        /// </summary>
        public const double Inch2Centimeter = 2.54000508001016;

    }

    public static class TileOriginPrOj
    {
        /// <summary>
        /// 切图原点X
        /// </summary>
        public const double OriginX = -20037508.342787001;
        /// <summary>
        /// 切图原点Y
        /// </summary>
        public const double OriginY = 20037508.342787001;
    }

    public static class TileOriginGeo
    {
        /// <summary>
        /// 切图原点X
        /// </summary>
        public const double OriginX = -400;
        /// <summary>
        /// 切图原点Y
        /// </summary>
        public const double OriginY = 400;
    }
}
