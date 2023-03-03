    using SSR.WebAPI.Data;
using SSR.WebAPI.Exceptions;
using SSR.WebAPI.Extensions;
using SSR.WebAPI.Helpers;
using SSR.WebAPI.Interfaces;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using MongoDB.Driver;
using EResultResponse = SSR.WebAPI.Exceptions.EResultResponse;

namespace SSR.WebAPI.Services;

public class ProjectService : BaseService, IProjectService
{
    private DataContext _context;
    private BaseMongoDb<Project, string> BaseMongoDb;
    private IMongoCollection<Project> _collection;

    public ProjectService(DataContext context,
        IHttpContextAccessor contextAccessor)
        : base(context, contextAccessor)
    {
        _context = context;
        BaseMongoDb = new BaseMongoDb<Project, string>(_context.Project);
        _collection = context.Project;
    }

    public async Task<Project> Create(Project model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var code = CommonExtensions.GenerateNewRandomDigit();
        var slug = CommonExtensions.ProgressSlug(model.Slug);
        if (slug == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage("Slug không được để trống!");
        }
        var findSlug = _context.Project.Find(x => x.Slug == slug && x.IsDeleted != true).FirstOrDefault();
        if (findSlug != null)
        {
            throw new ResponseMessageException()
               .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage("Slug đã tồn tại");
        }

        var entity = new Project
        {
            
            Description = model.Description,
            Name = model.Name,
            Group = model.Group,
            Label = model.Label,
            Member = model.Member,
            Slug = model.Slug,
            Files = model.Files,
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

    public async Task<Project> Update(Project model)
    {
        if (model == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_EMPTY);
        }

        var entity = _context.Project.Find(x => x.Id == model.Id).FirstOrDefault();
        if (entity == default)
        {
            throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage(DefaultMessage.DATA_NOT_FOUND);
        }

     var slug = CommonExtensions.ProgressSlug(model.Slug);
    if (slug == default)
    {
      throw new ResponseMessageException()
            .WithCode(EResultResponse.FAIL.ToString())
              .WithMessage("Slug không được để trống!");
        }
       var findSlug = _context.Project.Find(x => x.Id != model.Id && x.Slug == slug && x.IsDeleted != true).FirstOrDefault();
     if (findSlug != null)
        {
           throw new ResponseMessageException()
                .WithCode(EResultResponse.FAIL.ToString())
                .WithMessage("Slug đã tồn tại");
        }

        entity.Description = model.Description;
        entity.Name = model.Name;
        entity.Label = model.Label;
        entity.Group = model.Group;
        entity.Member = model.Member;
        entity.Slug = model.Slug;
        entity.Files = model.Files;
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


        var entity = _context.Project.Find(x => x.Id == id && x.IsDeleted != true).FirstOrDefault();
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

    public async Task<List<Project>> Get()
    {
        return await _context.Project.Find(x => x.IsDeleted != true).SortBy(x => x.ModifiedAt)
            .ToListAsync();
    }

    public async Task<Project> GetById(string id)
    {
        return await _context.Project.Find(x => x.Id == id && x.IsDeleted != true)
            .FirstOrDefaultAsync();
    }

    public async Task<Project> GetBySlug(string slug)
    {
        return await _context.Project.Find(x => x.Slug == slug && x.IsDeleted != true)
            .FirstOrDefaultAsync();
    }

    public async Task<PagingModel<Project>> GetPaging(ProjectParams param)
    {
        PagingModel<Project> result = new PagingModel<Project>();
        var builder = Builders<Project>.Filter;
        var filter = builder.Empty;
        filter = builder.And(filter, builder.Where(x => x.IsDeleted == false));
        //if (!string.IsNullOrEmpty(param.Code))
        //    filter = builder.And(filter, builder.Where(x => x.Category != default && x.Category.Code == param.Code));
        if (!String.IsNullOrEmpty(param.Content))
        {
            filter = builder.And(filter,
                builder.Where(x => x.Name.Trim().ToLower().Contains(param.Content.Trim().ToLower())));
        }
        string sortBy = nameof(Project.ModifiedAt);
        result.TotalRows = await _collection.CountDocumentsAsync(filter);
        result.Data = await _collection.Find(filter)
            .Sort(param.SortDesc
                ? Builders<Project>
                    .Sort.Descending(sortBy)
                : Builders<Project>
                    .Sort.Ascending(sortBy))
            .Skip(param.Skip)
            .Limit(param.Limit)
            .ToListAsync();
        return result;
    }

    
}
