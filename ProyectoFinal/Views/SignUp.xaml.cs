using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ProyectoFinal.Models;
using System.IO;
using ProyectoFinal.Api;
using Acr.UserDialogs;
using System.ComponentModel.DataAnnotations;
using Xamarin.Essentials;
using System.Net.Mail;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUp : ContentPage
    {
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

        Plugin.Media.Abstractions.MediaFile FileFoto = null;
        byte[] FileFotoBytes = null;

        public SignUp()
        {
            InitializeComponent();

            try
            {
                //CORRECION el dato de DateTime.Now debe ser traido desde la API no localmente
                dtfechanacimiento.MaximumDate = DateTime.Parse(DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + (DateTime.Now.Year - 18) );

            }
            catch (Exception error)
            {

            }

        }


        private async void imgpersona_Tapped(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Obtener fotografía", "Cancelar", null, "Seleccionar de galería", "Tomar foto");
            
            if(action == "Seleccionar de galería") { seleccionarfoto(); }
            if(action == "Tomar foto") { tomarfoto(); }
        }


        private async void goSingUp2(object sender, EventArgs e)
        {
            
            try
            {
                if (FileFotoBytes == null)
                {
                    bool resp = await DisplayAlert("Aviso", "Tomarse una fotografía es requerido para poder aperturar su cuenta de usuario", "Tomar Foto", "OK");
                    if (resp) { tomarfoto(); }
                    return;
                }

                if (txtnombrecompleto.Text == null || txtnombrecompleto.Text == "")
                {
                    await DisplayAlert("Aviso", "Su nombre completo es requerido para poder aperturar su cuenta de usuario", "OK"); return;
                }

                if(dtfechanacimiento.Date == null)
                {
                    await DisplayAlert("Aviso", "Es requerido colocar su fecha de nacimiento para poder aperturar su cuenta de usuario", "OK"); return;
                }

                if(pcksexo.SelectedItem == null)
                {
                    await DisplayAlert("Aviso", "Es requerido seleccionar su sexo para poder aperturar su cuenta de usuario", "OK"); return;
                }

                if(txtdireccion.Text == null || txtdireccion.Text == "")
                {
                    await DisplayAlert("Aviso", "Su dirección es requerida para poder aperturar su cuenta de usuario", "OK"); return;
                }
                
            }
            catch (Exception error)
            {
                Console.WriteLine(error);
                return;
            }
          
            //await Navigation.PushAsync(new SignUp2(usuario));
        }


        private async void tomarfoto()
        {
            FileFoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "Fotos_starbank",
                Name = "fotografia.jpg",
                SaveToAlbum = true,
                CompressionQuality = 10
            });


            if (FileFoto != null)
            {
                imgpersona.Source = ImageSource.FromStream(() =>
                {
                    return FileFoto.GetStream();
                });

                //Pasamos la foto a imagen a byte[] almacenandola en FileFotoBytes
                using (System.IO.MemoryStream memory = new MemoryStream())
                {
                    Stream stream = FileFoto.GetStream();
                    stream.CopyTo(memory);
                    FileFotoBytes = memory.ToArray();
                    
                }
            }
        }

        private async void seleccionarfoto()
        {
            /*if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }*/

            FileFoto = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Custom,
                CustomPhotoSize = 10
            });


            if (FileFoto == null)
                return;

            imgpersona.Source = ImageSource.FromStream(() =>
            {
                return FileFoto.GetStream();
            });

            //Pasamos la foto a imagen a byte[] almacenandola en FileFotoBytes
            using (System.IO.MemoryStream memory = new MemoryStream())
            {
                Stream stream = FileFoto.GetStream();
                stream.CopyTo(memory);
                FileFotoBytes = memory.ToArray();
                
            }

            /*Imagen.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });*/
        }

        //Aqui esta la parte del segundo SignUp
       

        private async void btnregistrarme(object sender, EventArgs e)
        {

            try
            {
                if (txtnumeroidentidad.Text == null || txtnumeroidentidad.Text == "")
                {
                    await DisplayAlert("Aviso", "Su número de identidad es requerido para poder aperturar su cuenta de usuario", "OK"); return;
                }
                else if (await App.DBase.obtenerUsuario(5, txtnumeroidentidad.Text) != null)
                {
                    await DisplayAlert("Aviso", "Su número de identidad pertenece a otra cuenta de usuario", "OK"); return;
                }
                else if (txtnumeroidentidad.Text.Length < 13)
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
                else if (!validateEmail(txtemail.Text))
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
            //enviarcorreo(usuario);

            Usuario usuario = new Usuario
            {
                Fotografia = FileFotoBytes,
                NombreCompleto = txtnombrecompleto.Text,
                FechaNacimiento = dtfechanacimiento.Date.ToString("yyyy/MM/dd"),
                Sexo = pcksexo.SelectedItem.ToString(),
                Direccion = txtdireccion.Text,
                NumeroIdentidad = txtnumeroidentidad.Text,
                NombreUsuario = txtusuario.Text,
                Email = txtemail.Text,
                Contraseña = txtcontraseña.Text,
                CodigoVerificacion = ""

            };

            //Añadimos Codigo Temporal a Usuario
            //usuario.CodigoVerificacion = CodigoAleatorio(1);

            //Validacion que el id de cliente no venga repetido y se asigna
            bool ciclo = true;
            while (ciclo)
            {
                var idcliente = CodigoAleatorio(2);
                if (await App.DBase.obtenerUsuario(4, idcliente) == null)
                {
                    usuario.IdCliente = idcliente;
                    break;
                }
            }


            try
            {
                //SQLITE
                var usuariosqlite = await App.DBase.obtenerUsuario(2, usuario.NombreUsuario);

                if (usuariosqlite == null)
                {
                    UserDialogs.Instance.ShowLoading("Creando usuario", MaskType.Clear);
                    //guardar en API
                    bool apiresult = await UsuarioApi.CreateUsuario(usuario);
                    //Crearle deudas servicios de agua y electricidad
                    var respuesta = await PagosApi.SetDeudasUsuario(usuario.NumeroIdentidad);
                    //guardar en SQLite
                    var result = await App.DBase.UsuarioSave(usuario);
                    persistenciaSUsuario(usuario);
                    enviarcorreo(usuario);
                    UserDialogs.Instance.HideLoading();

                    await DisplayAlert("Registro Completado", "Hemos enviado    un código de verificación a su correo electrónico que se le solicitará únicamente la primera vez que entre a su cuenta.", "OK");
                    for (var counter = 1; counter < 2; counter++) //2 es el numero de paginas a retroceder
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
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
        string CodigoAleatorio(int op)
        {
            //op = 1 codigo verificacion
            //op = 2 id cliente

            Random rdn = new Random();

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

        void enviarcorreo(Usuario usuariocompleto)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("sbstarbank123@gmail.com");
                mail.To.Add(usuariocompleto.Email);
                mail.Subject = "STARBANK | Código de verificación";
                mail.Body = "¡Hola <b>" + usuariocompleto.NombreCompleto + "!</b> <br> Gracias por elegir STARBANK. Este es tu código de verificación: <br> <h3>" + usuariocompleto.CodigoVerificacion + "</h3>";
                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("sbstarbank123@gmail.com", "yqunobanjkefbehh");

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                DisplayAlert("Mensaje del Programador", ex.Message, "OK");
            }
        }

    }
}