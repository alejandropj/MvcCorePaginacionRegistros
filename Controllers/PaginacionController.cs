using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;
        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }
        #region Vista
/*        CREATE VIEW V_DEPARTAMENTOS_INDIVIDUAL
AS

    SELECT CAST(
    ROW_NUMBER() OVER (ORDER BY DEPT_NO) AS INT) AS POSICION,
    ISNULL(DEPT_NO, 0) AS DEPT_NO, DNOMBRE, LOC FROM DEPT
GO

SELECT* FROM V_DEPARTAMENTOS_INDIVIDUAL WHERE POSICION=1*/
        #endregion
        public async Task<IActionResult> PaginarRegistroVistaDept(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;

            }
            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepts();
            int siguiente = posicion.Value + 1;

            if(siguiente > numeroRegistros)
            {
                siguiente = numeroRegistros;
            }
            int anterior = posicion.Value - 1;
            if(anterior < 1)
            {
                anterior = 1;
            }
            VistaDepartamento vista = await this.repo.GetVistaDeptAsync(posicion.Value);
            ViewData["ULTIMO"] = numeroRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;

            return View(vista);
        }        
        public async Task<IActionResult> PaginarGrupoVistaDept(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;

            }
            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepts();
            ViewData["REGISTROS"] = numeroRegistros;
            List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDeptAsync(posicion.Value);
            return View(departamentos);
        }              
        public async Task<IActionResult> PaginarGrupoDept(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;

            }
            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepts();
            ViewData["REGISTROS"] = numeroRegistros;
            List<Departamento> departamentos = await this.repo.GetGrupoDeptAsync(posicion.Value);
            return View(departamentos);
        }        
/*        public async Task<IActionResult> PaginarGrupoVistaDept(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;

            }
            int numeroPagina = 1;
            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepts();
            int siguiente = posicion.Value + 1;
            string html = "<div>";
            for(int i =1; i <=numeroRegistros; i += 2)
            {
                html += "<a href='PaginarGrupoVistaDept?posicion=" 
                    + i + "'>Página " + numeroPagina + "</a>";
                numeroPagina++;
            }
            html += "</div>";
            ViewData["LINKS"] = html;
            List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDeptAsync(posicion.Value);
            return View(departamentos);
        }*/
    }
}
