using System;

namespace TaskManagement.ObjectModel
{
	/// <summary>
	/// Um objeto necess�rio para a opera��o n�o foi encontrado.
	/// </summary>
	public class ObjectNotFoundException : ApplicationException
	{
		public ObjectNotFoundException(string message) : base(message)
		{
		}
	}
}
