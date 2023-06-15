using System;
using System.Data;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Services
{
    public interface IHistoryNotesService
    {
        Task InsertNotes(HistoryNotes history);
    }

    public class HistoryNotesService : IHistoryNotesService
    {
        private readonly IDbFacade facade;

        public HistoryNotesService(IDbFacade fa)
        {
            facade = fa ?? throw new ArgumentNullException(nameof(fa)); 
        }

        public async Task InsertNotes(HistoryNotes h)
        {
            var values = new
            {
                TransID = h.TransID,
                ColumnName = h.ColumnName,
                OldValue = h.OldValue,
                NewValue = h.NewValue,
                ChangedBy = h.ChangedBy,
                ChangedOn = h.ChangedOn,
                Notes = h.Notes
            };
            await facade.QueryProcAsync<HistoryNotes>("loan.AppTransCreate", values);
        }
    }
}
