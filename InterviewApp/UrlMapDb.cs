using System.Collections.Generic;

public class UrlMapDb : IUrlMapDb
{
    private readonly Dictionary<string, string> _urlMappings = new Dictionary<string, string>();

    public string GetLongUrl(string shortUrl)
    {
        return _urlMappings.ContainsKey(shortUrl) ? _urlMappings[shortUrl] : null;
    }

    public void SaveUrlMapping(string shortUrl, string longUrl)
    {
        if (!_urlMappings.ContainsKey(shortUrl))
        {
            _urlMappings.Add(shortUrl, longUrl);
        }
    }
}