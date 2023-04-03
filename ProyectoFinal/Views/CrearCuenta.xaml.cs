using Acr.UserDialogs;
using ProyectoFinal.Api;
using ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CrearCuenta : ContentPage
    {
        Usuario pusuario;

        public CrearCuenta(Usuario usuario)
        {
            InitializeComponent();

            //stackpantalla.Margin = new Thickness(0, Application.Current.MainPage.Height, 0, 0);
            //stackpantalla.HeightRequest = Application.Current.MainPage.Height;

            pusuario = usuario;
        }

        protected override async void OnAppearing()
        {
            List<string> monedas = new List<string>();
            monedas.Add("HNL");
            monedas.Add("USD");

            var ListCuentas = await App.DBase.obtenerCuentasUsuario(pusuario.NumeroIdentidad);
            
            if(ListCuentas.Count != 0)
            {
                for (int i = 0; i < ListCuentas.Count; i++)
                {
                    if (ListCuentas[i].Moneda == "HNL") { monedas.Remove("HNL"); }
                    if (ListCuentas[i].Moneda == "USD") { monedas.Remove("USD"); }
                }
            }

            pckmoneda.ItemsSource = monedas;
        }

        private async void btncrear_Clicked(object sender, EventArgs e)
        {
            bool ciclo = true;
            while (ciclo)
            {
                Cuenta cuenta = new Cuenta
                {
                    CodigoCuenta = CodigoAleatorio(),
                    CodigoUsuario = pusuario.NumeroIdentidad,
                    Moneda = pckmoneda.SelectedItem.ToString(),
                    Saldo = double.Parse(pcksaldo.SelectedItem.ToString()),
                    Tipo = "ahorro"
                };

                UserDialogs.Instance.ShowLoading("cargando...", MaskType.Clear);

                int resultado = await App.DBase.CuentaSave(1, cuenta); //1 operacion save
                await CuentaApi.CreateCuenta(cuenta);

                UserDialogs.Instance.HideLoading();

                if (resultado == 0)
                {
                    await DisplayAlert("Error", "No se ha podido crear tu cuenta debido a un error interno", "OK");
                    ciclo = false; //no se creó la cuenta (error sqlite)
                }
                else if (resultado == 1)
                {
                    await DisplayAlert("Éxito", "Cuenta creada exitósamente", "OK");
                    ciclo = false; //se creó la cuenta
                    await Navigation.PopAsync();
                }
                else if (resultado == 2)
                {
                    ciclo = true; //el codigo aleatorio repitio un codigo de cuenta existente, se vuelve a generar otro distinto
                }
            }
        }

        string CodigoAleatorio()
        {
            Random rdn = new Random();
            //string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890%$#@";
            string caracteres = "1234567890";
            int longitud = caracteres.Length;
            char letra;
            int longitudContrasenia = 7;
            string contraseniaAleatoria = string.Empty;
            for (int i = 0; i < longitudContrasenia; i++)
            {
                letra = caracteres[rdn.Next(longitud)];
                contraseniaAleatoria += letra.ToString();
            }
            return contraseniaAleatoria;
        }
    }
}