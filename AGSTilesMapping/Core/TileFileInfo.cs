using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGSTilesMapping.Core
{
    public class TileFileInfo
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string filePath { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public double fileSize { get; set; }
        /// 空间数据类型
        /// </summary>
        public Enums.EnumSpatialType SpatialType { get; set; }
        /// <summary>
        /// Xmin
        /// </summary>
        public double MinX { get; set; }
        /// <summary>
        /// MinY
        /// </summary>
        public double MinY { get; set; }
        /// <summary>
        /// MaxX
        /// </summary>
        public double MaxX { get; set; }
        /// <summary>
        /// MaxY
        /// </summary>
        public double MaxY { get; set; }

        public Enums.SpatialReferenceType SpatialReferenceType { get; set; }
    }

    


}
