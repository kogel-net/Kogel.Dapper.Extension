using System;
using Kogel.Dapper.Extension.Attributes;

namespace Lige.Model
{
	/// <summary>
	/// 類Advert。
	/// </summary>
	[Serializable]
	[Display(Rename = "LiGe_Advert")]
	public partial class Advert
	{
		public Advert()
		{}
		#region Model
		private int _id;
		private string _title_cn;
		private string _title_en;
		private string _imgurl_cn;
		private string _imgurl_en;
		private string _content_cn;
		private string _content_en;
		private int _type;
		private int _assoid;
		private string _createuser;
		private DateTime _createdate= DateTime.Now;
		private string _updateuser;
		private DateTime _updatedate= DateTime.Now;
		private bool _isdelete=false;
		/// <summary>
		/// 
		/// </summary>
		[Identity]
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Title_CN
		{
			set{ _title_cn=value;}
			get{return _title_cn;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Title_EN
		{
			set{ _title_en=value;}
			get{return _title_en;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ImgUrl_CN
		{
			set{ _imgurl_cn=value;}
			get{return _imgurl_cn;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ImgUrl_EN
		{
			set{ _imgurl_en=value;}
			get{return _imgurl_en;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Content_CN
		{
			set{ _content_cn=value;}
			get{return _content_cn;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Content_EN
		{
			set{ _content_en=value;}
			get{return _content_en;}
		}
		/// <summary>
		/// 廣告類型（1推廣廣告,2商品優惠券,3首頁廣告）
		/// </summary>
		public int Type
		{
			set{ _type=value;}
			get{return _type;}
		}
		/// <summary>
		/// 關聯id(廣告類型為2時對應商戶id)
		/// </summary>
		public int AssoId
		{
			set{ _assoid=value;}
			get{return _assoid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CreateUser
		{
			set{ _createuser=value;}
			get{return _createuser;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateDate
		{
			set{ _createdate=value;}
			get{return _createdate;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string UpdateUser
		{
			set{ _updateuser=value;}
			get{return _updateuser;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime UpdateDate
		{
			set{ _updatedate=value;}
			get{return _updatedate;}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsDelete
		{
			set{ _isdelete=value;}
			get{return _isdelete;}
		}

        /// <summary>
		/// 是否上線
		/// </summary>
		public bool IsOnline { get; set; }

		/// <summary>
		/// 排序字段
		/// </summary>
		public int Sort { get; set; }

		/// <summary>
		/// 商场id
		/// </summary>
		public string MarketId { get; set; }

		/// <summary>
		/// 排序号
		/// </summary>
		public int Seq { get; set; }

		/// <summary>
		/// 预览图(CN)
		/// </summary>
		public string DetailImgUrl_CN { get; set; }

		/// <summary>
		/// 预览图(EN)
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

		#endregion Model
	}
}