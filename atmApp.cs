using System;
using System.Collections.Generic;

namespace ATMSystem
{
    public class Client
    {
        public string ClientName { get; set; }
        public string PAN { get; set; }
        public float Balance { get; private set; }

        public Client(string clientName, string pan, float balance)
        {
            ClientName = clientName;
            PAN = pan;
            Balance = balance;
        }

        public void EnterPIN(string pin)
        {
            Console.WriteLine($"Клиент {ClientName}: метод EnterPIN вызван.");
        }

        public void SelectOperation()
        {
            Console.WriteLine($"Клиент {ClientName}: метод SelectOperation вызван.");
        }

        public void DepositCash(float amount)
        {
            Balance += amount;
            Console.WriteLine($"Клиент {ClientName}: метод DepositCash вызван. Баланс пополнен на {amount}. Новый баланс: {Balance}");
        }

        public void CollectCash(float amount)
        {
            if (amount > Balance)
            {
                Console.WriteLine($"Клиент {ClientName}: недостаточно средств.");
                return;
            }

            Balance -= amount;
            Console.WriteLine($"Клиент {ClientName}: метод CollectCash вызван. Снято {amount}. Остаток: {Balance}");
        }
    }

    public class ATM
    {
        public int ID { get; set; }
        public string Location { get; set; }
        public string Condition { get; set; }

        public ATM(int id, string location, string condition)
        {
            ID = id;
            Location = location;
            Condition = condition;
        }

        public void InsertCard()
        {
            Console.WriteLine($"ATM {ID}: метод InsertCard вызван.");
        }

        public void AcceptCash(float amount)
        {
            Console.WriteLine($"ATM {ID}: метод AcceptCash вызван. Принято {amount} наличных.");
        }

        public void GiveCash(float amount)
        {
            Console.WriteLine($"ATM {ID}: метод GiveCash вызван. Выдано {amount} наличных.");
        }

        public void PrintReceipt()
        {
            Console.WriteLine($"ATM {ID}: метод PrintReceipt вызван. Чек напечатан.");
        }
    }

    public class Transaction
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public float Amount { get; set; }
        public DateTime Date { get; set; }

        public Transaction(int id, string type, float amount, DateTime date)
        {
            ID = id;
            Type = type;
            Amount = amount;
            Date = date;
        }

        public void RecordTransaction()
        {
            Console.WriteLine($"Transaction {ID}: метод RecordTransaction вызван. Тип: {Type}, Сумма: {Amount}, Дата: {Date}.");
        }

        public void CheckBalance(Client client)
        {
            Console.WriteLine($"Transaction {ID}: метод CheckBalance вызван. Баланс клиента {client.ClientName}: {client.Balance}.");
        }
    }

    public class Bank
    {
        public string BankName { get; set; }
        public string BankLocation { get; set; }

        public Bank(string bankName, string bankLocation)
        {
            BankName = bankName;
            BankLocation = bankLocation;
        }

        public void AuthorizeClient(Client client)
        {
            Console.WriteLine($"Банк {BankName}: метод AuthorizeClient вызван. Клиент {client.ClientName} авторизован.");
        }

        public void UpdateBalance(Client client, float amount)
        {
            client.DepositCash(amount);
            Console.WriteLine($"Банк {BankName}: метод UpdateBalance вызван. Баланс клиента {client.ClientName} обновлен на {amount}.");
        }

        public void BlockCard(string pan)
        {
            Console.WriteLine($"Банк {BankName}: метод BlockCard вызван. Карта с PAN {pan} заблокирована.");
        }

        public void TransferCash(Client fromClient, Client toClient, float amount)
        {
            if (fromClient.Balance < amount)
            {
                Console.WriteLine($"Банк {BankName}: недостаточно средств для перевода.");
                return;
            }

            fromClient.CollectCash(amount);
            toClient.DepositCash(amount);

            Console.WriteLine($"Банк {BankName}: метод TransferCash вызван. Переведено {amount} от {fromClient.ClientName} к {toClient.ClientName}.");
        }
    }

    public class Support
    {
        public string ContactNumber { get; set; }

        public Support(string contactNumber)
        {
            ContactNumber = contactNumber;
        }

        public void ReportCrash()
        {
            Console.WriteLine($"Support: метод ReportCrash вызван. Сообщение о неисправности принято.");
        }

        public void RestoreWork()
        {
            Console.WriteLine($"Support: метод RestoreWork вызван. Работа восстановлена.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Пример использования
            Client client = new Client("Иван Иванов", "1234-5678-9012-3456", 10000);
            ATM atm = new ATM(1, "Центр города", "Рабочий");
            Bank bank = new Bank("Национальный банк", "Улица Ленина");
            Transaction transaction = new Transaction(1, "Снятие", 2000, DateTime.Now);
            Support support = new Support("8-800-555-35-35");

            atm.InsertCard();
            client.EnterPIN("4321");
            client.CollectCash(2000);
            transaction.RecordTransaction();
            bank.UpdateBalance(client, -2000);
            support.ReportCrash();
        }
    }
}

