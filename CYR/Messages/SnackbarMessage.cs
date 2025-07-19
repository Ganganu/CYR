using System.Windows.Media;

namespace CYR.Messages;

public record SnackbarMessage(string Message, string Icon, ImageSource? ImageSource = null);
