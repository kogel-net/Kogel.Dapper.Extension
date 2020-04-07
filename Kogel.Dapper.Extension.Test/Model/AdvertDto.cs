using System;
using System.Collections.Generic;
using System.Text;

namespace Lige.ViewModel.Web
{
    /// <summary>
    /// 廣告列表
    /// </summary>
    public class AdvertListDto
    {
        /// <summary>
		/// ID
		/// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 標題（中文）
        /// </summary>
        public string Title_CN { get; set; }
        /// <summary>
        /// 標題（英文）
        /// </summary>
        public string Title_EN { get; set; }
        /// <summary>
        /// 圖片（中文）
        /// </summary>
        public string ImgUrl_CN { get; set; }
        /// <summary>
        /// 圖片（英文）
        /// </summary>
        public string ImgUrl_EN { get; set; }
        /// <summary>
        /// 內容（中文）
        /// </summary>
        public string Content_CN { get; set; }
        /// <summary>
        /// 內容（英文）
        /// </summary>
        public string Content_EN { get; set; }
        /// <summary>
        /// 廣告類型(1推廣廣告,2商品優惠券,3首頁廣告)
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 關聯id(廣告類型為2時對應商戶id)
        /// </summary>
        public int AssoId { get; set; }
        /// <summary>
        /// 所屬商戶
        /// </summary>
        public string AssoName { get; set; }
        /// <summary>
		/// 是否上線
		/// </summary>
		public bool IsOnline { get; set; }
        /// <summary>
        /// 排序字段（推广广告专用）
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 商场ID
        /// </summary>
        public string MarketId { get; set; }
		/// <summary>
		/// 排序字段（cms顺序）
		/// </summary>
		public int Seq { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>
        public string DetailImgUrl_CN { get; set; }

        /// <summary>
        /// 预览图（英文）
        /// </summary>
        public string DetailImgUrl_EN { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime EndTime { get; set; }
    }
    /// <summary>
    /// 廣告添加
    /// </summary>
    public class AdvertAddDto
    {
        /// <summary>
        /// 標題（中文）
        /// </summary>
        public string Title_CN { get; set; }
        /// <summary>
        /// 標題（英文）
        /// </summary>
        public string Title_EN { get; set; }
        /// <summary>
        /// 圖片（中文）
        /// </summary>
        public string ImgUrl_CN { get; set; }
        /// <summary>
        /// 圖片（英文）
        /// </summary>
        public string ImgUrl_EN { get; set; }
        /// <summary>
        /// 內容（中文）
        /// </summary>
        public string Content_CN { get; set; }
        /// <summary>
        /// 內容（英文）
        /// </summary>
        public string Content_EN { get; set; }
        /// <summary>
        /// 廣告類型(1推廣廣告,2商品優惠券,3首頁廣告)
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 關聯id(廣告類型為2時對應商戶id)
        /// </summary>
        public int AssoId { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 商场ID
        /// </summary>
        public string MarketId { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>
        public string DetailImgUrl_CN { get; set; }

        /// <summary>
        /// 预览图（英文）
        /// </summary>
        public string DetailImgUrl_EN { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }
    /// <summary>
    /// 廣告編輯
    /// </summary>
    public class AdvertEditDto
    {
        /// <summary>
		/// ID
		/// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 標題（中文）
        /// </summary>
        public string Title_CN { get; set; }
        /// <summary>
        /// 標題（英文）
        /// </summary>
        public string Title_EN { get; set; }
        /// <summary>
        /// 圖片（中文）
        /// </summary>
        public string ImgUrl_CN { get; set; }
        /// <summary>
        /// 圖片（英文）
        /// </summary>
        public string ImgUrl_EN { get; set; }
        /// <summary>
        /// 內容（中文）
        /// </summary>
        public string Content_CN { get; set; }
        /// <summary>
        /// 內容（英文）
        /// </summary>
        public string Content_EN { get; set; }
        /// <summary>
        /// 廣告類型(1推廣廣告,2商品優惠券,3首頁廣告)
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 關聯id(廣告類型為2時對應商戶id)
        /// </summary>
        public int AssoId { get; set; }
        /// <summary>
		/// 排序字段
		/// </summary>
		public int Sort { get; set; }

        /// <summary>
        /// 商场ID
        /// </summary>
        public string MarketId { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>
        public string DetailImgUrl_CN { get; set; }

        /// <summary>
        /// 预览图（英文）
        /// </summary>
        public string DetailImgUrl_EN { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
    }
    /// <summary>
    /// 商戶下拉列表
    /// </summary>
    public class StoreDropdownList
    {
        /// <summary>
		/// Id
		/// </summary>
		public int Id { get; set; }
        /// <summary>
        /// 商店名稱
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 商场ID
        /// </summary>
        public string MarketId { get; set; }
    }
}
