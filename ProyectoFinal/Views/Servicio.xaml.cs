using Acr.UserDialogs;
using ProyectoFinal.Api;
using ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProyectoFinal.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Servicio : ContentPage
    {
        Usuario pusuario;
        Cuenta cuenta;
        Models.Servicio pservicio;
        Dolar pdolar;
        Pagos pago;
        Cuenta pcuenta;

        bool cuentaIsSelected = false;

        List<Pagos> pagos = new List<Pagos>();

        List<Pagos> ppagos = new List<Pagos>();

        string messeleccionado;

        public Servicio(Usuario usuario, Models.Servicio servicio, Dolar dolar)
        {
            InitializeComponent();
            pusuario = usuario;
            idcliente.Text = "" + pusuario.IdCliente;

            if (servicio.Id == 1) { calendario.IsVisible = true; }
            else if (servicio.Id == 2) { calendario.IsVisible = true; }
            else
            {
                adebitar.Text = string.Format("{0:C}", servicio.Precio).Replace("$", string.Empty);
                //btntransferir.IsEnabled = true;
            }

            pservicio = servicio;
            pdolar = dolar;

            calendario.IsEnabled = false;
        }

        public Servicio(Usuario usuario, Models.Servicio servicio, Dolar dolar, Cuenta cuenta)
        {
            InitializeComponent();
            pusuario = usuario;
            idcliente.Text = "" + pusuario.IdCliente;

            if (servicio.Id == 1) { calendario.IsVisible = true; }
            else if (servicio.Id == 2) { calendario.IsVisible = true; }
            else
            {
                adebitar.Text = string.Format("{0:C}", servicio.Precio).Replace("$", string.Empty);
                btntransferir.IsEnabled = true;
            }
            pservicio = servicio;
            pdolar = dolar;
            pcuenta = cuenta;

            codigocuenta.Text = cuenta.CodigoCuenta;
            moneda.Text = cuenta.Moneda;
            saldo.Text = string.Format("{0:C}", cuenta.Saldo).Replace("$", string.Empty);

            cardcuenta.IsVisible = true;
            cuentaIsSelected = true;
        }

        protected override async void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("cargando...", MaskType.Clear);

            monedaconversion.Text = "USD";
            string valor = string.Format("{0:C}", (double.Parse(adebitar.Text) / pdolar.Compra));
            valorconversion.Text = valor.Replace("$", string.Empty);

            ppagos = await PagosApi.GetPagosUsuario(pusuario.NumeroIdentidad);

            for (int i = 0; i < ppagos.Count; i++)
            {
                if (ppagos[i].Servicio == pservicio.Id)
                {
                    pagos.Add(ppagos[i]);
                }
            }

            txttitulo.Text = pservicio.Nombre;
            txtdescripcion.Text = pservicio.Descripcion;
            imgservicio.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(pservicio.Imagen));
            //adebitar.Text = "" + pservicio.Precio;

            string date = await UsuarioApi.GetFechaServidor();

            int mes = int.Parse(date.Substring(5, 2));

            for (int i = mes; i <= 12; i++)
            {
                switch (i)
                {
                    case 1:
                        box01.IsEnabled = false;
                        break;
                    case 2:
                        box02.IsEnabled = false;
                        break;
                    case 3:
                        box03.IsEnabled = false;
                        break;
                    case 4:
                        box04.IsEnabled = false;
                        break;
                    case 5:
                        box05.IsEnabled = false;
                        break;
                    case 6:
                        box06.IsEnabled = false;
                        break;
                    case 7:
                        box07.IsEnabled = false;
                        break;
                    case 8:
                        box08.IsEnabled = false;
                        break;
                    case 9:
                        box09.IsEnabled = false;
                        break;
                    case 10:
                        box10.IsEnabled = false;
                        break;
                    case 11:
                        box11.IsEnabled = false;
                        break;
                    case 12:
                        box12.IsEnabled = false;
                        break;

                }
            }

            UserDialogs.Instance.HideLoading();
        }

        private async void btnvolver_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void btntransferir_Clicked(object sender, EventArgs e)
        {
            btntransferir.IsEnabled = false;
            string comentario;

            if (pservicio.Id == 1 || pservicio.Id == 2)
            {
                comentario = "Pago del servicio " + pservicio.Nombre;
            }
            else
            {
                comentario = "Pago del evento: " + pservicio.Nombre;
            }

            double valor;

            if(pcuenta.Moneda == "USD") { valor = double.Parse(adebitar.Text) / pdolar.Precio; }
            else { valor = double.Parse(adebitar.Text); }

            Transferencia transferencia = new Transferencia
            {
                Accion = "débito",
                Moneda = pcuenta.Moneda,
                Valor = valor,
                Envia = pcuenta.CodigoCuenta,
                Recibe = "servicioID" + pservicio.Id,
                Fecha = await UsuarioApi.GetFechaServidor(),
                Comentario = comentario
            };

            if (pcuenta.Moneda == "HNL") //el pago de servicios se maneja relativo a HNL
            {
                pcuenta.Saldo -= valor;
            }
            else
            {
                pcuenta.Saldo -= valor;
            }

            UserDialogs.Instance.ShowLoading("realizando transacción...", MaskType.Clear);

            var resultado = await App.DBase.CuentaSave(2, pcuenta);
            await CuentaApi.UpdateCuenta(pcuenta);

            //await App.DBase.TransferenciaSave(transferencia);
            enviarcorreo(pusuario, pcuenta, transferencia);
            await TransferenciaApi.CreateTransferencia(transferencia);

            UserDialogs.Instance.HideLoading();

            if (pservicio.Id == 1 || pservicio.Id == 2)
            {
                pago.Pago = "si";
                if (await PagosApi.UpdatePago(pago))
                {
                    await DisplayAlert("Éxito", "Pago de servicio realizado con éxito", "OK");
                    await Navigation.PushAsync(new Tablero(pusuario, pdolar));
                }
            }
            else
            {
                await DisplayAlert("Éxito", "Pago de evento realizado con éxito", "OK");
            }

            
        }

        private void EneroTapped(object sender, EventArgs e)
        {
            actualizarCalendario("enero");
            messeleccionado = "enero";

            verificarCalendario(01, messeleccionado);
        }

        private void FebreroTapped(object sender, EventArgs e)
        {
            actualizarCalendario("febrero");
            messeleccionado = "febrero";

            verificarCalendario(02, messeleccionado);
        }

        private void MarzoTapped(object sender, EventArgs e)
        {
            actualizarCalendario("marzo");
            messeleccionado = "marzo";

            verificarCalendario(03, messeleccionado);
        }

        private void AbrilTapped(object sender, EventArgs e)
        {
            actualizarCalendario("abril");
            messeleccionado = "abril";

            verificarCalendario(04, messeleccionado);
        }

        private void MayoTapped(object sender, EventArgs e)
        {
            actualizarCalendario("mayo");
            messeleccionado = "mayo";

            verificarCalendario(05, messeleccionado);
        }

        private void JunioTapped(object sender, EventArgs e)
        {
            actualizarCalendario("junio");
            messeleccionado = "junio";

            verificarCalendario(06, messeleccionado);
        }

        private void JulioTapped(object sender, EventArgs e)
        {
            actualizarCalendario("julio");
            messeleccionado = "julio";

            verificarCalendario(07, messeleccionado);
        }

        private void AgostoTapped(object sender, EventArgs e)
        {
            actualizarCalendario("agosto");
            messeleccionado = "agosto";

            verificarCalendario(08, messeleccionado);
        }

        private void SeptiembreTapped(object sender, EventArgs e)
        {
            actualizarCalendario("septiembre");
            messeleccionado = "septiembre";

            verificarCalendario(09, messeleccionado);
        }

        private void OctubreTapped(object sender, EventArgs e)
        {
            actualizarCalendario("octubre");
            messeleccionado = "octubre";

            verificarCalendario(10, messeleccionado);
        }

        private void NoviembreTapped(object sender, EventArgs e)
        {
            actualizarCalendario("noviembre");
            messeleccionado = "noviembre";

            verificarCalendario(11, messeleccionado);
        }

        private void DiciembreTapped(object sender, EventArgs e)
        {
            actualizarCalendario("diciembre");
            messeleccionado = "diciembre";

            verificarCalendario(12, messeleccionado);
        }

        async void actualizarCalendario(string mes)
        {
            enero.TextColor = Color.FromHex("#000000");
            enerobox.BackgroundColor = Color.FromHex("#FFFFFF");

            febrero.TextColor = Color.FromHex("#000000");
            febrerobox.BackgroundColor = Color.FromHex("#FFFFFF");

            marzo.TextColor = Color.FromHex("#000000");
            marzobox.BackgroundColor = Color.FromHex("#FFFFFF");

            abril.TextColor = Color.FromHex("#000000");
            abrilbox.BackgroundColor = Color.FromHex("#FFFFFF");

            mayo.TextColor = Color.FromHex("#000000");
            mayobox.BackgroundColor = Color.FromHex("#FFFFFF");

            junio.TextColor = Color.FromHex("#000000");
            juniobox.BackgroundColor = Color.FromHex("#FFFFFF");

            julio.TextColor = Color.FromHex("#000000");
            juliobox.BackgroundColor = Color.FromHex("#FFFFFF");

            agosto.TextColor = Color.FromHex("#000000");
            agostobox.BackgroundColor = Color.FromHex("#FFFFFF");

            septiembre.TextColor = Color.FromHex("#000000");
            septiembrebox.BackgroundColor = Color.FromHex("#FFFFFF");

            octubre.TextColor = Color.FromHex("#000000");
            octubrebox.BackgroundColor = Color.FromHex("#FFFFFF");

            noviembre.TextColor = Color.FromHex("#000000");
            noviembrebox.BackgroundColor = Color.FromHex("#FFFFFF");

            diciembre.TextColor = Color.FromHex("#000000");
            diciembrebox.BackgroundColor = Color.FromHex("#FFFFFF");

            switch (mes)
            {
                case "enero":
                    enero.TextColor = Color.FromHex("#FFFFFF");
                    enerobox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "febrero":
                    febrero.TextColor = Color.FromHex("#FFFFFF");
                    febrerobox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "marzo":
                    marzo.TextColor = Color.FromHex("#FFFFFF");
                    marzobox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "abril":
                    abril.TextColor = Color.FromHex("#FFFFFF");
                    abrilbox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "mayo":
                    mayo.TextColor = Color.FromHex("#FFFFFF");
                    mayobox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "junio":
                    junio.TextColor = Color.FromHex("#FFFFFF");
                    juniobox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "julio":
                    julio.TextColor = Color.FromHex("#FFFFFF");
                    juliobox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "agosto":
                    agosto.TextColor = Color.FromHex("#FFFFFF");
                    agostobox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "septiembre":
                    septiembre.TextColor = Color.FromHex("#FFFFFF");
                    septiembrebox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "octubre":
                    octubre.TextColor = Color.FromHex("#FFFFFF");
                    octubrebox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "noviembre":
                    noviembre.TextColor = Color.FromHex("#FFFFFF");
                    noviembrebox.BackgroundColor = Color.FromHex("#465173");
                    break;
                case "diciembre":
                    diciembre.TextColor = Color.FromHex("#FFFFFF");
                    diciembrebox.BackgroundColor = Color.FromHex("#465173");
                    break;
            }
        }

        async void verificarCalendario(int nummes, string messeleccionado)
        {
            for (int i = 0; i < pagos.Count; i++)
            {
                if (pagos[i].Mes == nummes)
                {
                    if (pagos[i].Pago == "si")
                    {
                        btntransferir.IsEnabled = false;
                        nota.IsVisible = true;
                        notatexto.Text = "El mes de " + messeleccionado + " ya está pagado.";
                        adebitar.Text = "0.00";
                    }
                    else
                    {
                        pago = pagos[i];
                        btntransferir.IsEnabled = true;
                        adebitar.Text = string.Format("{0:C}", pagos[i].Valor).Replace("$", string.Empty);
                        nota.IsVisible = false;
                        notatexto.Text = "";
                    }
                }
            }

            monedaconversion.Text = "USD";
            string valor = string.Format("{0:C}", (double.Parse(adebitar.Text) / pdolar.Compra));
            valorconversion.Text = valor.Replace("$", string.Empty);
        }

        private async void btnscuenta_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Cuentas(pusuario, 2, pdolar, pservicio)); //2 para ocultar boton volver (1es solamente la vista de seleccion de cuentas)
        }

        async void enviarcorreo(Usuario usuariod, Cuenta cuentad, Transferencia transferencia)
        {
            try
            {
                string valortransferencia = string.Format("{0:C}", transferencia.Valor).Replace("$", string.Empty);

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("starbankteam@gmail.com");
                mail.To.Add(usuariod.Email);
                mail.Subject = "STARBANK | Código de verificación";
                mail.Body = "<html> <Body> <h1>Comprobante de Transacción</h1> <br><br> <h3>Cambio del dólar, hoy " + obtenerFecha((await UsuarioApi.GetFechaServidor()).Substring(0, 10)) + "</h3> <p>Compra: <b>" + pdolar.Compra + "</b> | Venta: <b>" + pdolar.Venta + "</b></p> <br><br> <p>Cliente: <b>" + usuariod.NombreCompleto + "</b></p> <br> <p>Cuenta Saliente: <b>" + cuentad.CodigoCuenta + "</b></p> <br> <p>Pago del servicio: <b>" + pservicio.Nombre + "</b></p> <br><br> <h3>MONTO DE LA TRANSFERENCIA: " + cuentad.Moneda + valortransferencia + "</h3><br><br><b>Nota del debitante: </b><p>" + transferencia.Comentario + "</p></Body> </html>";
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
    }
}