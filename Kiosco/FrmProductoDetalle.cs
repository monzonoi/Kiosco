﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Controlador;
using Model;
using NLog;

namespace Kiosco
{
    public partial class FrmProductoDetalle : Form, ISelectorProducto
    {
        public FrmProductoDetalle()
        {
            InitializeComponent();
        }

        private void FrmProductoDetalle_Load(object sender, EventArgs e)
        {
            SetControles();


        }


        public void SetControles()
        {
            txtIdProducto.Visible = false;
            txtIdMarca.Visible = false;
            txtIdRubro.Visible = false;
            txtCodigoBarras.MaxLength = 13;
            txtDescripcion.Enabled = false;
            txtMarca.Enabled = false;
            txtRubroDescripcion.Enabled = false;
            txtNotas.Enabled = false;
            txtPrecio.Enabled = false;
            txtPrecioCosto.Enabled = false;

            //TODO: CUIDADO CON ESTO. EXPERIMENTAL
            btnRegistrarVentaRapida.Enabled = true;
        }


        private void txtIdMarca_TextChanged(object sender, EventArgs e)
        {
            if (txtIdMarca.Text.Trim() == "")
                return;
            var codigo = Convert.ToInt32(txtIdMarca.Text.Trim());
            var c = MarcaControlador.GetByPrimaryKey(codigo);
            txtMarca.Text = c.Descripcion;
        }


        private void txtIdRubro_TextChanged(object sender, EventArgs e)
        {
            if (txtIdRubro.Text.Trim() == "")
                return;
            var codigo = Convert.ToInt32(txtIdRubro.Text.Trim());
            var c = RubroControlador.GetByPrimaryKey(codigo);
            txtRubroDescripcion.Text = c.Descripcion;
        }

        private void tsbNuevo_Click(object sender, EventArgs e)
        {
            LimpiarControles();
        }

        private void LimpiarControles()
        {
            txtIdProducto.Clear();
            txtIdMarca.Clear();
            txtIdRubro.Clear();
            txtCodigoBarras.Clear();
            txtDescripcion.Clear();
            txtRubroDescripcion.Clear();
            txtMarca.Clear();
            txtNotas.Clear();
            txtPrecio.Clear();
            txtPrecioCosto.Clear();
        }


        private void txtCodigoBarras_KeyDown(object sender, KeyEventArgs e)
        {
            //si se produce este evento con Enter, es porque se ha ingresado con la lectora,
            //o bien el usuario presiono enter a proposito...
            if (e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                VerificarProducto();
                txtCodigoBarras.Select(0, txtCodigoBarras.Text.Length);
            }

        }


        //private static Logger logger = LogManager.GetCurrentClassLogger();

        private void VerificarProducto()
        {
            var productoEncontrado = false;
            var productoIngresado = false;

            var codigo = txtCodigoBarras.Text.Trim();

            //logger.Error("Consulta de Producto: " + codigo);
            //logger.Debug("Consulta de Producto: " + codigo);

            var p = ProductoControlador.GetByCodigoBarras(codigo);

            productoEncontrado = p.IdProducto != 0;
            txtIdProducto.Text = p.IdProducto.ToString();

            ucNotification.Text = !productoEncontrado ?
                    "Producto no encontrado en la Base de Datos." :
                    "Producto encontrado.";

            ucNotification.BackColor = !productoEncontrado ?
                Color.LightCoral :
                Color.LightGreen;

            ucNotification.Ocultar();

            //esto sirve en ambos casos.
            txtDescripcion.Text = p.Descripcion;
            txtPrecio.Text = p.PrecioVenta.ToString();
            txtPrecioCosto.Text = p.PrecioCostoPromedio.ToString();

            txtIdRubro.Text = p.IdRubro.ToString();
            txtIdMarca.Text = p.IdMarca.ToString();
            chkSoloAdultos.Checked = p.SoloAdultos ?? false;
            txtNotas.Text = p.Notas;

            ucProveedorList1.IdProducto = p.IdProducto;

            CargarStockActual(p.IdProducto);
        }


        private void CargarStockActual(long idProducto)
        {
            var s = new Stock { IdProducto = idProducto, IdDeposito = Deposito.IdDepositoNegocio };
            var cantidad = StockControlador.GetByParameters(s).Cantidad;
            nudStockActual.Value = cantidad;
        }


        private const int colCount = 6;

        private List<ProductoProveedorView> origenDatos = null;


        private void txtIdProducto_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO: REVISAR ESTA PRUEBA.
            //Codigo de prueba para realizar una venta rapida,
            //de un solo producto, en cantidad m.

            VenderProducto(Convert.ToInt64(txtIdProducto.Text));

            //TODO: encapsular.
            ucNotification2.Text ="Venta registrada con Exito.";
            ucNotification2.BackColor = Color.LightGreen;
            ucNotification2.Ocultar();

            //Importante. Esto evita a priori registrar ventas accidentales!
            txtCodigoBarras.Focus();

            //TODO: Nota, debe haber una forma de DESHACER una Venta.
            //Es muy facil cometer errores en esto, por la velocidad y el caos.

        }


        private void VenderProducto(long idProducto)
        {
            const int idUsuarioActual = Usuario.IdUsuarioPredeterminado;

            var codigoBarras = txtCodigoBarras.Text.Trim();
            var cantidad = (int)nudCantidadVenta.Value;
            var precio = Convert.ToDecimal(txtPrecio.Text);
            var importe = cantidad * precio;


            //=====================================================================
            var modelMovimientoCaja = new MovimientoCaja {
                IdMovimientoCaja = -1,
                IdUsuario = idUsuarioActual,
                IdClaseMovimientoCaja = MovimientoCaja.IdClaseMovimientoCajaVenta,
                Monto = importe,
                Fecha = Convert.ToDateTime(DateTime.Now.ToShortDateString()),
                Hora = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
            };

            //=====================================================================

            modelMovimientoCaja.IdMovimientoCaja = MovimientoCajaControlador.Insert(modelMovimientoCaja);

            //TODO: Tratar de validar antes de esto.
            //Todo lo que venga de la pantalla es poco confiable.

            //TODO: Tambien deberia validar si la insercion anterior fue exitosa.
            //El proceso puede ser reversible tambien si se han hecho algunas inserciones y no todas.

            var modelVenta = new Venta {
                IdVenta = -1,
                IdCliente = 1,
                Total = modelMovimientoCaja.Monto,
                Fecha = Convert.ToDateTime(DateTime.Now.ToShortDateString()),
                IdMovimientoCaja = modelMovimientoCaja.IdMovimientoCaja,
                PendientePago = false,
                Notas = ""
            };

            modelVenta.IdVenta = VentaControlador.Insert(modelVenta);

            //=====================================================================


            var mp = new MovimientoProducto {
                IdMovimientoProducto = -1,
                IdProducto = idProducto,
                Cantidad = cantidad,
                Fecha = modelVenta.Fecha,
                IdClaseMovimientoProducto = MovimientoProducto.IdClaseMovimientoProductoVenta,
                IdUsuario = idUsuarioActual
            };
            //A partir del codigobarras recuperar el producto, si no tengo su id en grilla.

            //por cada elemento, persistir en BD.
            mp.IdMovimientoProducto = MovimientoProductoControlador.Insert(mp);

            //=====================================================================
            var vd = new VentaDetalle {
                IdVentaDetalle = -1,
                IdVenta = modelVenta.IdVenta,
                Cantidad = cantidad,
                Importe = importe,
                IdMovimientoProducto = mp.IdMovimientoProducto,
                IdProducto = mp.IdProducto
            };

            vd.IdVentaDetalle = VentaDetalleControlador.Insert(vd);

            //=====================================================================
            var s = new Stock {
                IdStock = -1,
                Cantidad = -cantidad,
                IdDeposito = 1,
                IdProducto = mp.IdProducto
            };

            s.IdStock = StockControlador.Update(s);
            //=====================================================================


            return;

        }


        private void txtCodigoBarras_TextChanged_1(object sender, EventArgs e)
        {
            //TODO: Ver si puedo meter codigos especiales... tipo "rb" para red bus. O usar iconos.
        }

        private void btnUpdateStock_Click(object sender, EventArgs e)
        {
            var cantidad = (int)nudStockActual.Value;
            var idProducto = Convert.ToInt64(txtIdProducto.Text.Trim());
            var idDeposito = Deposito.IdDepositoNegocio;

            var s = new Stock {
                IdProducto = idProducto,
                IdDeposito = idDeposito,
                Cantidad = cantidad,
                IdStock = -1

            };

            // El codigo IdStock no se conoce a priori, y no deberia ser importante.
            //invocar a metodo update de Clase Stock
            var res = StockControlador.Update(s);
        }

        private void btnSeleccionarProducto_Click(object sender, EventArgs e)
        {
            var f = new FrmSeleccionarProducto(this);
            f.Show();
        }

        [Description("IdProducto. Su evento de cambio genera DataBinding."), Category("Data")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public long IdProducto
        {
            get {
                long v = long.TryParse(txtIdProducto.Text.Trim(), out v) ? v : 0;
                return v;
            }
            set {
                txtIdProducto.Text = value.ToString();
                //OnProductoChanged(new ValueChangedEventArgs(value));
                var p = ProductoControlador.GetByPrimaryKey(Convert.ToInt64(txtIdProducto.Text));
                txtCodigoBarras.Text = p.CodigoBarras;
                VerificarProducto();
            }
        }
    }
}
