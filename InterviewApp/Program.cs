using System;

class Program
{
    static void Main(string[] args)
    {
        IUrlMapDb urlMapDb = new UrlMapDb();

        UrlShortener urlShortener = new UrlShortener(urlMapDb);

        Console.WriteLine("Enter a URL to shorten:");
        string longUrl = Console.ReadLine();

        try
        {
            string shortUrl = urlShortener.ShortenUrl(longUrl);
            Console.WriteLine($"Shortened URL: {shortUrl}");

            string retrievedUrl = urlShortener.RetrieveUrl(shortUrl);
            Console.WriteLine($"Retrieved URL: {retrievedUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
