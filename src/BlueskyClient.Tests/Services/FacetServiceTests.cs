using BlueskyClient.Services;
using Moq;

namespace BlueskyClient.Tests.Services;

public class FacetServiceTests
{
    [Fact]
    public async Task ExtractHashtagsAsync()
    {
        var facetService = new FacetService(Mock.Of<IProfileService>());

        var results = await facetService.ExtractFacetsAsync("This is a #canucks test #123 #vancouver!!! #thishashtagisinvalidbecauseitpassesthemaximumtaglengthallowedddd", default);

        Assert.NotEmpty(results);
        Assert.Equal(2, results.Count);

        var firstFacet = results[0];
        Assert.Equal("canucks", firstFacet.Features?[0].Tag);
        Assert.Equal(10, firstFacet.Index?.ByteStart ?? -1);
        Assert.Equal(18, firstFacet.Index?.ByteEnd ?? -1);

        var secondFacet = results[1];
        Assert.Equal("vancouver", secondFacet.Features?[0].Tag);
        Assert.Equal(29, secondFacet.Index?.ByteStart ?? -1);
        Assert.Equal(39, secondFacet.Index?.ByteEnd ?? -1);
    }

    [Fact]
    public async Task ExtractLinksAsync()
    {
        var facetService = new FacetService(Mock.Of<IProfileService>());

        var results = await facetService.ExtractFacetsAsync("Ambie is here: https://ambieapp.com!!! Or ambieapp.com. Or (http://ambieapp.com)", default);

        Assert.Equal(3, results.Count);

        var firstFacet = results[0];
        Assert.Equal("https://ambieapp.com", firstFacet.Features?[0].Uri);
        Assert.Equal(15, firstFacet.Index?.ByteStart ?? -1);
        Assert.Equal(35, firstFacet.Index?.ByteEnd ?? -1);

        var secondFacet = results[1];
        Assert.Equal("https://ambieapp.com", secondFacet.Features?[0].Uri);
        Assert.Equal(42, secondFacet.Index?.ByteStart ?? -1);
        Assert.Equal(54, secondFacet.Index?.ByteEnd ?? -1);

        var thirdFacet = results[2];
        Assert.Equal("http://ambieapp.com", thirdFacet.Features?[0].Uri);
        Assert.Equal(60, thirdFacet.Index?.ByteStart ?? -1);
        Assert.Equal(79, thirdFacet.Index?.ByteEnd ?? -1);
    }
}
