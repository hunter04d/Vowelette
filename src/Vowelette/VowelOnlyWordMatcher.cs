using System;
using System.Linq;
using Vowelette.Abstractions;

namespace Vowelette
{
    /// <summary>
    /// Matches words that are made of vowels only
    /// </summary>
    public class VowelOnlyWordMatcher : IWordMatcher
    {
        private static readonly char[] EnglishVowels = {'a', 'o', 'e', 'i', 'u'};

        public bool Matches(ReadOnlySpan<char> word)
        {
            // sadly linq does not work on spans as you would have to store it on the heap
            // we have to iterate manually
            foreach (var ch in word)
            {
                // char.ToLowerInvariant is slow, so we do a manual bit flip
                if (!EnglishVowels.Contains((char) (ch | 0x20))) return false;
            }

            return true;
        }
    }
}
