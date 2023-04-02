using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ProyectoFinal.Models;
using Acr.UserDialogs;
using ProyectoFinal.Api;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cuentas : ContentPage
    {
        Usuario pusuario;
        //Dolar pdolar;
        int operacion = 0;

        Models.Servicio pservicio;

        //0 pagina normal por defecto (dirige a administracion de cuenta)
        //1 seleccionar cuenta para redirigir a la pantalla Transferencias
        //2 seleccionar cuenta para redirigir a la pantalla Servicio

        public Cuentas(Usuario usuario/*, Dolar dolar*/)
        {
            InitializeComponent();
            pusuario = usuario;
            //pdolar = dolar;
        }

        public Cuentas(Usuario usuario, int op/*, Dolar dolar*/)
        {
            InitializeComponent();
            pusuario = usuario;
            operacion = op;
            btnvolver.IsVisible = false;
            btncrearcuenta.IsVisible = true;

            //pdolar = dolar;
        }

        public Cuentas(Usuario usuario, int op/*, Dolar dolar*/, Models.Servicio servicio)
        {
            InitializeComponent();
            pusuario = usuario;
            operacion = op;
            btnvolver.IsVisible = false;
            btncrearcuenta.IsVisible = true;
            pservicio = servicio;

           // pdolar = dolar;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Tablero(pusuario/*, pdolar*/));
        }

        protected override async void OnAppearing()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("cargando...", MaskType.Clear);

                await App.DBase.ListaCuentasSave(await CuentaApi.GetAllCuentas());

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception error)
            {
                UserDialogs.Instance.HideLoading();
            }

            var cuentas = await App.DBase.obtenerCuentasUsuario(pusuario.NumeroIdentidad);

            List<_cuenta> _cuentas = new List<_cuenta>();

            for (int i = 0; i < cuentas.Count; i++)
            {
                _cuentas.Add(new _cuenta
                {
                    CodigoCuenta = cuentas[i].CodigoCuenta,
                    Moneda = cuentas[i].Moneda,
                    Saldo = string.Format("{0:C}", cuentas[i].Saldo).Replace("$", string.Empty)
                });
            }

            ListCuentas.ItemsSource = _cuentas;
                
        }

        private async void btncreacuenta_Clicked(object sender, EventArgs e)
        {
            var resultado = await App.DBase.obtenerCuentasUsuario(pusuario.NumeroIdentidad);

            if (resultado.Count >= 3)
            {
                await DisplayAlert("Aviso", "Ya tienes 2 cuentas de ahorro, haz alcanzado el límite.", "OK");
            }
            else
            {
                await Navigation.PushAsync(new CrearCuenta(pusuario));
            }
            
        }

        private async void btnvolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void ListCuentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _cuenta __cuenta = (_cuenta)e.CurrentSelection[0];
            //[0] porque es el indice de los elementos seleccionados, como es seleccion unica (se configura como parametro en el xaml) siempre sera el indice [0]

            var cuenta = await App.DBase.obtenerCuenta(__cuenta.CodigoCuenta);

            if (operacion == 0)
            {
                await Navigation.PushAsync(new AdministracionCuenta(cuenta, pusuario/*, pdolar*/));
            }
            else if (operacion == 1)
            {
                await Navigation.PushAsync(new Transferencias(pusuario, cuenta /*, pdolar*/));
            }
            else if (operacion == 2)
            {
                await Navigation.PushAsync(new Servicio(pusuario, pservicio /*, pdolar*/, cuenta));
            }
        }
    }

    public class _cuenta
    {
        public string CodigoCuenta { get; set; }
        public string Moneda { get; set; }
        public string Saldo { get; set; }
        public string Accion { get; set; }
    }
}