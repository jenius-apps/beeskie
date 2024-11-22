namespace BlueskyClient.Constants;

public sealed class TelemetryConstants
{
    private const string Error = "error:";
    public const string ApiError = Error + "apiError";

    private const string App = "app:";
    public const string Launched = App + "launched";

    private const string SignInPage = "signInPage:";
    public const string AuthSuccessFromSignInPage = SignInPage + "authSuccessful";
    public const string AuthFailFromSignInPage = SignInPage + "authFail";
    public const string AppPasswordHelpClicked = SignInPage + "appPasswordHelpClicked";
    public const string SignInClicked = SignInPage + "signInClicked";

    private const string ShellPage = "shellPage:";
    public const string AuthSuccessFromShellPage = ShellPage + "authSuccessful";
    public const string AuthFailFromShellPage = ShellPage + "authFail";
    public const string MenuItemClicked = ShellPage + "menuItemClicked";
    public const string ShellNewPostClicked = ShellPage + "newPostClicked";
    public const string ImageViewerOpened = ShellPage + "imageViewerOpened";
    public const string ImageViewerClosed = ShellPage + "imageViewerClosed";
    public const string FeedbackClicked = ShellPage + "feedbackClicked";

    private const string NewPostDialog = "newPostDialog:";
    public const string NewPostSubmitted = NewPostDialog + "newPostSubmitted";
    public const string ReplySubmitted = NewPostDialog + "replySubmitted";
}
