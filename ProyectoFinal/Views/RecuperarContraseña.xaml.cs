using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ProyectoFinal.Models;
using System.Net.Mail;
using Acr.UserDialogs;
using System.ComponentModel.DataAnnotations;
using ProyectoFinal.Api;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecuperarContraseña : ContentPage
    {
        public RecuperarContraseña()
        {
            InitializeComponent();
        }

        private async void goLogIn(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void btnenviarcontraseñat_Clicked(object sender, EventArgs e)
        {
            if (txtemail.Text == null || txtemail.Text == "")
            {
                await DisplayAlert("Aviso", "Ingrese el correo electrónico ligado a su cunta para poder recuperarla", "OK"); return;
            }
            else if (!validateEmail(txtemail.Text))
            {
                await DisplayAlert("Aviso", "El correo electrónico que ha ingresado no es válido", "OK"); return;
            }

            var usuario = await App.DBase.obtenerUsuario(3, txtemail.Text);
            if (usuario != null)
            {
                if(usuario.CodigoVerificacion == "")
                {
                    usuario.ContraseñaTemporal = CodigoAleatorio();

                    UserDialogs.Instance.ShowLoading("Procesando solicitud", MaskType.Clear);
                    await App.DBase.UsuarioSave(usuario);
                    await UsuarioApi.UpdateUsuario(usuario);
                    enviarcorreo(usuario);
                    UserDialogs.Instance.HideLoading();
                    await DisplayAlert("Envío exitoso", "Revisa tu bandeja de entrada y sigue las instrucciones para habilitar tu nueva contraseña.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Aviso", "Eres un usuario nuevo, debes iniciar sesión por lo menos una vez para cambiar tu contraseña.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Aviso", "El correo electrónico que ingresaste no está ligado a ninguna cuenta existente.", "OK");
            }
            
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
        void enviarcorreo(Usuario user)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("starbankteam@gmail.com");
                mail.To.Add(user.Email);
                mail.Subject = "STARBANK | Código de verificación";
                mail.Body = "<html><body><p>¡Hola <b>" + user.NombreCompleto + "!</b></p><br><br><p>Gracias por elegir STARBANK.</p><br><br><p>Esta es tu contraseña temporal: <b>"+user.ContraseñaTemporal+"<b></p><br><p>Con ella ingresarás a la aplicación STARBANK y se te solicitará que escribas una contraseña nueva la cual <b>será tu nueva contraseña.</b></p><br<br><p>Si tú no has solicitado este cambio de contraseña puedes ignorar este correo electrónico.</p></body></html>";
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

        string CodigoAleatorio()
        {
            Random rdn = new Random();
            //string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890%$#@";
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890#@";
            int longitud = caracteres.Length;
            char letra;
            int longitudContrasenia = 8;
            string contraseniaAleatoria = string.Empty;
            for (int i = 0; i < longitudContrasenia; i++)
            {
                letra = caracteres[rdn.Next(longitud)];
                contraseniaAleatoria += letra.ToString();
            }
            return contraseniaAleatoria;
        }

        private void txtemail_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnenviarcontraseñat.IsEnabled = true;
            if (txtemail.Text == "") {
                btnenviarcontraseñat.IsEnabled = false; 
            }
            
        }

        private void makeToast(string mensaje, double duracion)
        {
            var ToastConfig = new ToastConfig(mensaje);
            ToastConfig.SetDuration(TimeSpan.FromSeconds(duracion));
            ToastConfig.SetPosition(ToastPosition.Bottom);
            UserDialogs.Instance.Toast(ToastConfig);
        }
    }
}