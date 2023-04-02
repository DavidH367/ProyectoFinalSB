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
    public partial class Servicios : ContentPage
    {
        Usuario pusuario;
        Dolar pdolar;
        List<Models.Servicio> pservicios = new List<Models.Servicio>();
        List<Models.Servicio> eventos = new List<Models.Servicio>();

        public Servicios(Usuario usuario, Dolar dolar)
        {
            InitializeComponent();

            pusuario = usuario;
            pdolar = dolar;

        }

        protected override async void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("Obteniendo servicios", MaskType.Clear);
            pservicios = await ServicioApi.GetAllServicios();

            for (int i = 0; i < pservicios.Count; i++)
            {
                if (pservicios[i].Id != 1 && pservicios[i].Id != 2)
                {
                    eventos.Add(pservicios[i]);
                }
            }

            if(eventos.Count < 1)
            {
                mensajesineventos.IsVisible = true;
                ListPromociones.IsVisible = false;
            }
            else
            {
                ListPromociones.ItemsSource = eventos;
            }

            ListPromociones.SelectedItem = null;

            UserDialogs.Instance.HideLoading();
        }

        private async void TapGestureRecognizer_agua(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Servicio(pusuario, pservicios[1], pdolar));
        }

        private async void TapGestureRecognizer_energia(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Servicio(pusuario, pservicios[0], pdolar));
        }

        private async void ListPromociones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _evento = ListPromociones.SelectedItem;

            if(_evento != null)
            {
                Models.Servicio evento = _evento as Models.Servicio;
                await Navigation.PushAsync(new Servicio(pusuario, evento, pdolar));
            }

            //Models.Servicio evento = (Models.Servicio)e.CurrentSelection[0];
        }
    }
}