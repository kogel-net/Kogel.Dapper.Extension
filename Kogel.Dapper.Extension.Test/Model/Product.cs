using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using Kogel.Dapper.Extension.Attributes;

namespace Lige.Model
{
	/// <summary>
	/// 類Product。
	/// </summary>
	[Serializable]
	[Display(Rename = "LiGe_Product")]
	public partial class Product
	{
		public Product()
		{}
		#region Model
		private int _id;
		private string _name_cn;
		private string _name_en;
		private string _productcode;
		private decimal _price;
		private string _detail_cn;
		private string _detail_en;
		private string _CategoryCode;
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
		/// 
		/// </summary>
		public string ProductCode
		{
			set{ _productcode=value;}
			get{return _productcode;}
		}
		/// <summary>
		/// 
		/// </summary>
		public decimal Price
		{
			set{ _price=value;}
			get{return _price;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Detail_CN
		{
			set{ _detail_cn=value;}
			get{return _detail_cn;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Detail_EN
		{
			set{ _detail_en=value;}
			get{return _detail_en;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CategoryCode
		{
			set{ _CategoryCode = value;}
			get{return _CategoryCode; }
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
		/// 積分
		/// </summary>
		public decimal Point { get; set; }
		/// <summary>
		/// 擴展字段
		/// </summary>
		public string Ext_F1 { get; set; }
		#endregion Model
	}
}

