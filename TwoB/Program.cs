namespace TwoB
{
    public class Program
    {
        // Lets keep Program and our Main method clean and instead do all our logic in another class.
        static void Main(string[] args) => new Bot().Start().GetAwaiter().GetResult();
    }
}