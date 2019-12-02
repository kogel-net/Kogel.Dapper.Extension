using Kogel.Dapper.Extension.Core.SetC;
using Kogel.Dapper.Extension.Core.SetQ;
using Kogel.Dapper.Extension.Helper;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Kogel.Dapper.Extension.Attributes;
using Kogel.Dapper.Extension.Model;
using System.Reflection;
using Dapper;
using Kogel.Dapper.Extension.Exception;
using System.Linq.Expressions;
using System;
using Kogel.Dapper.Extension.Attributes;

namespace Kogel.Dapper.Extension
{
	public class IBaseEntity<T> //where T : class
	{
		//[Identity]
		//public virtual T Id { get; set; }
	}
}
