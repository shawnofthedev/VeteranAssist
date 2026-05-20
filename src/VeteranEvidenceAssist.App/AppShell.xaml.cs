namespace VeteranEvidenceAssist.App;

public partial class AppShell : Shell
{
    public const string DocumentReviewRoute = "document-review";
    public const string DocumentReviewAbsoluteRoute = $"//{DocumentReviewRoute}";
    public const string DocumentIdQueryKey = "documentId";

    public AppShell()
    {
        InitializeComponent();
    }
}
