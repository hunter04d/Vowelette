using System;
using System.IO.Abstractions;

namespace Vowelette
{
    public static class Program
    {
        private const string ValidTextFileExtension = ".txt";

        private static void Main()
        {
            Console.WriteLine("Vowelette");
            Console.Write("Input the file: ");

            var input = Console.ReadLine();

            var fs = new FileSystem();
            var validator = new FileInputValidator(fs, ValidTextFileExtension);

            var validationResult = validator.Validate(input);
            if (!validationResult.IsSuccessful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {validationResult.ErrorMessage}");
                return;
            }

            try
            {
                var wordCounter = new WordCounter(fs, new InputTokenizer(), new VowelOnlyWordMatcher());
                var count = wordCounter.Count(input);
                Console.WriteLine($"The file contains {count} words that consist of only vowels");
            }
            catch (ArgumentException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
            }
        }
    }
}
