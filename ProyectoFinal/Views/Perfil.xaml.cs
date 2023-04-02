using Acr.UserDialogs;
using Plugin.Media;
using ProyectoFinal.Api;
using ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Perfil : ContentPage
    {
        Plugin.Media.Abstractions.MediaFile FileFoto = null;
        byte[] FileFotoBytes = null;


        Usuario pusuario;
        Dolar pdolar;

        public Perfil()
        {
            InitializeComponent();
        }

        public Perfil(Usuario usuario, Dolar dolar)
        {
            InitializeComponent();
            pusuario = usuario;
            pdolar = dolar;

            
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected async override void OnAppearing()
        {
            recargar();
        }

        public string obtenerFecha(string fecha)
        {
            var _fecha = DateTime.Parse(fecha);
            string mes = "";

            switch (_fecha.Month)
            {
                case 1:
                    mes = "enero";
                    break;
                case 2:
                    mes = "febrero";
                    break;
                case 3:
                    mes = "marzo";
                    break;
                case 4:
                    mes = "abril";
                    break;
                case 5:
                    mes = "mayo";
                    break;
                case 6:
                    mes = "junio";
                    break;
                case 7:
                    mes = "julio";
                    break;
                case 8:
                    mes = "agosto";
                    break;
                case 9:
                    mes = "septiembre";
                    break;
                case 10:
                    mes = "octubre";
                    break;
                case 11:
                    mes = "noviembre";
                    break;
                case 12:
                    mes = "diciembre";
                    break;
            }

            return _fecha.Day + " de " + mes + " del " + _fecha.Year;
        }

        public string ocultarContraseña(string contra)
        {
            string clave = "";

            for (int i = 0; i < contra.Length; i++)
            {
                clave += "* ";
            }

            clave.Remove(contra.Length-1);

            return clave;
        }

        private async void btneditar_Clicked(object sender, EventArgs e)
        {
            if(btneditar.Text != "LISTO") {
                perfilmodificar();
            }
            else {
                if (!validateEmail(entryemail.Text))
                {
                    await DisplayAlert("Aviso", "El correo electrónico que ha ingresado no es válido", "OK"); return;
                }

                pusuario.Fotografia = FileFotoBytes;
                pusuario.Email = entryemail.Text;
                pusuario.Sexo = sexo.Text;
                pusuario.Direccion = direccion.Text;

                UserDialogs.Instance.ShowLoading("actualizando...", MaskType.Clear);

                var result = await App.DBase.UsuarioSave(pusuario);
                await UsuarioApi.UpdateUsuario(pusuario);

                UserDialogs.Instance.HideLoading();


                if (result==1) { await DisplayAlert("Actualizado", "Tus datos se han actualizado correctamente", "OK"); }

                perfilnormal();
                recargar();
            }

        }


        private async void btncambiarc_Clicked(object sender, EventArgs e)
        {
            bool ciclo = true;

            while(ciclo == true)
            {
                var promptConfig = new PromptConfig();
                promptConfig.Title = "Cambio de contraseña";
                promptConfig.Message = "A continuación ingrese su actual contraseña:";
                promptConfig.InputType = InputType.Password;
                promptConfig.OkText = "Siguiente";
                promptConfig.CancelText = "Cancelar";
                //promptConfig.IsCancellable = true;

                var result = await UserDialogs.Instance.PromptAsync(promptConfig);

                if (result.Ok)
                {
                    if (result.Text == pusuario.Contraseña)
                    {
                        bool ciclo2 = true;
                        while (ciclo2)
                        {
                            var promptConfig2 = new PromptConfig();
                            promptConfig2.Title = "Cambio de contraseña";
                            promptConfig2.Message = "La contraseña es correcta.\n\nAhora ingrese su nueva contraseña:";
                            promptConfig2.InputType = InputType.Password;
                            promptConfig2.OkText = "Siguiente";
                            promptConfig2.CancelText = "Cancelar";
                            //promptConfig.IsCancellable = true;

                            var result2 = await UserDialogs.Instance.PromptAsync(promptConfig2);

                            if (result2.Ok)
                            {
                                if (result2.Text.Length < 8)
                                {
                                    await DisplayAlert("Aviso", "La nueva contraseña debe contener como mínimo 8 caractéres", "OK");
                                }
                                else
                                {
                                    bool ciclo3 = true;
                                    while (ciclo3)
                                    {
                                        var promptConfig3 = new PromptConfig();
                                        promptConfig3.Title = "Cambio de contraseña";
                                        promptConfig3.Message = "Repita su nueva contraseña:";
                                        promptConfig3.InputType = InputType.Password;
                                        promptConfig3.OkText = "Siguiente";
                                        promptConfig3.CancelText = "Volver";
                                        //promptConfig.IsCancellable = true;

                                        var result3 = await UserDialogs.Instance.PromptAsync(promptConfig3);

                                        if (result3.Ok)
                                        {
                                            if (result3.Text == result2.Text)
                                            {
                                                pusuario.Contraseña = result2.Text;

                                                UserDialogs.Instance.ShowLoading("actualizando...", MaskType.Clear);

                                                var resultado = await App.DBase.UsuarioSave(pusuario);
                                                await UsuarioApi.UpdateUsuario(pusuario);
                                                
                                                UserDialogs.Instance.HideLoading();


                                                if (resultado == 1) { await DisplayAlert("Éxito", "Su contraseña se ha actualizado correctamente", "OK"); }
                                                ciclo3 = false;
                                                ciclo2 = false;
                                                ciclo = false;
                                                recargar();
                                            }
                                            else
                                            {
                                                await DisplayAlert("Aviso", "Repite correctamente la nueva contraseña, vuelve a intentarlo", "OK");
                                            }
                                        }
                                        else
                                        {
                                            ciclo3 = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ciclo2 = false;
                                ciclo = false;
                            }
                        }
                    }
                    else
                    {
                        var respuesta = await DisplayAlert("Aviso", "La contraseña no es correcta", "Volver a intentarlo", "Cancelar");
                        if (!respuesta) { ciclo = false; }
                    }
                }
                else
                {
                    ciclo = false;
                }
            }
        }

        private async void frmsexo_Tapped(object sender, EventArgs e)
        {
            if(btneditar.Text == "LISTO") {
                string action = await DisplayActionSheet("Sexo", "Cancelar", null, "Masculino", "Femenino");
                sexo.Text = action;
            }
        }

        private void btncancelar_Clicked(object sender, EventArgs e)
        {
            btncancelar.IsVisible = false;

            recargar();
            perfilnormal();
        }

        public async void recargar()
        {
            idcliente.Text = pusuario.IdCliente;
            imgusuario.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(pusuario.Fotografia));
            FileFotoBytes = pusuario.Fotografia;
            nombrecompleto.Text = pusuario.NombreCompleto;
            nombreusuario.Text = pusuario.NombreUsuario;
            idcliente.Text = pusuario.IdCliente;

            var cuentas = await App.DBase.obtenerCuentasUsuario(pusuario.NumeroIdentidad);
            cuentasaperturadas.Text = "" + cuentas.Count;

            email.Text = pusuario.Email;
            clave.Text = ocultarContraseña(pusuario.Contraseña);
            nidentidad.Text = pusuario.NumeroIdentidad;
            fechanac.Text = obtenerFecha(pusuario.FechaNacimiento);
            sexo.Text = pusuario.Sexo;
            direccion.Text = pusuario.Direccion;
        }

        public void perfilnormal()
        {
            btneditar.Text = "EDITAR";
            btncancelar.IsVisible = false;

            btncambiarc.IsVisible = true;

            frmemail.BackgroundColor = Color.Default;
            frmsexo.BackgroundColor = Color.Default;
            frmdireccion.BackgroundColor = Color.Default;

            frmemail.Padding = 20;
            frmclave.Padding = new Thickness(20, 10, 20, 10);

            lblemail.TextColor = Color.Default;
            lblclave.TextColor = Color.Default;
            lblsexo.TextColor = Color.Default;
            lbldireccion.TextColor = Color.Default;

            clave.TextColor = Color.Default;
            direccion.TextColor = Color.Default;
            sexo.TextColor = Color.Default;

            email.IsVisible = true;
            entryemail.IsVisible = false;
            direccion.IsEnabled = false;
        }

        public void perfilmodificar()
        {
            btncancelar.IsVisible = true;
            btneditar.Text = "LISTO";

            frmemail.BackgroundColor = Color.FromHex("#0b1745");
            frmsexo.BackgroundColor = Color.FromHex("#0b1745");
            frmdireccion.BackgroundColor = Color.FromHex("#0b1745");

            frmemail.Padding = new Thickness(20, 10, 20, 10);
            frmclave.Padding = new Thickness(20, 19, 20, 19);

            lblemail.TextColor = Color.White;
            lblsexo.TextColor = Color.White;
            lbldireccion.TextColor = Color.White;
            direccion.TextColor = Color.White;
            sexo.TextColor = Color.White;

            entryemail.IsVisible = true;
            entryemail.Text = email.Text;
            email.IsVisible = false;
            direccion.IsEnabled = true;

            btncambiarc.IsVisible = false;
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

        private async void imgpersona_Tapped(object sender, EventArgs e)
        {
            if (btneditar.Text != "LISTO") { return; }

            string action = await DisplayActionSheet("Obtener fotografía", "Cancelar", null, "Seleccionar de galería", "Tomar foto");

            if (action == "Seleccionar de galería") { seleccionarfoto(); }
            if (action == "Tomar foto") { tomarfoto(); }
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
            // await DisplayAlert("Path directorio", FileFoto.Path, "OK");


            if (FileFoto != null)
            {
                imgusuario.Source = ImageSource.FromStream(() =>
                {
                    return FileFoto.GetStream();
                });

                //Pasamos la foto a imagen a byte[] almacenandola en FileFotoBytes
                using (System.IO.MemoryStream memory = new MemoryStream())
                {
                    Stream stream = FileFoto.GetStream();
                    stream.CopyTo(memory);
                    FileFotoBytes = memory.ToArray();
                    /*string base64Val = Convert.ToBase64String(FileFotoBytes);
                    FileFotoBytes = Convert.FromBase64String(base64Val);*/
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

            imgusuario.Source = ImageSource.FromStream(() =>
            {
                return FileFoto.GetStream();
            });

            //Pasamos la foto a imagen a byte[] almacenandola en FileFotoBytes
            using (System.IO.MemoryStream memory = new MemoryStream())
            {
                Stream stream = FileFoto.GetStream();
                stream.CopyTo(memory);
                FileFotoBytes = memory.ToArray();
                /*string base64Val = Convert.ToBase64String(FileFotoBytes);
                FileFotoBytes = Convert.FromBase64String(base64Val);*/
            }

            /*Imagen.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                file.Dispose();
                return stream;
            });*/
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Tablero(pusuario, pdolar));
        }
    }
}