using Common.Models.Common;
using Common.Models.Projects;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProjectsService
    {
        Task<ListModel<ProjectModel>> Search(PagingSortingModel<SearchModel> searchModel);
        Task<ProjectModel> Get(string id);
        Task<BaseModel> Add(ProjectModel projectModel);
        Task<BaseModel> Edit(ProjectModel projectModel);
        Task<BaseModel> Delete(string id);
    }
}
