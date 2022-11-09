using System;
using System.Text;
using System.Linq;
using static System.Console;
using System.Threading.Tasks;
using System.Collections.Generic;

using SistemaVenta.Entity;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SistemaVenta.BLL.Implementacion
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Categoria> _repositorio;

        public CategoriaService(IGenericRepository<Categoria> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<Categoria>> Lista()
        {
            IQueryable<Categoria> query = await _repositorio.Consultar();
            return query.ToList();
        }
        
        public async Task<Categoria> Crear(Categoria entidad)
        {
            try
            {
                Categoria CategoriaCreada = await _repositorio.Crear(entidad);

                if (CategoriaCreada.IdCategoria == 0) {
                    throw new TaskCanceledException("Error creando el Categoria");
                }

                return CategoriaCreada;                
            }
            catch (System.Exception ex)
            {
                throw;
            }

        }
        
        public async Task<Categoria> Editar(Categoria entidad)
        {
            try
            {
                Categoria CategoriaEditar = await _repositorio.Obtener(c => c.IdCategoria == entidad.IdCategoria);
                CategoriaEditar.Descripcion = entidad.Descripcion;
                CategoriaEditar.EsActivo = entidad.EsActivo;

                bool respuesta = await _repositorio.Editar(CategoriaEditar);
                if (!respuesta) {
                    throw new TaskCanceledException("No se pudo modificar la Categoria");
                }

                return CategoriaEditar;

            }
            catch (System.Exception)
            {
                throw;
            }

        
        }

        public async Task<bool> Eliminar(int idCategoria)
        {
            try
            {
                Categoria CategoriaEncontrada = await _repositorio.Obtener(c => c.IdCategoria == idCategoria);
                if (CategoriaEncontrada == null) {
                    throw new TaskCanceledException("El Categoria no existe");
                }
                bool respuesta = await _repositorio.Eliminar(CategoriaEncontrada);
                return true;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        
    }
}