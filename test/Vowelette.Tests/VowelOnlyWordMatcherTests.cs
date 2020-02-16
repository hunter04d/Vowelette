using Xunit;

namespace Vowelette.Tests
{
    public class VowelOnlyWordMatcherTests
    {
        public static object[][] Data =
        {
            new object[] {"aoeiu", true},
            new object[] {"aoeiu".ToUpperInvariant(), true},
            new object[] {new string('a', 1000), true},
            new object[] {"hello", false},
            new object[] {"OtHeR wOrD", false},
            new object[] {"\toeui\0", false},
        };

        [Theory]
        [MemberData(nameof(Data))]
        void Matches_ShouldMatchProperly(string input, bool matches)
        {
            var sut = new VowelOnlyWordMatcher();

            Assert.Equal(matches, sut.Matches(input));
        }
    }
}
