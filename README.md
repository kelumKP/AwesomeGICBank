# AwesomeGIC Bank Console Application

Welcome to the **AwesomeGIC Bank Console Application**! This application is a simple banking system that allows users to manage transactions, define interest rules, and generate account statements. It is built using C# and leverages SQLite for data persistence.

## Features

1. **Input Transactions**:
   - Users can input transactions in the format `<Date> <Account> <Type> <Amount>`.
   - Supported transaction types are `D` (Deposit) and `W` (Withdrawal).
   - Transactions are processed and stored in the database.

2. **Define Interest Rules**:
   - Users can define interest rules in the format `<Date> <RuleId> <Rate in %>`.
   - Interest rules are applied to calculate interest for accounts.

3. **Print Statement**:
   - Users can generate a statement for a specific account and month.
   - The statement includes all transactions and the calculated interest for the month.

4. **Quit**:
   - Users can exit the application.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed on the machine.
- SQLite (included in the project via `SQLitePCL`).

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/kelumkp/AwesomeGICBank.git

2. Navigate to the project directory:
   ```bash
   cd AwesomeGICBank/AwesomeGICBank.ConsoleApp
3. Restore the NuGet packages:
   ```bash
   dotnet restore
4. Build the project:
   ```bash
   dotnet build
5. Run the application: 
   ```bash
   dotnet run

### Usage

1. **Input Transactions:**
   - Enter the transaction details in the format `<Date> <Account> <Type> <Amount>`.
   - Example: `20231001 ACC001 D 100.00`

2. **Define Interest Rules:**
   - Enter the interest rule details in the format `<Date> <RuleId> <Rate in %>`.
   - Example: `20231001 RULE01 1.95`

3. **Print Statement:**
   - Enter the account and month in the format `<Account> <Year><Month>`.
   - Example: `ACC001 202310`

4. **Quit:**
   - Enter `Q` to exit the application.

### Project Structure

- **AwesomeGICBank.ConsoleApp:** The main console application.
  - `Program.cs`: Entry point of the application.

- **AwesomeGICBank.Application:** Contains the core business logic.
  - `Services`: Business services like `BankingService` and `InterestRuleService`.

- **AwesomeGICBank.Core:** Contains the core entities.
  - `Entities`: Domain entities like `Account`, `Transaction`, and `InterestRule`.

- **AwesomeGICBank.Infrastructure:** Contains the data access layer.
  - `Repositories`: Repository implementations for `Account`, `Transaction`, and `InterestRule`.
  - `Interfaces`: Repository interfaces.

- **AwesomeGICBank.Tests:** Unit tests for the application.
  - `Services`: Tests for `BankingService` and `InterestRuleService`.


### Testing

The project includes unit tests for the core services. To run the tests:

1. Navigate to the test project:
   ```bash
   cd AwesomeGICBank.Tests

2. Run the tests:

   ```bash
   dotnet test

### Database
The application uses SQLite for data persistence. The database file (`awesomegicbank.db`) is automatically created in the project directory when the application is first run.

for an example in order to add first transaction for `AC001` bank account can use `20230626 AC001 D 100.00`


### Contributing
Contributions are welcome! Please fork the repository and submit a pull request with the changes.

### License
This project is licensed under the MIT License. See the LICENSE file for details.

### Acknowledgments
 - `SQLitePCL` for SQLite database access.
 - `Moq` for mocking in unit tests.
 - `NUnit` for unit testing.
	
Thank you for using AwesomeGIC Bank! We hope you find this application useful. If you have any questions or feedback, please feel free to reach out.
