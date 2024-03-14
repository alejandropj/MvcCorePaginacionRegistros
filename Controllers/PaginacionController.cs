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

        public async Task<IActionResult> DepartamentoEmpleado(int? posicion, int iddept)
        {
            ModelDepartamentoEmpleados model = new ModelDepartamentoEmpleados
            {
                Departamento = await this.repo.FindDepartamentoAsync(iddept),

            };
            if (posicion == null)
            {
                ModelDepartamentoEmpleados modelNew = await this.repo.GetGrupoEmpleadoDeptOutAsync(1, iddept);
                modelNew.Departamento = model.Departamento;
                ViewData["REGISTROS"] = modelNew.NumeroRegistros;
                ViewData["IDDEPT"] = iddept;
                return View(modelNew);
            }
            else
            {
                ModelDepartamentoEmpleados modelNew = await this.repo.GetGrupoEmpleadoDeptOutAsync(posicion.Value, iddept);
                modelNew.Departamento = model.Departamento;
                ViewData["REGISTROS"] = modelNew.NumeroRegistros;
                ViewData["IDDEPT"] = iddept;
                return View(modelNew);

            }
        }
/*        [HttpPost]
        public async Task<IActionResult> DepartamentoEmpleado(int iddept)
        {
            ModelDepartamentoEmpleados model = await this.repo.GetGrupoEmpleadoDeptOutAsync(1, iddept);
            model.Departamento = await this.repo.FindDepartamentoAsync(iddept);
            int numeroRegistros = model.NumeroRegistros;
            ViewData["REGISTROS"] = numeroRegistros;
            ViewData["IDDEPT"] = iddept;
            return View(model);
        }*/
        public async Task<IActionResult> PaginarGrupoEmpleados(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;

            }
            int numeroRegistros = await this.repo.GetNumeroEmpleadosAsync();
            List<Empleado> empleados = await this.repo.GetGrupoempleadosAsync(posicion.Value);
            ViewData["REGISTROS"] = numeroRegistros;
            return View(empleados);
        }
        public async Task<IActionResult> EmpleadosOficio(int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(posicion.Value, oficio);
                int registros = await this.repo.GetNumeroEmpleadosOficioAsync(oficio);
                ViewData["REGISTROS"] = registros;
                ViewData["OFICIO"] = oficio;
                return View(empleados);

            }
        }
        [HttpPost]
        public async Task<IActionResult> EmpleadosOficio(string oficio)
        {
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosOficioAsync(1, oficio);
            int numeroRegistros = await this.repo.GetNumeroEmpleadosOficioAsync(oficio);
            ViewData["REGISTROS"] = numeroRegistros;
            ViewData["OFICIO"] = oficio;
            return View(empleados);
        }        
        public async Task<IActionResult> EmpleadosOficioOut(int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                ModelPaginacionEmpleados model = await this.repo.GetGrupoEmpleadosOficioOutAsync(posicion.Value, oficio);
                ViewData["REGISTROS"] = model.NumeroRegistros;
                ViewData["OFICIO"] = oficio;
                return View(model.Empleados);

            }
        }
        [HttpPost]
        public async Task<IActionResult> EmpleadosOficioOut(string oficio)
        {
            ModelPaginacionEmpleados model = await this.repo.GetGrupoEmpleadosOficioOutAsync(1, oficio);
            int numeroRegistros = model.NumeroRegistros;
            ViewData["REGISTROS"] = numeroRegistros;
            ViewData["OFICIO"] = oficio;
            return View(model.Empleados);
        }

        public async Task<IActionResult> PaginarRegistroVistaDept(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;

            }
            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepts();
            int siguiente = posicion.Value + 1;

            if (siguiente > numeroRegistros)
            {
                siguiente = numeroRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
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

        public async Task<IActionResult> EmpleadosDepartamento
    (int? posicion, int iddepartamento)
        {
            if (posicion == null)
            {
                //POSICION PARA EL EMPLEADO
                posicion = 1;
            }
            ModelEmpleadoPaginacion model = await
                this.repo.GetEmpleadoDepartamentoAsync
                (posicion.Value, iddepartamento);
            Departamento departamento =
                await this.repo.FindDepartamentosAsync(iddepartamento);
            ViewData["DEPARTAMENTOSELECCIONADO"] = departamento;
            ViewData["REGISTROS"] = model.Registros;
            ViewData["DEPARTAMENTO"] = iddepartamento;
            int siguiente = posicion.Value + 1;
            //DEBEMOS COMPROBAR QUE NO PASAMOS DEL NUMERO DE REGISTROS
            if (siguiente > model.Registros)
            {
                //EFECTO OPTICO
                siguiente = model.Registros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = model.Registros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["POSICION"] = posicion;
            return View(model.Empleado);
        }        
        public async Task<IActionResult> EmpleadosDepartamentoAjax
        (int? posicion, int iddepartamento)
        {
            if (posicion == null)
            {
                //POSICION PARA EL EMPLEADO
                posicion = 1;
            }
            ModelEmpleadoPaginacion model = await
                this.repo.GetEmpleadoDepartamentoAsync
                (posicion.Value, iddepartamento);
            Departamento departamento =
                await this.repo.FindDepartamentosAsync(iddepartamento);
            ViewData["DEPARTAMENTOSELECCIONADO"] = departamento;
            ViewData["REGISTROS"] = model.Registros;
            ViewData["DEPARTAMENTO"] = iddepartamento;
            int siguiente = posicion.Value + 1;
            //DEBEMOS COMPROBAR QUE NO PASAMOS DEL NUMERO DE REGISTROS
            if (siguiente > model.Registros)
            {
                //EFECTO OPTICO
                siguiente = model.Registros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = model.Registros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["POSICION"] = posicion;
            return View(model.Empleado);
        }        
        public async Task<IActionResult> _PaginacionPartial
        (int? posicion, int iddepartamento)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            ModelEmpleadoPaginacion model = await
                this.repo.GetEmpleadoDepartamentoAsync
                (posicion.Value, iddepartamento);
            Departamento departamento =
                await this.repo.FindDepartamentosAsync(iddepartamento);
            ViewData["DEPARTAMENTOSELECCIONADO"] = departamento;
            ViewData["REGISTROS"] = model.Registros;
            ViewData["DEPARTAMENTO"] = iddepartamento;
            int siguiente = posicion.Value + 1;
            if (siguiente > model.Registros)
            {
                //EFECTO OPTICO
                siguiente = model.Registros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = model.Registros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["POSICION"] = posicion;
            return PartialView("_PaginacionPartial", model.Empleado);
        }
        public async Task<IActionResult> Departamentos()
        {
            List<Departamento> departamentos = await this.repo.GetDepartamentosAsync();
            return View(departamentos);
        }        
        public async Task<IActionResult> DepartamentosAjax()
        {
            List<Departamento> departamentos = await this.repo.GetDepartamentosAsync();
            return View(departamentos);
        }

    }
}
