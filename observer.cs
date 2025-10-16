using System;
using System.Collections.Generic;

namespace WeatherObserverExample
{
    public interface IObserver
    {
        void Update(float temperature);
        string Name { get; }
    }

    public interface ISubject
    {
        void RegisterObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
        void NotifyObservers();
    }

    public class WeatherStation : ISubject
    {
        private List<IObserver> _observers = new List<IObserver>();
        private float _temperature;

        public void RegisterObserver(IObserver observer)
        {
            if (_observers.Contains(observer))
            {
                Console.WriteLine($"Наблюдатель '{observer.Name}' уже зарегистрирован.");
                return;
            }
            _observers.Add(observer);
            Console.WriteLine($"Наблюдатель '{observer.Name}' добавлен.");
        }

        public void RemoveObserver(IObserver observer)
        {
            if (_observers.Remove(observer))
            {
                Console.WriteLine($"Наблюдатель '{observer.Name}' успешно удалён.");
            }
            else
            {
                Console.WriteLine($"Ошибка: наблюдатель '{observer.Name}' не найден.");
            }
        }

        public void NotifyObservers()
        {
            foreach (var obs in _observers)
            {
                try
                {
                    obs.Update(_temperature);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при уведомлении '{obs.Name}': {ex.Message}");
                }
            }
        }

        public void SetTemperature(float newTemperature)
        {
            if (newTemperature < -100f || newTemperature > 100f)
            {
                Console.WriteLine("Предупреждение: введено необычное значение температуры.");
            }

            _temperature = newTemperature;
            Console.WriteLine($"\n[WeatherStation] Температура установлена: {newTemperature}°C. Уведомляем наблюдателей...");
            NotifyObservers();
        }

        public void ListObservers()
        {
            Console.WriteLine("\nЗарегистрированные наблюдатели:");
            if (_observers.Count == 0) Console.WriteLine("  (нет)");
            else
            {
                int i = 1;
                foreach (var o in _observers)
                {
                    Console.WriteLine($"  {i++}. {o.Name}");
                }
            }
        }
    }

    public class WeatherDisplay : IObserver
    {
        public string Name { get; private set; }
        public WeatherDisplay(string name) { Name = name; }

        public void Update(float temperature)
        {
            Console.WriteLine($"{Name} отображает температуру: {temperature}°C");
        }
    }

    public class EmailAlert : IObserver
    {
        public string Name { get; private set; }
        private string _email;
        public EmailAlert(string name, string email)
        {
            Name = name;
            _email = email;
        }

        public void Update(float temperature)
        {
            Console.WriteLine($"{Name} ({_email}): отправлено email-уведомление. Текущая темп.: {temperature}°C");
        }
    }

    public class SoundAlarm : IObserver
    {
        public string Name { get; private set; }
        public SoundAlarm(string name) { Name = name; }

        public void Update(float temperature)
        {
            if (temperature > 35.0f)
                Console.WriteLine($"{Name}: Внимание! Высокая температура {temperature}°C — звуковая сигнализация включена!");
            else
                Console.WriteLine($"{Name}: Температура {temperature}°C — звуковая сигнализация не требуется.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            WeatherStation station = new WeatherStation();

            var mobileApp = new WeatherDisplay("Мобильное приложение");
            var billboard = new WeatherDisplay("Электронное табло");
            var emailAlert = new EmailAlert("Email-уведомление", "user@example.com");
            var alarm = new SoundAlarm("Звуковая сигнализация");

            station.RegisterObserver(mobileApp);
            station.RegisterObserver(billboard);
            station.RegisterObserver(emailAlert);

            Console.WriteLine(" Система мониторинга погоды ");

            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1 - Установить температуру (уведомить всех)");
                Console.WriteLine("2 - Добавить наблюдателя");
                Console.WriteLine("3 - Удалить наблюдателя");
                Console.WriteLine("4 - Показать список наблюдателей");
                Console.WriteLine("5 - Зарегистрировать тестовую сигнализацию");
                Console.WriteLine("0 - Выход");
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine();

                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        Console.Write("Введите температуру (°C): ");
                        if (float.TryParse(Console.ReadLine(), out float temp))
                        {
                            station.SetTemperature(temp);
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: введено некорректное значение температуры.");
                        }
                        break;

                    case "2":
                        Console.WriteLine("Выберите тип наблюдателя: 1-Display, 2-Email, 3-Sound");
                        Console.Write("Тип: ");
                        string t = Console.ReadLine();
                        if (t == "1")
                        {
                            Console.Write("Введите имя дисплея: ");
                            string name = Console.ReadLine();
                            station.RegisterObserver(new WeatherDisplay(name));
                        }
                        else if (t == "2")
                        {
                            Console.Write("Введите имя (например, Email-1): ");
                            string name = Console.ReadLine();
                            Console.Write("Введите email: ");
                            string email = Console.ReadLine();
                            station.RegisterObserver(new EmailAlert(name, email));
                        }
                        else if (t == "3")
                        {
                            Console.Write("Введите имя сигнализации: ");
                            string name = Console.ReadLine();
                            station.RegisterObserver(new SoundAlarm(name));
                        }
                        else
                        {
                            Console.WriteLine("Некорректный тип.");
                        }
                        break;

                    case "3":
                        station.ListObservers();
                        Console.Write("Введите имя наблюдателя для удаления: ");
                        string removeName = Console.ReadLine();
                        IObserver found = null;
                        foreach (var field in station.GetType().GetField("_observers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(station) as List<IObserver>)
                        {
                            if (field.Name.Equals(removeName, StringComparison.OrdinalIgnoreCase))
                            {
                                found = field;
                                break;
                            }
                        }
                        if (found != null)
                        {
                            station.RemoveObserver(found);
                        }
                        else
                        {
                            Console.WriteLine($"Наблюдатель с именем '{removeName}' не найден.");
                        }
                        break;

                    case "4":
                        station.ListObservers();
                        break;

                    case "5":
                        station.RegisterObserver(alarm);
                        break;

                    default:
                        Console.WriteLine("Некорректный выбор.");
                        break;
                }
            }

            Console.WriteLine("Завершение программы. Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
