using fixflow.web.Dto;

namespace fixflow.web.Services
{
    public interface IAiService
    {
        Task<ServiceResult<NoteDto>> GetSummaryOfNotes(List<NoteDto> _accountNotes);
    }
}
