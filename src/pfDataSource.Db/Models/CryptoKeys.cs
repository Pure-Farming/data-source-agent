using System;
namespace pfDataSource.Db.Models
{
	public class CryptoKeys
	{
		public int Id { get; set; }
		public byte[] Key { get; set; }
		public byte[] InitialisationVector { get; set; }
	}
}

