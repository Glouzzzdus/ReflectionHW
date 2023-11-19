using HWReflection;
using Microsoft.Extensions.Configuration;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MyConfigurationClass myConfig = new MyConfigurationClass();
            ConfigurationComponentBase configComponent = new ConfigurationComponentBase();


            configComponent.LoadSettings(ProviderType.File, myConfig);

            Console.WriteLine("=== Before changes ===");
            Console.WriteLine("Setting1: " + myConfig.Setting1);
            Console.WriteLine("Setting2: " + myConfig.Setting2);
            Console.WriteLine("Setting3: " + myConfig.Setting3);
            Console.WriteLine("Setting4: " + myConfig.Setting4);
            Console.WriteLine("Setting5: " + myConfig.Setting5);

            configComponent.LoadSettings(ProviderType.ConfigurationManager, myConfig);

            Console.WriteLine("Setting6: " + myConfig.Setting6);
            Console.WriteLine("Setting7: " + myConfig.Setting7);
            Console.WriteLine("Setting8: " + myConfig.Setting8);
            Console.WriteLine("Setting9: " + myConfig.Setting9);
            Console.WriteLine("Setting10: " + myConfig.Setting10);

            myConfig.Setting1 = "New Message";
            myConfig.Setting2 = 456;
            myConfig.Setting3 = 77.77;
            myConfig.Setting4 = TimeSpan.FromSeconds(10);
            myConfig.Setting5 = new DateTime(2025, 12, 31);

            configComponent.SaveSettings(ProviderType.File, myConfig);

            myConfig.Setting6 = "New Message 1";
            myConfig.Setting7 = 789;
            myConfig.Setting8 = 88.88;
            myConfig.Setting9 = TimeSpan.FromSeconds(40);
            myConfig.Setting10 = new DateTime(1999, 01, 01);

            configComponent.SaveSettings(ProviderType.File, myConfig);

            configComponent.LoadSettings(ProviderType.ConfigurationManager, myConfig);

            Console.WriteLine("=== After changes ===");
            Console.WriteLine("Setting1: " + myConfig.Setting1);
            Console.WriteLine("Setting2: " + myConfig.Setting2);
            Console.WriteLine("Setting3: " + myConfig.Setting3);
            Console.WriteLine("Setting4: " + myConfig.Setting4);
            Console.WriteLine("Setting5: " + myConfig.Setting5);

            configComponent.LoadSettings(ProviderType.ConfigurationManager, myConfig);

            Console.WriteLine("Setting6: " + myConfig.Setting6);
            Console.WriteLine("Setting7: " + myConfig.Setting7);
            Console.WriteLine("Setting8: " + myConfig.Setting8);
            Console.WriteLine("Setting9: " + myConfig.Setting9);
            Console.WriteLine("Setting10: " + myConfig.Setting10);

            Console.ReadLine();
        }
    }
}