namespace PrimeKare.Api.Tests;

public class UnitTest1
{
    [Fact]
    public void Addition_ShouldReturnCorrectResult()
    {
        int firstNum = 10;
        int secondNum = 20;

        int result = firstNum + secondNum;

        Assert.Equal(30, result);
    }
}
