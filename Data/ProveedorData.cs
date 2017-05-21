﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;

namespace Data
{
    public class ProveedorData
    {
        public static List<ProveedorView> GetAll()
        {
            var list = new List<ProveedorView>();
            var conn = new SqlConnection(GeneralData.CadenaConexion);
            SqlDataReader rdr = null;
            var cmd = new SqlCommand("Proveedor_GetAll", conn) { CommandType = CommandType.StoredProcedure };

            try {
                conn.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read()) {
                    var p = new ProveedorView {
                        IdProveedor = (int)rdr["IdProveedor"],
                        RazonSocial = (string)rdr["RazonSocial"],
                        Direccion = rdr["Direccion"] != DBNull.Value ? (string)rdr["Direccion"] : "",
                        Telefono = rdr["Telefono"] != DBNull.Value ? (string)rdr["Telefono"] : "",
                        Notas = rdr["Notas"] != DBNull.Value ? (string)rdr["Notas"] : ""
                    };

                    list.Add(p);
                }
            }
            finally {
                rdr?.Close();
                conn.Close();
            }

            return list;
        }


        public static Proveedor GetByPrimaryKey(int id)
        {
            var c = new Proveedor();
            var conn = new SqlConnection(GeneralData.CadenaConexion);
            SqlDataReader rdr = null;
            var cmd = new SqlCommand("Proveedor_GetByPrimaryKey", conn) { CommandType = CommandType.StoredProcedure };

            var p1 = new SqlParameter("IdProveedor", SqlDbType.BigInt) { Value = id };
            cmd.Parameters.Add(p1);

            try {
                conn.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read()) {
                    c.IdProveedor = (int)rdr["IdProveedor"];
                    c.RazonSocial = (string)rdr["RazonSocial"];
                    c.Direccion = rdr["Direccion"] != DBNull.Value ? (string)rdr["Direccion"] : "";
                    c.Telefono = rdr["Telefono"] != DBNull.Value ? (string)rdr["Telefono"] : "";
                    c.PersonaContacto = rdr["PersonaContacto"] != DBNull.Value ? (string)rdr["PersonaContacto"] : "";
                    c.HorarioAtencion = rdr["HorarioAtencion"] != DBNull.Value ? (string)rdr["HorarioAtencion"] : "";
                    c.DiasDeVisita = rdr["DiasDeVisita"] != DBNull.Value ? (string)rdr["DiasDeVisita"] : "";
                    c.Notas = rdr["Notas"] != DBNull.Value ? (string)rdr["Notas"] : "";
                    c.IdRubro = rdr["IdRubro"] != DBNull.Value ? (int)rdr["IdRubro"] : 0;

                }
            }
            finally {
                rdr?.Close();
                conn.Close();
            }

            return c;
        }


        public static List<ProveedorView> GetAll_GetByRazonSocial(string razonSocial)
        {
            //TODO: ESTO ES TERRIBLEMENTE REDUNDANTE, ES PARA UNA PRUEBA
            var list = new List<ProveedorView>();
            var conn = new SqlConnection(GeneralData.CadenaConexion);
            SqlDataReader rdr = null;
            var cmd = new SqlCommand("Proveedor_GetByRazonSocial", conn) {
                CommandType = CommandType.StoredProcedure
            };

            var p1 = new SqlParameter("RazonSocial", SqlDbType.VarChar) { Value = razonSocial };

            cmd.Parameters.Add(p1);

            try {
                conn.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read()) {
                    var p = new ProveedorView {
                        IdProveedor = (int)rdr["IdProveedor"],
                        RazonSocial = (string)rdr["RazonSocial"],
                        Direccion = rdr["Direccion"] != DBNull.Value ? (string)rdr["Direccion"] : "",
                        Telefono = rdr["Telefono"] != DBNull.Value ? (string)rdr["Telefono"] : "",
                        Notas = rdr["Notas"] != DBNull.Value ? (string)rdr["Notas"] : ""
                    };

                    list.Add(p);
                }
            }
            finally {
                rdr?.Close();
                conn.Close();
            }

            return list;

        }


        public static int Insert(Proveedor model)
        {
            int idProveedor;

            var conn = new SqlConnection(GeneralData.CadenaConexion);

            var cmd = new SqlCommand("Proveedor_Insert", conn) { CommandType = CommandType.StoredProcedure };

            var p1 = new SqlParameter("RazonSocial", SqlDbType.VarChar) { Value = model.RazonSocial };
            var p2 = new SqlParameter("Direccion", SqlDbType.VarChar) { Value = model.Direccion };
            var p3 = new SqlParameter("Telefono", SqlDbType.VarChar) { Value = model.Telefono };
            var p4 = new SqlParameter("PersonaContacto", SqlDbType.VarChar) { Value = model.PersonaContacto };
            var p5 = new SqlParameter("HorarioAtencion", SqlDbType.VarChar) { Value = model.HorarioAtencion };
            var p6 = new SqlParameter("DiasDeVisita", SqlDbType.VarChar) { Value = model.DiasDeVisita };

            var p7 = new SqlParameter("Notas", SqlDbType.VarChar) { Value = model.Notas };
            var p8 = new SqlParameter("IdRubro", SqlDbType.Int) { Value = model.IdRubro };

            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.Parameters.Add(p6);
            cmd.Parameters.Add(p7);
            cmd.Parameters.Add(p8);

            try {
                conn.Open();
                idProveedor = (int)cmd.ExecuteScalar();

            }
            finally {
                conn.Close();
            }

            return idProveedor;
        }


        public static ProveedorView GetByPrimaryKeyView(int id)
        {
            var c = new ProveedorView();
            var conn = new SqlConnection(GeneralData.CadenaConexion);
            SqlDataReader rdr = null;
            var cmd = new SqlCommand("Proveedor_GetByPrimaryKey", conn) { CommandType = CommandType.StoredProcedure };

            var p1 = new SqlParameter("IdProveedor", SqlDbType.Int) { Value = id };
            cmd.Parameters.Add(p1);

            try {
                conn.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read()) {
                    c.IdProveedor = (int)rdr["IdProveedor"];
                    c.RazonSocial = (string)rdr["RazonSocial"];
                    c.Direccion = rdr["Direccion"] != DBNull.Value ? (string)rdr["Direccion"] : "";
                    c.Telefono = rdr["Telefono"] != DBNull.Value ? (string)rdr["Telefono"] : "";
                    c.Notas = rdr["Notas"] != DBNull.Value ? (string)rdr["Notas"] : "";
                }
            }
            finally {
                rdr?.Close();
                conn.Close();
            }

            return c;
        }


        public static bool Delete(Proveedor model)
        {
            var conn = new SqlConnection(GeneralData.CadenaConexion);

            var cmd = new SqlCommand("Proveedor_Delete", conn) { CommandType = CommandType.StoredProcedure };

            var p0 = new SqlParameter("IdProveedor", SqlDbType.Int) { Value = model.IdProveedor };

            cmd.Parameters.Add(p0);

            try {
                conn.Open();
                model.IdProveedor = (int)cmd.ExecuteScalar();
                return true;
            }
            catch {
                return false;
            }
            finally {
                conn.Close();
            }
        }


        public static int Update(Proveedor model)
        {
            var conn = new SqlConnection(GeneralData.CadenaConexion);

            var cmd = new SqlCommand("Proveedor_Update", conn) { CommandType = CommandType.StoredProcedure };

            var p0 = new SqlParameter("IdProveedor", SqlDbType.Int) { Value = model.IdProveedor };
            var p1 = new SqlParameter("RazonSocial", SqlDbType.VarChar) { Value = model.RazonSocial };
            var p2 = new SqlParameter("Direccion", SqlDbType.VarChar) { Value = model.Direccion };
            var p3 = new SqlParameter("Telefono", SqlDbType.VarChar) { Value = model.Telefono };
            var p4 = new SqlParameter("PersonaContacto", SqlDbType.VarChar) { Value = model.PersonaContacto };
            var p5 = new SqlParameter("HorarioAtencion", SqlDbType.VarChar) { Value = model.HorarioAtencion };
            var p6 = new SqlParameter("DiasDeVisita", SqlDbType.VarChar) { Value = model.DiasDeVisita };
            var p7 = new SqlParameter("Notas", SqlDbType.VarChar) { Value = model.Notas };
            var p8 = new SqlParameter("IdRubro", SqlDbType.Int) { Value = model.IdRubro };

            cmd.Parameters.Add(p0);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.Parameters.Add(p4);
            cmd.Parameters.Add(p5);
            cmd.Parameters.Add(p6);
            cmd.Parameters.Add(p7);
            cmd.Parameters.Add(p8);

            try {
                conn.Open();
                model.IdProveedor = (int)cmd.ExecuteScalar();

            }
            finally {
                conn.Close();
            }

            return model.IdProveedor;
        }
    }
}