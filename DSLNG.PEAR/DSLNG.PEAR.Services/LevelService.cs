using AutoMapper;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Level;
using DSLNG.PEAR.Services.Responses.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class LevelService : BaseService, ILevelService
    {
        public LevelService(IDataContext dataContext) : base(dataContext)
        {

        }
        public GetLevelResponse GetLevel(GetLevelRequest request)
        {
            var level = DataContext.Levels.First(x => x.Id == request.Id);
            var response = Mapper.Map<GetLevelResponse>(level);

            return response;
        }


        public GetLevelsResponse GetLevels(GetLevelsRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetLevelsResponse
            {
                TotalRecords = totalRecords,
                Levels = data.ToList().MapTo<GetLevelsResponse.Level>()
            };

            //var levels = DataContext.Levels.ToList();
            //var response = new GetLevelsResponse();
            //response.Levels = levels.MapTo<GetLevelsResponse.Level>();
            ////response.Levels = levels.MapTo<GetLevelResponse>();

            //return response;
        }

        public CreateLevelResponse Create(CreateLevelRequest request)
        {
            var response = new CreateLevelResponse();
            try {
                var level = request.MapTo<Level>();
                DataContext.Levels.Add(level);
                DataContext.SaveChanges();
                response.IsSuccess = true; 
                response.Message = "Level item has been added successfully";
            }
            catch (DbUpdateException dbUpdateException) {
                response.IsSuccess = false;
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        public UpdateLevelResponse Update(UpdateLevelRequest request)
        {
            var response = new UpdateLevelResponse();
            try {
                var _level = request.MapTo<Level>();
                DataContext.Levels.Attach(_level);
                DataContext.Entry(_level).State = EntityState.Modified;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Level item has been updated successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.IsSuccess = false;
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        public DeleteLevelResponse Delete(int id)
        {
            var response = new DeleteLevelResponse();
            try {
                var _level = new Level { Id = id};
                DataContext.Levels.Attach(_level);
                DataContext.Entry(_level).State = EntityState.Deleted;
                DataContext.SaveChanges();
                response.IsSuccess = true;
                response.Message = "Level item has been Deleted successfully";
            }
            catch (DbUpdateException dbUpdateException)
            {
                response.IsSuccess = false;
                response.Message = dbUpdateException.Message;
            }
            return response;
        }

        public IEnumerable<Level> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Levels.AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Code.Contains(search) || x.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Code":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Code).ThenBy(x => x.Number)
                            : data.OrderByDescending(x => x.Code).ThenBy(x => x.Number);
                        break;
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name).ThenBy(x => x.Number)
                            : data.OrderByDescending(x => x.Name).ThenBy(x => x.Number);
                        break;
                    case "Number":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Number)
                            : data.OrderByDescending(x => x.Number);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.IsActive);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }

    }
}
