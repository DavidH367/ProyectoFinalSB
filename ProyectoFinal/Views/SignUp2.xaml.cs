using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Net.Mail;

using ProyectoFinal.Models;
using ProyectoFinal.Api;

using System.ComponentModel.DataAnnotations;
using Acr.UserDialogs;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUp2 : ContentPage
    {
        Usuario usuariocompleto;

        public SignUp2(Usuario usuario)
        {
            InitializeComponent();

            //Se llena el objeto usuario con los datos de la anterior pagina
           /* usuariocompleto.Fotografia = usuario.Fotografia;
            usuariocompleto.NombreCompleto = usuario.NombreCompleto;
            usuariocompleto.FechaNacimiento = usuario.FechaNacimiento;
            usuariocompleto.Sexo = usuario.Sexo;
            usuariocompleto.Direccion = usuario.Direccion;
           */
            usuariocompleto = usuario;
        }

        protected override async void OnAppearing()
        {
            try
            {
                await App.DBase.ListaUsuarioSave(await UsuarioApi.GetUsuarioI());
            }
            catch (Exception error)
            {

            }
        }

        private async void btnregistrarme(object sender, EventArgs e)
        {

            try
            {
                if (txtnumeroidentidad.Text == null || txtnumeroidentidad.Text == "")
                {
                    await DisplayAlert("Aviso", "Su número de identidad es requerido para poder aperturar su cuenta de usuario", "OK"); return;
                } else if (await App.DBase.obtenerUsuario(5, txtnumeroidentidad.Text) != null) {
                    await DisplayAlert("Aviso", "Su número de identidad pertenece a otra cuenta de usuario", "OK"); return;
                } else if (txtnumeroidentidad.Text.Length < 13)
                {
                    await DisplayAlert("Aviso", "El número de identidad no está escrito correctamente.\n\nFaltan dígitos", "OK"); return;
                }

                if (txtusuario.Text == null || txtusuario.Text == "")
                {
                    await DisplayAlert("Aviso", "Su nombre de usuario es requerido para poder aperturar su cuenta de usuario", "OK"); return;
                }
                else if (await App.DBase.obtenerUsuario(2, txtusuario.Text) != null)
                {
                    await DisplayAlert("Aviso", "El nombre de usuario pertenece a otra cuenta", "OK"); return;
                }

                if (txtemail.Text == null || txtemail.Text == "")
                {
                    await DisplayAlert("Aviso", "Ingrese su correo electrónico para poder aperturar su cuenta de usuario.\n\nEnviaremos un código de verificación a este correo que usted ingrese.", "OK"); return;
                }
                else if (await App.DBase.obtenerUsuario(3, txtemail.Text) != null)
                {
                    await DisplayAlert("Aviso", "El correo electrónico pertenece a otra cuenta de usuario", "OK"); return;
                }
                else if(!validateEmail(txtemail.Text))
                {
                    await DisplayAlert("Aviso", "El correo electrónico que ha ingresado no es válido", "OK"); return;
                }

                if (txtcontraseña.Text == null || txtcontraseña.Text == "")
                {
                    await DisplayAlert("Aviso", "Debe ingresar una contraseña para completar el registro.", "OK"); return;
                }
                else
                {
                    if (txtcontraseña.Text.Length < 8) { await DisplayAlert("Aviso", "La contraseña debe tener por lo menos 8 caractéres", "OK"); return; }
                    else if (txtcontraseña.Text != txtcontraseñarepetida.Text) { await DisplayAlert("Aviso", "Repite tu contraseña correctamente para finalizar el registro.", "OK"); return; }
                }

            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return;
            }

            btnregistrar.IsEnabled = false;
            //enviarcorreo();

            usuariocompleto.NumeroIdentidad = txtnumeroidentidad.Text;
            usuariocompleto.NombreUsuario = txtusuario.Text;
            usuariocompleto.Email = txtemail.Text;
            usuariocompleto.Contraseña = txtcontraseña.Text;

            //Añadimos Codigo Temporal a Usuario
            //usuariocompleto.CodigoVerificacion = CodigoAleatorio(1);

            //Validacion que el id de cliente no venga repetido y se asigna
            bool ciclo = true;
            while (ciclo)
            {
                var idcliente = CodigoAleatorio(2);
                if (await App.DBase.obtenerUsuario(4, idcliente) == null)
                {
                    usuariocompleto.IdCliente = idcliente;
                    break;
                }
            }


            try
            {
                //SQLITE
                var usuariosqlite = await App.DBase.obtenerUsuario(2, usuariocompleto.NombreUsuario);

                if (usuariosqlite == null)
                {
                    UserDialogs.Instance.ShowLoading("Creando usuario", MaskType.Clear);
                    //guardar en API
                    bool apiresult = await UsuarioApi.CreateUsuario(usuariocompleto);
                    //Crearle deudas servicios de agua y electricidad
                    var respuesta = await PagosApi.SetDeudasUsuario(usuariocompleto.NumeroIdentidad);
                    //guardar en SQLite
                    var result = await App.DBase.UsuarioSave(usuariocompleto);
                    persistenciaSUsuario(usuariocompleto);
                    enviarcorreo(usuariocompleto);
                    UserDialogs.Instance.HideLoading();

                    await DisplayAlert("Registro Completado", "Hemos enviado un código de verificación a su correo electrónico que se le solicitará únicamente la primera vez que entre a su cuenta.", "OK");
                    for (var counter = 1; counter < 2; counter++) //2 es el numero de paginas a retroceder
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                    }
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Aviso", "El nombre de usuario que usted ha ingresado ya existe.\n\nIngrese otro nombre de usuario.", "OK");
                }
            }
            catch (Exception error)
            {
                await DisplayAlert("Error", "Se produjo un error al enviarte el correo", "OK");
            }

            /*bool estado = await UsuarioApi.CreateUsuario(usuariocompleto);
            if (estado) { await DisplayAlert("Aviso", "Usuario adicionado con éxito", "OK"); }*/
            
        }

        string CodigoAleatorio(int op)
        {
            //op = 1 codigo verificacion
            //op = 2 id cliente

            Random rdn = new Random();
            //string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890%$#@";

            string caracteres = "";
            int longitudContrasenia = 0;

            if (op == 1) { caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890%$#@"; longitudContrasenia = 6; }
            if (op == 2) { caracteres = "1234567890"; longitudContrasenia = 6; }

            int longitud = caracteres.Length;
            char letra;
            longitudContrasenia = 6;
            string contraseniaAleatoria = string.Empty;
            for (int i = 0; i < longitudContrasenia; i++)
            {
                letra = caracteres[rdn.Next(longitud)];
                contraseniaAleatoria += letra.ToString();
            }
            return contraseniaAleatoria;
        }

        static bool validateEmail(string email)
        {
            if (email == null)
            {
                return false;
            }
            if (new EmailAddressAttribute().IsValid(email))
            {
                return true;
            }
            else
            {

                return false;
            }
        }

        #region Enviar e-mail
        void enviarcorreo(Usuario usuariocompleto)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("starbankteam@gmail.com");
                mail.To.Add(usuariocompleto.Email);
                mail.Subject = "STARBANK | Código de verificación";
                mail.Body = "¡Hola <b>"+usuariocompleto.NombreCompleto+"!</b> <br> Gracias por elegir STARBANK. Este es tu código de verificación: <br> <h3>"+usuariocompleto.CodigoVerificacion+"</h3>";
                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("starbankteam@gmail.com", "ptkyllujqfluvnls");

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                DisplayAlert("Mensaje del Programador", ex.Message, "OK");
            }
        }
        #endregion

        public async void persistenciaSUsuario(Usuario usuario)
        {
            //PERSISTENCIA insertar
            var persistencia = new Persistencia
            {
                Id = 1,
                Campo = "" + usuario.Id
            }; //1 porque es Usuario (ver má
               //s en Persistencia.cs)
            var estado = await App.DBase.PersistenciaSave(persistencia);
        }

        private async void btninicio_Clicked(object sender, EventArgs e)
        {
            if (btnregistrar.IsEnabled)
            {
                await Navigation.PopAsync();
            }
            
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LogIn());
        }
    }
}