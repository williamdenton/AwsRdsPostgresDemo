using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace WilliamDenton.AwsRdsPostgresDemo.Models
{
	public class Customer
	{
		public long Id { get; set; }

		[MaxLength(100)]
		public string Name { get; set; }

		[MaxLength(10)]
		public string CustomerCode { get; set; }

		public Instant CreatedOn { get; set; }
	}

}
