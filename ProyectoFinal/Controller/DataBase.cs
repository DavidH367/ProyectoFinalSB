using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProyectoFinal.Models;
using SQLite;

namespace ProyectoFinal.Controller
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection dbase;
        /* Constructor de clase */
        public DataBase(string dbpath)
        {
            dbase = new SQLiteAsyncConnection(dbpath);

            // Crearemos las tablas de la base de datos
            dbase.CreateTableAsync<Usuario>(); // Creando la tabla de Usuarios
            dbase.CreateTableAsync<Persistencia>(); // Creando la tabla de Persistencia
            dbase.CreateTableAsync<Cuenta>(); // Creando la tabla de Cuentas (dinero)
            dbase.CreateTableAsync<Transferencia>(); // Creando la tabla de Transferencias
            dbase.CreateTableAsync<Cliente>(); // Creando la tabla de Cliente
            dbase.CreateTableAsync<Dolar>(); // Creando la tabla de Dolares (registro de precios del dolar)
        }

        #region Usuario
        // CRUD - Create - Read - Update - Delete
        // Create
        public Task<int> UsuarioSave(Usuario usuario)
        {
            if (usuario.Id != 0)
            {
                return dbase.UpdateAsync(usuario); // Update
            }
            else
            {
                return dbase.InsertAsync(usuario);
            }
        }

        public async Task<int> ListaUsuarioSave(List<Usuario> usuario)
        {
            for (int i = 0; i < usuario.Count; i++)
            {
                var user = await obtenerUsuario(2, usuario[i].NombreUsuario);

                if(user == null) { await dbase.InsertAsync(usuario[i]); }
                else { await dbase.UpdateAsync(usuario[i]); }
            }

            return 1;
        }




        // Read
        public Task<List<Usuario>> obtenerListaUsuario()
        {
            return dbase.Table<Usuario>().ToListAsync();
        }

        // Read un registro
        public Task<Usuario> obtenerUsuario(int operacion, string dato)
        {
            //1 Obtener usuario por ID
            //2 Obtener usuario por nombre de usuario
            //3 Obtener usuario por correo electrónico
            //4 obtener usuario por id de cliente
            //5 obtener usuario por numero de identidad
            switch (operacion)
            {
                case 1:
                    int usuarioid = int.Parse(dato);
                    return dbase.Table<Usuario>()
                    .Where(i => i.Id == usuarioid)
                    .FirstOrDefaultAsync();
                case 2:
                    return dbase.Table<Usuario>()
                    .Where(i => i.NombreUsuario == dato)
                    .FirstOrDefaultAsync();
                case 3:
                    return dbase.Table<Usuario>()
                    .Where(i => i.Email == dato)
                    .FirstOrDefaultAsync();
                case 4:
                    return dbase.Table<Usuario>()
                    .Where(i => i.IdCliente == dato)
                    .FirstOrDefaultAsync();
                case 5:
                    return dbase.Table<Usuario>()
                    .Where(i => i.NumeroIdentidad == dato)
                    .FirstOrDefaultAsync();
            }

            return null;
        }

        // Delete
        public Task<int> UsuarioDelete(Usuario usuario)
        {
            return dbase.DeleteAsync(usuario);
        }

        //Delete ALL
        public Task<int> UsuarioDeleteAll()
        {
            return dbase.DropTableAsync<Usuario>();
        }

        #endregion

        #region Persistencia
        public async Task<int> PersistenciaSave(Persistencia persistencia)
        {
            var registro = await obtenerPersistencia(persistencia.Id);

            if (registro != null) {  return await dbase.UpdateAsync(persistencia); }

            return await dbase.InsertAsync(persistencia);
        }

        // Read
        public Task<List<Persistencia>> obtenerListaPersistencia()
        {
            return dbase.Table<Persistencia>().ToListAsync();
        }

        // Read V2
        public Task<Persistencia> obtenerPersistencia(int pid)
        {
            return dbase.Table<Persistencia>()
                .Where(i => i.Id == pid)
                .FirstOrDefaultAsync();
        }

        // Delete
        public Task<int> PersistenciaDelete(Persistencia persistencia)
        {
            return dbase.DeleteAsync(persistencia);
        }

        //Delete ALL
        public Task<int> PersistenciaDeleteAll()
        {
            return dbase.DropTableAsync<Persistencia>();
        }
        #endregion

        #region Cuenta
        public async Task<int> CuentaSave(int operacion, Cuenta cuenta)
        {
            //1 Save
            //2 Update

            //RETURNS
            //2 ya existe una cuenta con ese codigo de cuenta
            //3 el usuario ya tiene 2 cuentas, no puede crear más

            switch (operacion)
            {
                case 1:
                    if(await obtenerCuenta(cuenta.CodigoCuenta) == null)
                    {
                        /*List<Cuenta> cuentas = new List<Cuenta>();
                        cuentas = await obtenerCuentasUsuario(cuenta.CodigoUsuario);

                        if(cuentas.Count >= 2)
                        {
                            return 3;
                        }
                        else
                        {
                            return await dbase.InsertAsync(cuenta);
                        }*/

                        return await dbase.InsertAsync(cuenta);
                    }
                    else
                    {
                        return 2;
                    }
                case 2:
                    return await dbase.UpdateAsync(cuenta); // Update
            }

            return 0;
        }

        // Read
        public Task<List<Cuenta>> obtenerListaCuenta()
        {
            return dbase.Table<Cuenta>().ToListAsync();
        }

        // Read un registro
        public Task<Cuenta> obtenerCuenta(string ccuenta)
        {
            return dbase.Table<Cuenta>()
                    .Where(i => i.CodigoCuenta == ccuenta)
                    .FirstOrDefaultAsync();
        }

        // Trae las cuentas ligadas a un Id de Usuario
        public Task<List<Cuenta>> obtenerCuentasUsuario(string nidentidad)
        {
            return dbase.Table<Cuenta>()
                    .Where(i => i.CodigoUsuario == nidentidad)
                    .ToListAsync();
        }

        public async Task<int> ListaCuentasSave(List<Cuenta> cuentas)
        {
            for (int i = 0; i < cuentas.Count; i++)
            {
                var cuenta = await obtenerCuenta(cuentas[i].CodigoCuenta);

                if (cuenta == null) { await dbase.InsertAsync(cuentas[i]); }
                else { await dbase.UpdateAsync(cuentas[i]); }
            }

            return 1;
        }

        // Delete
        public Task<int> CuentaDelete(Cuenta cuenta)
        {
            return dbase.DeleteAsync(cuenta);
        }

        //Delete ALL
        public Task<int> CuentaDeleteAll()
        {
            return dbase.DropTableAsync<Cuenta>();
        }

        #endregion

        #region Transferencia
        public Task<int> TransferenciaSave(Transferencia transferencia)
        {
            return dbase.InsertAsync(transferencia);
        }

        public async Task<int> ListaTransferenciaSave(List<Transferencia> transferencias)
        {
            for (int i = 0; i < transferencias.Count; i++)
            {
                var transferencia = await obtenerTransferencia(transferencias[i].Id);

                if (transferencia == null) { await dbase.InsertAsync(transferencias[i]); }
                else { await dbase.UpdateAsync(transferencias[i]); }
            }


            return 1;
        }

        // Read un registro
        public Task<Transferencia> obtenerTransferencia(int tid)
        {
            return dbase.Table<Transferencia>()
                    .Where(i => i.Id == tid)
                    .FirstOrDefaultAsync();
        }

        // Trae las transferencias ligadas a un Codigo de Cuenta
        public Task<List<Transferencia>> obtenerTransferenciasCuenta(int op, string codigocuenta)
        {
            //1 Todos
            //2 Créditos
            //3 Débitos

            switch (op)
            {
                case 1:
                    return dbase.Table<Transferencia>()
                .Where(i => i.Envia == codigocuenta || i.Recibe == codigocuenta)
                .ToListAsync();
                case 2:
                    return dbase.Table<Transferencia>()
                .Where(i => i.Recibe == codigocuenta)
                .ToListAsync();
                case 3:
                    return dbase.Table<Transferencia>()
                .Where(i => i.Envia == codigocuenta)
                .ToListAsync();
            }

            return null;
        }

        // Trae las transferencias ligadas a una lista con Codigos de Cuenta
        public async Task<List<Transferencia>> obtenerTransferenciasCuentas(int op, List<string> codigoscuenta)
        {
            //1 Todos
            //2 Créditos
            //3 Débitos

            List<Transferencia> transferencias = new List<Transferencia>();

            for (int j = 0; j < codigoscuenta.Count; j++)
            {
                transferencias.AddRange(await obtenerTransferenciasCuenta(op, codigoscuenta[j]));
            }

            return transferencias;
        }

        #endregion

        #region Cliente
        public Task<int> ClienteSave(Cliente cliente)
        {
            return dbase.InsertAsync(cliente);
        }

        // Read un registro
        public Task<Cliente> obtenerCliente(int cid)
        {
            return dbase.Table<Cliente>()
                    .Where(i => i.IdUsuario == cid)
                    .FirstOrDefaultAsync();
        }

        #endregion

        #region Dolar
        public Task<int> DolarSave(Dolar precio)
        {
            return dbase.InsertAsync(precio);
        }
        
        public void DolarSave(List<Dolar> listaprecios)
        {
            for (int i = 0; i < listaprecios.Count; i++)
            {
                dbase.InsertAsync(listaprecios[i]);
            }
        }

        public Task<Dolar> obtenerPrecioDolar(string fecha)
        {
            return dbase.Table<Dolar>()
                .Where(i => i.Fecha == fecha)
                .FirstOrDefaultAsync();
        }

        public Task<List<Dolar>> obtenerListaPrecioDolar()
        {
            return dbase.Table<Dolar>().ToListAsync();
        }
        #endregion
    }
}