using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Moq;
using Vowelette.Abstractions;
using Vowelette.Tests.Helpers;
using Xunit;
using Range = Moq.Range;

namespace Vowelette.Tests
{
    public class WordCounterTests
    {
        private const string TestFilePath = "test.txt";

        private readonly WordCounter _sut;

        private readonly MockFileSystem _fs;

        private readonly Mock<WordMatcherDouble> _wordMatcher;

        private readonly Mock<IInputTokenizer> _inputTokenizer;

        public WordCounterTests()
        {
            _fs = new MockFileSystem();
            _wordMatcher = new Mock<WordMatcherDouble>();
            _inputTokenizer = new Mock<IInputTokenizer>();
            _sut = new WordCounter(_fs, _inputTokenizer.Object, _wordMatcher.Object);
        }

        private const string MatchingString = "hello";
        private const string NonMatchingString = "doesnotmatch";

        public static IEnumerable<object[]> CountData()
        {
            yield return new object[] {string.Join(' ', Enumerable.Repeat(MatchingString, 100)), 100, 100};

            yield return new object[] {string.Join(' ', Enumerable.Repeat(NonMatchingString, 100)), 0, 100};

            var sb = new StringBuilder();
            sb.Append(NonMatchingString).Append(' ');
            for (int i = 0; i < 50; i++)
            {
                sb.Append(MatchingString).Append(' ').Append(NonMatchingString).Append(' ');
            }

            sb.Append(MatchingString);

            yield return new object[] {sb.ToString(), 51, 102};
        }

        [Theory]
        [MemberData(nameof(CountData))]
        void Count_ShouldCountProperly(string inputData, int expectedCount, int totalWords)
        {
            _inputTokenizer.Setup(t => t.IsWordCharacter(It.IsInRange('a', 'z', Range.Inclusive))).Returns(true);
            _inputTokenizer.Setup(t => t.IsSeparatorCharacter(' ')).Returns(true);

            _wordMatcher.Setup(t => t.Matches(MatchingString)).Returns(true);

            _fs.AddFile(TestFilePath, new MockFileData(inputData));

            var count = _sut.Count(TestFilePath);

            Assert.Equal(expectedCount, count);
            _wordMatcher.Verify(m => m.Matches(MatchingString), Times.Exactly(expectedCount));
            _wordMatcher.Verify(m => m.Matches(It.IsAny<string>()), Times.Exactly(totalWords));
        }

        [Fact]
        void Count_ShouldThrow_WhenInputFileContainsInvalidChars()
        {
            _inputTokenizer.Setup(t => t.IsWordCharacter(It.IsAny<char>())).Returns(false);
            _inputTokenizer.Setup(t => t.IsSeparatorCharacter(It.IsAny<char>())).Returns(false);
            _fs.AddFile(TestFilePath, new MockFileData("a"));

            Assert.Throws<ArgumentException>(() => _sut.Count(TestFilePath));
        }

        [Fact]
        void Count_ShouldHandleLargeWords()
        {
            var inputData = new string('a', 100_000);
            _inputTokenizer.Setup(t => t.IsWordCharacter('a')).Returns(true);
            _inputTokenizer.Setup(t => t.IsSeparatorCharacter(It.IsAny<char>())).Returns(false);

            _wordMatcher.Setup(m => m.Matches(It.IsAny<string>()))
                .Returns(true);

            _fs.AddFile(TestFilePath, new MockFileData(inputData));


            var count = _sut.Count(TestFilePath);

            Assert.Equal(1, count);
            _wordMatcher.Verify(m => m.Matches(It.IsAny<string>()), Times.Once());
        }
    }
}
