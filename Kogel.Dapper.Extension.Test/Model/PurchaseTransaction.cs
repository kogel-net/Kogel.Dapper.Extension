using Kogel.Dapper.Extension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kogel.Dapper.Extension.Test.Model
{
	public class PurchaseTransaction
	{
		[Identity]
		public int SqlId { get; set; }

		public string PhysicalCardId { get; set; }

		public decimal TransactionAmount { get; set; }

		public string MachineId { get; set; }

		public string N5_ProfileId { get; set; }

		public int Point { get; set; }

		public DateTime CreateDateTime { get; set; }

		public string InvoiceNumber { get; set; }
	}
}
