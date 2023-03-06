using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Exceptions.EResultResponse;
using Microsoft.VisualBasic;

namespace SSR.WebAPI.Services;

public class IssueService : BaseService, IIssueService
{
    private DataContext _context;
    private BaseMongoDb<Issue, string> BaseMongoDb;
    private IMongoCollection<Issue> _collection;

    public IssueService(DataContext context,
        IHttpContextAccessor contextAccessor)
        : base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Issue, string>(_context.Issue);
        _collection = context.Issue;
    }

    public async Task<Issue> Create(Issue model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var code = CommonExtensions.GenerateNewRandomDigit();
        
        var entity = new Issue
        {
            
            Description = model.Description,
            Title = model.Title,
            Assignee = model.Assignee,
            DonVi = model.DonVi,
            ProjectId = model.ProjectId,
            DueDate = model.DueDate,
            Label = model.Label,
            Files = model.Files,
            StepId = model.StepId,
        };

        var result = await BaseMongoDb.CreateAsync(entity);
        if (result.Entity.Id == default || !result.Success)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.CREATE_FAILURE);
        }

        return entity;
    }

    public async Task<Issue> Update(Issue model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var entity = _context.Issue.Find(x => x.Id == model.Id).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }

        entity.Description = model.Description;
        entity.Title = model.Title;
        entity.Assignee = model.Assignee;
        entity.DonVi = model.DonVi;
        entity.ProjectId = model.ProjectId;
        entity.DueDate = model.DueDate;
        entity.Label = model.Label;
        entity.DueDate = model.DueDate;
        entity.StepId = model.StepId;
        entity.ModifiedAt = DateTime.Now;
        entity.ModifiedBy = CurrentUserName;

        var result = await BaseMongoDb.UpdateAsync(entity);
        if (!result.Success)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.UPDATE_FAILURE);
        }

        return entity;
    }

    public async Task Delete(string id)
    {
        if (id == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }


        var entity = _context.Issue.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }

        entity.ModifiedAt = DateTime.Now;
        entity.ModifiedBy = CurrentUserName;
        entity.IsDeleted = true;
        var result = await BaseMongoDb.DeleteAsync(entity);

        if (!result.Success)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DELETE_FAILURE);
        }
    }

    public async Task<List<Issue>> Get()
    {
        return await _context.Issue.Find(x => x.IsDeleted != true).SortBy(x => x.ModifiedAt)
            .ToListAsync();
    }

    public async Task<Issue> GetById(string id)
    {
        return await _context.Issue.Find(x => x.Id == id && x.IsDeleted != true)
            .FirstOrDefaultAsync();
    }


    public async Task<PagingModel<Issue>> GetPaging(PagingParam param)
    {
        PagingModel<Issue> result = new PagingModel<Issue>();
        var builder = Builders<Issue>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        //if (!string.IsNullOrEmpty(param.Code))
        //    filter = builder.And(filter, builder.Where(x => x.Category != default && x.Category.Code == param.Code));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Title.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }
        string sortBy = nameof(Issue.ModifiedAt);
        result.TotalRows = await _collection.CountDocumentsAsync(filter);
        result.Data = await _collection.Find(filter)
            .Sort(param.SortDesc
                ? Builders<Issue>
                    .Sort.Descending(sortBy)
                : Builders<Issue>
                    .Sort.Ascending(sortBy))
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }

    
}
