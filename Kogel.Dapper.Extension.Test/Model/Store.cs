using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Kogel.Dapper.Extension.Attributes;

namespace Lige.Model
{
	/// <summary>
	/// 類Store。
	/// </summary>
	[Serializable]
	[Display(Rename = "LiGe_Store")]
	public partial class Store
	{
		public Store()
		{
			IsDelete = false;
		}
		#region Model
		private int _id;
		private string _name_cn;
		private string _name_en;
		private int _storecategoryid;
		private string _storecategoryname;
		private string _address_cn;
		private string _address_en;
		private string _imgurl_cn;
		private string _imgurl_en;
		private string _showimgurl_cn;
		private string _showimgurl_en;
		private string _businesstime;
		private string _phone;
		private string _createuser;
		private DateTime _createdate= DateTime.Now;
		private string _updateuser;
		private DateTime _updatedate= DateTime.Now;
		private bool _isdelete=false;
		private string _marketid;
		private bool _isonline;
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
		public string Name_CN
		{
			set{ _name_cn=value;}
			get{return _name_cn;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Name_EN
		{
			set{ _name_en=value;}
			get{return _name_en;}
		}
		/// <summary>
		/// 商戶分類id
		/// </summary>
		public int StoreCategoryId
		{
			set{ _storecategoryid=value;}
			get{return _storecategoryid;}
		}
		/// <summary>
		/// 商戶分類名稱
		/// </summary>
		public string StoreCategoryName
		{
			set{ _storecategoryname=value;}
			get{return _storecategoryname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Address_CN
		{
			set{ _address_cn=value;}
			get{return _address_cn;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Address_EN
		{
			set{ _address_en=value;}
			get{return _address_en;}
		}
		/// <summary>
		/// 商戶圖片
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
		/// 商戶展示圖片
		/// </summary>
		public string ShowImgUrl_CN
		{
			set{ _showimgurl_cn=value;}
			get{return _showimgurl_cn;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ShowImgUrl_EN
		{
			set{ _showimgurl_en=value;}
			get{return _showimgurl_en;}
		}
		/// <summary>
		///  工作時間(字符串，例如 工作時間 xxx到xxxx)
		/// </summary>
		public string BusinessTime
		{
			set{ _businesstime=value;}
			get{return _businesstime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Phone
		{
			set{ _phone=value;}
			get{return _phone;}
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
			set { _isdelete = value; }
			get { return _isdelete; }
		}
		/// <summary>
		/// 商場id
		/// </summary>
		public string MarketId
		{
			set{ _marketid=value;}
			get{return _marketid;}
		}
		/// <summary>
		/// 是否上線
		/// </summary>
		public bool IsOnline { get => _isonline; set => _isonline = value; }
		/// <summary>
		/// 是否是支持商戶
		/// </summary>
		public bool IsSupport { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Secretkey { get; set; }
		/// <summary>
		/// 商户logo
		/// </summary>
		public string LogoUrl { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int Seq { get; set; }
        #endregion Model

    }
}

