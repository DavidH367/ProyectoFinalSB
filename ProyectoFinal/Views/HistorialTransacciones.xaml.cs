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
    public partial class HistorialTransacciones : ContentPage
    {
        Cuenta pcuenta;
        public HistorialTransacciones(Cuenta cuenta)
        {
            InitializeComponent();

            pcuenta = cuenta;
        }

        protected override async void OnAppearing()
        {
            actualizarLista(1);
        }

        private async void btntodos_Clicked(object sender, EventArgs e)
        {
            btntodos.BackgroundColor = Color.FromHex("#465173");
            btntodos.TextColor = Color.White;
            btntodos.FontAttributes = FontAttributes.Bold;

            btndebitos.BackgroundColor = Color.White;
            btndebitos.TextColor = Color.FromHex("#6d758c");
            btncreditos.BackgroundColor = Color.White;
            btncreditos.TextColor = Color.FromHex("#6d758c");

            actualizarLista(1);
        }

        private async void btncreditos_Clicked(object sender, EventArgs e)
        {
            btncreditos.BackgroundColor = Color.FromHex("#465173");
            btncreditos.TextColor = Color.White;
            btncreditos.FontAttributes = FontAttributes.Bold;

            btndebitos.BackgroundColor = Color.White;
            btndebitos.TextColor = Color.FromHex("#6d758c");
            btntodos.BackgroundColor = Color.White;
            btntodos.TextColor = Color.FromHex("#6d758c");

            actualizarLista(2);
        }

        private async void btndebitos_Clicked(object sender, EventArgs e)
        {
            btndebitos.BackgroundColor = Color.FromHex("#465173");
            btndebitos.TextColor = Color.White;
            btndebitos.FontAttributes = FontAttributes.Bold;

            btncreditos.BackgroundColor = Color.White;
            btncreditos.TextColor = Color.FromHex("#6d758c");
            btntodos.BackgroundColor = Color.White;
            btntodos.TextColor = Color.FromHex("#6d758c");

            actualizarLista(3);
        }

        private void ListTransferencias_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        async void actualizarLista(int operacion)
        {
            //1 Todos
            //2 Créditos
            //3 Débitos

            List<Transferencia> lista = await App.DBase.obtenerTransferenciasCuenta(operacion, pcuenta.CodigoCuenta); //trae las transferencias que tengan que ver con esta cuenta de usuario que accede a la app (del usuario pusuario)
            List<detallesT> detalles = new List<detallesT>();
            detallesT detalle = new detallesT();

            lista = Enumerable.Reverse(lista).ToList(); //Invierte la lista, la ultima transaccion hecha tiene que estar mas arriba

            for (int i = 0; i < lista.Count; i++)
            {
                var date = lista[i].Fecha;
                date = date.Substring(0, 10);
                var dolar = await App.DBase.obtenerPrecioDolar(date);

                detalle.moneda = pcuenta.Moneda;
                
                if (lista[i].Envia != pcuenta.CodigoCuenta)
                {
                    lista[i].Accion = "crédito";

                    var cuenta = await App.DBase.obtenerCuenta(lista[i].Envia);
                    var usuario = await App.DBase.obtenerUsuario(5, "" + cuenta.CodigoUsuario);

                    detalle.imagen = "arrowleft.png";
                    detalle.color = "#18cf25";
                    detalle.concepto = "Transferencia de " + usuario.NombreCompleto;
                }
                else
                {
                    var cuenta = await App.DBase.obtenerCuenta(lista[i].Recibe);

                    if(cuenta != null)
                    {
                        var usuario = await App.DBase.obtenerUsuario(5, "" + cuenta.CodigoUsuario);

                        detalle.imagen = "arrowright.png";
                        detalle.color = "#e81313";
                        detalle.concepto = "Transferencia a " + usuario.NombreCompleto;
                    }
                    else
                    {
                        detalle.imagen = "arrowright.png";
                        detalle.color = "#e81313";

                        string nombre = "";
                        if(lista[i].Recibe == "servicioID1") { nombre = "Pago de Servicio: Empresa Energía Honduras"; }
                        else if (lista[i].Recibe == "servicioID2") { nombre = "Pago de Servicio: Servicio de Agua Potable"; }
                        else
                        {
                            nombre = lista[i].Comentario;
                        }

                        detalle.concepto =  nombre;
                    }
                    

                    
                }

                detalle.accion = lista[i].Accion;
                detalle.fecha = lista[i].Fecha;

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



                //detalle.moneda = lista[i].Moneda;
                detalle.valor = string.Format("{0:C}", lista[i].Valor).Replace("$", string.Empty);

                detalles.Add(new detallesT() { imagen = detalle.imagen, accion = detalle.accion, color = detalle.color, concepto = detalle.concepto, moneda = detalle.moneda, valor = detalle.valor , fecha = detalle.fecha});
            }

            //var usuario = await App.DBase.obtenerUsuario();

            ListTransferencias.ItemsSource = detalles;
        }
    }

    public class detallesT
    {
        public string imagen { get; set; }
        public string color { get; set; }
        public string concepto { get; set; }
        public string accion { get; set; }
        public string moneda { get; set; }
        public string valor { get; set; }
        public string fecha { get; set; }
    }
}