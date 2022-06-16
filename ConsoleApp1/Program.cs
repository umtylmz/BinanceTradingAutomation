//Console.WriteLine($"Main Thread Started");
//Task<double> task1 = Task.Run(() =>
//{
//    double sum = 0;
//    for (int count = 1; count <= 10; count++)
//    {
//        sum += count;
//    }
//    return sum;
//});

//Console.WriteLine($"Sum is: {task1.Result}");
//Console.WriteLine($"Main Thread Completed");
//Console.ReadKey();




int counter = 0;
Console.WriteLine($"Main Thread Started");
Task<double> task1 = Task.Run(() =>
{
    counter++;
    return CalculateSum(10);
});
Console.WriteLine($"Sum is: {task1.Result}");
task1.Start();
Console.WriteLine($"Main Thread Completed");
Console.ReadKey();

static double CalculateSum(int num)
{
    double sum = 0;
    for (int count = 1; count <= num; count++)
    {
        sum += count;
    }
    return sum;
}