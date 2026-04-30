using Mavrynt.Modules.Notifications.Application.Services;
using Xunit;

namespace Mavrynt.Modules.Notifications.Application.Tests;

public sealed class EmailTemplateRendererTests
{
    private readonly EmailTemplateRenderer _renderer = new();

    [Fact]
    public void Render_Should_Replace_All_Known_Placeholders()
    {
        var placeholders = new Dictionary<string, string>
        {
            ["UserEmail"] = "john@example.com",
            ["DisplayName"] = "John",
        };

        var result = _renderer.Render(
            "Hello {{DisplayName}}",
            "<p>Dear {{DisplayName}}, your email is {{UserEmail}}</p>",
            "Dear {{DisplayName}}, your email is {{UserEmail}}",
            placeholders);

        Assert.True(result.IsSuccess);
        Assert.Equal("Hello John", result.Value.Subject);
        Assert.Contains("John", result.Value.HtmlBody);
        Assert.Contains("john@example.com", result.Value.HtmlBody);
        Assert.Contains("John", result.Value.TextBody);
    }

    [Fact]
    public void Render_Should_HtmlEncode_Values_In_Html_Body()
    {
        var placeholders = new Dictionary<string, string>
        {
            ["UserEmail"] = "<script>alert('xss')</script>",
        };

        var result = _renderer.Render(
            "Subject",
            "<p>{{UserEmail}}</p>",
            null,
            placeholders);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain("<script>", result.Value.HtmlBody);
        Assert.Contains("&lt;script&gt;", result.Value.HtmlBody);
    }

    [Fact]
    public void Render_Should_Fail_For_Unknown_Placeholder_In_Subject()
    {
        var placeholders = new Dictionary<string, string> { ["KnownKey"] = "value" };

        var result = _renderer.Render(
            "Hello {{UnknownKey}}",
            "<p>{{KnownKey}}</p>",
            null,
            placeholders);

        Assert.True(result.IsFailure);
        Assert.Equal("Notifications.Email.UnknownPlaceholder", result.Error.Code);
        Assert.Contains("UnknownKey", result.Error.Message);
    }

    [Fact]
    public void Render_Should_Fail_For_Unknown_Placeholder_In_Html_Body()
    {
        var placeholders = new Dictionary<string, string> { ["KnownKey"] = "value" };

        var result = _renderer.Render(
            "{{KnownKey}}",
            "<p>{{UnknownHtmlKey}}</p>",
            null,
            placeholders);

        Assert.True(result.IsFailure);
        Assert.Contains("UnknownHtmlKey", result.Error.Message);
    }

    [Fact]
    public void Render_Should_Fail_For_Unknown_Placeholder_In_Text_Body()
    {
        var placeholders = new Dictionary<string, string> { ["KnownKey"] = "value" };

        var result = _renderer.Render(
            "{{KnownKey}}",
            "<p>{{KnownKey}}</p>",
            "{{UnknownTextKey}}",
            placeholders);

        Assert.True(result.IsFailure);
        Assert.Contains("UnknownTextKey", result.Error.Message);
    }

    [Fact]
    public void Render_Should_Return_Null_TextBody_When_Template_Is_Null()
    {
        var placeholders = new Dictionary<string, string> { ["Name"] = "Alice" };

        var result = _renderer.Render("{{Name}}", "<p>{{Name}}</p>", null, placeholders);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.TextBody);
    }

    [Fact]
    public void Render_Should_Handle_No_Placeholders()
    {
        var result = _renderer.Render(
            "Hello World",
            "<p>Static HTML</p>",
            null,
            new Dictionary<string, string>());

        Assert.True(result.IsSuccess);
        Assert.Equal("Hello World", result.Value.Subject);
    }

    [Fact]
    public void Render_Should_Not_HtmlEncode_Values_In_Subject()
    {
        var placeholders = new Dictionary<string, string>
        {
            ["Name"] = "John & Jane",
        };

        var result = _renderer.Render("Hello {{Name}}", "<p>{{Name}}</p>", null, placeholders);

        Assert.True(result.IsSuccess);
        Assert.Equal("Hello John & Jane", result.Value.Subject);
        Assert.Contains("John &amp; Jane", result.Value.HtmlBody);
    }
}
