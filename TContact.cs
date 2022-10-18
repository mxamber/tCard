using System;

namespace tCard
{
	/// <summary>
	/// Stores Telegram contact information
	/// </summary>
	public class TContact
	{
		public string FirstName;
		public string LastName;
		public string FullName;
		public long UUID;
		public string PhoneNumber;
		public DateTime DateCreated;
		public KindOfContact Kind;
		
		public enum KindOfContact {
			NULL,
			Individual,
			Group,
			Org,
			Location
		}
		
		public TContact(string namefirst = "", string namelast = "", string namefull = "", long id = 0, string phone = "", string date = null, KindOfContact kind = 0)
		{
			if(!String.IsNullOrWhiteSpace(namefirst)) { this.FirstName = namefirst; }
			if(!String.IsNullOrWhiteSpace(namelast)) { this.LastName = namelast; }
			if(!String.IsNullOrWhiteSpace(namefull)) { this.FullName = namefull; }
			if(id != 0) { this.UUID = id; }
			if(!String.IsNullOrWhiteSpace(phone)) { this.PhoneNumber = phone; }
		}
		
		public override string ToString() {
			string stringified = "";
			if(!String.IsNullOrWhiteSpace(FullName)) { stringified += FullName; }
			else {
				stringified += FirstName.Length > 0 ? FirstName : "";
				stringified += LastName.Length > 0 ? " " + LastName : "";	
			}
			stringified += (UUID != 0) ? ", ID: " + UUID : "";
			stringified += !String.IsNullOrWhiteSpace(PhoneNumber) ? ", phone: " + PhoneNumber : "";
			stringified += DateCreated != default(DateTime) ? ", account created: " + DateCreated.ToUniversalTime() : "";
			return stringified;
		}
	}
}
