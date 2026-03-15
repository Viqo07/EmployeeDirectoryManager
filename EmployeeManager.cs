using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace EmployeeDirectoryManager
{
	public sealed class EmployeeManager
	{
		// Public BindingList that updates the DataGridView automatically
		public BindingList<Employee> Employees { get; } = new BindingList<Employee>();

		// Add with validation (unique Id)
		public void AddEmployee(Employee e)
		{
			// Check for duplicate ID
			if (Employees.Any(emp => emp.Id == e.Id))
				throw new InvalidOperationException("Employee ID already exists.");

			// Add employee
			Employees.Add(e);
		}

		// Update by Id (replace fields)
		public void UpdateEmployee(Employee updated)
		{
			// Find employee by ID
			var existing = Employees.FirstOrDefault(emp => emp.Id == updated.Id);

			if (existing == null)
				throw new InvalidOperationException("Employee not found.");

			// Replace employee
			Employees.Remove(existing);
			Employees.Add(updated);
		}

		// Delete by Id
		public bool RemoveEmployee(string id)
		{
			// Find employee
			var emp = Employees.FirstOrDefault(e => e.Id == id);

			// If not found return false
			if (emp == null)
				return false;

			// Remove employee
			Employees.Remove(emp);

			return true;
		}

		//!!! Helper Methods for the CSV. Do not modify anything below this line

		// -------- Persistence (CSV) --------
		public void SaveToCsv(string path)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)) ?? ".");
			using var sw = new StreamWriter(path, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
			sw.WriteLine("Id,FullName,Department,Role,Salary,HireDate");

			foreach (var e in Employees)
				sw.WriteLine(e.ToCsv());
		}

		public void LoadFromCsv(string path)
		{
			using var sr = new StreamReader(path, Encoding.UTF8);
			Employees.Clear();

			string? header = sr.ReadLine(); // skip header
			if (header is null) throw new InvalidDataException("File is empty.");

			int line = 1, loaded = 0, skipped = 0;

			while (!sr.EndOfStream)
			{
				string? row = sr.ReadLine();
				line++;

				if (string.IsNullOrWhiteSpace(row)) continue;

				try
				{
					var e = Employee.FromCsv(row);

					if (Employees.Any(x => string.Equals(x.Id, e.Id, StringComparison.OrdinalIgnoreCase)))
						throw new InvalidOperationException($"Duplicate ID '{e.Id}' in file.");

					Employees.Add(e);
					loaded++;
				}
				catch (Exception ex)
				{
					skipped++;
					Console.WriteLine($"Skipped line {line}: {ex.Message}");
				}
			}

			Console.WriteLine($"Load complete. Loaded: {loaded}, Skipped: {skipped}");
		}
	}
}