using CYR.Core;
using CYR.User;

namespace CYR.Invoice.UseCases;

public class DuplicateInvoiceUseCase(IDatabaseConnection databaseConnection, UserContext userContext)
{
    public async Task<bool> DuplicateInvoice(string newInvoiceNumber, string originalInvoiceNumber)
    {
		try
		{
			await databaseConnection.ExecuteTransactionAsync(async (transaction) =>
			{
                //insert invoice                
                string insertInvoice = @"INSERT INTO Rechnungen (Rechnungsnummer,Kundennummer,Rechnungsdatum,Fälligkeitsdatum, 
                Nettobetrag, Bruttobetrag,Status, commentstop, commentsbottom, user_id) 
				select @newInvoiceNumber,Kundennummer,CURRENT_DATE,CURRENT_DATE, 
                Nettobetrag, Bruttobetrag,Status, commentstop, commentsbottom, user_id from Rechnungen 
                where user_id = @user_id and Rechnungsnummer = @originalInvoiceNumber";
                var insertInvoiceParams = new Dictionary<string, object>
                {
                    { "@newInvoiceNumber", newInvoiceNumber },
                    { "@originalInvoiceNumber", originalInvoiceNumber },
                    { "@user_id", userContext.CurrentUser.Id }
                };

                await databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, insertInvoice,insertInvoiceParams);

                //insert positions
                string insertInvoicePositions = @"INSERT INTO Rechnungspositionen (Rechnungsnummer,Beschreibung,Menge, 
                Einheit, Einheitspreis, user_id) 
                select @newInvoiceNumber, Beschreibung, Menge, Einheit, Einheitspreis,user_id from Rechnungspositionen 
                where user_id = @user_id and Rechnungsnummer = @originalInvoiceNumber";
                var insertInvoicePositionParams = new Dictionary<string, object>
                {
                    { "@newInvoiceNumber", newInvoiceNumber },
                    { "@originalInvoiceNumber", originalInvoiceNumber },
                    { "@user_id", userContext.CurrentUser.Id }
                };
                await databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, insertInvoicePositions, insertInvoicePositionParams);
            });
		}
		catch (Exception)
		{
            return false;
		}
        return true;
    }
}
