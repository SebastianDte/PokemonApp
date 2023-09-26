using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dominio;
using Negocio;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Negocio
{
    public class PokemonNegocio
    {
        public List<Pokemon> listar()
        {
            List<Pokemon> lista = new List<Pokemon>();
            //Establecer Coneccion
            AccesoDatos datos = new AccesoDatos();
            
            try
            {
                datos.setearConsulta("select Numero,Nombre,P.Descripcion,UrlImagen,E.Descripcion Tipo, D.Descripcion Debilidad,P.IdTipo , P.IdDebilidad, P.Id  From POKEMONS P, Elementos E, Elementos D  where E.Id = P.IdTipo  and D.Id = P.IdDebilidad And P.Activo = 1");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Pokemon aux = new Pokemon();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Numero = (int)datos.Lector["Numero"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector["urlImagen"] is DBNull))
                    aux.UrlImagen = (string)datos.Lector["UrlImagen"];
                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)datos.Lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)datos.Lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)datos.Lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)datos.Lector["Debilidad"];
                    lista.Add(aux);
                }
                    
               return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally 
            {
                datos.cerrarConexion();
            }
        }

        public void agregar(Pokemon nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("Insert into pokemons (Numero,Nombre,Descripcion,Activo, IdTipo, IdDebilidad, urlImagen) values (" + nuevo.Numero +  " ,'"+ nuevo.Nombre + "','" + nuevo.Descripcion + "',1, @idTipo, @IdDebilidad, @urlImagen)");
                datos.setearParametro("@idTipo", nuevo.Tipo.Id);
                datos.setearParametro("@idDebilidad", nuevo.Debilidad.Id);
                datos.setearParametro("@urlImagen", nuevo.UrlImagen);
                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            { datos.cerrarConexion();}
        }

        public void modificar(Pokemon modificar)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update POKEMONS set numero = @numero, Nombre = @nombre, Descripcion = @descripcion, UrlImagen = @urlImagen, IdTipo = @idTipo, IdDebilidad = @idDebilidad where Id = @id; ");
                datos.setearParametro("@numero",modificar.Numero );
                datos.setearParametro("@nombre",modificar.Nombre );
                datos.setearParametro("@descripcion",modificar.Descripcion);
                datos.setearParametro("@urlImagen",modificar.UrlImagen );
                datos.setearParametro("@idTipo",modificar.Tipo.Id );
                datos.setearParametro("@idDebilidad", modificar.Debilidad.Id );
                datos.setearParametro("@id",modificar.Id );

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally { datos.cerrarConexion();}

        }

        public void eliminar(int id)
        {
           
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("delete from POKEMONS where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        
        public void eliminarLogico(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("update POKEMONS set Activo = 0 Where id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarLectura();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Pokemon> filtrar(string campo, string criterio, string filtro)
        {
            List<Pokemon> lista = new List<Pokemon>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = "select Numero,Nombre,P.Descripcion,UrlImagen,E.Descripcion Tipo, D.Descripcion Debilidad,P.IdTipo , P.IdDebilidad, P.Id  From POKEMONS P, Elementos E, Elementos D  where E.Id = P.IdTipo  and D.Id = P.IdDebilidad And P.Activo = 1 And ";

                switch (campo)
                {
                    case "Número":
                        switch (criterio)
                        {
                            case "Mayor a":
                                consulta += "Numero > " + filtro;
                                break;
                            case "Menor a":
                                consulta += "Numero < " + filtro;
                                break;
                            default:
                                consulta += "Numero = " + filtro;
                                break;
                        }
                        break;

                    case "Nombre":
                        switch (criterio)
                        {
                            case "Comienza con":
                                consulta += "Nombre like '" + filtro + "%'";
                                break;
                            case "Termina con":
                                consulta += "Nombre like '%" + filtro + "'";
                                break;
                            default:
                                consulta += "Nombre like '%" + filtro + "%'";
                                break;
                        }
                        break;
                    case "Descripción":
                        switch (criterio)
                        {
                            case "Comienza con":
                                consulta += "P.Descripcion like '%" + filtro + "'";
                                break;
                            case "Termina con":
                                consulta += "P.Descripcion like '" + filtro + "%'";
                                break;
                            default:
                                consulta += "P.Descripcion like '%" + filtro + "%'";
                                break;
                        }
                        break;
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Pokemon aux = new Pokemon();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Numero = (int)datos.Lector["Numero"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    if (!(datos.Lector["urlImagen"] is DBNull))
                        aux.UrlImagen = (string)datos.Lector["UrlImagen"];
                    aux.Tipo = new Elemento();
                    aux.Tipo.Id = (int)datos.Lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)datos.Lector["Tipo"];
                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)datos.Lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)datos.Lector["Debilidad"];
                    lista.Add(aux);
                }
                return lista;

            }
            catch (Exception ex )
            {

                throw;
            }
        }
    }
}

                    

                            



    
                
                

               
