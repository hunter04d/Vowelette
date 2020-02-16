using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Vowelette.Tests
{
    public class FileInputValidatorTests
    {
        private readonly MockFileSystem _fs;

        private readonly FileInputValidator _sut;

        public FileInputValidatorTests()
        {
            _fs = new MockFileSystem();
            _sut = new FileInputValidator(_fs, ".txt");
        }

        public static readonly object[][] ValidFilesData =
        {
            new object[] {@"input.txt"},
            new object[] {@"/var/lib/file.txt"},
            new object[] {@"c:\file.txt"},
            new object[] {@"d:\directory\file.txt"},
            new object[] {@"c:\directory\..\file.txt"},
            new object[] {@"dir/input.txt"},
            new object[] {@"dir\input.txt"},
            new object[] {@"../../../input.txt"},
            new object[] {@"..\..\..\input.txt"},
        };

        [Theory]
        [MemberData(nameof(ValidFilesData))]
        void Validate_ShouldValidateValidFiles_WhenAllTheInvariantsAreHeld(string input)
        {
            _fs.AddFile(input, MockFileData.NullObject);

            var result = _sut.Validate(input);

            Assert.True(result.IsSuccessful, $"Validation failed {result.ErrorMessage}");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\n\n\n")]
        [InlineData("      ")]
        [InlineData("\t\r\n")]
        void Validate_ShouldFail_WhenInputIsEmptyOrWhiteSpace(string? input)
        {
            var result = _sut.Validate(input);

            Assert.False(result.IsSuccessful);
        }

        [Theory]
        [MemberData(nameof(ValidFilesData))]
        void Validate_ShouldFail_WhenFileDoesNotExist(string? input)
        {
            var result = _sut.Validate(input);

            Assert.False(result.IsSuccessful);
        }

        [Theory]
        [InlineData("input.cs")]
        [InlineData("input.java")]
        [InlineData("input")]
        void Validate_ShouldFail_WhenExtensionDoesNotMatch(string? input)
        {
            _fs.AddFile(input, MockFileData.NullObject);

            var result = _sut.Validate(input);

            Assert.False(result.IsSuccessful);
        }
    }
}
