// Core/Singletons/AppConfiguration.cs
using System;
namespace YetenekPusulasi.Core.Singletons
{
    public sealed class AppConfiguration
    {
        private static readonly Lazy<AppConfiguration> lazy =
            new Lazy<AppConfiguration>(() => new AppConfiguration());
        public static AppConfiguration Instance => lazy.Value;

        public string DefaultLanguage { get; private set; }
        public int MaxLoginAttempts { get; private set; }

        private AppConfiguration()
        {
            // Bu değerler normalde bir config dosyasından okunur
            DefaultLanguage = "tr-TR";
            MaxLoginAttempts = 5;
            Console.WriteLine("AppConfiguration initialized.");
        }
        public string GetWelcomeMessage() => $"Welcome! Default language is {DefaultLanguage}.";
    }
}