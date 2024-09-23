using Moq;
using Xunit;

public class UrlShortenerTests
{
    private readonly UrlShortener _urlShortener;
    private readonly Mock<IUrlMapDb> _mockUrlMapDb;

    public UrlShortenerTests()
    {
        _mockUrlMapDb = new Mock<IUrlMapDb>();
        _urlShortener = new UrlShortener(_mockUrlMapDb.Object);
    }

    [Fact]
    public void ShortenUrl_ValidUrl_ReturnsShortUrl()
    {
        // Arrange
        var longUrl = "https://www.example.com/asdfghjjkljk";
        _mockUrlMapDb.Setup(db => db.GetLongUrl(It.IsAny<string>())).Returns((string)null); // No collision

        // Act
        var shortUrl = _urlShortener.ShortenUrl(longUrl);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(shortUrl));
        _mockUrlMapDb.Verify(db => db.SaveUrlMapping(shortUrl, longUrl), Times.Once);
    }

    [Fact]
    public void RetrieveUrl_ValidShortUrl_ReturnsLongUrl()
    {
        // Arrange
        var shortUrl = "http://short.url/abc123";
        var longUrl = "https://www.example.com/asdfghjjkljk";
        _mockUrlMapDb.Setup(db => db.GetLongUrl(shortUrl)).Returns(longUrl);

        // Act
        var result = _urlShortener.RetrieveUrl(shortUrl);

        // Assert
        Assert.Equal(longUrl, result);
    }

    [Fact]
    public void RetrieveUrl_InvalidShortUrl_ThrowsException()
    {
        // Arrange
        var shortUrl = "http://short.url/invalid";
        _mockUrlMapDb.Setup(db => db.GetLongUrl(shortUrl)).Returns((string)null);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _urlShortener.RetrieveUrl(shortUrl));
        Assert.Equal("Short URL does not exist.", ex.Message);
    }

    [Fact]
    public void ShortenUrl_UrlAlreadyExistsInDb_GeneratesNewShortUrl()
    {
        // Arrange
        var longUrl = "https://www.example.com/asdfghjjkljk";
        var existingShortUrl = "http://short.url/abc123";

        // First short URL already exists
        _mockUrlMapDb.SetupSequence(db => db.GetLongUrl(It.IsAny<string>()))
            .Returns(longUrl)    // Collision on first attempt
            .Returns((string)null); // No collision on second attempt

        // Act
        var shortUrl = _urlShortener.ShortenUrl(longUrl);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(shortUrl));
        Assert.NotEqual(existingShortUrl, shortUrl);
        _mockUrlMapDb.Verify(db => db.SaveUrlMapping(shortUrl, longUrl), Times.Once);
    }

    [Fact]
    public void ShortenUrl_MaxRetriesExceeded_ThrowsException()
    {
        // Arrange
        var longUrl = "https://www.example.com/asdfghjjkljk";

        // Simulate collision for all attempts
        _mockUrlMapDb.Setup(db => db.GetLongUrl(It.IsAny<string>())).Returns(longUrl);

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _urlShortener.ShortenUrl(longUrl));
        Assert.Equal("Unable to generate a unique short URL after multiple attempts.", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public void ShortenUrl_InvalidUrl_ThrowsArgumentException(string invalidUrl)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _urlShortener.ShortenUrl(invalidUrl));
        Assert.Equal("Invalid URL", ex.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public void RetrieveUrl_InvalidShortUrl_ThrowsArgumentException(string invalidShortUrl)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => _urlShortener.RetrieveUrl(invalidShortUrl));
        Assert.Equal("Invalid short URL", ex.Message);
    }
}