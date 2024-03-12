using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Diagnostics.Metrics;
using Microsoft.Win32;
using System.Data;

namespace MvcCorePaginacionRegistros.Repositories
{

    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }
        #region Procedimiento Out
      /*ALTER PROCEDURE SP_GRUPO_EMPLEADOS_OFICIO_OUT
        (@POSICION INT, @OFICIO NVARCHAR(50)
        ,@REGISTROS INT OUT)
        AS
            SELECT @REGISTROS = COUNT(EMP_NO) FROM EMP

            WHERE OFICIO = @OFICIO


            SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM
                (
                SELECT CAST(ROW_NUMBER() OVER (ORDER BY APELLIDO) AS INT) AS POSICION,
                EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP

                WHERE OFICIO = @OFICIO) AS QUERY

            WHERE QUERY.POSICION >= @POSICION AND QUERY.POSICION<(@POSICION+2)
        GO*/
        #endregion
        public async Task<ModelPaginacionEmpleados> GetGrupoEmpleadosOficioOutAsync
            (int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO_OUT @POSICION, @OFICIO, @REGISTROS OUT";
            SqlParameter pamPosicion = new SqlParameter("@POSICION", posicion);
            SqlParameter pamOficio = new SqlParameter("@OFICIO", oficio);
            SqlParameter pamRegistros = new SqlParameter("@REGISTROS", -1);
            pamRegistros.Direction = System.Data.ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw
                (sql, pamPosicion, pamOficio, pamRegistros);
            List<Empleado> empleados = await consulta.ToListAsync();
            int registros = (int)pamRegistros.Value;
            return new ModelPaginacionEmpleados
            {
                NumeroRegistros = registros,
                Empleados = empleados
            };
        }

        #region PROCEDIMIENTO Y VISTA EMPLEADOS
        /*    CREATE VIEW V_GRUPO_EMPLEADOS
        AS

            SELECT CAST(ROW_NUMBER() OVER (ORDER BY APELLIDO) AS INT) AS POSICION,
            ISNULL(EMP_NO, 0) AS EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP
        GO

        CREATE PROCEDURE SP_GRUPO_EMPLEADOS
        (@POSICION INT)
        AS
            SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
            FROM V_GRUPO_EMPLEADOS
            WHERE POSICION >= @POSICION AND POSICION<(@POSICION +3)
        GO

        EXEC SP_GRUPO_EMPLEADOS 4*/
        #endregion
        public async Task<int> GetNumeroEmpleadosAsync()
        {
            return await this.context.Empleados.CountAsync();
        }
        public async Task<List<Empleado>> GetGrupoempleadosAsync(int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS @POSICION";
            SqlParameter pamPosicion = new SqlParameter("@POSICION", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        #region PROCEDIMIENTO FILTER_EMPLEADOS
        /*ALTER PROCEDURE SP_GRUPO_EMPLEADOS_OFICIO
          (@POSICION INT, @OFICIO NVARCHAR(50))
          AS
              SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM
                  (
                  SELECT CAST(ROW_NUMBER() OVER (ORDER BY APELLIDO) AS INT) AS POSICION,
                  EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP

                  WHERE OFICIO = @OFICIO) AS QUERY

              WHERE QUERY.POSICION >= @POSICION AND QUERY.POSICION<(@POSICION+2)
          GO

          EXEC SP_GRUPO_EMPLEADOS_OFICIO 1, 'EMPLEADO'*/
        #endregion
        public async Task<int> GetNumeroEmpleadosOficioAsync(string oficio)
        {
            return await this.context.Empleados
                .Where(z => z.Oficio == oficio).CountAsync();
        }
        public async Task<List<Empleado>> GetGrupoEmpleadosOficioAsync
            (int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @POSICION, @OFICIO";
            SqlParameter pamPosicion = new SqlParameter("@POSICION", posicion);
            SqlParameter pamOficio = new SqlParameter("@OFICIO", oficio);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio);
            return await consulta.ToListAsync();
        }


        #region Vista DEPT
        /*        CREATE VIEW V_DEPARTAMENTOS_INDIVIDUAL
        AS

            SELECT CAST(
            ROW_NUMBER() OVER (ORDER BY DEPT_NO) AS INT) AS POSICION,
            ISNULL(DEPT_NO, 0) AS DEPT_NO, DNOMBRE, LOC FROM DEPT
        GO

        SELECT* FROM V_DEPARTAMENTOS_INDIVIDUAL WHERE POSICION=1*/
        #endregion
        public async Task<int> GetNumeroRegistrosVistaDepts()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }

        public async Task<VistaDepartamento> GetVistaDeptAsync(int pos)
        {
            VistaDepartamento vista = await this.context.VistaDepartamentos.Where(z => z.Posicion == pos).FirstOrDefaultAsync();
            return vista;

        }

        #region Procedimiento DEPT
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
        public async Task<Departamento> FindDepartamentoAsync(int idDept)
        {
            return await this.context.Departamentos.FirstOrDefaultAsync(x => x.IdDepartamento == idDept);
        }
        public async Task<ModelDepartamentoEmpleados> GetGrupoEmpleadoDeptOutAsync(int posicion, int idDept)
        {
            string sql = "SP_GRUPO_EMPLEADO_DEPT_OUT @POSICION, @IDDEPT, @REGISTROS OUT";
            SqlParameter pamPosicion = new SqlParameter("@POSICION", posicion);
            SqlParameter pamId = new SqlParameter("@IDDEPT", idDept);
            SqlParameter pamRegistros = new SqlParameter("@REGISTROS", -1);
            pamRegistros.Direction = System.Data.ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw
                (sql, pamPosicion, pamId, pamRegistros);
            Empleado empleado = consulta.AsEnumerable().FirstOrDefault();
            int registros = (int)pamRegistros.Value;
            return new ModelDepartamentoEmpleados
            {
                NumeroRegistros = registros,
                Empleado = empleado
            };
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
        public async Task<Departamento> FindDepartamentosAsync(int id)
        {
            return await this.context.Departamentos
                .FirstOrDefaultAsync(x => x.IdDepartamento == id);
        }

        #region PROCEDIMIENTO PROFE
        /*        create procedure SP_REGISTRO_EMPLEADO_DEPARTAMENTO
        (@posicion int, @departamento int
        , @registros int out)
        as
        select @registros = count(EMP_NO) from EMP
        where DEPT_NO = @departamento
        select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO from
            (select cast(
            ROW_NUMBER() OVER (ORDER BY APELLIDO) as int) AS POSICION
            , EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
            from EMP
            where DEPT_NO = @departamento) as QUERY
            where QUERY.POSICION = @posicion
        go*/
        #endregion
        public async Task<ModelEmpleadoPaginacion>
           GetEmpleadoDepartamentoAsync
           (int posicion, int iddepartamento)
        {
            string sql = "SP_REGISTRO_EMPLEADO_DEPARTAMENTO @posicion, @departamento, "
                + " @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamDepartamento =
                new SqlParameter("@departamento", iddepartamento);
            SqlParameter pamRegistros = new SqlParameter("@registros", -1);
            pamRegistros.Direction = ParameterDirection.Output;
            var consulta =
                this.context.Empleados.FromSqlRaw
                (sql, pamPosicion, pamDepartamento, pamRegistros);
            //PRIMERO DEBEMOS EJECUTAR LA CONSULTA PARA PODER RECUPERAR 
            //LOS PARAMETROS DE SALIDA
            var datos = await consulta.ToListAsync();
            Empleado empleado = datos.FirstOrDefault();
            int registros = (int)pamRegistros.Value;
            return new ModelEmpleadoPaginacion
            {
                Registros = registros,
                Empleado = empleado
            };
        }


    }
}
