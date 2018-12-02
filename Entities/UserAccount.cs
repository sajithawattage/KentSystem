using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Entities
{
	public class UserAccount : GenericPrincipal, IComparable
	{

		public UserAccount(string userName)
			: base(new GenericIdentity(userName), null)
		{

		}

		public string UserName { get; set;}
		public string DisplayName { get; set;}
		public string Email { get; set;}
		public int Role { get; set;}
		public int Status { get; set;}
		public string EmployeeCode { get;set;}
		public string EncryptedPassword { get; set; }

		#region IComparable Members

		public int CompareTo(object obj)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
