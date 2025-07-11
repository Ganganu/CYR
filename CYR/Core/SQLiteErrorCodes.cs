using System.ComponentModel;

namespace CYR.Core;

public enum SQLiteErrorCodes
{
    [Description("Der Kunde existiert bereits.")]
    ForeignKeyError = 19
}
