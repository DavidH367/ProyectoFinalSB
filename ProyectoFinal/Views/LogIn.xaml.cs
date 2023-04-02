using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ProyectoFinal.Models;
using ProyectoFinal.Api;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogIn : ContentPage
    {
        Dolar dolar;
       
        public LogIn()
        {
            InitializeComponent();

        }

        protected async override void OnAppearing()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("cargando...", MaskType.Clear);

                await App.DBase.ListaUsuarioSave(await UsuarioApi.GetUsuarioI());

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception error)
            {
                UserDialogs.Instance.HideLoading();
            }

            chkrecordarc.IsChecked = true;

            try
            {
                //PERSISTENCIA obtener
                var persistencia = await App.DBase.obtenerPersistencia(1);
                if (persistencia != null)
                {
                    try
                    {
                        var usuario = await App.DBase.obtenerUsuario(1, persistencia.Campo);
                        txtusuario.Text = usuario.NombreUsuario;
                        txtcontraseña.Text = usuario.Contraseña;
                    }
                    catch (Exception error)
                    {

                    }
                }
            }
            catch (Exception error)
            {

            }

            
        }

       async Task<bool> actualizarDolar()
        {
            UserDialogs.Instance.ShowLoading("Obteniendo datos", MaskType.Clear);
            try
            {
                var listaprecios = await PrecioDolar.GetListaPrecioDolar();
                if (listaprecios.Count == 0) { await DisplayAlert("Aviso", "Ha ocurrido un error con el servidor, vuelva a intentarlo", "OK"); UserDialogs.Instance.HideLoading();  return false; }
                else
                {
                    App.DBase.DolarSave(listaprecios);
                }
            }
            catch (Exception error)
            {
                await DisplayAlert("Aviso", "Ha ocurrido un error con el servidor, vuelva a intentarlo.", "OK");
                return false;
            }

            try
            {
                var date = await UsuarioApi.GetFechaServidor();
                date = date.Substring(0, 10);
                var _dolar = await App.DBase.obtenerPrecioDolar(date);
                dolar = _dolar;
            }
            catch (Exception error)
            {
                await DisplayAlert("Aviso", "Ha ocurrido un error con el servidor, vuelva a intentarlo.", "OK");
                return false;
            }

            UserDialogs.Instance.HideLoading();

            return true;
        }

        private async void goRecuperarContraseña(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RecuperarContraseña());
        }

        private async void goSingUp(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUp());
        }

        private async void btningresar(object sender, EventArgs e)
        {
            if (!await actualizarDolar()) { return; }

            try
            {
                if(txtusuario.Text == null || txtusuario.Text == "") { await DisplayAlert("Aviso", "Ingrese un usuario", "OK"); return; }
            }
            catch(Exception error)
            {
                
            }

            try
            {
                var usuario = await App.DBase.obtenerUsuario(2, txtusuario.Text);

                if (usuario != null)
                {
                    
                        if (usuario.Contraseña == txtcontraseña.Text)
                        {
                            if (chkrecordarc.IsChecked) { persistenciaSUsuario(1, usuario); }
                            else { persistenciaSUsuario(2, usuario); }

                            await DisplayAlert("Aviso", "Bienvenido de vuelta: " + usuario.NombreCompleto, "OK");

                            UserDialogs.Instance.ShowLoading("cargando tus datos...", MaskType.Clear);

                            usuario = await UsuarioApi.GetUsuarioI(usuario);

                            UserDialogs.Instance.HideLoading();

                            await Navigation.PushAsync(new Tablero(usuario, dolar));
                        }
                        else if (usuario.ContraseñaTemporal == txtcontraseña.Text)
                        {
                            bool ciclo = true;
                            while (ciclo)
                            {
                                var promptConfig = new PromptConfig();
                                promptConfig.Title = "Restablecer contraseña";
                                promptConfig.Message = "A continuación ingrese su nueva contraseña:";
                                promptConfig.InputType = InputType.Password;
                                promptConfig.OkText = "Siguiente";
                                promptConfig.CancelText = "Cancelar";
                                promptConfig.MaxLength = 8;
                                //promptConfig.IsCancellable = true;

                                var result = await UserDialogs.Instance.PromptAsync(promptConfig);

                                if (result.Ok)
                                {
                                    var promptConfig2 = new PromptConfig();
                                    promptConfig2.Title = "Restablecer contraseña";
                                    promptConfig2.Message = "A continuación repita su nueva contraseña:";
                                    promptConfig2.InputType = InputType.Password;
                                    promptConfig2.OkText = "Entrar";
                                    promptConfig2.CancelText = "Volver";
                                    promptConfig2.MaxLength = 8;

                                    var result2 = await UserDialogs.Instance.PromptAsync(promptConfig2);

                                    if (result2.Ok)
                                    {
                                        if (result.Text == result2.Text)
                                        {
                                            UserDialogs.Instance.ShowLoading("Actualizando datos", MaskType.Clear);
                                            await App.DBase.UsuarioSave(usuario);
                                            await UsuarioApi.UpdateUsuario(usuario);
                                            UserDialogs.Instance.HideLoading();

                                            await DisplayAlert("Éxito", "Se ha restablecido su contraseña.\n\nUtilice su nueva contraseña la próxima vez que ingrese a la aplicación", "OK");
                                            usuario.Contraseña = result.Text;
                                            usuario.ContraseñaTemporal = "";

                                            ciclo = false;
                                        }
                                        else
                                        {
                                            await DisplayAlert("Aviso", "Las contraseñas no coincidieron, vuelva a intentarlo.", "OK");
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }

                                //string result = await DisplayPromptAsync("Código de Verificación", "A continuación ingrese el código que se envió a su correo electrónico:", "Entrar", "Volver", maxLength: 6);  
                            }

                            if (!ciclo)
                            {
                                if (chkrecordarc.IsChecked) { persistenciaSUsuario(1, usuario); }
                                else { persistenciaSUsuario(2, usuario); }
                                await Navigation.PushAsync(new Tablero(usuario, dolar));
                            }
                        }
                        else
                        {
                            await DisplayAlert("Contraseña Incorrecta", "La contraseña que ha ingresado no coincide con la de su usuario.", "OK");
                        }
                    
                    
                        if (usuario.Contraseña == txtcontraseña.Text)
                        {
                            bool ciclo = true;
                            while (ciclo)
                            {
                                //var promptConfig = new PromptConfig();
                               /* promptConfig.Title = "Código de Verificación";
                                promptConfig.Message = "A continuación ingrese el código que se envió a su correo electrónico:";
                                promptConfig.InputType = InputType.Name;
                                promptConfig.OkText = "Entrar";
                                promptConfig.CancelText = "Volver";
                                promptConfig.MaxLength = 6;
                               */
                                //promptConfig.IsCancellable = true;

                               // var result = await UserDialogs.Instance.PromptAsync(promptConfig);

                                
                                    
                                        usuario.CodigoVerificacion = "";
                                        UserDialogs.Instance.ShowLoading("Actualizando datos", MaskType.Clear);
                                        await App.DBase.UsuarioSave(usuario);
                                        await UsuarioApi.UpdateUsuario(usuario);
                                        UserDialogs.Instance.HideLoading();

                                        //await DisplayAlert("Bienvenido", "Acabas de aperturar tu cuenta exitósamente.\n\nSerás redirigido a tu menú dentro de la aplicación.", "¡Gracias!");
                                        //Aqui pude haber creado una bool bandera pero poniendo ciclo en flase cumplo la funcion del break y de saber que ya pase por aqui
                                       ciclo = false;
                                    
                                
                                //string result = await DisplayPromptAsync("Código de Verificación", "A continuación ingrese el código que se envió a su correo electrónico:", "Entrar", "Volver", maxLength: 6);  
                            }




                            if (!ciclo)
                            {
                                if (chkrecordarc.IsChecked) { persistenciaSUsuario(1, usuario); }
                                else { persistenciaSUsuario(2, usuario); }
                                await Navigation.PushAsync(new Tablero(usuario, dolar));
                            }
                        }
                        else
                        {
                            await DisplayAlert("Contraseña Incorrecta", "La contraseña que ha ingresado no coincide con la de su usuario.", "OK");
                        }
                    
                }
                else
                {
                    await DisplayAlert("Usuario Incorrecto", "El usuario que ha ingresado no existe.", "OK");
                }
            }
            catch (Exception error)
            {
                await DisplayAlert("Aviso del Programador", "" + error, "OK");
            }
        }

        public async void persistenciaSUsuario(int op, Usuario usuario)
        {
            string campo = "";
            if (op == 1) { campo = ""+usuario.Id; }
            if(op == 2) { campo = ""; }

            //PERSISTENCIA insertar
            var persistencia = new Persistencia
            {
                Id = 1,
                Campo = campo
            }; //1 porque es Usuario (ver más en Persistencia.cs)
            var estado = await App.DBase.PersistenciaSave(persistencia);
        }

        private void lblrecuperarc_Tapped(object sender, EventArgs e)
        {
            if (chkrecordarc.IsChecked) { chkrecordarc.IsChecked = false; }
            else { chkrecordarc.IsChecked = true; }
        }
    }
}