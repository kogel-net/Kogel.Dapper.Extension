using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
namespace Kogel.Dapper.Core.Test.Models.Model
{
	/// <summary>
	/// 类Product。
	/// </summary>
	[Serializable]
	public partial class Product
	{
		#region Model
		private int _id;
		private bool _isdelete;
		private string _createuser;
		private DateTime _credatetime;
		private string _updateuser;
		private DateTime _updatetime;
		private string _productcode;
		private string _productname_en;
		private string _productname_ch;
		private string _productdetails_en;
		private string _productdetails_ch;
		private int _productcategoryid;
		private string _component_ch;
		private string _component_en;
		private string _cookingmethod_ch;
		private string _cookingmethod_en;
		private decimal _price=0.0M;
		private int _pickupdoodstime;
		private int _producttype=0;
		/// <summary>
		/// 
		/// </summary>
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
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
		public DateTime CredateTime
		{
			set{ _credatetime=value;}
			get{return _credatetime;}
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
		public DateTime UpdateTime
		{
			set{ _updatetime=value;}
			get{return _updatetime;}
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
		public string ProductName_EN
		{
			set{ _productname_en=value;}
			get{return _productname_en;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ProductName_CH
		{
			set{ _productname_ch=value;}
			get{return _productname_ch;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ProductDetails_EN
		{
			set{ _productdetails_en=value;}
			get{return _productdetails_en;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ProductDetails_CH
		{
			set{ _productdetails_ch=value;}
			get{return _productdetails_ch;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int ProductCategoryId
		{
			set{ _productcategoryid=value;}
			get{return _productcategoryid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Component_CH
		{
			set{ _component_ch=value;}
			get{return _component_ch;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Component_EN
		{
			set{ _component_en=value;}
			get{return _component_en;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CookingMethod_CH
		{
			set{ _cookingmethod_ch=value;}
			get{return _cookingmethod_ch;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CookingMethod_EN
		{
			set{ _cookingmethod_en=value;}
			get{return _cookingmethod_en;}
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
		public int PickUpDoodsTime
		{
			set{ _pickupdoodstime=value;}
			get{return _pickupdoodstime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int ProductType
		{
			set{ _producttype=value;}
			get{return _producttype;}
		}
		#endregion Model
	}
}

