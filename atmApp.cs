using System;
using System.Collections.Generic;

namespace ATMSystem
{
    // Базовый класс для человека
    public abstract class Person
    {
        public string Name { get; set; }

        protected Person(string name)
        {
            Name = name;
        }

        public abstract void Authenticate();
    }

    // Клиент
    public class Client : Person
    {
        public string PAN { get; private set; }
        public float Balance { get; private set; }

        public Client(string name, string pan, float balance) : base(name)
        {
            PAN = pan;
            Balance = balance;
        }

        public override void Authenticate()
        {
            Console.WriteLine($"Клиент {Name} прошёл аутентификацию.");
        }

        public void DepositCash(float amount)
        {
            Balance += amount;
            Console.WriteLine($"Клиент {Name}: Баланс пополнен на {amount}. Новый баланс: {Balance}");
        }

        public void WithdrawCash(float amount)
        {
            if (amount > Balance)
            {
                Console.WriteLine($"Клиент {Name}: Недостаточно средств.");
                return;
            }

            Balance -= amount;
            Console.WriteLine($"Клиент {Name}: Снято {amount}. Остаток: {Balance}");
        }
    }

    // Базовый класс для устройства
    public abstract class Device
    {
        public int ID { get; private set; }
        public string Location { get; private set; }

        protected Device(int id, string location)
        {
            ID = id;
            Location = location;
        }

        public abstract void Operate();
    }

    // Банкомат
    public class ATM : Device
    {
        public string Condition { get; private set; }

        public ATM(int id, string location, string condition) : base(id, location)
        {
            Condition = condition;
        }

        public override void Operate()
        {
            Console.WriteLine($"ATM {ID}: Готов к работе на {Location}.");
        }

        public void InsertCard()
        {
            Console.WriteLine($"ATM {ID}: Карта вставлена.");
        }

        public void DispenseCash(float amount)
        {
            Console.WriteLine($"ATM {ID}: Выдано {amount} наличных.");
        }

        public void AcceptCash(float amount)
        {
            Console.WriteLine($"ATM {ID}: Принято {amount} наличных.");
        }
    }

    // Транзакция
    public abstract class Transaction
    {
        public int ID { get; private set; }
        public float Amount { get; private set; }
        public DateTime Date { get; private set; }

        protected Transaction(int id, float amount, DateTime date)
        {
            ID = id;
            Amount = amount;
            Date = date;
        }

        public abstract void Execute(Client client);

        public void RecordTransaction()
        {
            Console.WriteLine($"Транзакция {ID}: Сумма {Amount}, Дата {Date}");
        }
    }

    // Транзакция снятия наличных
    public class WithdrawalTransaction : Transaction
    {
        public WithdrawalTransaction(int id, float amount, DateTime date) : base(id, amount, date) { }

        public override void Execute(Client client)
        {
            client.WithdrawCash(Amount);
            RecordTransaction();
        }
    }

    // Транзакция пополнения
    public class DepositTransaction : Transaction
    {
        public DepositTransaction(int id, float amount, DateTime date) : base(id, amount, date) { }

        public override void Execute(Client client)
        {
            client.DepositCash(Amount);
            RecordTransaction();
        }
    }

    // Банк
    public class Bank
    {
        public string BankName { get; private set; }

        public Bank(string bankName)
        {
            BankName = bankName;
        }

        public void AuthorizeClient(Client client)
        {
            Console.WriteLine($"Банк {BankName}: Клиент {client.Name} авторизован.");
        }

        public void TransferFunds(Client fromClient, Client toClient, float amount)
        {
            if (fromClient.Balance < amount)
            {
                Console.WriteLine($"Банк {BankName}: Недостаточно средств для перевода.");
                return;
            }

            fromClient.WithdrawCash(amount);
            toClient.DepositCash(amount);
            Console.WriteLine($"Банк {BankName}: Переведено {amount} от {fromClient.Name} к {toClient.Name}.");
        }
    }

    // Техническая поддержка
    public class Support
    {
        public string ContactNumber { get; private set; }

        public Support(string contactNumber)
        {
            ContactNumber = contactNumber;
        }

        public void ReportCrash()
        {
            Console.WriteLine($"Support: Принято сообщение о неисправности.");
        }

        public void RestoreWork()
        {
            Console.WriteLine($"Support: Работа восстановлена.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Пример использования
            Client client1 = new Client("Иван Иванов", "1234-5678-9012-3456", 10000);
            Client client2 = new Client("Петр Петров", "5678-9012-3456-7890", 5000);

            ATM atm = new ATM(1, "Центр города", "Рабочий");
            Bank bank = new Bank("Национальный банк");
            Support support = new Support("8-800-555-35-35");

            atm.Operate();
            atm.InsertCard();

            bank.AuthorizeClient(client1);

            WithdrawalTransaction withdrawal = new WithdrawalTransaction(1, 2000, DateTime.Now);
            withdrawal.Execute(client1);

            DepositTransaction deposit = new DepositTransaction(2, 1000, DateTime.Now);
            deposit.Execute(client2);

            bank.TransferFunds(client1, client2, 3000);

            support.ReportCrash();
        }
    }
}
