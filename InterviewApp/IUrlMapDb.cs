public interface IUrlMapDb
{
    string GetLongUrl(string shortUrl);
    void SaveUrlMapping(string shortUrl, string longUrl);
}
