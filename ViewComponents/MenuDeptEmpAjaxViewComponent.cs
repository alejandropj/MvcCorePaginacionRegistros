using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.ViewComponents
{
    public class MenuDeptEmpAjaxViewComponent:ViewComponent
    {
        private RepositoryHospital repo;
        public MenuDeptEmpAjaxViewComponent(RepositoryHospital repo)
        {
            this.repo = repo;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Departamento> depts = await this.repo.GetDepartamentosAsync();
            return View(depts);
        }
    }
}
