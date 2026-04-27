
namespace QuranHub.Web.Controllers;

[ApiController]
public class QuranController : ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private IQuranRepository _quranRepository;
    private UserManager<QuranHubUser> _userManager;
    private HttpContext _httpContext;
    private QuranHubUser _currentUser;

    public QuranController(
        Serilog.ILogger logger,
        IQuranRepository quranRepository,
        UserManager<QuranHubUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _quranRepository = quranRepository ?? throw new ArgumentNullException(nameof(quranRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));

        ClaimsPrincipal claimsPrincipal = this._httpContext.User;

        if (claimsPrincipal.Identity.IsAuthenticated)
        {
            _currentUser = _userManager.GetUserAsync(this._httpContext.User).Result;
        }
    }

    [HttpGet(Router.Quran.QuranInfo)]
    [ResponseCache(Duration = 24 * 60 * 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] {"type"} )]
    public ActionResult<IEnumerable<object>> GetQuranInfo(string type)
    {
        try
        {
            _logger.Information("Getting Quran info with type: {type}", type);
            return Ok(_quranRepository.GetQuranInfo(type));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return BadRequest();
        }
    }

    [HttpGet(Router.Quran.MindMap)]
    [ResponseCache(Duration = 24 * 60 * 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new string[] { "id" })]
    public async Task<ActionResult<byte[]>> GetMindMap(long id)
    {
        try
        {
            return Ok(await _quranRepository.GetMindMap(id));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return BadRequest();
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet(Router.Quran.Note)]
    public async Task<ActionResult<Note>> GetNote(long index)
    {
        try
        {
            return Ok(await _quranRepository.GetNote(index, _currentUser));
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return BadRequest();
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost(Router.Quran.CreateNote)]
    public async Task<ActionResult> CreateNote([FromBody] AddNoteRequestModel note)
    {
        try
        {
            if (await _quranRepository.AddNote(note.NoteId, note.Index, note.Sura, note.Aya, note.Text, _currentUser))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return BadRequest();
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet(Router.Quran.Groups)]
    public async Task<ActionResult<List<GroupsResponseModel>>> GetGroups()
    {
        try
        {
            var result = await _quranRepository.GetGroups(_currentUser);

            var groupsResponseModelList = new List<GroupsResponseModel>();

            foreach (var group in result)
            {
                List<VerseResponseModel> verseResponseModels = new List<VerseResponseModel>();

                foreach (var verse in group.Verses)
                {
                    verseResponseModels.Add(new VerseResponseModel()
                    {
                        Index = verse.Index,
                        Aya = verse.Aya,
                        Text = verse.Text,
                        Sura = verse.Sura,
                        VerseId = verse.VerseId

                    });
                }
                var groupResponseMode = new GroupsResponseModel()
                {
                    Id = group.GroupId,
                    Name = group.Name,
                    Verses = verseResponseModels
                };

                groupsResponseModelList.Add(groupResponseMode);
            }

            return Ok(groupsResponseModelList);



        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return BadRequest();
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost(Router.Quran.AddGroup)]
    public async Task<ActionResult> AddGroup([FromBody] AddGroupRequestModel group)
    {
        try
        {
            if (await _quranRepository.AddGroup(group.name, group.versesId, _currentUser))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return BadRequest();
        }
    }
    [HttpDelete(Router.Quran.DeleteGroup)]
    public async Task<ActionResult> DeleteGroup(int groupId)
    {
        try
        {
            if (await _quranRepository.DeleteGroup(groupId))
            {
                return Ok(true);
            }
            else
            {
                return BadRequest();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return BadRequest();
        }
    }

}
