﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Controlador;
using Model;
using NLog;

namespace Kiosco
{
    public partial class FrmProductoDetalle : Form
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
            SetGrid(dgv);
        }


        private static void SetGrid(DataGridView dgv)
        {
            //TODO: Ver si se puede parametrizar dentro de las opciones del programa.
            dgv.AutoGenerateColumns = false;
            dgv.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgv.BorderStyle = BorderStyle.None;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeight = 20;

            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;

            dgv.RowsDefaultCellStyle.BackColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.Azure;
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
            if (e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                VerificarProducto();
                txtCodigoBarras.Select(0, txtCodigoBarras.Text.Length);
            }

            //si se produce este evento con Enter, es porque se ha ingresado con la lectora,
            //o bien el usuario presiono enter a proposito...



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

            CargarProveedores(p.IdProducto);

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

        private void CargarProveedores(long idProducto)
        {
            dgv.Columns.Clear();

            var c = new DataGridViewColumn[colCount];

            for (var i = 0; i < colCount; i++) {
                c[i] = new DataGridViewTextBoxColumn();
            }

            c[(int)ProductoProveedorView.GridColumn.IdProductoProveedor].Width = 0;
            c[(int)ProductoProveedorView.GridColumn.IdProductoProveedor].Visible = false;
            c[(int)ProductoProveedorView.GridColumn.IdProducto].Width = 0;
            c[(int)ProductoProveedorView.GridColumn.IdProducto].Visible = false;

            Util.SetColumn(c[(int)ProductoProveedorView.GridColumn.IdProductoProveedor], "IdProductoProveedor", "IdProductoProveedor", 0);
            Util.SetColumn(c[(int)ProductoProveedorView.GridColumn.IdProducto], "IdProducto", "IdProducto", 1);
            Util.SetColumn(c[(int)ProductoProveedorView.GridColumn.Producto], "Producto", "Producto", 2);
            Util.SetColumn(c[(int)ProductoProveedorView.GridColumn.Proveedor], "Proveedor", "Proveedor", 3);
            Util.SetColumn(c[(int)ProductoProveedorView.GridColumn.PrecioProveedor], "PrecioProveedor", "Precio Proveedor", 4);
            Util.SetColumn(c[(int)ProductoProveedorView.GridColumn.PrecioVenta], "PrecioVenta", "Precio Venta", 5);
            dgv.Columns.AddRange(c);


            Util.SetColumnsReadOnly(dgv);

            origenDatos = ProductoProveedorControlador.GetGrid_GetByIdProducto(idProducto);

            var bindingList = new MySortableBindingList<ProductoProveedorView>(origenDatos);
            var source = new BindingSource(bindingList, null);
            dgv.DataSource = source;

            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;
        }


        private void txtIdProducto_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void txtCodigoBarras_TextChanged_1(object sender, EventArgs e)
        {

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
    }
}
