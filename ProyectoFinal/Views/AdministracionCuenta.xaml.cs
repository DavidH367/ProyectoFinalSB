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
    public partial class AdministracionCuenta : ContentPage
    {
        Cuenta pcuenta;
        Usuario pusuario;
        Dolar pdolar;

        public AdministracionCuenta(Cuenta cuenta, Usuario usuario, Dolar dolar)
        {
            InitializeComponent();

            pcuenta = cuenta;
            pusuario = usuario;
            pdolar = dolar;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Tablero(pusuario, pdolar));
        }

        protected override async void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("cargando...", MaskType.Clear);

            await App.DBase.ListaTransferenciaSave(await TransferenciaApi.GetTransferencias());

            UserDialogs.Instance.HideLoading();

            if (pcuenta.Tipo == "ahorro") { txttipocuenta.Text = "Cuenta de ahorros"; }
            txtmoneda.Text = pcuenta.Moneda;
            txtsaldo.Text = string.Format("{0:C}", pcuenta.Saldo).Replace("$", string.Empty);
            txtcodigocuenta.Text = pcuenta.CodigoCuenta;
            txtmesactual.Text = await obtenerMesServidor();
            List <Transferencia> lista = await App.DBase.obtenerTransferenciasCuenta(1, pcuenta.CodigoCuenta);
            List<_transferencia> _transferencias = new List<_transferencia>();

            lista = Enumerable.Reverse(lista).ToList(); //Invierte la lista, la ultima transaccion hecha tiene que estar mas arriba

            for (int i = 0; i < lista.Count; i++)
            {
                var date = lista[i].Fecha;
                date = date.Substring(0, 10);
                var dolar = await App.DBase.obtenerPrecioDolar(date);

                //Convertir a la moneda de la cuenta

                if (pcuenta.Moneda != lista[i].Moneda)
                {
                    if (pcuenta.Moneda == "HNL")
                    {
                        lista[i].Valor = lista[i].Valor * dolar.Precio; // transferencia fue en dolares
                        lista[i].Moneda = "HNL";
                    }
                    else
                    {
                        lista[i].Valor = lista[i].Valor / dolar.Compra; // transferencia fue en lempiras
                        lista[i].Moneda = "USD";
                    }
                }

                if (lista[i].Envia != pcuenta.CodigoCuenta) { lista[i].Accion = "crédito"; }

                _transferencias.Add(new _transferencia
                {
                    IdTransferencia = lista[i].Id,
                    Accion = lista[i].Accion,
                    Moneda = lista[i].Moneda,
                    Valor = string.Format("{0:C}", lista[i].Valor).Replace("$", string.Empty)
                });
            }

            ListTransferenciasMes.ItemsSource = _transferencias;
        }

        private async void ListTransferenciasMes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _transferencia __transferencia = (_transferencia)e.CurrentSelection[0];
            //[0] porque es el indice de los elementos seleccionados, como es seleccion unica (se configura como parametro en el xaml) siempre sera el indice [0]

            var transferencia = await App.DBase.obtenerTransferencia(__transferencia.IdTransferencia);
        }

        private async Task<string> obtenerMesServidor()
        {
            string date = await UsuarioApi.GetFechaServidor();

            date = date.Substring(5, 2); //el primer valor es el indice del cual empieza a obtener el texto y el otro es la longitud de ahi en adelante a donde termina la extraccion dentro del string

            switch (date)
            {
                case "01":
                    date = "Enero";
                    break;
                case "02":
                    date = "Febrero";
                    break;
                case "03":
                    date = "Marzo";
                    break;
                case "04":
                    date = "Abril";
                    break;
                case "05":
                    date = "Mayo";
                    break;
                case "06":
                    date = "Junio";
                    break;
                case "07":
                    date = "Julio";
                    break;
                case "08":
                    date = "Agosto";
                    break;
                case "09":
                    date = "Septiembre";
                    break;
                case "10":
                    date = "Octubre";
                    break;
                case "11":
                    date = "Noviembre";
                    break;
                case "12":
                    date = "Diciembre";
                    break;
            }

            return date;
        }

        private async void btnhtransacciones_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HistorialTransacciones(pcuenta));
        }

        private async void btnvolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void btncontrolp_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ControlPresupuestario(pusuario, pcuenta));
        }
    }

    public class _transferencia
    {
        public int IdTransferencia { get; set; }
        public string Accion { get; set; }
        public string Moneda { get; set; }
        public string Valor { get; set; }
    }
}