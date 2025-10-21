partial class Program
{
     static void Main(string[] args)
    {
        Welcome2283();
        Welcome8244();

    }

     static void Welcome2283()
    {
        Console.Write("Enter your name:");
        string name = Console.ReadLine();
        Console.WriteLine("{0}, welcome to my first console application", name);
    }

    static partial void Welcome8244();
}
