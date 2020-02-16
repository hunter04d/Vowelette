using System;
using System.IO.Abstractions;
using System.Text;
using Vowelette.Abstractions;

namespace Vowelette
{
    /// <summary>
    /// Counts the number of words that satisfy the given <see cref="IWordMatcher"/> from an input file
    /// <para>
    /// The definition of word and separator is provided by <see cref="IInputTokenizer"/>
    /// </para>
    /// </summary>
    ///
    /// <remarks>
    /// The implementation uses stack allocated <see cref="Span{T}"/> to avoid allocations
    /// </remarks>
    public class WordCounter
    {
        private readonly IFileSystem _fs;

        private readonly IInputTokenizer _tokenizer;

        private readonly IWordMatcher _wordMatcher;

        private long _count;

        // the size of the buffer where the file is written to
        private const int BufferSize = 1024;

        // this buffer is for large words (larger than BufferSize) or words that happen to land on a block boundary
        private readonly StringBuilder _allocatedBuffer = new StringBuilder();

        /// <summary>
        /// Create a new word counter
        /// </summary>
        /// <param name="fs">the filesystem</param>
        /// <param name="tokenizer">
        /// a concrete <see cref="IInputTokenizer"/> that decides what characters
        /// are allowed in words and what characters are separators
        /// </param>
        /// <param name="wordMatcher"> <see cref="IWordMatcher"/> that matches the words</param>
        public WordCounter(IFileSystem fs, IInputTokenizer tokenizer, IWordMatcher wordMatcher)
        {
            _fs = fs;
            _tokenizer = tokenizer;
            _wordMatcher = wordMatcher;
        }

        /// <summary>
        /// Count the number of word in the input file
        /// </summary>
        /// <param name="input">the input file to count</param>
        /// <returns>the number of words in the input file that satisfies the <see cref="IWordMatcher.Matches"/> provided in the constructor</returns>
        /// <exception cref="ArgumentException">when the input file contains invalid tokens</exception>
        public long Count(string input)
        {
            _count = 0;

            // using stack allocated buffer to prevent allocations when words are small
            Span<char> stackBuffer = stackalloc char[BufferSize];
            using var reader = _fs.File.OpenText(input);
            int read;
            while ((read = reader.ReadBlock(stackBuffer)) != 0)
            {
                int lastWordStart = 0;
                int lastWordLength = 0;
                for (var i = 0; i < read; i++)
                {
                    var currentChar = stackBuffer[i];
                    if (_tokenizer.IsWordCharacter(currentChar))
                    {
                        if (lastWordLength == 0)
                        {
                            // start a new word
                            lastWordStart = i;
                        }

                        lastWordLength++;
                    }
                    else if (_tokenizer.IsSeparatorCharacter(currentChar))
                    {
                        MatchWord(stackBuffer.Slice(lastWordStart, lastWordLength));

                        lastWordLength = 0;
                    }
                    else
                    {
                        throw new ArgumentException($"input file contains invalid character: '{currentChar}'");
                    }
                }

                if (lastWordLength != 0)
                {
                    // we have a word on a block boundary
                    // have to allocate it, so that we can match it whole later
                    _allocatedBuffer.Append(stackBuffer.Slice(lastWordStart, lastWordLength));
                }
            }

            // word at the end of the file
            MatchWord();

            return _count;
        }

        /// <summary>
        /// Matches the input word
        /// <para>If allocated buffer is empty then match as is</para>
        /// <para>If allocated buffer is *not* empty then add it to the buffer and match the whole word</para>
        /// </summary>
        /// <param name="word">The word to match</param>
        private void MatchWord(ReadOnlySpan<char> word = default)
        {
            if (_allocatedBuffer.Length != 0)
            {
                // use the word from the allocated buffer
                _allocatedBuffer.Append(word);
                word = _allocatedBuffer.ToString();

                _allocatedBuffer.Clear();
            }

            if (!word.IsEmpty && _wordMatcher.Matches(word))
            {
                _count++;
            }
        }
    }
}
