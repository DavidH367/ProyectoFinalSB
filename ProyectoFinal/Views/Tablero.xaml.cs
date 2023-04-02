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
        Dolar pdolar;
        

        public Tablero(Usuario usuario, Dolar dolar)
        {
            InitializeComponent();

            pusuario = usuario;
            pdolar = dolar;
    
            Console.WriteLine(pusuario.NombreCompleto.ToString());
            //Console.WriteLine(pdolar.Precio.ToString());
            
           /* if (pdolar != null)
            {
                preciodolarc.Text = string.Format("{0:f4}", pdolar.Compra);
                preciodolarv.Text = string.Format("{0:f4}", pdolar.Venta);
                // acceder a las propiedades de pdolar aquí
            }
           */
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
            

            //preciodolarv.Text = pdolar.Compra;


            /*preciodolarc.Text = "24.2656";
            preciodolarv.Text = pdolar.Compra.ToString("F4");
            */
            //preciodolarc.Text = string.Format("{0:C}", pdolar.Compra);
            // Console.WriteLine("Este es el que busco: ");
            //Console.WriteLine(pdolar.Compra.ToString());

            //preciodolarc.Text = valorcompra.ToString();

            //Console.WriteLine(pusuario.NombreCompleto.ToString());


            UserDialogs.Instance.HideLoading();
;        }

        private async void btncuentas_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Cuentas(pusuario, pdolar));
        }

        private async void btnservicios_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Servicios(pusuario, pdolar));
        }

        private async void btntransferencias_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Transferencias(pusuario, pdolar));
        }

        private async void btncontrolp_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ControlPresupuestario(pusuario, pdolar));
        }



        private async void btnsoporte_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Soporte(pusuario, pdolar));
        }

        private async void btnlogout_Clicked(object sender, EventArgs e)
        {
            bool respuesta = await DisplayAlert("Cerrando sesión", "¿Realmente quieres cerrar sesión?", "Si", "No");

            if (respuesta) {
                await Navigation.PushAsync(new LogIn());
                //await Navigation.PushAsync(new LogIn());
            }
            
        }

        private async void perfil_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Perfil(pusuario, pdolar));
        }
    }
}