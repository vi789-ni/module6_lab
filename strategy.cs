using System;

namespace ShippingStrategyExample
{
    public interface IShippingStrategy
    {
        decimal CalculateShippingCost(decimal weight, decimal distance);
        string Name { get; }
    }

    public class StandardShippingStrategy : IShippingStrategy
    {
        public string Name => "Стандартная доставка";
        public decimal CalculateShippingCost(decimal weight, decimal distance)
        {
            return weight * 0.5m + distance * 0.1m;
        }
    }

    public class ExpressShippingStrategy : IShippingStrategy
    {
        public string Name => "Экспресс-доставка";
        public decimal CalculateShippingCost(decimal weight, decimal distance)
        {
            return (weight * 0.75m + distance * 0.2m) + 10m; 
        }
    }

    public class InternationalShippingStrategy : IShippingStrategy
    {
        public string Name => "Международная доставка";
        public decimal CalculateShippingCost(decimal weight, decimal distance)
        {
            return weight * 1.0m + distance * 0.5m + 15m; 
        }
    }

    public class NightShippingStrategy : IShippingStrategy
    {
        public string Name => "Ночная доставка";
        private decimal _nightFee = 7.5m;

        public decimal CalculateShippingCost(decimal weight, decimal distance)
        {
            decimal baseCost = weight * 0.5m + distance * 0.1m;
            return baseCost + _nightFee;
        }
    }

    public class DeliveryContext
    {
        private IShippingStrategy _shippingStrategy;
        public void SetShippingStrategy(IShippingStrategy strategy)
        {
            _shippingStrategy = strategy;
        }

        public decimal CalculateCost(decimal weight, decimal distance)
        {
            if (_shippingStrategy == null)
                throw new InvalidOperationException("Стратегия доставки не установлена.");
            return _shippingStrategy.CalculateShippingCost(weight, distance);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DeliveryContext context = new DeliveryContext();

            Console.WriteLine(" Система расчёта стоимости доставки ");

            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1 - Выбрать стратегию доставки");
                Console.WriteLine("2 - Рассчитать стоимость по текущей стратегии");
                Console.WriteLine("3 - Показать доступные стратегии");
                Console.WriteLine("0 - Выход");
                Console.Write("Ваш выбор: ");
                string mainChoice = Console.ReadLine();

                if (mainChoice == "0") break;

                switch (mainChoice)
                {
                    case "1":
                        ChooseStrategy(context);
                        break;
                    case "2":
                        Calculate(context);
                        break;
                    case "3":
                        ShowStrategies();
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор, попробуйте снова.");
                        break;
                }
            }

            Console.WriteLine("Выход. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        static void ShowStrategies()
        {
            Console.WriteLine("\nДоступные стратегии:");
            Console.WriteLine("1 - Стандартная");
            Console.WriteLine("2 - Экспресс");
            Console.WriteLine("3 - Международная");
            Console.WriteLine("4 - Ночная (новая)");
        }

        static void ChooseStrategy(DeliveryContext context)
        {
            ShowStrategies();
            Console.Write("Введите номер стратегии: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    context.SetShippingStrategy(new StandardShippingStrategy());
                    Console.WriteLine("Установлена стратегия: Стандартная");
                    break;
                case "2":
                    context.SetShippingStrategy(new ExpressShippingStrategy());
                    Console.WriteLine("Установлена стратегия: Экспресс");
                    break;
                case "3":
                    context.SetShippingStrategy(new InternationalShippingStrategy());
                    Console.WriteLine("Установлена стратегия: Международная");
                    break;
                case "4":
                    context.SetShippingStrategy(new NightShippingStrategy());
                    Console.WriteLine("Установлена стратегия: Ночная");
                    break;
                default:
                    Console.WriteLine("Неверный выбор стратегии.");
                    break;
            }
        }

        static void Calculate(DeliveryContext context)
        {
            try
            {
                Console.Write("Введите вес посылки (кг): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal weight) || weight < 0)
                {
                    Console.WriteLine("Ошибка: вес должен быть числом >= 0.");
                    return;
                }

                Console.Write("Введите расстояние доставки (км): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal distance) || distance < 0)
                {
                    Console.WriteLine("Ошибка: расстояние должно быть числом >= 0.");
                    return;
                }

                decimal cost = context.CalculateCost(weight, distance);
                Console.WriteLine($"Стоимость доставки: {cost:F2}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неожиданная ошибка: {ex.Message}");
            }
        }
    }
}
