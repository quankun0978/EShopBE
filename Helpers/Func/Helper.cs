namespace EShopBE.Helpers.Func;
public static class Helper
{
    // Xử lý tính trung bình cộng
    public static long CalculateAverage(long[] numbers)
    {
        if (numbers.Length == 0)
        {
            return 0;
        }

        double sum = 0;

        foreach (double number in numbers)
        {
            sum += number;
        }
        return (long)(sum / numbers.Length);
    }
}
