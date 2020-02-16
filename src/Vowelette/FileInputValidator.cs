using System;
using System.IO.Abstractions;
using static Vowelette.ValidationResult;

namespace Vowelette
{
    public class FileInputValidator
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _validExtension;

        /// <summary>
        /// Creates a new FileInputValidator
        /// </summary>
        /// <param name="fileSystem">The filesystem to use for validation</param>
        /// <param name="validExtension">Valid extension for files that will be validated</param>
        /// <remarks>
        /// <remarks>Note that <paramref name="validExtension"/> should start with the '.'</remarks>
        /// </remarks>
        public FileInputValidator(IFileSystem fileSystem, string validExtension)
        {
            _fileSystem = fileSystem;
            _validExtension = validExtension;
        }

        /// <summary>
        /// Validates the input string as a file that can be consumed by this program
        /// </summary>
        /// <param name="input">The input string to validate</param>
        /// <returns>
        /// <see cref="ValidationResult"/> that will be in <see cref="ValidationResult.Success"/> state
        /// if input string is valid
        /// and otherwise <see cref="ValidationResult.Failure"/> with the error message describing the failure
        /// </returns>
        public ValidationResult Validate(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return ReportEmptyInput();
            }

            IFileInfo fileInfo;
            try
            {
                // `FromFileName` will throw if input is not a valid path
                // this behavior is OS dependant
                fileInfo = _fileSystem.FileInfo.FromFileName(input);
            }
            catch (Exception e)
            {
                return Failure(e.Message);
            }

            if (!fileInfo.Exists)
            {
                return ReportFileDoesNotExist(fileInfo);
            }

            if (_validExtension != fileInfo.Extension)
            {
                return ReportFileExtensionInvalid(fileInfo);
            }

            return Success;
        }

        private static ValidationResult ReportEmptyInput() => Failure("the input was an empty!");

        private static ValidationResult ReportFileDoesNotExist(IFileInfo file) =>
            Failure($"File {file.FullName} does not exist");

        private ValidationResult ReportFileExtensionInvalid(IFileInfo file) =>
            Failure($"{file.FullName} does not have a valid extension. Valid extension is \"{_validExtension}\"");
    }
}
