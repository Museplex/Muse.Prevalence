using System;

namespace TaskManagement.ObjectModel
{
	/// <summary>
	/// Um registro de horas trabalhadas.
	/// </summary>
	[Serializable]
	public class WorkRecord
	{		
		protected DateTime _startTime;

		protected DateTime _endTime;

		/// <summary>
		/// Cria um novo registro para o per�odo indicado.
		/// </summary>
		/// <param name="startTime">in�cio do per�odo</param>
		/// <param name="endTime">fim do per�odo</param>
		public WorkRecord(DateTime startTime, DateTime endTime)
		{
			if (endTime < startTime)
			{
				throw new ArgumentException("A hora de fim deve ser maior que a hora de in�cio!", "endTime");
			}

			_startTime = startTime;
			_endTime = endTime;
		}

		/// <summary>
		/// In�cio do per�odo trabalhado.
		/// </summary>
		public DateTime StartTime
		{
			get
			{
				return _startTime;
			}
		}

		/// <summary>
		/// Fim do per�odo trabalhado.
		/// </summary>
		public DateTime EndTime
		{
			get
			{
				return _endTime;
			}
		}
	}
}
