using System;
using System.Globalization;

namespace EmployeeDirectoryManager
{
	public sealed class Employee
	{
		// Class Variables / Properties
		public string Id { get; }
		public string FullName { get; }
		public string Department { get; }
		public string Role { get; }
		public double Salary { get; }
		public DateTime HireDate { get; }

		// Constructor
		public Employee(string id, string fullName, string department, string role,
			double salary, DateTime hireDate)
		{
			// Validation
			if (string.IsNullOrWhiteSpace(id))
				throw new ArgumentException("Employee ID is required.");

			if (string.IsNullOrWhiteSpace(fullName))
				throw new ArgumentException("Full name is required.");

			if (string.IsNullOrWhiteSpace(department))
				throw new ArgumentException("Department is required.");

			if (string.IsNullOrWhiteSpace(role))
				throw new ArgumentException("Role is required.");

			if (salary < 0)
				throw new ArgumentException("Salary must be greater than or equal to 0.");

			if (hireDate > DateTime.Today)
				throw new ArgumentException("Hire date cannot be in the future.");

			// Assign Class Variables
			Id = id;
			FullName = fullName;
			Department = department;
			Role = role;
			Salary = salary;
			HireDate = hireDate;
		}

		public override string ToString()
			=> $"{Id} | {FullName} | {Department} | {Role} | {Salary:C0} | {HireDate:yyyy-MM-dd}";

		// !!! No need to modify anything below this line.

		// -------- CSV helpers (comma-quote safe, ISO dates) --------
		public string ToCsv()
		{
			return string.Join(",",
				CsvEscape(Id),
				CsvEscape(FullName),
				CsvEscape(Department),
				CsvEscape(Role),
				Salary.ToString(CultureInfo.InvariantCulture),
				HireDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
			);
		}

		public static Employee FromCsv(string csvLine)
		{
			var cols = CsvParse(csvLine);
			if (cols.Count != 6) throw new FormatException("Invalid employee record (expected 6 columns).");

			string id = cols[0];
			string fullName = cols[1];
			string department = cols[2];
			string role = cols[3];

			if (!double.TryParse(cols[4], NumberStyles.Float,
				CultureInfo.InvariantCulture, out var salary))
				throw new FormatException("Invalid salary value.");

			if (!DateTime.TryParseExact(cols[5], "yyyy-MM-dd",
				CultureInfo.InvariantCulture, DateTimeStyles.None, out var hire))
				throw new FormatException("Invalid hire date.");

			return new Employee(id, fullName, department, role, salary, hire);
		}

		private static string CsvEscape(string input)
		{
			bool needs = input.Contains(',') || input.Contains('"') ||
				input.Contains('\n') || input.Contains('\r');

			if (!needs) return input;

			return "\"" + input.Replace("\"", "\"\"") + "\"";
		}

		private static System.Collections.Generic.List<string> CsvParse(string line)
		{
			var result = new System.Collections.Generic.List<string>();
			var sb = new System.Text.StringBuilder();
			bool inQuotes = false;

			for (int i = 0; i < line.Length; i++)
			{
				char c = line[i];

				if (inQuotes)
				{
					if (c == '"')
					{
						if (i + 1 < line.Length && line[i + 1] == '"')
						{
							sb.Append('"');
							i++;
						}
						else
						{
							inQuotes = false;
						}
					}
					else sb.Append(c);
				}
				else
				{
					if (c == ',')
					{
						result.Add(sb.ToString());
						sb.Clear();
					}
					else if (c == '"')
					{
						inQuotes = true;
					}
					else sb.Append(c);
				}
			}

			result.Add(sb.ToString());
			return result;
		}
	}
}