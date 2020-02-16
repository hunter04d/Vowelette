namespace Vowelette.Abstractions
{
    /// <summary>
    /// Categorizes the input stream into words and separators
    /// </summary>
    public interface IInputTokenizer
    {
        /// <summary>
        /// Signifies the word character
        /// </summary>
        /// <returns>'true' if this character can appear in words, 'false' otherwise</returns>
        bool IsWordCharacter(char ch);

        /// <summary>
        /// Signifies the separator character
        /// </summary>
        /// <returns>'true' if this character is a valid separator, 'false' otherwise</returns>
        bool IsSeparatorCharacter(char ch);
    }
}
