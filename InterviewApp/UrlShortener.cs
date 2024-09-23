using System;
using System.Text.RegularExpressions;

public class UrlShortener
{
    private readonly IUrlMapDb _urlMapDb;
    private readonly Random _random = new Random();
    private readonly string _baseUrl = "http://short.url/";
    private const int MaxRetries = 5;

    public UrlShortener(IUrlMapDb urlMapDb)
    {
        _urlMapDb = urlMapDb;
    }

    public string ShortenUrl(string longUrl)
    {
        if (string.IsNullOrWhiteSpace(longUrl) || !IsValidUrl(longUrl))
        {
            throw new ArgumentException("Invalid URL");
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
        if (string.IsNullOrWhiteSpace(shortUrl) || !shortUrl.StartsWith(_baseUrl))
        {
            throw new ArgumentException("Invalid short URL");
        }

        var longUrl = _urlMapDb.GetLongUrl(shortUrl);

        if (longUrl == null)
        {
            throw new Exception("Short URL does not exist.");
        }

        return longUrl;
    }

    private bool IsValidUrl(string url)
    {
        var regex = new Regex(@"^(http|https)://");
        return regex.IsMatch(url);
    }

    private string GenerateShortUrl()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var shortUrl = new char[7];

        for (int i = 0; i < shortUrl.Length; i++)
        {
            shortUrl[i] = chars[_random.Next(chars.Length)];
        }

        return _baseUrl + new string(shortUrl);
    }
}
