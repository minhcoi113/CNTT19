using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces;

public interface IIssueService
{
    Task<Issue> Create(Issue model);
    Task<Issue> Update(Issue model);
    Task Delete(string id);
    Task<List<Issue>> Get();
    Task<Issue> GetById(string id);
    Task<PagingModel<Issue>> GetPaging(PagingParam param);
}