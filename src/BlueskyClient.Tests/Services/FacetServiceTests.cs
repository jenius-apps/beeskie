using BlueskyClient.Services;

namespace BlueskyClient.Tests.Services;

public class FacetServiceTests
{
    [Fact]
    public async Task ExtractHashtagsAsync()
    {
        var facetService = new FacetService();

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
}
