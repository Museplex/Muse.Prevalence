using System;
using NUnit.Framework;
using TaskManagement.ObjectModel;

namespace TaskManagement.ObjectModel.Tests
{
	/// <summary>
	/// Testes para a classe Task.
	/// </summary>
	[TestFixture]
	public class TaskTestCase : Assertion
	{
		protected Task _task;

		[SetUp]
		public void SetUp()
		{
			_task = new Task("Preval�ncia de Objetos");
		}

		[Test]
		public void TestConstruct()
		{
			AssertEquals("Tarefa deve armazenar seu nome!", "Preval�ncia de Objetos", _task.Name);
			AssertNotNull("Log de trabalho n�o pode ser nulo!", _task.WorkRecords);
			AssertEquals("Log de trabalho deve estar vazio!", 0, _task.WorkRecords.Count);
		}
	}
}
