using System;

class Program
{
    static void Main(string[] args)
    {
        // Create an instance of the UrlMapDb to store URL mappings
        IUrlMapDb urlMapDb = new UrlMapDb();

        // Create an instance of UrlShortener, injecting the UrlMapDb dependency
        UrlShortener urlShortener = new UrlShortener(urlMapDb);

        // Example usage
        Console.WriteLine("Enter a URL to shorten:");
        string longUrl = Console.ReadLine();

        try
        {
            // Shorten the URL
            string shortUrl = urlShortener.ShortenUrl(longUrl);
            Console.WriteLine($"Shortened URL: {shortUrl}");

            // Retrieve the original URL using the shortened URL
            string retrievedUrl = urlShortener.RetrieveUrl(shortUrl);
            Console.WriteLine($"Retrieved URL: {retrievedUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        // Keep the console window open
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
