using System;
using Vowelette.Abstractions;

namespace Vowelette.Tests.Helpers
{
    /// <summary>
    /// Helper class to use for testing because ref structs cannot be used in Moq
    /// </summary>
    public abstract class WordMatcherDouble : IWordMatcher
    {
        /// <summary>
        /// Delegates the implementation to <see cref="Matches(string)"/>
        /// </summary>
        /// <param name="word">The word to match</param>
        /// <returns>'true' if matches and 'false' otherwise</returns>
        ///
        /// <remarks>
        /// Since it's impossible to use ref structs in expression trees, we delegate <see cref="Matches(string)"/>,
        /// and in Moq we actually use that method
        /// </remarks>
        public bool Matches(ReadOnlySpan<char> word) => Matches(word.ToString());

        public abstract bool Matches(string word);
    }
}
