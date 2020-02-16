using System.Linq;
using Vowelette.Abstractions;

namespace Vowelette
{
    /// <summary>
    /// Input tokenizer for the program
    /// <para>
    /// Word characters are english alphabet (lower and upper) case.
    /// </para>
    /// <para>
    /// Separators are space and newline characters
    /// </para>
    /// </summary>
    public class InputTokenizer : IInputTokenizer
    {
        public bool IsWordCharacter(char ch) => ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z';

        private static readonly char[] ValidSeparators = {' ', '\r', '\n'};

        public bool IsSeparatorCharacter(char ch) => ValidSeparators.Contains(ch);
    }
}
