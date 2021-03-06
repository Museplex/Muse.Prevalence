#region license
// Bamboo.Prevalence - a .NET object prevalence engine
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Based on the original concept and implementation of Prevayler (TM)
// by Klaus Wuestefeld. Visit http://www.prevayler.org for details.
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// http://bbooprevalence.sourceforge.net
// mailto:rodrigobamboo@users.sourceforge.net
#endregion

using System;
using System.IO;
using System.Collections;
using Bamboo.Prevalence;
using Bamboo.Prevalence.Attributes;

namespace ObjectModel
{
	/// <summary>
	/// An object required to complete the operation was not found.
	/// </summary>
	[Serializable]
	public class BusinessObjectNotFoundException : ApplicationException
	{
		public BusinessObjectNotFoundException(string message) : base(message)
		{
		}
	}

	/// <summary>
	/// An object with an ID that uniquely identifies it in the system.
	/// </summary>
	[Serializable]
	public class BusinessObject
	{
		protected Guid _id;

		protected BusinessObject()
		{
			_id = Guid.NewGuid();
		}		

		/// <summary>
		/// Constructs a new BusinessObject that acts as a
		/// reference to an existing BusinessObject in the
		/// system.
		/// </summary>
		/// <param name="existingObjectID"></param>
		protected BusinessObject(Guid existingObjectID)
		{
			_id = existingObjectID;
		}

		public Guid ID
		{
			get
			{
				return _id;
			}
		}
	}

	[Serializable]
	public class BusinessObjectCollection : CollectionBase
	{
		protected BusinessObjectCollection()
		{
		}

		protected BusinessObject GetByID(Guid id)
		{
			lock (InnerList.SyncRoot)
			{
				foreach (BusinessObject bo in InnerList)
				{
					if (id == bo.ID)
					{
						return bo;
					}
				}
				return null;
			}
		}

		internal void Remove(BusinessObject bo)
		{
			lock (InnerList.SyncRoot)
			{
				InnerList.Remove(bo);
			}
		}

		public object SyncRoot
		{
			get
			{
				return InnerList.SyncRoot;
			}
		}
	}

	[Serializable]
	public class NamedObject : BusinessObject
	{
		string _name;

		public NamedObject()
		{
		}

		public NamedObject(Guid id) : base(id)
		{
		}

		public NamedObject(string name)
		{
			_name = name;
		}

		public string Name
		{
			get
			{
				return _name;
			}

			set
			{
				_name = value;
			}
		}

		protected void Update(NamedObject other)
		{
			if (null != other.Name)
			{
				_name = other.Name;
			}
		}
	}
	
	[Serializable]
	public class Employee : NamedObject
	{		
		Department _department;

		public Employee(string name) : base(name)
		{				
		}

		public Employee(Guid existingObjectID) : base(existingObjectID)
		{
		}

		public Department Department
		{
			get
			{
				return _department;
			}

			set
			{
				_department = value;
			}
		}

		internal void Update(Employee other)
		{
			base.Update(other);
		}
	}

	[Serializable]
	public class EmployeeCollection : BusinessObjectCollection
	{
		public EmployeeCollection()
		{
		}

		public Employee this[int index]
		{
			get
			{
				lock (InnerList.SyncRoot)
				{
					return InnerList[index] as Employee;
				}
			}
		}

		public Employee this[Guid id]
		{
			get
			{
				return GetByID(id) as Employee;
			}
		}

		internal void Add(Employee employee)
		{
			if (null == employee)
			{
				throw new ArgumentNullException("employee");
			}

			lock (InnerList.SyncRoot)
			{
				InnerList.Add(employee);
			}
		}
	}

	[Serializable]
	public class Department : NamedObject
	{	
		protected EmployeeCollection _employees;

		public Department(string name) : base(name)
		{
			_employees = new EmployeeCollection();
		}

		public Department(Guid existingObjectID) : base(existingObjectID)
		{
		}

		public EmployeeCollection Employees
		{
			get
			{
				return _employees;
			}
		}

		internal void AddEmployee(Employee employee)
		{
			employee.Department = this;
			_employees.Add(employee);
		}

		internal void RemoveEmployee(Employee employee)
		{
			_employees.Remove(employee);
		}
	}

	[Serializable]
	public class DepartmentCollection : BusinessObjectCollection
	{
		public DepartmentCollection()
		{
		}

		public Department this[int index]
		{
			get
			{
				lock (InnerList.SyncRoot)
				{
					return InnerList[index] as Department;
				}
			}
		}

		public Department this[Guid id]
		{
			get
			{
				Department department = GetByID(id) as Department;
				if (null == department)
				{
					throw new BusinessObjectNotFoundException(string.Format("Department {0} not found!", id));
				}
				return department;
			}
		}

		internal void Add(Department department)
		{
			if (null == department)
			{
				throw new ArgumentNullException("department");
			}

			lock (InnerList.SyncRoot)
			{
				InnerList.Add(department);
			}
		}
	}

	[Serializable]
	class Company : System.MarshalByRefObject
	{		
		protected DepartmentCollection _departments;

		/// <summary>
		/// A hashtable to make possible to instantly find
		/// any object in the system.		
		/// </summary>
		protected Hashtable _objects;

		public Company()
		{
			_departments = new DepartmentCollection();
			_objects = Hashtable.Synchronized(new Hashtable());
		}

		public DepartmentCollection Departments
		{
			// We don't want/need the engine to synchronize
			// this accessor so let's mark it as a PassThough
			// method...
			[PassThrough]
			get
			{
				return _departments;
			}
		}

		public void AddDepartment(Department department)
		{
			_departments.Add(department);
			RegisterObject(department);
		}

		public void AddEmployee(Employee employee)
		{
			if (null == employee)
			{
				throw new ArgumentNullException("employee");
			}

			Department department = employee.Department;
			if (null == department)
			{
				throw new ArgumentException("Employee.Department must be set!", "employee");
			}

			// map it to the correct department object			
			_departments[department.ID].AddEmployee(employee);
			RegisterObject(employee);
		}
		
		[Query]
		public Employee GetEmployee(Guid employeeID)
		{
			Employee existing = (Employee)_objects[employeeID];
			if (null == existing)
			{
				throw new BusinessObjectNotFoundException(string.Format("Employee {0} not found!", employeeID));
			}
			return existing;
		}

		public void UpdateEmployee(Employee employee)
		{			
			GetEmployee(employee.ID).Update(employee);
		}

		public void RemoveEmployee(Guid employeeID)
		{
			Employee existing = GetEmployee(employeeID);
			existing.Department.RemoveEmployee(existing);
			UnregisterObject(employeeID);
		}

		void RegisterObject(BusinessObject bo)
		{
			_objects[bo.ID] = bo;
		}

		void UnregisterObject(Guid objectID)
		{
			_objects.Remove(objectID);
		}
	}
}

namespace CompanySample
{
	class App
	{
		[STAThread]
		static void Main(string[] args)
		{
			PrevalenceEngine engine = PrevalenceActivator.CreateTransparentEngine(typeof(ObjectModel.Company), Path.Combine(Path.GetTempPath(), "CompanySystem"));
			ObjectModel.Company company = engine.PrevalentSystem as ObjectModel.Company;

			// adding a new department is easy...
			ObjectModel.Department sales = new ObjectModel.Department("Sales");
			company.AddDepartment(sales);

			// adding a user is easy too, you only have
			// to remember to put the right department
			// reference...
			ObjectModel.Employee employee = new ObjectModel.Employee("John Salesman");
			employee.Department = new ObjectModel.Department(sales.ID);
			company.AddEmployee(employee);

			DisplayObjects(company);

			// updating an employee...
			ObjectModel.Employee updEmployee = new ObjectModel.Employee(employee.ID);
			updEmployee.Name = "Rodrigo B. de Oliveira";
			company.UpdateEmployee(updEmployee);

			DisplayObjects(company);

			company.RemoveEmployee(employee.ID);

			DisplayObjects(company);
		}

		static void DisplayObjects(ObjectModel.Company company)
		{
			// navigating through the object model couldn't be simpler
			foreach (ObjectModel.Department d in company.Departments)
			{
				Console.WriteLine(d.Name);
				foreach (ObjectModel.Employee e in d.Employees)
				{
					Console.WriteLine("\t{0} from {1}", e.Name, e.Department.Name);
				}
			}
		}
	}
}
