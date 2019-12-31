using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Test.ViewModel
{
	public class NewsDto: IBaseEntity<NewsDto, long>
	{
		public override long Id { get; set; }

		public string Contents { get; set; }

		public List<CommentDto> Comments { get; set; }
	}
}
