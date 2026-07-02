/*
MIT License

Copyright (c) 2026 Sarayut Chaisuriya

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Note on dataset:
The included MalwareBazaar sample CSV has been modified:
- Limited to first 500 rows
- Header format adjusted for teaching purposes
See README.md for full details.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FileProcessing
{
	public partial class frmTextView : Form
	{
		// ===== Added controls for Partial Loading + Filtering (Homework Part A) =====
		private Label lblFrom;
		private TextBox tbFrom;
		private Label lblTo;
		private TextBox tbTo;
		private Label lblFilter;
		private TextBox tbFilterType;
		private Button btLoadFiltered;

		/// <summary>
		/// Initializes a new instance of the frmTextView class.
		/// </summary>
		public frmTextView()
		{
			InitializeComponent();
			SetupExtraControls();
		}

		/// <summary>
		/// Creates the extra controls used for partial loading (m-n) and filtering,
		/// and places them on the CSV tab next to the existing "read as csv" button.
		/// Built in code (instead of editing the Designer file) to keep the change simple.
		/// </summary>
		private void SetupExtraControls()
		{
			lblFrom = new Label { Text = "From (m):", AutoSize = true, Location = new System.Drawing.Point(180, 18) };
			tbFrom = new TextBox { Location = new System.Drawing.Point(260, 14), Width = 60 };

			lblTo = new Label { Text = "To (n):", AutoSize = true, Location = new System.Drawing.Point(330, 18) };
			tbTo = new TextBox { Location = new System.Drawing.Point(390, 14), Width = 60 };

			lblFilter = new Label { Text = "Filter file_type:", AutoSize = true, Location = new System.Drawing.Point(465, 18) };
			tbFilterType = new TextBox { Location = new System.Drawing.Point(575, 14), Width = 100 };

			btLoadFiltered = new Button
			{
				Text = "Load / Filter",
				Location = new System.Drawing.Point(690, 8),
				Size = new System.Drawing.Size(120, 30)
			};
			btLoadFiltered.Click += btLoadFiltered_Click;

			tabpCSV.Controls.Add(lblFrom);
			tabpCSV.Controls.Add(tbFrom);
			tabpCSV.Controls.Add(lblTo);
			tabpCSV.Controls.Add(tbTo);
			tabpCSV.Controls.Add(lblFilter);
			tabpCSV.Controls.Add(tbFilterType);
			tabpCSV.Controls.Add(btLoadFiltered);
		}
		/// <summary>
		/// Handles the Click event of the Read button by loading the contents of the specified file into the display area.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event data.</param>
		private void btRead_Click(object sender, EventArgs e)
		{			
            string content = File.ReadAllText(tbFileName.Text);
            rtbShow.Text = content;
		}
        /// <summary>
        /// Handles the Click event of the btReadCSV button, reading CSV data from the specified file and populating the
        /// DataGridView with its contents.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
		private void btReadCSV_Click(object sender, EventArgs e)
		{
			bool bLimitReached = false;

            using (StreamReader srReader = new StreamReader(tbFileName.Text))
            {
                string strLine; // Variable to hold each line read from the file
				bool bHeaderRead = false;   // Flag to indicate whether the header line has been read
				int dataRowCount = 0;

				// Main loop: Read the file line by line
				while ((strLine = srReader.ReadLine()) != null)
                {
                    string[] strHeaders_arr = null;
					// Skip comment lines and check for header line
					if (strLine.StartsWith("#")) 
                    { 
                        if (    strLine.Length > 8
                           &&   strLine.Substring(0, 8).Equals("#HEADER") 
                           )
                        {
							// Read the header line and split it into an array of headers
							strHeaders_arr = strLine.Substring(8).Split(',');
						}
                        continue;
                    }
					// Split the current line into an array of values
					string[] strValues_arr = strLine.Split(',');

					// If the header has not been read yet, add the headers to the DataGridView columns
					if (!bHeaderRead)
                    {
						// Add the headers to the DataGridView columns, using the header names from the header line if available
						foreach (string strHeader in strValues_arr)
                        {
                            if ( strHeaders_arr == null )
                                dgvData.Columns.Add(strHeader.Trim(), strHeader.Trim());
                            else
                                dgvData.Columns.Add(strHeader.Trim(), strHeaders_arr[dgvData.Columns.Count].Trim());
						}
                        bHeaderRead = true;
                    }
                    else
                    {
						if (dataRowCount >= MAX_ROWS_TO_READ)
						{
							bLimitReached = true;
							break; // Stop reading once we hit the row cap, so huge files don't hang/crash
						}
						// Add the values to the DataGridView rows
						dgvData.Rows.Add(strValues_arr);
						dataRowCount++;
                    }
				}   // Main loop: Read the file line by line
			}

			if (bLimitReached)
			{
				MessageBox.Show("Note: the file has more than " + MAX_ROWS_TO_READ +
					" rows, so only the first " + MAX_ROWS_TO_READ + " were loaded.",
					"Row Limit", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}
		// ================================================================
		// Homework Part A additions:
		//   2) Partial Loading (m-n)
		//   3) Filter by file type (file_type_guess column)
		//   Optional bonus: use m-n and filter together
		// ================================================================

		/// <summary>
		/// Reads the CSV file into a list of rows (each row is a string array),
		/// separating out the header. Comment lines (starting with '#') are skipped,
		/// except for the one that contains the actual column names.
		/// </summary>
		private const int MAX_ROWS_TO_READ = 50000; // Limit reading so large files (~1M rows) don't slow down / crash the program

		private List<string[]> ReadCsvRows(string path, out string[] headers)
		{
			var rows = new List<string[]>();
			string headerLine = null;

			foreach (string rawLine in File.ReadLines(path))
			{
				if (rows.Count >= MAX_ROWS_TO_READ)
					break; // stop reading once we have enough rows

				string line = rawLine;

				if (line.StartsWith("#"))
				{
					// The real header line looks like: # "first_seen_utc","sha256_hash",...
					if (line.Contains(",") && line.Contains("\""))
						headerLine = line;
					continue;
				}

				if (string.IsNullOrWhiteSpace(line))
					continue;

				rows.Add(SplitCsvLine(line));
			}

			if (headerLine != null)
			{
				headers = SplitCsvLine(headerLine.TrimStart('#', ' '));
			}
			else if (rows.Count > 0)
			{
				// Fallback: treat the first data row as header if no header comment was found
				headers = rows[0];
				rows.RemoveAt(0);
			}
			else
			{
				headers = new string[0];
			}

			return rows;
		}

		/// <summary>
		/// Splits one CSV line by comma and trims surrounding spaces/quotes from each value.
		/// </summary>
		private string[] SplitCsvLine(string line)
		{
			string[] parts = line.Split(',');
			for (int i = 0; i < parts.Length; i++)
				parts[i] = parts[i].Trim().Trim('"');
			return parts;
		}

		/// <summary>
		/// Clears the grid and displays the given headers/rows.
		/// </summary>
		private void DisplayRows(string[] headers, List<string[]> rows)
		{
			dgvData.Columns.Clear();
			dgvData.Rows.Clear();

			foreach (string h in headers)
				dgvData.Columns.Add(h, h);

			foreach (string[] row in rows)
				dgvData.Rows.Add(row);
		}

		/// <summary>
		/// Handles the Click event of "Load / Filter". Supports:
		///  - Partial loading using the From (m) / To (n) record numbers (1-based, inclusive)
		///  - Filtering by file type (matches the file_type_guess column, case-insensitive)
		///  - Both combined: filter is applied first, then the m-n range is taken from the filtered results
		/// Leaving From/To empty loads the full range. Leaving Filter empty skips filtering.
		/// </summary>
		private void btLoadFiltered_Click(object sender, EventArgs e)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(tbFileName.Text) || !File.Exists(tbFileName.Text))
				{
					MessageBox.Show("Error: file not found. Please browse a valid CSV file first.",
						"Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				string[] headers;
				List<string[]> allRows = ReadCsvRows(tbFileName.Text, out headers);

				// --- Step 1: Filter (optional) ---
				List<string[]> workingSet = allRows;
				string filterText = tbFilterType.Text.Trim();

				if (!string.IsNullOrEmpty(filterText))
				{
					int typeCol = Array.FindIndex(headers,
						h => h.Equals("file_type_guess", StringComparison.OrdinalIgnoreCase));

					if (typeCol < 0)
					{
						MessageBox.Show("Error: this file has no \"file_type_guess\" column, so it can't be filtered by file type.",
							"Column Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					workingSet = allRows
						.Where(r => typeCol < r.Length &&
									r[typeCol].IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0)
						.ToList();

					if (workingSet.Count == 0)
					{
						DisplayRows(headers, workingSet);
						MessageBox.Show("No records found with file type \"" + filterText + "\".",
							"File Type Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}
				}

				// --- Step 2: Partial loading m-n (optional) ---
				string mText = tbFrom.Text.Trim();
				string nText = tbTo.Text.Trim();
				bool hasRange = !string.IsNullOrEmpty(mText) || !string.IsNullOrEmpty(nText);

				// Warn about the row cap when:
				//  - the user asked for the whole file (no filter, no range), or
				//  - the requested "To (n)" goes beyond what was actually loaded
				bool rangeExceedsCap = hasRange && int.TryParse(nText, out int nCheck) && nCheck > MAX_ROWS_TO_READ;
				if (allRows.Count == MAX_ROWS_TO_READ && string.IsNullOrEmpty(filterText) &&
					(!hasRange || rangeExceedsCap))
				{
					MessageBox.Show("Note: the file has more than " + MAX_ROWS_TO_READ +
						" rows, so only the first " + MAX_ROWS_TO_READ + " were loaded. " +
						"Records beyond that are not available.",
						"Row Limit", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

				if (hasRange)
				{
					int m, n;
					bool mOk = int.TryParse(mText, out m);
					bool nOk = int.TryParse(nText, out n);

					if (!mOk || !nOk)
					{
						MessageBox.Show("Error: From (m) and To (n) must be whole numbers.",
							"Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					if (m < 1 || n < 1 || m > n)
					{
						MessageBox.Show("Error: invalid range. Make sure 1 <= m <= n.",
							"Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					if (m > workingSet.Count)
					{
						MessageBox.Show("Error: 'From' is beyond the number of available records (" +
							workingSet.Count + ").", "Invalid Range", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					int startIndex = m - 1; // convert to 0-based
					int endIndex = Math.Min(n, workingSet.Count) - 1;
					int count = endIndex - startIndex + 1;

					workingSet = workingSet.GetRange(startIndex, count);
				}

				DisplayRows(headers, workingSet);

				if (workingSet.Count == 0)
					MessageBox.Show("No records matched your criteria.", "No Results",
						MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				// The program must never crash on bad input / large files - show a friendly message instead.
				MessageBox.Show("Error: " + ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// Handles the Click event of the Browse button, allowing the user to select a file and displaying its path in the
		/// file name text box.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event data.</param>
		private void btBrowse_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					tbFileName.Text = ofd.FileName;
				}
			}
		}
	}   // End of frmTextView class
}
