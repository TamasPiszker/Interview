using System;

public class UrlShortener
{
    private readonly IUrlMapDb _urlMapDb;
    private readonly Random _random = new Random();
    private const int MaxRetries = 5;

    public UrlShortener(IUrlMapDb urlMapDb)
    {
        _urlMapDb = urlMapDb;
    }

    public string ShortenUrl(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
        {
            throw new ArgumentException("URL cannot be empty or null");
        }

        string shortUrl;
        int retryCount = 0;

        do
        {
            shortUrl = GenerateShortUrl();
            retryCount++;
        } while (_urlMapDb.GetLongUrl(shortUrl) != null && retryCount < MaxRetries);

        if (_urlMapDb.GetLongUrl(shortUrl) != null)
        {
            throw new Exception("Unable to generate a unique short URL after multiple attempts.");
        }

        _urlMapDb.SaveUrlMapping(shortUrl, longUrl);

        return shortUrl;
    }

    public string RetrieveUrl(string shortUrl)
    {
        if (string.IsNullOrWhiteSpace(shortUrl))
        {
            throw new ArgumentException("Short URL cannot be empty or null");
        }

        var longUrl = _urlMapDb.GetLongUrl(shortUrl);

        if (longUrl == null)
        {
            throw new Exception("Short URL does not exist.");
        }

        return longUrl;
    }

    private string GenerateShortUrl()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var shortUrl = new char[7];

        for (int i = 0; i < shortUrl.Length; i++)
        {
            shortUrl[i] = chars[_random.Next(chars.Length)];
        }

        return new string(shortUrl);
    }
}
