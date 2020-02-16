using System;

namespace Vowelette.Abstractions
{
    /// <summary>
    /// Checks if input word satisfies the given condition
    /// </summary>
    public interface IWordMatcher
    {
        /// <summary>
        /// Matches the given <paramref name="word"/>
        /// <param name="word">The word to match</param>
        /// <returns>'true' is the input word matches, and 'false' if it does not match</returns>
        /// </summary>
        bool Matches(ReadOnlySpan<char> word);
    }
}
