using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;

namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<int> GetNumeroRegistrosVistaDepts()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }

        public async Task<VistaDepartamento> GetVistaDeptAsync(int pos)
        {
            VistaDepartamento vista = await this.context.VistaDepartamentos.Where(z => z.Posicion == pos).FirstOrDefaultAsync();
            return vista;

        }
        #region procedimientos almacenados
/*        CREATE PROCEDURE SP_GRUPO_DEPARTAMENTOS
        (@POSICION INT)
AS
        SELECT DEPT_NO, DNOMBRE, LOC
    FROM V_DEPARTAMENTOS_INDIVIDUAL
    WHERE POSICION >= @POSICION AND POSICION<(@POSICION + 2)
GO

EXEC SP_GRUPO_DEPARTAMENTOS 1*/
        #endregion
        public async Task<List<Departamento>> GetGrupoDeptAsync(int pos)
        {
            string sql = "SP_GRUPO_DEPARTAMENTOS @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", pos);
            var consulta = this.context.Departamentos.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();

        }
        public async Task<List<VistaDepartamento>> GetGrupoVistaDeptAsync(int pos)
        {
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= pos
                           && datos.Posicion < (pos + 2)
                           select datos;
            return await consulta.ToListAsync();

        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            return await this.context.Departamentos.ToListAsync();
        }
        public async Task<List<Empleado>> GetEmpleadosDepartamentoAsync(int idDepartamento)
        {
            var empleados = this.context.Empleados.Where(x=> x.IdDepartamento == idDepartamento);
            if (empleados.Count() == 0)
            {
                return null;
            }
            else
            {
                return await empleados.ToListAsync();
            }
        }

    }
}
