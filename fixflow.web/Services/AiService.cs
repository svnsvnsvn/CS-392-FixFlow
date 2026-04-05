using fixflow.web.Dto;
using System.Net.Http.Json;
using System.Text.Json;

namespace fixflow.web.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AiService> _logger;
        private readonly string _apiKey;
        private readonly string _model;

        public AiService(HttpClient httpClient, IConfiguration config, ILogger<AiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var aiSettings = config.GetSection("AISettings");
            _apiKey = aiSettings["ApiKey"] ?? throw new InvalidOperationException("AISettings:ApiKey is not configured.");
            _model = aiSettings["Model"] ?? "gemini-2.5-flash";
            var baseAddress = aiSettings["BaseAddress"] ?? "https://generativelanguage.googleapis.com/v1beta/";
            _httpClient.BaseAddress = new Uri(baseAddress);
        }

        public async Task<ServiceResult<NoteDto>> GetSummaryOfNotes(List<NoteDto> _accountNotes)
        {
            try
            {
                if (_accountNotes.Count <= 2)
                {
                    ServiceResult<NoteDto>.Fail("More than 2 notes required for summarization.");
                }

                var prompt = string.Join(
                    "\n",
                    _accountNotes.Select(n =>
                        $"{n.TimeStamp}: {n.NoteText}")
                );

                prompt = "Summarize the following ticket notes in 2-3 concise sentences.\r\nDo not list every note individually.\r\nFocus only on meaningful issues, actions taken, changes in status, or recurring problems.\r\nIf the notes contain little useful information, say so briefly.:\r\n" + prompt;

                var requestBody = new
                {
                    contents = new[]
                    {
                            new
                            {
                                parts = new[]
                                {
                                    new { text = prompt }
                                }
                            }
                    }
                };

                var response = await _httpClient.PostAsJsonAsync($"models/{_model}:generateContent?key={_apiKey}", requestBody);

                if (response == null)
                {
                    return ServiceResult<NoteDto>.Fail("No Response from AI service provider.");
                }

                var json = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);

                var summary = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                NoteDto summaryNote = new NoteDto();
                summaryNote.TimeStamp = DateTime.Now;
                summaryNote.TicketId = _accountNotes[0].TicketId;
                summaryNote.InternalOnly = _accountNotes.Any(x => x.InternalOnly);      // If any note was internal, the summary is as well
                summaryNote.NoteText = summary;
                summaryNote.EnteredByUserName = "AI Summary";
                summaryNote.EnteredByUserId = null;

                return ServiceResult<NoteDto>.Ok(summaryNote);

            }
            catch (Exception ex)
            {
                return ServiceResult<NoteDto>.Fail(ex.Message);
            }
        }
    }
}
