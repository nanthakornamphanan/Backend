# 📄 Text/CSV Viewer (C# Learning Project)

## 📌 Overview

This project is a **simple Text/CSV Viewer written in C#**.  
It is designed as a **learning project for students** to practice:

* Basic C# programming
* File I/O handling
* Working with CSV data
* Git and version control
* Open-source and community practices

***

## 🎯 Learning Objectives

By completing this project, students will learn:

### 💻 C# Fundamentals

* Reading text and CSV files
* String manipulation and parsing
* Basic UI or console output (depending on your version)
* Error handling and input validation

### 🗂️ Software Engineering Basics

* Project structure
* Code readability and maintainability
* Separation of concerns

### 🔧 Git & Version Control

* Initialize a Git repository
* Commit changes with meaningful messages
* Use branches (optional for advanced students)
* Collaborate using pull requests (if working in groups)

### 🌍 Open Source & Community Discipline

* Understanding open-source licenses (MIT License)
* Giving proper credit to data sources
* Writing clean documentation
* Respecting third-party Terms of Use

***

## 🧱 Project Structure

```
/TextCsvViewer
│
├── Program.cs
├── CsvViewer.cs
├── LICENSE
├── README.md
└── data/
    └── malware_500.csv
```

> Note: Some files such as `*.Designer.cs` and `*.resx` are auto-generated and should not be manually edited.

***

## ▶️ How to Run

1. Open the project in **Visual Studio** or compatible IDE
2. Build the solution
3. Run the program
4. Load a `.txt` or `.csv` file to view its contents

***

## Data Source

This project uses malware metadata from MalwareBazaar
(https://bazaar.abuse.ch/), operated by abuse.ch.

Terms of Use:
https://bazaar.abuse.ch/faq/#tos

### Modifications to Dataset

The original dataset has been modified for educational purposes:

- Only the first 500 records are included
- The header line has been adjusted:
  - From:
    # "first_seen_utc", ...
  - To:
    #HEADER: "first_seen_utc", ...

These changes are intended to simplify usage and improve clarity for students.

The original dataset structure and attribution remain unchanged where applicable.
``
***

## 📜 License

This project is licensed under the **MIT License**.

You are free to:

* Use
* Modify
* Distribute

As long as you include the original license notice.

See the `LICENSE` file for full details.

***

## ✅ Student Tasks

Students are encouraged to:

### Beginner

* Load and display a CSV file
* Format output neatly
* Handle missing or invalid data

### Intermediate

* Add search/filter functionality
* Highlight specific columns
* Support large file handling

### Advanced

* Build a GUI (WinForms or WPF)
* Implement sorting and column selection
* Improve performance and memory usage

***

## 🧪 Suggested Git Exercises

* Create your repository
* Commit your initial version
* Add new features step by step
* Write clear commit messages:
  * ✅ `Add CSV file reader`
  * ✅ `Fix parsing bug in column handling`
  * ❌ `fix stuff`

***

## 🤝 Contribution Guidelines (For Students)

* Write clean, readable code
* Add comments when necessary
* Respect original authors and data sources
* Do not remove license or attribution
* Follow project structure and naming conventions

***

## Contribution Policy (Current Stage)

This repository is provided for **learning and reference purposes only**.

At this stage of the course:

- Students should NOT submit Pull Requests
- Do NOT attempt to modify the original repository
- Work should be done in your own copy of the project

### Student Instructions

1. Clone or download this repository
2. Create your own repository
3. Practice and modify code independently

Future assignments may introduce collaboration and Pull Requests.
***

## ⚠️ Disclaimer

This project may use metadata related to malware samples for educational purposes only.

* Do **not** execute or download actual malware
* Use data responsibly
* Follow all applicable laws and policies

***
## ⚠️ Ethical Use and Dual-Use Awareness

This project uses data related to malware for **educational purposes only**.

Students must understand the concept of **dual-use technology**:

- Software and technical knowledge can be used for **both beneficial and harmful purposes**
- The same skills used to analyze malware can also be misused to create it

### Our Objective

This project is designed to:

- Introduce students to real-world data handling
- Build skills in software development and analysis
- Promote **ethical awareness in computing**

We emphasize that:

- Students are expected to act as **responsible software developers**
- The goal is to **understand and defend**, not to exploit
- This course does **not support or encourage malicious activities**

### Professional Responsibility

As future professionals, students should:

- Follow ethical and legal guidelines
- Respect data sources and licenses
- Use their knowledge to **protect systems and improve security**
- Contribute positively to the software community

> We aim to educate **ethical programmers**, not individuals who misuse technology.

***

## 👨‍🏫 Instructor

**Sarayut Chaisuriya**  
C# Instructor / Software Development Educator

***

## 💡 Final Note

This project is not just about coding—it is about becoming a **responsible software developer**:

* Write clean code
* Use proper tools
* Respect licenses
* Learn continuously

***

***

## ✅ Homework Submission — Added Features

### 📖 Program Description

**Text/CSV Viewer** is a Windows Forms (C#) desktop application for browsing large text and CSV files. This version was extended to handle the MalwareBazaar sample dataset (`data/malware_500.csv`) with three main capabilities beyond the original template:

1. **Partial Loading (m–n)** — load only a specific range of records instead of the whole file
2. **Filtering by file type** — show only rows matching a given `file_type_guess` value (e.g. `exe`)
3. **Combined filter + range**, plus safe handling of large files and bad input so the program never crashes

The program has two tabs:
* **Text** — reads a plain text file and shows its raw content
* **CSV** — reads a CSV file into a data grid, with the original `read as csv` button and the new controls described below

### 🖥️ New UI Controls (CSV tab)

| Control | Purpose |
|---|---|
| **From (m)** | Start record number (1-based, inclusive) for partial loading |
| **To (n)** | End record number (1-based, inclusive) for partial loading |
| **Filter file_type** | Text to match against the `file_type_guess` column (case-insensitive, partial match) |
| **Load / Filter** | Runs the load, using whichever of the fields above are filled in |

Leaving **From/To** empty loads the full (capped) file. Leaving **Filter file_type** empty skips filtering. Both can be filled in together — the filter is applied first, then the m–n range is taken from the filtered results.

### 🛡️ Error Handling & Safety

The program is designed to **never crash**, even with bad input or huge files:

| Situation | Behavior |
|---|---|
| Invalid range (`m > n`, non-numeric, `m`/`n` < 1) | Shows an error message box, no crash |
| `m` beyond the number of available records | Shows an error message box |
| File doesn't exist | Shows an error message box |
| Filter text matches no rows | Grid is cleared, shows an info message |
| CSV has no `file_type_guess` column | Shows an error message instead of silently guessing the wrong column |
| File has more rows than the load cap (currently **50,000**) | Shows a "Row Limit" notice; only the first 50,000 data rows are loaded, so the app doesn't hang or throw `OutOfMemoryException` on very large files |

This row cap applies to **both** the original `read as csv` button and the new `Load / Filter` button.

### ⚙️ How It Works (Implementation Notes)

* `ReadCsvRows()` — reads the CSV line by line (up to the row cap), skips comment lines (`#...`), and picks up the real header from the `#` line that contains the quoted column names (e.g. `# "first_seen_utc","sha256_hash",...`). Falls back to treating the first data row as the header if no such comment line is found.
* `SplitCsvLine()` — splits a line by comma and trims stray spaces/quotes from each value.
* `DisplayRows()` — clears and repopulates the `DataGridView` with the given headers/rows.
* `btLoadFiltered_Click()` — the handler for the new "Load / Filter" button: validates the file, applies the optional filter, applies the optional m–n range, then displays the result. All steps are wrapped in error handling.
* `SetupExtraControls()` — creates the new labels/textboxes/button in code (instead of editing the WinForms Designer file) and adds them to the CSV tab.

### 🧪 Testing

All features above were tested against `data/malware_500.csv` (491 data rows) and a large real-world `full.csv` file (to trigger the row-limit path). Test cases, steps, expected vs. actual results, and pass/fail status are documented in **`Basev100.xlsx`** (submitted alongside this repo), covering normal cases, edge cases, and error cases — including range validation, filtering, missing columns, and the 50,000-row cap.

### ▶️ How to Run & Test

1. Open the solution in Visual Studio and **Rebuild Solution**
2. Run the program, go to the **CSV** tab
3. Click **Browse** and select a CSV file (e.g. `data/malware_500.csv`)
4. Try:
   * `read as csv` — loads the whole file (capped at 50,000 rows)
   * `Load / Filter` with From/To filled in — partial loading
   * `Load / Filter` with Filter file_type filled in — filtering
   * `Load / Filter` with both filled in — combined filter + range
   * Invalid inputs (e.g. `From=200, To=100`) — confirm the error messages appear and the app doesn't crash
