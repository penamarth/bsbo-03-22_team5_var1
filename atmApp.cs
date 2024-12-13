using System;
using System.Collections.Generic;
using System.Linq;

namespace ATMSystem
{
    // Интерфейс для банка
    public interface IBank
    {
        void AuthorizeClient(Client client);
        Client GetClientByPAN(string pan);
        void TransferFunds(Client fromClient, string toPAN, float amount, IBank recipientBank);
    }

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

    // Клиент банка
    public class Client : Person
    {
        public int Id { get; private set; }
        public string PAN { get; private set; }
        public float Balance { get; private set; }

        public Client(int id, string name, string pan, float balance) : base(name)
        {
            Id = id;
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

    // Старый банк
    public class Bank
    {
        public string BankName { get; private set; }
        private List<Client> Clients { get; set; }

        public Bank(string bankName)
        {
            BankName = bankName;
            Clients = new List<Client>();
        }

        public void AddClient(Client client)
        {
            Clients.Add(client);
        }

        public Client GetClientByPAN(string pan)
        {
            return Clients.FirstOrDefault(c => c.PAN == pan);
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

    // Адаптер для старого банка
    public class BankAdapter : IBank
    {
        private readonly Bank _bank;

        public BankAdapter(Bank bank)
        {
            _bank = bank;
        }

        public void AuthorizeClient(Client client)
        {
            _bank.AuthorizeClient(client);
        }

        public Client GetClientByPAN(string pan)
        {
            return _bank.GetClientByPAN(pan);
        }

        public void TransferFunds(Client fromClient, string toPAN, float amount, IBank recipientBank)
        {
            Client recipient = recipientBank.GetClientByPAN(toPAN);
            if (recipient != null)
            {
                _bank.TransferFunds(fromClient, recipient, amount);
            }
            else
            {
                Console.WriteLine("Получатель не найден в другом банке.");
            }
        }
    }

    // Новый банк
    public class NewBank
    {
        private Dictionary<string, Client> Clients { get; set; } = new Dictionary<string, Client>();

        public void AddClient(Client client)
        {
            Clients[client.PAN] = client;
        }

        public bool ValidateClientPAN(string pan)
        {
            return Clients.ContainsKey(pan);
        }

        public Client FetchClientByPAN(string pan)
        {
            return Clients.ContainsKey(pan) ? Clients[pan] : null;
        }

        public void PerformTransfer(string fromPAN, string toPAN, float amount)
        {
            if (!Clients.ContainsKey(fromPAN) || !Clients.ContainsKey(toPAN))
            {
                Console.WriteLine("Новый банк: Клиент не найден.");
                return;
            }

            var fromClient = Clients[fromPAN];
            var toClient = Clients[toPAN];

            if (fromClient.Balance < amount)
            {
                Console.WriteLine("Новый банк: Недостаточно средств для перевода.");
                return;
            }

            fromClient.WithdrawCash(amount);
            toClient.DepositCash(amount);
            Console.WriteLine($"Новый банк: Переведено {amount} от {fromClient.Name} к {toClient.Name}.");
        }
    }

    // Адаптер для нового банка
    public class NewBankAdapter : IBank
    {
        private readonly NewBank _newBank;

        public NewBankAdapter(NewBank newBank)
        {
            _newBank = newBank;
        }

        public void AuthorizeClient(Client client)
        {
            if (_newBank.ValidateClientPAN(client.PAN))
                Console.WriteLine($"Новый банк: Клиент {client.Name} авторизован.");
            else
                Console.WriteLine("Новый банк: Ошибка авторизации клиента.");
        }

        public Client GetClientByPAN(string pan)
        {
            return _newBank.FetchClientByPAN(pan);
        }

        public void TransferFunds(Client fromClient, string toPAN, float amount, IBank recipientBank)
        {
            Client recipient = recipientBank.GetClientByPAN(toPAN);
            if (recipient != null)
            {
                _newBank.PerformTransfer(fromClient.PAN, toPAN, amount);
            }
            else
            {
                Console.WriteLine("Получатель не найден в другом банке.");
            }
        }
    }

    // Банкомат
    public class ATM
    {
        public int ID { get; private set; }
        public string Location { get; private set; }
        public string Condition { get; private set; }
        private IBank Bank { get; set; }
        private Client CurrentClient { get; set; }

        public ATM(int id, string location, string condition, IBank bank)
        {
            ID = id;
            Location = location;
            Condition = condition;
            Bank = bank;
        }

        public void Operate()
        {
            Console.WriteLine($"ATM {ID}: Готов к работе на {Location}.");
        }

        public void InsertCard(string pan)
        {
            CurrentClient = Bank.GetClientByPAN(pan);
            if (CurrentClient != null)
            {
                Console.WriteLine($"ATM {ID}: Карта клиента {CurrentClient.Name} вставлена.");
                Bank.AuthorizeClient(CurrentClient);
            }
            else
            {
                Console.WriteLine($"ATM {ID}: Клиент с картой {pan} не найден.");
            }
        }

        public void PerformWithdrawal(float amount)
        {
            if (CurrentClient == null)
            {
                Console.WriteLine($"ATM {ID}: Нет авторизованного клиента.");
                return;
            }

            CurrentClient.WithdrawCash(amount);
        }

        public void PerformDeposit(float amount)
        {
            if (CurrentClient == null)
            {
                Console.WriteLine($"ATM {ID}: Нет авторизованного клиента.");
                return;
            }

            CurrentClient.DepositCash(amount);
        }

        public void TransferFunds(string recipientPAN, float amount, IBank recipientBank)
        {
            if (CurrentClient == null)
            {
                Console.WriteLine($"ATM {ID}: Нет авторизованного клиента.");
                return;
            }

            Bank.TransferFunds(CurrentClient, recipientPAN, amount, recipientBank);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Старый банк
            Bank oldBank = new Bank("Национальный банк");
            oldBank.AddClient(new Client(1, "Иван Иванов", "1234-5678-9012-3456", 10000));
            IBank oldBankAdapter = new BankAdapter(oldBank);

            // Новый банк
            NewBank newBank = new NewBank();
            newBank.AddClient(new Client(2, "Анна Смирнова", "1111-2222-3333-4444", 7000));
            IBank newBankAdapter = new NewBankAdapter(newBank);

            // Банкомат для старого банка
            ATM atm1 = new ATM(1, "Центр города", "Рабочий", oldBankAdapter);
            atm1.Operate();
            atm1.InsertCard("1234-5678-9012-3456");
            atm1.PerformWithdrawal(2000);

            // Банкомат для нового банка
            ATM atm2 = new ATM(2, "Офис компании", "Рабочий", newBankAdapter);
            atm2.Operate();
            atm2.InsertCard("1111-2222-3333-4444");
            atm2.PerformDeposit(1500);

            // Перевод средств между банками
            atm1.TransferFunds("1111-2222-3333-4444", 1000, newBankAdapter);
        }
    }
}
