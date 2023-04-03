using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ProyectoFinal.Models;
using ProyectoFinal.Api;
using Acr.UserDialogs;

namespace ProyectoFinal.Views
{
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Tablero : ContentPage
    {
        
        Usuario pusuario;
        

        public Tablero(Usuario usuario)
        {
            InitializeComponent();

            pusuario = usuario;
            
               
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override async void OnAppearing()
        {
            

            UserDialogs.Instance.ShowLoading("cargando...", MaskType.Clear);

            try
            {
                await App.DBase.ListaTransferenciaSave(await TransferenciaApi.GetTransferencias());
            }
            catch (Exception error)
            {

            }

            try
            {
                await App.DBase.ListaCuentasSave(await CuentaApi.GetAllCuentas());
            }
            catch (Exception error)
            {

            }
            
            imgusuario.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(pusuario.Fotografia));
            txtnombrecompleto.Text = pusuario.NombreCompleto;
            txtnombreusuario.Text = pusuario.NombreUsuario;

            UserDialogs.Instance.HideLoading();
;        }

        private async void btncuentas_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Cuentas(pusuario));
        }

        private async void btnservicios_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Servicios(pusuario));
        }

        private async void btntransferencias_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Transferencias(pusuario));
        }

        private async void btncontrolp_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ControlPresupuestario(pusuario));
        }



        private async void btnsoporte_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Soporte(pusuario));
        }

        private async void btnlogout_Clicked(object sender, EventArgs e)
        {
            bool respuesta = await DisplayAlert("Cerrando sesión", "¿Realmente quieres cerrar sesión?", "Si", "No");

            if (respuesta) {
                await Navigation.PushAsync(new LogIn());
            }
            
        }

        private async void perfil_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Perfil(pusuario));
        }
    }
}